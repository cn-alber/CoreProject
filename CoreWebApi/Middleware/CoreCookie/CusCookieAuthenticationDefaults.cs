namespace CoreWebApi.Middleware
{
      public static class CusCookieAuthenticationDefaults
    {
        /// <summary>
        /// The default value used for CookieAuthenticationOptions.AuthenticationScheme
        /// </summary>
        public const string AuthenticationScheme = "Cookies";

        /// <summary>
        /// The prefix used to provide a default CookieAuthenticationOptions.CookieName
        /// </summary>
        public static readonly string CookiePrefix = ".";

        /// <summary>
        /// The default value used by CookieAuthenticationMiddleware for the
        /// CookieAuthenticationOptions.LoginPath
        /// </summary>
        // public static readonly PathString LoginPath = new PathString("/Account/");

        /// <summary>
        /// The default value used by CookieAuthenticationMiddleware for the
        /// CookieAuthenticationOptions.LogoutPath
        /// </summary>
        // public static readonly PathString LogoutPath = new PathString("/Account/Logout");

        /// <summary>
        /// The default value used by CookieAuthenticationMiddleware for the
        /// CookieAuthenticationOptions.AccessDeniedPath
        /// </summary>
        // public static readonly PathString AccessDeniedPath = new PathString("/Account/AccessDenied");

        /// <summary>
        /// The default value of the CookieAuthenticationOptions.ReturnUrlParameter
        /// </summary>
        // public static readonly string ReturnUrlParameter = "ReturnUrl";
    }
}