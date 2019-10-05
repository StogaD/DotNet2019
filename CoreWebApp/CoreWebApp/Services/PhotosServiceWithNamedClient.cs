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

        public PhotosServiceWithNamedClient(IHttpClientFactory httpClientFactory)
        {
            _clientFactory = httpClientFactory;
        }
        public async Task<Photo> GetPhotoItem(int page)
        {
            string _remoteServiceBaseUrl = "https://jsonplaceholder.typicode.com/photos";

            var httpClient = _clientFactory.CreateClient("photos");


            var ressponseString = await httpClient.GetStringAsync($"{_remoteServiceBaseUrl}/{page}");
            var category = JsonConvert.DeserializeObject<Photo>(ressponseString);

            return category;
        }

        public async Task<IEnumerable<Photo>> GetPhotos()
        {
            var httpClient = _clientFactory.CreateClient("photos");

            // create request explicitly
            var request = new HttpRequestMessage(HttpMethod.Get, "/phptos");
            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<Photo>>();
            }

            return null;
        }
    }
}