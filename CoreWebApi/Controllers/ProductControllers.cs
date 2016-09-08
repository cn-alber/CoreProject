using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CoreData;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using Microsoft.AspNetCore.Http.Authentication;

namespace CoreWebApi
{
    [Route("/api/products/RouteTest")]
    public class ProductsController
    {
        public SignInManager<Models.Login> signInManager{get;}
        private static List<Product> _products = new List<Product>(new[] {
            new Product() { Id = 1, Name = "Computer" },
            new Product() { Id = 2, Name = "Radio" },
            new Product() { Id = 3, Name = "Apple" },
        });

        public IActionResult Get()
        {
            return new OkObjectResult(MySqlData.GetRedisData());
        }

        [HttpGet("{id}")]
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> login(Models.Login lo)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.Name, "xishuai")
                         },
                         "CoreInstance"));
           await signInManager.SignInAsync(lo, new AuthenticationProperties
             {
                 ExpiresUtc = DateTime.UtcNow.AddMinutes(200),
                 IsPersistent = true,
                 AllowRefresh = false
             });
            // await AuthenticationManager.SignInAsync("CookieInstance", user,
            //  new AuthenticationProperties
            //  {
            //      ExpiresUtc = DateTime.UtcNow.AddMinutes(200),
            //      IsPersistent = true,
            //      AllowRefresh = false
            //  });             
            // await SignInAsync("CoreInstance", user, new AuthenticationProperties() { IsPersistent = true });
            return new OkObjectResult("123");
        }


    }
}