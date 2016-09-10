using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using Microsoft.AspNetCore.Http;

namespace CoreWebApi
{
    public class ProductsController : Controller
    {
        private static List<Product> _products = new List<Product>(new[] {
            new Product() { Id = 1, Name = "Computer" },
            new Product() { Id = 2, Name = "Radio" },
            new Product() { Id = 3, Name = "Apple" },
        });

        [Authorize(ActiveAuthenticationSchemes = "CoreInstance")]
        [HttpGet("/api/products/RouteTest")]
        public IActionResult Get()
        {
            var test = new Models.Login();
            test.account = "admin";
            test.password = "admin";
            test.vcode = "123123";

            var ss = JsonConvert.SerializeObject(test);
            return new OkObjectResult(ss);
        }

        [AllowAnonymous]
        [HttpGet("/api/products/RouteTest/{id}")]
        public IActionResult Get(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(product);
        }

        [HttpPost]
        public void Post([FromBody]Product product)
        {
            _products.Add(product);
        }

        [Authorize(ActiveAuthenticationSchemes = "CoreInstance")]
        [RouteAttribute("/sign/false")]
        public ResponseResult LoginFalse()
        {
            var cc = new CookieOptions();
            // HttpContext.Response.Cookies.Append("aaa", "1111", cc);

            return new ResponseResult(100, null, HttpContext.Session.Keys.Count().ToString());
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/sign/in")]
        public async Task<ResponseResult> login(Models.Login lo)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.Name, "xishuai")
                         },
                         "CoreInstance"));
            string Issuer = "urn:microsoft.example";
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.DateOfBirth, new DateTime(2000, 01, 01).ToString("u"), ClaimValueTypes.DateTime, Issuer));
            claims.Add(new Claim(ClaimTypes.Role, "User", ClaimValueTypes.String, Issuer));
            claims.Add(new Claim("Documents", "CRUD", ClaimValueTypes.String, "urn:microsoft.com"));
            claims.Add(new Claim("CanWeFixIt", "YesWeCan", ClaimValueTypes.String, "urn:bobthebuilder.com"));
            claims.Add(new Claim("CanWeFixIt", "NoWeCant", ClaimValueTypes.String, "urn:bobthebuilder.com"));
            var identity = new ClaimsIdentity(claims, "sampleAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.Authentication.SignInAsync("CoreInstance", new ClaimsPrincipal(),
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                    IsPersistent = false
                });
            return new ResponseResult(100, lo, "");
        }

        [HttpGetAttribute]
        [Route("/sign/out")]
        public async Task login()
        {
            await HttpContext.Authentication.SignOutAsync("CoreInstance");
        }
    }
}