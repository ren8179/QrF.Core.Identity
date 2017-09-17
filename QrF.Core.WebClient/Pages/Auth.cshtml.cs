using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityModel.Client;
using System.Net.Http;

namespace QrF.Core.WebClient.Pages
{
    public class AuthModel : PageModel
    {
        public string TokenError { get; set; }
        public string TokenJson { get; set; }
        public string ApiError { get; set; }
        public string ApiContent { get; set; }
        public async Task OnGetAsync()
        {
            // 从元数据中发现客户端
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            // 请求令牌
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1");

            if (tokenResponse.IsError)
            {
                TokenError = tokenResponse.Error;
            }
            TokenJson = tokenResponse.Json.ToString();

            // 调用api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                ApiError = response.StatusCode.ToString();
            }
            else
            {
                ApiContent = await response.Content.ReadAsStringAsync();
            }
        }
    }
}