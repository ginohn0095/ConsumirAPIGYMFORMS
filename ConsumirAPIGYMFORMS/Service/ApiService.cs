using ConsumirAPIGYMFORMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsumirAPIGYMFORMS.Service
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string _token = string.Empty;


        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://10.0.0.148:5042/");
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            var loginData = new LoginRequest { Username = username, Password = password };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using var client = new HttpClient(handler);

            // Asegúrate de usar la URL completa
            // var response = await client.PostAsync("https://localhost:5042/api/auth/login", content);
            var response = await client.PostAsync("https://10.0.0.148:5042/api/auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var auth = JsonSerializer.Deserialize<AuthResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _token = auth?.Token ?? string.Empty;

                return !string.IsNullOrEmpty(_token);
            }

            return false;
        }

        public async Task<List<Equipo>> GetPokemonsAsync()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using var client = new HttpClient(handler);
            //client.BaseAddress = new Uri("https://localhost:5042/");
            client.BaseAddress = new Uri("https://10.0.0.148:5042/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.GetAsync("api/equipos");

            if (!response.IsSuccessStatusCode)
                return new List<Equipo>();

            var content = await response.Content.ReadAsStringAsync();
            var pokemons = JsonSerializer.Deserialize<List<Equipo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return pokemons ?? new List<Equipo>();
        }


    }
}
