using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachineImageRecognition
{
    public class CoffeeMachineApiClient
    {
        private readonly HttpClient _httpClient;

        public CoffeeMachineApiClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task Update(BeverageEnum beverageEnum)
        {
            string endPoint = "/api/Gb/";
            var body = GetHttpContent(new UpdateGbCommand() { Beverage = beverageEnum });
            await _httpClient.PutAsync(endPoint,body);
        }

        private HttpContent GetHttpContent(object data)
        {
            return JsonContent.Create(data);
        }
    }

    public class UpdateGbCommand
    {
        public BeverageEnum Beverage { get; set; }
    }
}
