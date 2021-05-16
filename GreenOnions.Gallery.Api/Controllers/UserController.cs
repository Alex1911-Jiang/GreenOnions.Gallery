using GreenOnions.Gallery.Api.Utility;
using GreenOnions.Gallery.Common;
using GreenOnions.Gallery.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenOnions.Gallery.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AccessFilter]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ISqlService _sqlService;
        private readonly IConfiguration _configuration;

        public UserController(ILogger<UserController> logger, ISqlService sqlService, IConfiguration configuration)
        {
            _logger = logger;
            _sqlService = sqlService;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize]
        public async Task<string> DestroyApiKey()
        {
            var authenticate = HttpContext?.AuthenticateAsync().GetAwaiter().GetResult().Principal;
            if (authenticate != null)
            {
                Dictionary<string, string> claims = authenticate.Claims.ToDictionary(c1 => c1.Type, c2 => c2.Value);
                _logger.LogWarning("有个dalao请求销毁ApiKey, 账号为=" + claims["Account"]);

                int i = await _sqlService.InsertUpdateDelete($"UPDATE [GreenOnionsGallery].[dbo].[Users] SET [ApiKey] = NULL WHERE Account = '{claims["Account"]}'");

                if (i > 0)
                {
                    string result = await WebApiHelper.InvokeApiPostAsync($"{_configuration["GatewayHost"]}/auth/Authentication/RebuildJWT", claims["Account"]);

                    if (string.IsNullOrEmpty(result))
                    {
                        return JsonConvert.SerializeObject(new
                        {
                            result = true,
                            message = "ApiKey已销毁，但登录服务发生问题，请稍后重新登录。",
                            token = ""
                        });
                    }
                    else
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                        if (!Convert.ToBoolean(jo["result"].ToString()))
                        {
                            return JsonConvert.SerializeObject(new
                            {
                                result = true,
                                message = "ApiKey已销毁，但更新Token失败，请重新登录。",
                                token = ""
                            });
                        }
                        else
                        {
                            return JsonConvert.SerializeObject(new
                            {
                                result = true,
                                message = "ApiKey已销毁。",
                                token = jo["token"].ToString()
                            });
                        }
                    }
                }
                return JsonConvert.SerializeObject(new
                {
                    result = false,
                    message = "没有查找到用户名，请重新登录。",
                    token = ""
                });
            }
            return JsonConvert.SerializeObject(new
            {
                result = false,
                message = "登录超时，请重新登录。",
                token = ""
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<string> CreateApiKey()
        {
            var authenticate = HttpContext?.AuthenticateAsync().GetAwaiter().GetResult().Principal;
            if (authenticate != null)
            {
                Dictionary<string, string> claims = authenticate.Claims.ToDictionary(c1 => c1.Type, c2 => c2.Value);
                string apiKey = Guid.NewGuid().ToString().Replace("-", "");

                int i = await _sqlService.InsertUpdateDelete($"UPDATE [GreenOnionsGallery].[dbo].[Users] SET [ApiKey] = '{apiKey}' WHERE Account = '{claims["Account"]}'");

                if (i > 0)
                {
                    string result = await WebApiHelper.InvokeApiPostAsync($"{_configuration["GatewayHost"]}/auth/Authentication/RebuildJWT", claims["Account"]);

                    if (string.IsNullOrEmpty(result))
                    {
                        return JsonConvert.SerializeObject(new
                        {
                            result = true,
                            message = "ApiKey已生成，但登录服务发生问题，请稍后重新登录。",
                            token = ""
                        });
                    }
                    else
                    {
                        JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                        if (!Convert.ToBoolean(jo["result"].ToString()))
                        {
                            return JsonConvert.SerializeObject(new
                            {
                                result = true,
                                message = "ApiKey已生成，但更新Token失败，请重新登录。",
                                token = ""
                            });
                        }
                        else
                        {
                            return JsonConvert.SerializeObject(new
                            {
                                result = true,
                                message = "ApiKey已生成。",
                                token = jo["token"].ToString()
                            });
                        }
                    }
                }
                return JsonConvert.SerializeObject(new
                {
                    result = false,
                    message = "没有查找到用户名，请重新登录。",
                    token = ""
                });
            }
            return JsonConvert.SerializeObject(new
            {
                result = false,
                message = "登录超时，请重新登录。",
                token = ""
            });
        }
    }
}
