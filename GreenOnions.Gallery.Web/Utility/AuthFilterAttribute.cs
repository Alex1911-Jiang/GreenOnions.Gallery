using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace GreenOnions.Gallery.Api
{
    public class AuthFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            string token = context.HttpContext.Session.GetString("Token");
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("您无权查看此页面。");

            var controller = context.Controller as Controller;
            controller.ViewBag.Token = token;
            JwtSecurityToken jwt = new(token);
            Dictionary<string, string> claims = jwt.Claims.ToDictionary(c1 => c1.Type, c2 => c2.Value);
            controller.ViewBag.Account = claims["Account"];
            controller.ViewBag.NickName = claims["NickName"];
            controller.ViewBag.Permission = claims["Permission"];
            controller.ViewBag.ApiKey = claims["ApiKey"];
            controller.ViewBag.Email = claims["Email"];
            if (Convert.ToInt32(claims["Permission"]) != 9)
                throw new Exception("您无权查看此页面。");

            base.OnResultExecuting(context);
        }
    }
}
