using Consul;
using GreenOnions.Gallery.Api;
using GreenOnions.Gallery.Common;
using GreenOnions.Gallery.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenOnions.Gallery.Web.Controllers
{
    [TokenFilter]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("有一个dalao进入了主页");

            string result = await WebApiHelper.InvokeApiGetAsync($"{_configuration["GatewayHost"]}/api/Manage/Count");

            if (!string.IsNullOrEmpty(result))
            {
                JObject jToken = (JObject)JsonConvert.DeserializeObject(result);

                ViewBag.PictureTotal = jToken["total"].ToString();
                ViewBag.PixivCount = jToken["pixivCount"].ToString();
                ViewBag.Pixiv0Count = jToken["pixiv0Count"].ToString();
                ViewBag.Pixiv1Count = jToken["pixiv1Count"].ToString();
                ViewBag.Pixiv2Count = jToken["pixiv2Count"].ToString();
                ViewBag.Pixiv3Count = jToken["pixiv3Count"].ToString();
                ViewBag.Pixiv9Count = jToken["pixiv9Count"].ToString();
                ViewBag.TwitterCount = jToken["twitterCount"].ToString();
                ViewBag.Twitter0Count = jToken["twitter0Count"].ToString();
                ViewBag.Twitter1Count = jToken["twitter1Count"].ToString();
                ViewBag.Twitter2Count = jToken["twitter2Count"].ToString();
                ViewBag.Twitter3Count = jToken["twitter3Count"].ToString();
                ViewBag.Twitter9Count = jToken["twitter9Count"].ToString();
            }

            return View();
        }

        public IActionResult Guide()
        {
            _logger.LogInformation("有一个dalao查看了食用方法");
            return View();
        }

        public IActionResult Example()
        {
            _logger.LogInformation("有一个dalao查看了调用例子");
            return View();
        }

        public async Task<IActionResult> Picture()
        {
            _logger.LogInformation("有一个LSP要了一张色图");

            Dictionary<string, string> header = new ();
            header.Add("Accept", "application/json");
            Dictionary<string, string> body = new();
            body.Add("Origin", "Pixiv");
            string token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrWhiteSpace(token))
            {
                header.Add("Authorization", $"bearer {token}");

                JwtSecurityToken jwt = new(token);
                Dictionary<string, string> claims = jwt.Claims.ToDictionary(c1 => c1.Type, c2 => c2.Value);
                body.Add("ApiKey", claims["ApiKey"]);
            }

            var result = await WebApiHelper.InvokeApiPostAsync($"{_configuration["GatewayHost"]}/api/Pictures", body, header);

            if (string.IsNullOrWhiteSpace(result))
                throw new Exception("Api没有响应，请稍后再试。");

            JObject jo = (JObject)JsonConvert.DeserializeObject(result);

            if (Convert.ToInt32(jo["code"].ToString()) == 200)
            {
                JToken item = (jo["data"] as JArray)[0];

                ViewData["ImageUrl"] = CacheHelper.PublicImageUrlCache = item["url"].ToString();

                int imageWidth = Convert.ToInt32(item["width"].ToString());
                int imageHeight = Convert.ToInt32(item["height"].ToString());

                ViewData["ImageWidth"] = CacheHelper.PublicImageWidthCache = imageWidth;
                ViewData["ImageHeight"] = CacheHelper.PublicImageHeightCache = imageHeight;

                return View();
            }
            throw new Exception(jo["msg"].ToString());
        }
    }
}
