using GreenOnions.Gallery.Api.Utility;
using GreenOnions.Gallery.Common;
using GreenOnions.Gallery.Interface;
using GreenOnions.Gallery.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GreenOnions.Gallery.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [AccessFilter]
    public class ManageController : ControllerBase
    {
        private readonly ILogger<ManageController> _logger;
        private readonly ISqlService _sqlService;

        public ManageController(ILogger<ManageController> logger, ISqlService sqlService)
        {
            _logger = logger;
            _sqlService = sqlService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<PictureCountModel> Count()
        {
            DataSet dsPicCount = await _sqlService.SelectMultiTable(
"SELECT [Grading], COUNT([Grading]) AS [Count] FROM [GreenOnionsGallery].[dbo].[PixivPictures] GROUP BY [Grading];" +
"SELECT [Grading], COUNT([Grading]) AS [Count] FROM [GreenOnionsGallery].[dbo].[TwitterPictures] GROUP BY [Grading];",
                new Dictionary<string, string>() {
                    { "Table", "Pixiv" },
                    { "Table1", "Twitter" }
                });

            PictureCountModel pictureCountModel = new ();
            pictureCountModel.Code = 200;
            foreach (DataRow row in dsPicCount.Tables["Pixiv"].Rows)
            {
                switch (row["Grading"].ToString())
                {
                    case "0":
                        pictureCountModel.Pixiv0Count = Convert.ToInt32(row["Count"]);
                        break;
                    case "1":
                        pictureCountModel.Pixiv1Count = Convert.ToInt32(row["Count"]);
                        break;
                    case "2":
                        pictureCountModel.Pixiv2Count = Convert.ToInt32(row["Count"]);
                        break;
                    case "3":
                        pictureCountModel.Pixiv3Count = Convert.ToInt32(row["Count"]);
                        break;
                    case "9":
                        pictureCountModel.Pixiv9Count = Convert.ToInt32(row["Count"]);
                        break;
                }
            }
            foreach (DataRow row in dsPicCount.Tables["Twitter"].Rows)
            {
                switch (row["Grading"].ToString())
                {
                    case "0":
                        pictureCountModel.Twitter0Count = Convert.ToInt32(row["Count"]);
                        break;
                    case "1":
                        pictureCountModel.Twitter1Count = Convert.ToInt32(row["Count"]);
                        break;
                    case "2":
                        pictureCountModel.Twitter2Count = Convert.ToInt32(row["Count"]);
                        break;
                    case "3":
                        pictureCountModel.Twitter3Count = Convert.ToInt32(row["Count"]);
                        break;
                    case "9":
                        pictureCountModel.Twitter9Count = Convert.ToInt32(row["Count"]);
                        break;
                }
            }

            return pictureCountModel;
        }

        [HttpGet]
        public async Task<string> RandomReadPixiv()
        {
            DataSet ds = await _sqlService.Select("SELECT TOP (1) [Pid],[P],[PageCount],[Title],[Uid],[Illustrator],[Alias],[Url],[Grading],[Width],[Height],[Tags] ,NEWID() AS random FROM [GreenOnionsGallery].[dbo].[PixivPictures] LEFT JOIN [GreenOnionsGallery].[dbo].[PixivIllustrator] ON [GreenOnionsGallery].[dbo].[PixivPictures].[Uid] = [GreenOnionsGallery].[dbo].[PixivIllustrator].[Id] WHERE [Grading] = 9 ORDER BY random");
            if (ds.Tables[0].Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    result = true,
                    pid = ds.Tables[0].Rows[0]["Pid"],
                    p = ds.Tables[0].Rows[0]["P"],
                    pageCount = ds.Tables[0].Rows[0]["PageCount"],
                    title = ds.Tables[0].Rows[0]["Title"],
                    uid = ds.Tables[0].Rows[0]["Uid"],
                    illustrator = ds.Tables[0].Rows[0]["Illustrator"],
                    alias = ds.Tables[0].Rows[0]["Alias"],
                    originalUrl = string.Format(ds.Tables[0].Rows[0]["Url"].ToString(), "i.pximg.net", "original", ""),
                    originalCatUrl = string.Format(ds.Tables[0].Rows[0]["Url"].ToString(), "i.pixiv.cat", "original", ""),
                    masterCatUrl = string.Format(ds.Tables[0].Rows[0]["Url"].ToString(), "i.pixiv.cat", "master", "_master1200").Replace(".png", ".jpg").Replace(".gif", ".jpg"),
                    grading = ds.Tables[0].Rows[0]["Grading"],
                    tags = ds.Tables[0].Rows[0]["Tags"]
                });
            }
            return JsonConvert.SerializeObject(new
            {
                result = false,
                message = "没有查到年龄分级为9的数据"
            });
        }

        [HttpGet]
        public async Task<string> FindPixivIllustrator(string uid)
        {
            if (uid != null)
            {
                DataSet ds = await _sqlService.Select($"SELECT [Illustrator],[Alias] FROM [GreenOnionsGallery].[dbo].[PixivIllustrator] WHERE [Id] = {uid}");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        result = true,
                        illustrator = ds.Tables[0].Rows[0]["Illustrator"],
                        alias = ds.Tables[0].Rows[0]["Alias"]
                    });
                }
            }
            return JsonConvert.SerializeObject(new
            {
                result = false
            });
        }

        [HttpGet]
        public async Task<string> FindTwitterIllustrator(string illustrator)
        {
            if (illustrator != null)
            {
                DataSet ds = await _sqlService.Select($"SELECT [UserName],[Alias] FROM [GreenOnionsGallery].[dbo].[TwitterIllustrator] WHERE Illustrator = {illustrator}");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return JsonConvert.SerializeObject(new
                    {
                        result = true,
                        userName = ds.Tables[0].Rows[0]["UserName"],
                        alias = ds.Tables[0].Rows[0]["Alias"]
                    });
                }
            }
            return JsonConvert.SerializeObject(new
            {
                result = false
            });
        }

        [HttpGet]
        public async Task<string> RandomReadTwitter()
        {
            DataSet ds = await _sqlService.Select("SELECT TOP (1) [Url],[Source],[Title],[Uid],[Illustrator],[Alias],[Grading],[Tags],NEWID() AS random FROM[GreenOnionsGallery].[dbo].[TwitterPictures] LEFT JOIN[GreenOnionsGallery].[dbo].[TwitterIllustrator] ON[GreenOnionsGallery].[dbo].[TwitterPictures].[Uid] = [GreenOnionsGallery].[dbo].[TwitterIllustrator].[Id] WHERE[Grading] = 9 ORDER BY random");
            if (ds.Tables[0].Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    result = true,
                    url = ds.Tables[0].Rows[0]["Url"],
                    source = ds.Tables[0].Rows[0]["Source"],
                    title = ds.Tables[0].Rows[0]["Title"],
                    illustrator = ds.Tables[0].Rows[0]["Illustrator"],
                    alias = ds.Tables[0].Rows[0]["Alias"],
                    grading = ds.Tables[0].Rows[0]["Grading"],
                    tags = ds.Tables[0].Rows[0]["Tags"]
                });
            }
            return JsonConvert.SerializeObject(new
            {
                result = false,
                message = "没有查到年龄分级为9的数据"
            });
        }

        [HttpGet]
        public async Task<string> ReadPixivByPidAndP(long pid, int p)
        {
            DataSet ds = await _sqlService.Select($"SELECT [Pid],[P],[PageCount],[Title],[Uid],[Illustrator],[Alias],[Url],[Grading],[Width],[Height],[Tags] FROM [GreenOnionsGallery].[dbo].[PixivPictures] LEFT JOIN [GreenOnionsGallery].[dbo].[PixivIllustrator] ON [GreenOnionsGallery].[dbo].[PixivPictures].[Uid] = [GreenOnionsGallery].[dbo].[PixivIllustrator].[Id] WHERE [Pid] = {pid} AND [P] = {p}");
            if (ds.Tables[0].Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    result = true,
                    pid = ds.Tables[0].Rows[0]["Pid"],
                    p = ds.Tables[0].Rows[0]["P"],
                    pageCount = ds.Tables[0].Rows[0]["PageCount"],
                    title = ds.Tables[0].Rows[0]["Title"],
                    uid = ds.Tables[0].Rows[0]["Uid"],
                    illustrator = ds.Tables[0].Rows[0]["Illustrator"],
                    alias = ds.Tables[0].Rows[0]["Alias"],
                    originalUrl = string.Format(ds.Tables[0].Rows[0]["Url"].ToString(), "i.pximg.net", "original", ""),
                    originalCatUrl = string.Format(ds.Tables[0].Rows[0]["Url"].ToString(), "i.pixiv.cat", "original", ""),
                    masterCatUrl = string.Format(ds.Tables[0].Rows[0]["Url"].ToString(), "i.pixiv.cat", "master", "_master1200").Replace(".png", ".jpg").Replace(".gif", ".jpg"),
                    grading = ds.Tables[0].Rows[0]["Grading"],
                    tags = ds.Tables[0].Rows[0]["Tags"]
                });
            }
            return JsonConvert.SerializeObject(new
            {
                result = false
            });
        }

        [HttpPost]
        public async Task<string> ReadTwitterByUrl([FromBody]string url)
        {
            DataSet ds = await _sqlService.Select($"SELECT [Url],[Source],[Title],[Uid],[Illustrator],[UserName],[Alias],[Grading],[Width],[Height],[Tags] FROM [GreenOnionsGallery].[dbo].[TwitterPictures] LEFT JOIN [GreenOnionsGallery].[dbo].[TwitterIllustrator] ON [GreenOnionsGallery].[dbo].[TwitterPictures].[Uid] = [GreenOnionsGallery].[dbo].[TwitterIllustrator].[Id] WHERE [Url] = '{url}'");
            if (ds.Tables[0].Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    result = true,
                    url = ds.Tables[0].Rows[0]["Url"],
                    source = ds.Tables[0].Rows[0]["Source"],
                    title = ds.Tables[0].Rows[0]["Title"],
                    illustrator = ds.Tables[0].Rows[0]["Illustrator"],
                    userName = ds.Tables[0].Rows[0]["UserName"],
                    alias = ds.Tables[0].Rows[0]["Alias"],
                    grading = ds.Tables[0].Rows[0]["Grading"],
                    width = ds.Tables[0].Rows[0]["Width"],
                    height = ds.Tables[0].Rows[0]["Height"],
                    tags = ds.Tables[0].Rows[0]["Tags"]
                });
            }
            return JsonConvert.SerializeObject(new
            {
                result = false
            });
        }

        public async Task<string> GetPixivUtlAndPageCountByPidAndP(long pid, int p)
        {
            DataSet ds = await _sqlService.Select($"SELECT [Pid],[P] FROM [GreenOnionsGallery].[dbo].[PixivPictures] WHERE [Pid] = {pid} AND [P] = {p}");
            if (ds.Tables[0].Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(new
                {
                    result = false  //表示数据已存在
                });
            }
            return JsonConvert.SerializeObject(new
            {
                result = true,

            });
        }

        public async Task<string> InsertPixivPicture(SavePixivPictureModel model)
        {
            _logger.LogWarning($"添加pixiv记录, pid={model.Pid}, p={model.P} 开始");

            string url = model.Url.Replace("i.pximg.net", "{0}").Replace("i.pixiv.cat", "{0}").Replace("pximg.net", "{0}").Replace("pixiv.cat", "{0}").Replace("-original", "-{1}").Replace("-master", "-{1}").Replace("_master1200", "{2}");
            if (!url.Contains("{2}"))
                url = url.Insert(url.LastIndexOf("."), "{2}");

            _logger.LogWarning($"调用pixiv.cat获取页面图片数量开始");
            int pageCount = await GetPageCount(model.Pid);
            _logger.LogWarning($"调用pixiv.cat获取页面图片数量结束, 数量为=" + pageCount);

            int i = await _sqlService.InsertUpdateDelete(
               $"BEGIN tran " +
               $"IF NOT EXISTS(SELECT 1 FROM [GreenOnionsGallery].[dbo].[PixivIllustrator] WHERE [Id] = {model.Uid}) " +
               $"  INSERT INTO [GreenOnionsGallery].[dbo].[PixivIllustrator] ([Id],[Illustrator],[Alias]) VALUES('{model.Uid}','{model.Illustrator}','{model.Alias}') " +
               $"ELSE " +
               $"  UPDATE [GreenOnionsGallery].[dbo].[PixivIllustrator] SET [Illustrator] = '{model.Illustrator}',[Alias] = '{model.Alias}' WHERE [Id] = {model.Uid} " +
               $"INSERT INTO [GreenOnionsGallery].[dbo].[PixivPictures] ([Pid],[P],[PageCount],[Title],[Uid],[Url],[Grading],[Width],[Height],[Tags]) VALUES ({model.Pid},{model.P},{pageCount},N'{model.Title}',{model.Uid},'{url}',{model.Grading},{model.Width},{model.Height},'{model.Tags.Replace("'", "''")}') " +
               $"COMMIT tran");
            _logger.LogWarning($"添加pixiv记录, pid={model.Pid}, p={model.P} 完成, 是否成功={i > 0}");


            string msg = "";
            if (pageCount == -1)
                msg = "添加数据成功, 但自动获取页面图片数量失败!";

            return JsonConvert.SerializeObject(new
            {
                result = true,
                message = msg
            });
        }

        private static async Task<int> GetPageCount(long pid)
        {
            Dictionary<string, string> keyValuePairs = new();
            keyValuePairs.Add("p", pid.ToString());
            string pixivCatApiResponse;

            try
            {
                pixivCatApiResponse = await WebApiHelper.InvokeApiPostAsync("https://api.pixiv.cat/v1/generate", keyValuePairs );

                JObject jPixivCatApiResponse = (JObject)JsonConvert.DeserializeObject(pixivCatApiResponse);

                if (jPixivCatApiResponse != null && !Convert.ToBoolean(jPixivCatApiResponse["success"].ToString()))
                    return -1;
                
                bool bMultiple = Convert.ToBoolean(jPixivCatApiResponse["multiple"].ToString());
                if (bMultiple)
                    return (jPixivCatApiResponse["original_urls"] as JArray).Count;
                else
                    return 0;
            }
            catch
            {
                return -1;
            }
        }


        public async Task<string> InsertTwitterPicture(SaveTwitterPictureModel model)
        {
            _logger.LogWarning($"添加twitter记录, url={model.Url} 开始");

            if (!model.UserName.StartsWith("@"))
                model.UserName = "@" + model.UserName;

            int i = await _sqlService.InsertUpdateDelete(
                $"BEGIN tran " +
                $"IF NOT EXISTS(SELECT 1 FROM [GreenOnionsGallery].[dbo].[TwitterIllustrator] WHERE [Illustrator] = '{model.Illustrator}') " +
                $"  INSERT INTO [GreenOnionsGallery].[dbo].[TwitterIllustrator] ([Illustrator],[UserName],[Alias]) VALUES('{model.Illustrator}','{model.UserName}','{model.Alias}') " +
                $"ELSE " +
                $"  UPDATE [GreenOnionsGallery].[dbo].[TwitterIllustrator] SET [UserName] = '{model.UserName}', [Alias] = '{model.Alias}' WHERE [Illustrator] = '{model.Illustrator}' " +
                $"INSERT INTO [GreenOnionsGallery].[dbo].[TwitterPictures] ([Url],[Source],[Title],[Uid],[Grading],[Width],[Height],[Tags]) VALUES ('{model.Url}','{model.Source}',N'{model.Title}',(SELECT [Id] FROM [GreenOnionsGallery].[dbo].[TwitterIllustrator] WHERE [Illustrator]='{model.Illustrator}'),{model.Grading},{model.Width},{model.Height},'{model.Tags.Replace("'", "''")}') " +
                $"COMMIT tran");

            _logger.LogWarning($"添加twitter记录, url={model.Url} 完成, 是否成功={i > 0}");
            return JsonConvert.SerializeObject(new
            {
                result = true
            });
        }

        [HttpPost]
        public async Task<string> UpdatePixivPicture(SavePixivPictureModel model)
        {
            string url = model.Url.Replace("i.pximg.net", "{0}").Replace("i.pixiv.cat", "{0}").Replace("pximg.net", "{0}").Replace("pixiv.cat", "{0}").Replace("-original", "-{1}").Replace("-master", "-{1}").Replace("_master1200", "{2}");
            if (!url.Contains("{2}"))
                url = url.Insert(url.LastIndexOf("."), "{2}");

            _logger.LogWarning($"更新pixiv记录, pid={model.Pid}, p={model.P} 开始");
            int i = await _sqlService.InsertUpdateDelete(
               $"BEGIN tran " +
               $"IF NOT EXISTS(SELECT 1 FROM [GreenOnionsGallery].[dbo].[PixivIllustrator] WHERE [Id] = {model.Uid}) " +
               $"  INSERT INTO [GreenOnionsGallery].[dbo].[PixivIllustrator] ([Id],[Illustrator],[Alias]) VALUES('{model.Uid}','{model.Illustrator}','{model.Alias}') " +
               $"ELSE " +
               $"  UPDATE [GreenOnionsGallery].[dbo].[PixivIllustrator] SET [Illustrator] = '{model.Illustrator}',[Alias] = '{model.Alias}' WHERE [Id] = {model.Uid} " +
               $"UPDATE [GreenOnionsGallery].[dbo].[PixivPictures] SET [PageCount]={model.PageCount},[Title]=N'{model.Title}',[Uid]={model.Uid},[Url]='{url}',[Grading]={model.Grading},[Width]={model.Width},[Height]={model.Height},[Tags]='{model.Tags.Replace("'","''")}' WHERE [Pid]={model.Pid} AND [P]={model.P} " +
               $"COMMIT tran");
            _logger.LogWarning($"更新pixiv记录, pid={model.Pid}, p={model.P} 完成, 是否成功={i > 0}");
            return JsonConvert.SerializeObject(new
            {
                result = true
            });
        }

        [HttpPost]
        public async Task<string> UpdateTwitterPicture(SaveTwitterPictureModel model)
        {
            _logger.LogWarning($"更新twitter记录, url={model.Url} 开始");

            if (!model.UserName.StartsWith("@"))
                model.UserName = "@" + model.UserName;

            int i = await _sqlService.InsertUpdateDelete(
                $"BEGIN tran " +
                $"IF NOT EXISTS(SELECT 1 FROM [GreenOnionsGallery].[dbo].[TwitterIllustrator] WHERE [Illustrator] = '{model.Illustrator}') " +
                $"  INSERT INTO [GreenOnionsGallery].[dbo].[TwitterIllustrator] ([Illustrator],[UserName],[Alias]) VALUES('{model.Illustrator}','{model.UserName}','{model.Alias}') " +
                $"ELSE " +
                $"  UPDATE [GreenOnionsGallery].[dbo].[TwitterIllustrator] SET [UserName] = '{model.UserName}', [Alias] = '{model.Alias}' WHERE [Illustrator] = '{model.Illustrator}' " +
                $"UPDATE [GreenOnionsGallery].[dbo].[TwitterPictures] SET [Source]='{model.Source}',[Title]=N'{model.Title}',[Uid]=(SELECT [Id] FROM [GreenOnionsGallery].[dbo].[TwitterIllustrator] WHERE [Illustrator]='{model.Illustrator}'),[Grading]={model.Grading},[Width]={model.Width},[Height]={model.Height},[Tags]='{model.Tags.Replace("'", "''")}' WHERE [Url]='{model.Url}' " +
                $"COMMIT tran");

            _logger.LogWarning($"更新twitter记录, url={model.Url} 完成, 是否成功={i > 0}");
            return JsonConvert.SerializeObject(new
            {
                result = true
            });
        }

        [HttpGet]
        public async Task<string> DeletePixivPicture(long pid, int p)
        {
            _logger.LogWarning($"删除pixiv记录, pid={pid},p={p} 开始");
            int i = await _sqlService.InsertUpdateDelete($"DELETE [GreenOnionsGallery].[dbo].[PixivPictures] WHERE [Pid] = '{pid}' AND [P] = {p}");
            _logger.LogWarning($"删除pixiv记录, pid={pid},p={p} 完成, 是否成功={i > 0}");
            return JsonConvert.SerializeObject(new
            {
                result = true
            });
        }

        [HttpPost]
        public async Task<string> DeleteTwitterPicture([FromBody]string url)
        {
            _logger.LogWarning($"删除twitter记录, url={url} 开始");
            int i = await _sqlService.InsertUpdateDelete($"DELETE [GreenOnionsGallery].[dbo].[TwitterPictures] WHERE [Url] = '{url}'");
            _logger.LogWarning($"删除twitter记录, url={url} 完成, 是否成功={i > 0}");
            return JsonConvert.SerializeObject(new
            {
                result = true
            });
        }
    }
}
