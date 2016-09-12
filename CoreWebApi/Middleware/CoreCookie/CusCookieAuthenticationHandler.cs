using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Net.Http.Headers;

namespace CoreWebApi.Middleware
{

    public class CusCookieAuthenticationHandler : AuthenticationHandler<CusCookieAuthenticationOptions>
    {
        private const string HeaderValueNoCache = "no-cache";
        private const string HeaderValueMinusOne = "-1";
        private const string SessionIdClaim = "Cookies-SessionId";

        private bool _shouldRefresh;
        private DateTimeOffset? _refreshIssuedUtc;
        private DateTimeOffset? _refreshExpiresUtc;
        private string _sessionKey;
        private Task<AuthenticateResult> _readCookieTask;
        private string _exceptionMessage;

        public CusCookieAuthenticationHandler() { }

        private Task<AuthenticateResult> EnsureCookieTicket()
        {
            // We only need to read the ticket once
            if (_readCookieTask == null)
            {
                _readCookieTask = ReadCookieTicket();
            }
            return _readCookieTask;
        }

        ///<summary>
        ///检查是否需要刷新session
        ///</summary>
        private void CheckForRefresh(AuthenticationTicket ticket)
        {
            var currentUtc = Options.SystemClock.UtcNow;
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;
            var allowRefresh = ticket.Properties.AllowRefresh ?? true;
            if (issuedUtc != null && expiresUtc != null && Options.SlidingExpiration && allowRefresh)
            {
                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                var timeRemaining = expiresUtc.Value.Subtract(currentUtc);

                if (timeRemaining < timeElapsed)
                {
                    RequestRefresh(ticket);
                }
            }
        }

        private void RequestRefresh(AuthenticationTicket ticket)
        {
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (issuedUtc != null && expiresUtc != null)
            {
                _shouldRefresh = true;
                var currentUtc = Options.SystemClock.UtcNow;
                _refreshIssuedUtc = currentUtc;
                var timeSpan = expiresUtc.Value.Subtract(issuedUtc.Value);
                _refreshExpiresUtc = currentUtc.Add(timeSpan);
            }
        }

        private async Task<AuthenticateResult> ReadCookieTicket()
        {
            var cookie = Options.CookieManager.GetRequestCookie(Context, Options.CookieName);
            if (string.IsNullOrEmpty(cookie))
            {
                _exceptionMessage = CoreResult.NewData(1002,null,"Basic");
                return AuthenticateResult.Fail(_exceptionMessage);
            }

            var ticket = Options.TicketDataFormat.Unprotect(cookie, GetTlsTokenBinding());
            if (ticket == null)
            {
                _exceptionMessage = CoreResult.NewData(1003,null,"Basic");
                return AuthenticateResult.Fail(_exceptionMessage);
            }

            if (Options.SessionStore != null)
            {
                var claim = ticket.Principal.Claims.FirstOrDefault(c => c.Type.Equals(SessionIdClaim));
                if (claim == null)
                {
                    _exceptionMessage = CoreResult.NewData(1007,null,"Basic");
                    return AuthenticateResult.Fail(_exceptionMessage);
                }
                _sessionKey = claim.Value;
                ticket = await Options.SessionStore.RetrieveAsync(_sessionKey);
                if (ticket == null)
                {
                    _exceptionMessage = CoreResult.NewData(1006,null,"Basic");
                    return AuthenticateResult.Fail(_exceptionMessage);
                }
            }

            var currentUtc = Options.SystemClock.UtcNow;
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (expiresUtc != null && expiresUtc.Value < currentUtc)
            {
                if (Options.SessionStore != null)
                {
                    await Options.SessionStore.RemoveAsync(_sessionKey);
                }
                _exceptionMessage = CoreResult.NewData(1004,null,"Basic");
                return AuthenticateResult.Fail(_exceptionMessage);
            }

            CheckForRefresh(ticket);

            // Finally we have a valid ticket
            return AuthenticateResult.Success(ticket);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var result = await EnsureCookieTicket();
            if (!result.Succeeded)
            {
                return result;
            }

            var context = new CusCookieValidatePrincipalContext(Context, result.Ticket, Options);
            await Options.Events.ValidatePrincipal(context);

            if (context.Principal == null)
            {
                _exceptionMessage = CoreResult.NewData(1005,null,"Basic");
                return AuthenticateResult.Fail(_exceptionMessage);
            }

            if (context.ShouldRenew)
            {
                RequestRefresh(result.Ticket);
            }

            return AuthenticateResult.Success(new AuthenticationTicket(context.Principal, context.Properties, Options.AuthenticationScheme));
        }

        private CookieOptions BuildCookieOptions()
        {
            var cookieOptions = new CookieOptions
            {
                Domain = Options.CookieDomain,
                HttpOnly = Options.CookieHttpOnly,
                Path = Options.CookiePath ?? (OriginalPathBase.HasValue ? OriginalPathBase.ToString() : "/"),
            };
            if (Options.CookieSecure == CookieSecurePolicy.SameAsRequest)
            {
                cookieOptions.Secure = Request.IsHttps;
            }
            else
            {
                cookieOptions.Secure = Options.CookieSecure == CookieSecurePolicy.Always;
            }
            return cookieOptions;
        }

        protected override async Task FinishResponseAsync()
        {
            // Only renew if requested, and neither sign in or sign out was called
            if (!_shouldRefresh || SignInAccepted || SignOutAccepted)
            {
                return;
            }

            var ticket = (await HandleAuthenticateOnceAsync())?.Ticket;
            if (ticket != null)
            {
                if (_refreshIssuedUtc.HasValue)
                {
                    ticket.Properties.IssuedUtc = _refreshIssuedUtc;
                }
                if (_refreshExpiresUtc.HasValue)
                {
                    ticket.Properties.ExpiresUtc = _refreshExpiresUtc;
                }

                if (Options.SessionStore != null && _sessionKey != null)
                {
                    await Options.SessionStore.RenewAsync(_sessionKey, ticket);
                    var principal = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new[] { new Claim(SessionIdClaim, _sessionKey, ClaimValueTypes.String, Options.ClaimsIssuer) },
                            Options.AuthenticationScheme));
                    ticket = new AuthenticationTicket(principal, null, Options.AuthenticationScheme);
                }

