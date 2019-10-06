using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApp.Repository
{
    public interface IPostRepository
    {
        [Get("/posts/{id}")]
        Task<Post> GetPost([Path] int id);

        [Get("/posts")]
        Task<IEnumerable<Post>> GetPosts();
    }
}
