using CoreWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreWebApp.Services
{
    public interface IUserService
    {
        Task<User> GetUser(int id);
        Task<IEnumerable<User>> GetUsers();
    }
    public class UserServiceWithBasicClientUsage : IUserService
    {
        private readonly IHttpClientFactory _clientFactory;

        private readonly string _remoteServiceBaseUrl = "https://jsonplaceholder.typicode.com/photos";
        public UserServiceWithBasicClientUsage(IHttpClientFactory httpClientFactory)
        {
            _clientFactory = httpClientFactory;
        }
        public async Task<User> GetUser(int page)
        {

            var httpClient = _clientFactory.CreateClient();


            var ressponseString = await httpClient.GetStringAsync($"{_remoteServiceBaseUrl}/{page}");
            var category = JsonConvert.DeserializeObject<User>(ressponseString);

            return category;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var httpClient = _clientFactory.CreateClient();
            var ressponseString = await httpClient.GetStringAsync(_remoteServiceBaseUrl);
            var category = JsonConvert.DeserializeObject<IEnumerable<User>>(ressponseString);

            return category;
        }
    }
}