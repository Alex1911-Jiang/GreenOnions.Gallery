using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GreenOnions.Gallery.AuthenticationCenter.Utility
{
    public interface IJWTService
    {
        string GetToken(HttpContext context, string account, string nickName, string permission, string apiKey, string email);
    }

    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken(HttpContext context, string account, string nickName, string permission, string apiKey, string email)
        {
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, account),
                new Claim("Account",account),
                new Claim("NickName",nickName),
                new Claim("Permission",permission),
                new Claim("ApiKey",apiKey),
                new Claim("Email",email)
            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(issuer: _configuration["issuer"],
                audience: _configuration["audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);

            context.Response.Headers.Add("Authorization", "bearer " + returnToken);

            return returnToken;
        }
    }
}
