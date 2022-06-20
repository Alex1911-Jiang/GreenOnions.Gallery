using GreenOnions.Gallery.Api.Utility;
using GreenOnions.Gallery.Interface;
using GreenOnions.Gallery.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GreenOnions.Gallery.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AccessFilter]
    public class PicturesController : ControllerBase
    {
        private readonly ILogger<PicturesController> _logger;
        private readonly ISqlService _sqlService;
        private readonly ICacheHelper _cacheHelper;

        public PicturesController(ILogger<PicturesController> logger, ISqlService sqlService, ICacheHelper cacheHelper)
        {
            _logger = logger;
            _sqlService = sqlService;
            _cacheHelper = cacheHelper;
        }

        [HttpPost]
        public async Task<PictureResponseModel> Pictures([FromBody]PictureRequestModel model)
        {
            _logger.LogWarning("有个LSP要了一张色图");
            string ApiKey = "Public";
            if (!string.IsNullOrWhiteSpace(model.Apikey))
            {
                ApiKey = model.Apikey;
            }
            if (!string.IsNullOrWhiteSpace(HttpContext.Request.Headers["ApiKey"]))
            {
                ApiKey = HttpContext.Request.Headers["ApiKey"];
            }
            var authenticate = HttpContext?.AuthenticateAsync().GetAwaiter().GetResult().Principal;
            if (authenticate != null)
            {
                Dictionary<string, string> claims = authenticate.Claims.ToDictionary(c1 => c1.Type, c2 => c2.Value);
                if (!string.IsNullOrWhiteSpace(claims["ApiKey"]))
                {
                    ApiKey = claims["ApiKey"];
                }
            }

            try
            {
                string grading = model.Grading.Replace("，", ",");
                string selectTop = "TOP 1";
                string selectGrading = $" AND [Grading] in ({grading})";
                if (string.IsNullOrWhiteSpace(ApiKey) || string.Equals(ApiKey, "Public", StringComparison.CurrentCultureIgnoreCase))  //没有ApiKey
                {
                    _logger.LogWarning("这个LSP没有ApiKey");
                    string[] arrGrading = grading.Split(",");
                    for (int i = 0; i < arrGrading.Length; i++)
                    {
                        int onceGrading = -1;
                        if (!int.TryParse(arrGrading[i], out onceGrading))
                        {
                            return new PictureResponseModel { Code = 403, Msg = "Grading类型不正确，无法查到对应年龄分级的图片" };
                        }
                        if (onceGrading > 1)
                        {
                            return new PictureResponseModel { Code = 403, Msg = "不携带ApiKey时只允许Grading为0或1" };
                        }
                    }

                    ApiKey = "Public";
                    if (_cacheHelper.LimitCacheTime.ContainsKey(ApiKey))
                    {
                        if (DateTime.Now < _cacheHelper.LimitCacheTime[ApiKey] && _cacheHelper.LimitCacheModel[ApiKey] != null)
                        {
                            return _cacheHelper.LimitCacheModel[ApiKey];
                        }
                        else
                        {
                            _cacheHelper.LimitCacheTime[ApiKey] = DateTime.Now.AddSeconds(30);
                        }
                    }
                    else
                    {
                        _cacheHelper.LimitCacheTime.Add(ApiKey, DateTime.Now.AddSeconds(30));
                    }
                }
                else  //有ApiKey
                {
                    _logger.LogWarning("这个LSP有ApiKey=" + ApiKey);
                    DataSet dsKey = await _sqlService.Select($"SELECT * FROM [GreenOnionsGallery].[dbo].[Users] WHERE [ApiKey] = '{ApiKey}'");
                    if (dsKey.Tables.Count == 0)
                    {
                        return new PictureResponseModel { Code = 401, Msg = "ApiKey不存在" };
                    }
                    if (model.Count > 1)
                    {
                        selectTop = $"TOP {model.Count}";
                    }

                    if (_cacheHelper.LimitCacheTime.ContainsKey(ApiKey))
                    {
                        if (DateTime.Now < _cacheHelper.LimitCacheTime[ApiKey] && _cacheHelper.LimitCacheModel[ApiKey] != null)
                        {
                            return _cacheHelper.LimitCacheModel[ApiKey];
                        }
                        else
                        {
                            _cacheHelper.LimitCacheTime[ApiKey] = DateTime.Now.AddSeconds(1);
                        }
                    }
                    else
                    {
                        _cacheHelper.LimitCacheTime.Add(ApiKey, DateTime.Now.AddSeconds(1));
                    }
                }

                bool bPixivTable = false;
                bool bTwitterTable = false;
                if (string.Equals(model.Origin, "Pixiv", StringComparison.CurrentCultureIgnoreCase))
                {
                    bPixivTable = true;
                }
                else if (string.Equals(model.Origin, "Twitter", StringComparison.CurrentCultureIgnoreCase))
                {
                    bTwitterTable = true;
                }
                else
                {
                    if (new Random(Guid.NewGuid().GetHashCode()).Next(1, 101) < 50)
                        bPixivTable = true;
                    else
                        bTwitterTable = true;
                }

                string selectTag = "";
                if (!string.IsNullOrWhiteSpace(model.Tag))
                {
                    selectTag = $" AND [Tags] like '%{model.Tag}%'";
                }
                string selectIllustrator = "";
                if (bTwitterTable)
                {
                    if (!string.IsNullOrWhiteSpace(model.Illustrator))
                    {
                        selectTag = $" AND [Uid] = (SELECT [Id] FROM [GreenOnionsGallery].[dbo].[TwitterIllustrators] WHERE [Name] = '{model.Illustrator}' or PATINDEX('%,{model.Illustrator},%',','+[Alias]+',') > 0)";
                    }

                    string sql = $"SELECT {selectTop} *,NEWID() as random FROM [GreenOnionsGallery].[dbo].[TwitterPictures] LEFT JOIN [GreenOnionsGallery].[dbo].[TwitterIllustrator] ON [GreenOnionsGallery].[dbo].[TwitterPictures].[Uid] = [GreenOnionsGallery].[dbo].[TwitterIllustrator].[Id] WHERE 1=1 {selectGrading} {selectTag} {selectIllustrator} ORDER BY random";

                    DataSet dsPicture = await _sqlService.Select(sql);

                    if (dsPicture.Tables[0].Rows.Count > 0)
                    {
                        PictureResponseModel pictureResponseModel = new() { Code = 200, Origin = "Twitter", Data = new List<PictureResponseDataModel>() };

                        foreach (DataRow row in dsPicture.Tables[0].Rows)
                        {
                            pictureResponseModel.Data.Add(new PictureResponseDataModel
                            {
                                Url = row["Url"].ToString(),
                                Source = row["Source"].ToString(),
                                Title = row["Title"].ToString(),
                                Illustrator = row["Illustrator"].ToString(),
                                Grading = row["Grading"].ToString(),
                                Width = row["Width"].ToString(),
                                Height = row["Height"].ToString(),
                                Tags = row["Tags"].ToString(),
                            });
                        }

                        if (_cacheHelper.LimitCacheModel.ContainsKey(ApiKey))
                            _cacheHelper.LimitCacheModel[ApiKey] = pictureResponseModel;
                        else
                            _cacheHelper.LimitCacheModel.Add(ApiKey, pictureResponseModel);
                        return pictureResponseModel;
                    }

                    return new PictureResponseModel { Code = 200 };
                }
                else if (bPixivTable)
                {
                    if (!string.IsNullOrWhiteSpace(model.Illustrator))
                    {
                        selectTag = $" AND [Uid] = (SELECT [Id] FROM [GreenOnionsGallery].[dbo].[PixivIllustrators] WHERE [Name] = '{model.Illustrator}' or PATINDEX('%,{model.Illustrator},%',','+[Alias]+',') > 0)";
                    }

                    string sql = $"SELECT {selectTop} *,NEWID() as random FROM [GreenOnionsGallery].[dbo].[PixivPictures] LEFT JOIN [GreenOnionsGallery].[dbo].[PixivIllustrator] ON [GreenOnionsGallery].[dbo].[PixivPictures].[Uid] = [GreenOnionsGallery].[dbo].[PixivIllustrator].[Id] WHERE 1=1 {selectGrading} {selectTag} {selectIllustrator} ORDER BY random";

                    DataSet dsPicture = await _sqlService.Select(sql);

                    if (dsPicture.Tables[0].Rows.Count > 0)
                    {
                        PictureResponseModel pictureResponseModel = new() { Code = 200, Origin = "Pixiv", Data = new List<PictureResponseDataModel>() };

                        foreach (DataRow row in dsPicture.Tables[0].Rows)
                        {
                            //占位符0：原站：i.pximg.net  镜像站：i.pixiv.cat
                            //占位符1：原图：original 1200图：master
                            //占位符2：原图：空字符串，1200图：_master1200
                            string url = "";
                            string pximg;
                            string proxy = model.Proxy;

                            if (row["Url"].ToString().Contains("/img/"))
                            {
                                pximg = "i.pximg.net.";
                            }
                            else
                            {
                                pximg = "pximg.net";
                                if (string.Equals(model.Proxy, "i.pixiv.cat", StringComparison.CurrentCultureIgnoreCase))
                                    proxy = "pixiv.cat";
                                if (string.Equals(model.Proxy, "i.pixiv.re", StringComparison.CurrentCultureIgnoreCase))
                                    proxy = "pixiv.re";
                            }

                            string host = string.IsNullOrWhiteSpace(model.Proxy) ? pximg : proxy;

                            if (model.Size1200)
                                url = string.Format(row["Url"].ToString(), host, "master", "_master1200").Replace(".png", ".jpg").Replace(".gif", ".jpg");
                            else
                                url = string.Format(row["Url"].ToString(), host, "original", "");

                            pictureResponseModel.Data.Add(new PictureResponseDataModel
                            {
                                Pid = row["Pid"].ToString(),
                                P = row["P"].ToString(),
                                Title = row["Title"].ToString(),
                                Uid = row["Uid"].ToString(),
                                Illustrator = row["Illustrator"].ToString(),
                                Url = url,
                                Grading = row["Grading"].ToString(),
                                Width = row["Width"].ToString(),
                                Height = row["Height"].ToString(),
                                Tags = row["Tags"].ToString(),
                            });
                        }

                        if (_cacheHelper.LimitCacheModel.ContainsKey(ApiKey))
                            _cacheHelper.LimitCacheModel[ApiKey] = pictureResponseModel;
                        else
                            _cacheHelper.LimitCacheModel.Add(ApiKey, pictureResponseModel);
                        return pictureResponseModel;
                    }

                    return new PictureResponseModel { Code = 404, Msg = $"没有找到tag为{model.Tag}的图片" };
                }
                else
                {
                    return new PictureResponseModel { Code = 404, Msg = $"没有找到origin为{model.Origin}的图片" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"异常信息={ex.Message}\r\n\r\n" +
                    $"apikey={model.Apikey }\r\n" +
                    $"grading={model.Grading}\r\n" +
                    $"tag={model.Tag}\r\n" +
                    $"illustrator={model.Illustrator}\r\n" +
                    $"count={model.Count}\r\n" +
                    $"proxy={model.Proxy}\r\n" +
                    $"size1200={model.Size1200}\r\n" +
                    $"origin={model.Origin}\r\n\r\n" +
                    $"异常发生时间:{DateTime.Now}");
                return new PictureResponseModel { Code = 500, Msg = ex.Message };
            }
        }
    }
}