                var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());

                var cookieOptions = BuildCookieOptions();
                if (ticket.Properties.IsPersistent && _refreshExpiresUtc.HasValue)
                {
                    cookieOptions.Expires = _refreshExpiresUtc.Value.ToUniversalTime();
                }

                Options.CookieManager.AppendResponseCookie(
                    Context,
                    Options.CookieName,
                    cookieValue,
                    cookieOptions);

                ApplyHeaders(properties: ticket.Properties);
            }
        }

        protected override async Task HandleSignInAsync(SignInContext signin)
        {
            // Process the request cookie to initialize members like _sessionKey.
            var result = await EnsureCookieTicket();
            var cookieOptions = BuildCookieOptions();

            var signInContext = new CusCookieSigningInContext(
                Context,
                Options,
                Options.AuthenticationScheme,
                signin.Principal,
                new AuthenticationProperties(signin.Properties),
                cookieOptions);

            DateTimeOffset issuedUtc;
            if (signInContext.Properties.IssuedUtc.HasValue)
            {
                issuedUtc = signInContext.Properties.IssuedUtc.Value;
            }
            else
            {
                issuedUtc = Options.SystemClock.UtcNow;
                signInContext.Properties.IssuedUtc = issuedUtc;
            }

            if (!signInContext.Properties.ExpiresUtc.HasValue)
            {
                signInContext.Properties.ExpiresUtc = issuedUtc.Add(Options.ExpireTimeSpan);
            }

            await Options.Events.SigningIn(signInContext);

            if (signInContext.Properties.IsPersistent)
            {
                var expiresUtc = signInContext.Properties.ExpiresUtc ?? issuedUtc.Add(Options.ExpireTimeSpan);
                signInContext.CookieOptions.Expires = expiresUtc.ToUniversalTime();
            }

            var ticket = new AuthenticationTicket(signInContext.Principal, signInContext.Properties, signInContext.AuthenticationScheme);
            if (Options.SessionStore != null)
            {
                if (_sessionKey != null)
                {
                    await Options.SessionStore.RemoveAsync(_sessionKey);
                }
                _sessionKey = await Options.SessionStore.StoreAsync(ticket);
                var principal = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new[] { new Claim(SessionIdClaim, _sessionKey, ClaimValueTypes.String, Options.ClaimsIssuer) },
                        Options.ClaimsIssuer));
                ticket = new AuthenticationTicket(principal, null, Options.AuthenticationScheme);
            }
            var cookieValue = Options.TicketDataFormat.Protect(ticket, GetTlsTokenBinding());

            Options.CookieManager.AppendResponseCookie(
                Context,
                Options.CookieName,
                cookieValue,
                signInContext.CookieOptions);

            var signedInContext = new CusCookieSignedInContext(
                Context,
                Options,
                Options.AuthenticationScheme,
                signInContext.Principal,
                signInContext.Properties);

            await Options.Events.SignedIn(signedInContext);

            // Only redirect on the login path
            // var shouldRedirect = Options.LoginPath.HasValue && OriginalPath == Options.LoginPath;
            ApplyHeaders(signedInContext.Properties);
        }

        protected override async Task HandleSignOutAsync(SignOutContext signOutContext)
        {
            // Process the request cookie to initialize members like _sessionKey.
            var ticket = await EnsureCookieTicket();
            var cookieOptions = BuildCookieOptions();
            if (Options.SessionStore != null && _sessionKey != null)
            {
                await Options.SessionStore.RemoveAsync(_sessionKey);
            }

            var context = new CusCookieSigningOutContext(
                Context,
                Options,
                new AuthenticationProperties(signOutContext.Properties),
                cookieOptions);

            await Options.Events.SigningOut(context);

            Options.CookieManager.DeleteCookie(
                Context,
                Options.CookieName,
                context.CookieOptions);

            // Only redirect on the logout path
            // var shouldRedirect = Options.LogoutPath.HasValue && OriginalPath == Options.LogoutPath;
            ApplyHeaders(context.Properties);
        }

        private void ApplyHeaders(AuthenticationProperties properties)
        {
            Response.Headers[HeaderNames.CacheControl] = HeaderValueNoCache;
            Response.Headers[HeaderNames.Pragma] = HeaderValueNoCache;
            Response.Headers[HeaderNames.Expires] = HeaderValueMinusOne;
        }

        private static bool IsHostRelative(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            if (path.Length == 1)
            {
                return path[0] == '/';
            }
            return path[0] == '/' && path[1] != '/' && path[1] != '\\';
        }

        protected override Task<bool> HandleForbiddenAsync(ChallengeContext context)
        {
            Response.Headers["WWW-Authenticate"] = Options.AuthenticationScheme;
            var ex = System.Text.Encoding.UTF8.GetBytes(CoreResult.NewData(1001,null,"Basic"));
            Response.Body.Write(ex,0,ex.Length);
            return Task.FromResult(true);
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            Response.Headers["WWW-Authenticate"] = Options.AuthenticationScheme;
            var ex = System.Text.Encoding.UTF8.GetBytes(_exceptionMessage);
            Response.Body.Write(ex,0,ex.Length);
            
            return Task.FromResult(false);

        }

        private string GetTlsTokenBinding()
        {
            var binding = Context.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }
}