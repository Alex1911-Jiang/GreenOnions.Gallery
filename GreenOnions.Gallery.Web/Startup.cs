using GreenOnions.Gallery.Common;
using GreenOnions.Gallery.Web.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GreenOnions.Gallery.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();

            services.AddControllersWithViews(
                options =>
                {
                    options.Filters.Add<GreenOnionsExceptionFilterAttribute>();//全局捕获异常
                });

            services.AddMvc(op =>
            {
                op.RespectBrowserAcceptHeader = true;
                op.InputFormatters.Add(new XmlSerializerInputFormatter(op));
                op.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            }).AddXmlSerializerFormatters().AddXmlDataContractSerializerFormatters();

            #region -- jwt校验 --
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,  //是否验证Issueer 签发人
                        ValidateAudience = true,  //是否验证Audience
                        ValidateLifetime = true,  //是否验证失效时间
                        ValidateIssuerSigningKey = true,  //是否验证SigningKey
                        ValidAudience = Configuration["audience"],  //Audience
                        ValidIssuer = Configuration["issuer"],  //Issuer
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"])),  //拿到SecurityKey
                    };
                });
            #endregion -- jwt校验 --
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            #region -- jwt --
            app.UseAuthentication();
            #endregion -- jwt --

            app.UseAuthorization();

            Configuration.ConsulRegist("GreenOnionsGallery");

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
