using GreenOnions.Gallery.AuthenticationCenter.Utility;
using GreenOnions.Gallery.Common;
using GreenOnions.Gallery.Interface;
using GreenOnions.Gallery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GreenOnions.Gallery.AuthenticationCenter.Controllers
{
    [Route("auth/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IJWTService _iJWTService;
        private readonly ISqlService _sqlService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(ILogger<AuthenticationController> logger, IJWTService iJWTService, ISqlService sqlService, IConfiguration configuration)
        {
            _logger = logger;
            _iJWTService = iJWTService;
            _sqlService = sqlService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<string> Login([FromBody] LoginModel login)
        {
            _logger.LogInformation("有一个dalao尝试登录");
            DataSet ds = await _sqlService.Select($"SELECT [Account],[NickName],[Email],[Permission],[ApiKey] FROM [GreenOnionsGallery].[dbo].[Users] WHERE ([Account] = '{login.Account}' OR [Email] = '{login.Account}') AND [Password] = '{login.Password}' ");
            
            if (ds.Tables[0].Rows.Count == 1)  //校验账号密码通过
            {
                DataRow row = ds.Tables[0].Rows[0];
                string token = _iJWTService.GetToken(HttpContext,row["Account"].ToString(), row["NickName"].ToString(), row["Permission"].ToString(), row["ApiKey"].ToString(), row["Email"].ToString());

                return JsonConvert.SerializeObject(new
                {
                    result = true,
                    message= "",
                    token
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    result = false,
                    message = "用户名或密码错误",
                    token = ""
                });
            }
        }

        [HttpPost]
        public async Task<string> RebuildJWT([FromBody] string account)
        {
            _logger.LogInformation("有一个dalao尝试登录");
            DataSet ds = await _sqlService.Select($"SELECT [Account],[NickName],[Email],[Permission],[ApiKey] FROM [GreenOnionsGallery].[dbo].[Users] WHERE ([Account] = '{account}')");

            if (ds.Tables[0].Rows.Count == 1)
            {
                DataRow row = ds.Tables[0].Rows[0];
                string token = _iJWTService.GetToken(HttpContext, row["Account"].ToString(), row["NickName"].ToString(), row["Permission"].ToString(), row["ApiKey"].ToString(), row["Email"].ToString());

                return JsonConvert.SerializeObject(new
                {
                    result = true,
                    message = "",
                    token
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    result = false,
                    message = "生成Token失败，请重新登录。",
                    token = ""
                });
            }
        }

        [HttpPost]
        public async Task<string> CheckAccount([FromBody] string account)
        {
            _logger.LogInformation("有一个dalao尝试创建账号");
            DataSet ds = await _sqlService.Select($"SELECT [Account],[NickName],[Email],[Permission],[ApiKey] FROM [GreenOnionsGallery].[dbo].[Users] WHERE [Account] = '{account}'");

            if (ds.Tables[0].Rows.Count > 0)  //已经存在相同的账号或邮箱
            {
                return JsonConvert.SerializeObject(new
                {
                    result = false,
                    message = "该用户名已存在！"
                });
            }
            return JsonConvert.SerializeObject(new
            {
                result = true,
                message = "恭喜，此用户名可以注册。"
            });
        }

        [HttpPost]
        public async Task<string> CheckEmail([FromBody] string email)
        {
            _logger.LogInformation("有一个dalao尝试创建账号");
            DataSet ds = await _sqlService.Select($"SELECT [Account],[NickName],[Email],[Permission],[ApiKey] FROM [GreenOnionsGallery].[dbo].[Users] WHERE [Email] = '{email}'");

            if (ds.Tables[0].Rows.Count > 0)  //已经存在相同的账号或邮箱
            {
                return JsonConvert.SerializeObject(new
                {
                    result = false,
                    message = "该邮箱已存在！"
                });
            }
            return JsonConvert.SerializeObject(new
            {
                result = true,
                message = "恭喜，此邮箱可以注册。"
            });
        }

        [HttpPost]
        public async Task<string> CreateVerify([FromBody] string email)
        {
            _logger.LogInformation("有一个dalao申请邮箱验证码");

            string emailVerify = VerifyCodeHelper.CreatgeVerifyCodeString(4);
                await _sqlService.InsertUpdateDelete(
                    $"IF NOT EXISTS(SELECT 1 FROM [GreenOnionsGallery].[dbo].[RegisterVerify] WHERE [Email] = '{email}') " +
                    $"   INSERT INTO [GreenOnionsGallery].[dbo].[RegisterVerify]([Email],[EmailVerifyCode],[CreateTime]) VALUES ('{email}', '{emailVerify}', GETDATE()) " +
                    $"ELSE" +
                    $"   UPDATE [GreenOnionsGallery].[dbo].[RegisterVerify] SET [EmailVerifyCode] = '{emailVerify}',[CreateTime]= GETDATE() WHERE [Email] = '{email}'");
            
            SendVerifyMail(email, emailVerify);

            //发送邮件
            return JsonConvert.SerializeObject(new
            {
                result = true,
                message = "验证码已生成。"
            });
        }

        private void SendVerifyMail(string mailTo, string mailVerify)
        {
            SmtpClient client = new ("smtp.163.com");
            MailAddress from = new (_configuration["EmailAccount"], "葱葱图库", Encoding.UTF8);
            MailAddress to = new (mailTo, mailTo, Encoding.UTF8);
            MailMessage message = new (from, to);
            message.Subject = "葱葱图库验证信息";
            message.SubjectEncoding = Encoding.UTF8;
            message.Body = $"您正在注册葱葱图库, 验证码为:{mailVerify} , 24小时内有效";
            message.BodyEncoding = Encoding.UTF8;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = false;
            client.EnableSsl = false;
            client.UseDefaultCredentials = false;
            NetworkCredential myCredentials = new(_configuration["EmailAccount"], _configuration["EmailPassword"]);
            client.Credentials = myCredentials;
            client.Send(message);
        }

        [HttpPost]
        public async Task<string> Register([FromBody]RegisterModel register)
        {
            DataSet ds = await _sqlService.Select($"SELECT [EmailVerifyCode] FROM [GreenOnionsGallery].[dbo].[RegisterVerify] WHERE [Email] = '{register.Email}'");
            if (ds.Tables[0].Rows.Count == 1)
            {
                string verify = ds.Tables[0].Rows[0]["EmailVerifyCode"].ToString();
                if (!string.IsNullOrWhiteSpace(verify) && !string.IsNullOrWhiteSpace(register.EmailVerifyCode) && verify == register.EmailVerifyCode)
                {
                    int i = await _sqlService.InsertUpdateDelete($"INSERT INTO [GreenOnionsGallery].[dbo].[Users] ([Account],[Password],[NickName],[Email],[Permission],[LastLoginTime],[CreateTime]) VALUES ('{register.Account}','{register.Password}','{register.NickName}','{register.Email}',1,GETDATE(),GETDATE())");

                    if (i > 0)
                    {
                        string token = _iJWTService.GetToken(HttpContext, register.Account, register.NickName, "1", "", register.Email); //返回token以自动登录

                        return JsonConvert.SerializeObject(new
                        {
                            result = true,
                            message = "注册成功",
                            token
                        });
                    }
                    return JsonConvert.SerializeObject(new
                    {
                        result = false,
                        message = "注册失败，写入数据库失败。"
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(new
                    {
                        result = false,
                        message = "邮箱验证码不正确！",
                    });
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new
                {
                    result = false,
                    message = "找不到邮箱验证码，请重新发送！",
                });
            }
        }

        [HttpPost]
        public void ResetNickName(string nickName)
        {
        }
    }
}
