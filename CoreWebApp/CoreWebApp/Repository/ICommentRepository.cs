using CoreWebApp.Models;
using Microsoft.AspNetCore.Http;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApp.Repository
{
    //[Header("User-Agent", "Demo-RestEase")]
    //[Header("Content-Type", "application/json")]

    public class SearchParams
    {
        public string Term { get; set; }
        public string Mode { get; set; }
    }

    public interface ICommentRepository
    {
        [Get("/comments/{id}")]
        Task<Comment> GetComment([Path] int id);
        [Get("/comments/{id}")]
        Task<Comment> GetCommentV2([Path(Format="X2")] int id);

        //Serialization of Variable Query Parameters
        [Get("/comments/search")]
        Task<Comment> SearchComment([Query(QuerySerializationMethod.Serialized)] SearchParams param);
        // in service: await api.SearchAsync(new SearchParams() { Term = "foo", Mode = "basic" });

        //Query Parameters Map
        [Get("/comments/searchBlog")]
        Task<Comment> SearchBlogPostsAsync([QueryMap] IDictionary<string, string[]> filters);
        /*in service: 
            var filters = new Dictionary<string, string[]>()
            {
                { "title", new[] { "bobby" } },
                { "tag", new[] { "c#", "programming" } }
            };

            Requests http://api.example.com/search?title=bobby&tag=c%23&tag=programming
            var searchResults = await api.SearchBlogPostsAsync(filters);
            */


        //Raw Query String Parameters
        [Get("searchbyQueryString")]
        Task<Comment> SearchAsync([RawQueryString] string customFilter);

        //var searchResults = await api.SearchAsync("id=2");



    }
}
