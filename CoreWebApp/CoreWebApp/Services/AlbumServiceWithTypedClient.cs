﻿using CoreWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreWebApp.Services
{
    public interface IAlbumService
    {
        Task<Album> GetAlbumItem(int id);
        Task<IEnumerable<Album>> GetAlbums();
    }
    public class AlbumServiceWithTypedClient : IAlbumService
    {
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl = "https://jsonplaceholder.typicode.com/albums";
        public AlbumServiceWithTypedClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<Album> GetCatalogItems(int id)
        {
            var ressponseString = await _httpClient.GetStringAsync($"{_remoteServiceBaseUrl}/{id}");
            var category = JsonConvert.DeserializeObject<Album>(ressponseString);

            return category;
        }

        public async Task<IEnumerable<Album>> GetAlbums()
        {
            var ressponseString = await _httpClient.GetStringAsync(_remoteServiceBaseUrl);
            var albums = JsonConvert.DeserializeObject<IEnumerable<Album>>(ressponseString);

            return albums;
        }

        public async Task<Album> GetAlbumItem(int id)
        {
            var url = $"{_remoteServiceBaseUrl}/{id}";
            var ressponseString = await _httpClient.GetStringAsync(url);
            var albums = JsonConvert.DeserializeObject<Album>(ressponseString);

            return albums;
        }
    }
}