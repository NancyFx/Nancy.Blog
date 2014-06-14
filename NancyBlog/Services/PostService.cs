namespace NancyBlog.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Model;

    public class PostService : IPostService
    {
        private readonly IFeedService feedService;

        public PostService(IFeedService feedService)
        {
            this.feedService = feedService;
        }

        public IndexViewModel GetViewModel(int feedCount, int pagenum)
        {
            var viewmodel = new IndexViewModel();
            var posts = feedService.GetItems(feedCount, pagenum);

            foreach (var blogPost in posts)
            {
                blogPost.Content = null; //Prevent serialization issues
            }
            viewmodel.Posts = GetPosts(posts, feedCount, pagenum);
            var totalPages = (int)Math.Ceiling((double)posts.Count() / feedCount);
            viewmodel.HasNextPage = pagenum < totalPages;
            viewmodel.HasPreviousPage = pagenum > 1 && totalPages > 1;
            viewmodel.NextPage = pagenum + 1;
            viewmodel.PreviousPage = pagenum - 1;

            return viewmodel;
        }

        private IEnumerable<BlogPost> GetPosts(IEnumerable<BlogPost> posts, int feedCount, int pagenum)
        {
            return posts.Skip(feedCount * (pagenum - 1))
                .Take(feedCount);
        }
    }
}
