using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        public async Task<string> GetUploadUrl(BeverageEnum beverage)
        {
            var endPoint = $"/api/UploadUrls/{((int)beverage)}";
            var response = await _httpClient.GetStringAsync(endPoint);
            return response;
        }

        public async Task UploadImage(Mat image, string uploadUrl)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    // Convert Mat to Image<Bgr, byte>
                    using (var img = image.ToImage<Bgr, byte>()) // Using Bgr, byte which is common for colored images
                    {
                        // Convert image to JPEG byte array and write to MemoryStream
                        var bytes = img.ToJpegData();
                        memoryStream.Write(bytes, 0, bytes.Length);
                        memoryStream.Position = 0; // Reset the position of the MemoryStream to the beginning after writing
                    }

                    var content = new StreamContent(memoryStream);
                    content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    content.Headers.Add("x-amz-acl", "public-read");
                    var response = await _httpClient.PutAsync(uploadUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to upload image: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception)
            {
            }
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
