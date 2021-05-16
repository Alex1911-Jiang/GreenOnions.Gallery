using Consul;
using GreenOnions.Gallery.Api;
using GreenOnions.Gallery.Common;
using GreenOnions.Gallery.Web.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GreenOnions.Gallery.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            _logger.LogInformation("有一个dalao进入了登录页面");
            return View("Login");
        }

        [HttpPost]
        public async Task<ActionResult> Login(string account, string password, string verify)
        {
            _logger.LogInformation("有一个dalao点击了登录");
            string verifyCode = HttpContext.Session.GetString("CheckCode");
            if (verifyCode != null && verifyCode.Equals(verify, StringComparison.CurrentCultureIgnoreCase))
            {
                if (account.Contains("'") || account.Contains("--") || account.Contains("\"") || password.Contains("/") || password.Contains("*") || password.Contains(";"))
                {
                    ViewBag.Msg = "宁搁这儿Sql注入呢？别给我整这些奇怪的符号，小心我顺着网线过去打你。";
                }
                else
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] output = md5.ComputeHash(Encoding.Default.GetBytes(password));
                    string strMd5Pwd = BitConverter.ToString(output).Replace("-", "");

                    Dictionary<string, string> body = new();
                    body.Add("Account", account);
                    body.Add("Password", strMd5Pwd);

                    string result = await WebApiHelper.InvokeApiPostAsync($"{_configuration["GatewayHost"]}/auth/Authentication/Login", body);

                    JObject jToken = (JObject)JsonConvert.DeserializeObject(result);

                    if (result != null)
                    {
                        HttpContext.Session.SetString("Token", jToken["token"].ToString());
                        return Redirect($"/Home/Index");
                    }
                    else
                    {
                        ViewBag.Msg = "用户名或密码错误";
                    }
                }
            }
            else
            {
                ViewBag.Msg = "验证码错误";
            }
            return View("Login");
        }

        public IActionResult Logout()
        {
            _logger.LogInformation("有一个dalao退出了登录");
            HttpContext.Session.Remove("Token");
            ViewBag.Token = null;
            ViewBag.NickName = null;
            ViewBag.Permission = null;
            ViewBag.ApiKey = null; 
            return Redirect($"/Home/Index");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(string account, string nickName, string email, string password, string verify, string emailverify)
        {
            _logger.LogInformation("有一个dalao点击了注册");
            string verifyCode = HttpContext.Session.GetString("CheckCode");
            if (verifyCode != null && verifyCode.Equals(verify, StringComparison.CurrentCultureIgnoreCase))
            {
                if (password.Contains("'") || password.Contains("--") || password.Contains("\"") || password.Contains("/") || password.Contains("*"))
                {
                    ViewBag.Msg = "宁搁这儿Sql注入呢？别给我整这些奇怪的符号，小心我顺着网线过去打你。";
                }
                else
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] output = md5.ComputeHash(Encoding.Default.GetBytes(password));
                    string strMd5Pwd = BitConverter.ToString(output).Replace("-", "");

                    Dictionary<string, string> body = new();
                    body.Add("Account", account);
                    body.Add("NickName", nickName);
                    body.Add("Email", email);
                    body.Add("Password", strMd5Pwd);
                    body.Add("EmailVerifyCode", emailverify);

                    string result = await WebApiHelper.InvokeApiPostAsync($"{_configuration["GatewayHost"]}/auth/Authentication/Register", body);

                    JObject jToken = (JObject)JsonConvert.DeserializeObject(result);

                    if (result != null)
                    {
                        HttpContext.Session.SetString("Token", jToken["token"].ToString());
                        return Redirect($"/Home/Index");
                    }
                    else
                    {
                        ViewBag.Msg = "注册服务异常，稍后重试！";
                    }
                }
            }
            else
            {
                ViewBag.Msg = "验证码错误！";
            }
            return View("Register");
        }

        [HttpPost]
        [TokenFilter]
        public IActionResult SetToken([FromBody]string token)
        {
            HttpContext.Session.SetString("Token", token);
            return View("Info");
        }

        [TokenFilter]
        public IActionResult Info()
        {
            _logger.LogInformation("有一个dalao查看了自己的用户信息");
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        public ActionResult VerifyCode()
        {
            Bitmap bitmap = VerifyCodeHelper.CreateVerifyCodeImage(out string code);
            HttpContext.Session.SetString("CheckCode", code);
            MemoryStream stream = new();
            bitmap.Save(stream, ImageFormat.Gif);
            return File(stream.ToArray(), "image/gif");
        }
    }
}
