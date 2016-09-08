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
using Newtonsoft.Json;

namespace CoreWebApi
{
    [Route("/api/products/RouteTest")]
    public class ProductsController:Controller
    {
        public SignInManager<Models.Login> signInManager{get;}
        private static List<Product> _products = new List<Product>(new[] {
            new Product() { Id = 1, Name = "Computer" },
            new Product() { Id = 2, Name = "Radio" },
            new Product() { Id = 3, Name = "Apple" },
        });

        public IActionResult Get()
        {
            var test = new Models.Login();
            test.account = "admin";
            test.password = "admin";
            test.vcode = "123123";

            var ss = JsonConvert.SerializeObject(test);
            return new OkObjectResult(ss);
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
        [Route("/sign/in")]
        public async Task<ResponseResult> login(Models.Login lo)
        {
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.Name, "xishuai")
                         },
                         "CoreInstance"));
           await HttpContext.Authentication.SignInAsync("CoreInstance", new ClaimsPrincipal());
            return new ResponseResult(100, lo, "");
        }


    }
}