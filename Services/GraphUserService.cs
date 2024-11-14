using App_Azure_OpenId.Models;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace App_Azure_OpenId.Services
{
    public class GraphUserService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tenantId;
        private readonly string _domain;
        private readonly HttpClient _httpClient;

        public GraphUserService(string clientId, string clientSecret, string tenantId, string domain)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _tenantId = tenantId;
            _domain = domain;  // Por ejemplo: "tudominio.onmicrosoft.com"
            _httpClient = new HttpClient();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var app = ConfidentialClientApplicationBuilder
                .Create(_clientId)
                .WithClientSecret(_clientSecret)
                .WithAuthority($"https://login.microsoftonline.com/{_tenantId}")
                .Build();

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            try
            {
                var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                return result.AccessToken;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el token de acceso", ex);
            }
        }

        public async Task<string> CreateUserAsync(string DisplayName, string Email, string Password)
        {
            var accessToken = await GetAccessTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            string username = Email.Split('@')[0];
            string userPrincipalName = $"{username}@{_domain}";

            var userJson = JsonSerializer.Serialize(new
            {
                accountEnabled = true,
                displayName = DisplayName,
                mailNickname = username,
                userPrincipalName = userPrincipalName,
                mail = Email,
                otherMails = new[] { Email },
                identities = new[]
                {
                    new
                    {
                        signInType = "emailAddress",
                        issuer = _domain,
                        issuerAssignedId = Email
                    }
                },
                passwordProfile = new
                {
                    forceChangePasswordNextSignIn = true,
                    password = Password
                }
            });

            var content = new StringContent(userJson, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "https://graph.microsoft.com/v1.0/users",
                content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear usuario: {errorContent}");
            }

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task UpdateUserAsync(UpdateUserViewModel userUpdateModel)
        {
            var accessToken = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var userData = new
            {
                displayName = userUpdateModel.DisplayName,
                givenName = userUpdateModel.GivenName,
                //surname = userUpdateModel.Surname,
                jobTitle = userUpdateModel.JobTitle
            };

            var jsonData = JsonSerializer.Serialize(userData);
            var url = $"https://graph.microsoft.com/v1.0/users/{userUpdateModel.Id}";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await httpClient.PatchAsync(url, new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }


    }
}
