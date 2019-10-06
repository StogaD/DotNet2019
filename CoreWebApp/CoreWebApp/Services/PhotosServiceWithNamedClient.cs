using CoreWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreWebApp.Services
{
    public interface IPhotosService
    {
        Task<Photo> GetPhotoItem(int id);
        Task<IEnumerable<Photo>> GetPhotos();
    }
    public class PhotosServiceWithNamedClient : IPhotosService
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string _clientName = "jsonplaceholderClient";
        private const string _remoteServiceBaseUrl = "https://jsonplaceholder.typicode.com/photos";
        public PhotosServiceWithNamedClient(IHttpClientFactory httpClientFactory)
        {
            _clientFactory = httpClientFactory;
        }
        public async Task<Photo> GetPhotoItem(int page)
        {
            var httpClient = CreateHttpCLient();

            var ressponseString = await httpClient.GetStringAsync($"{_remoteServiceBaseUrl}/{page}");
            var category = JsonConvert.DeserializeObject<Photo>(ressponseString);

            return category;
        }

        public async Task<IEnumerable<Photo>> GetPhotos()
        {
            var httpClient = CreateHttpCLient();

            // create request explicitly
            var request = new HttpRequestMessage(HttpMethod.Get, "/photos");
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<Photo>>();
            }

            return null;
        }

        private HttpClient CreateHttpCLient()
        {
            var httpClient = _clientFactory.CreateClient(_clientName);

            httpClient.BaseAddress = new Uri(_remoteServiceBaseUrl);

            return httpClient;
        }
    }
}