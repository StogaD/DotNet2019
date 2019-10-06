using CoreWebApp.Models;
using CoreWebApp.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreWebApp.Services
{
    public interface ICommentService
    {
        Task<Comment> GetCommentItem(int id);
        Task<IEnumerable<Comment>> GetComments();
    }
    public class CommentServiceWithRestEase : ICommentService
    {
        private readonly HttpClient _httpClient;
        private readonly ICommentRepository _commentRepo;
        private readonly string _remoteServiceBaseUrl = "https://jsonplaceholder.typicode.com/Comments";
        private readonly string _mockedBaseUrl = "https://testpolly.free.beeceptor.com/Comments";
        public CommentServiceWithRestEase(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }
  
        public async Task<IEnumerable<Comment>> GetComments()
        {
            var ressponseString = await _httpClient.GetStringAsync(_mockedBaseUrl);
            var Comments = JsonConvert.DeserializeObject<IEnumerable<Comment>>(ressponseString);

            return Comments;
        }

        public async Task<Comment> GetCommentItem(int id)
        {
            var comment = await _commentRepo.GetComment(id);

            return comment;
        }
    }
}
