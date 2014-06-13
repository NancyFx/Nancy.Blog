namespace NancyBlog
{
    using System.Linq;
    using Nancy;

    public class HomeModule : NancyModule
    {
        public HomeModule(IFeedService feedService)
        {
            Get["/"] = _ =>
            {
                int currentPage;
                int pageSize;

                if (!int.TryParse(Request.Query.currentpage.ToString(), out currentPage))
                {
                    currentPage = 0;
                }

                if (!int.TryParse(Request.Query.pagesize.ToString(), out pageSize))
                {
                    pageSize = 20;
                }
                var viewmodel = new IndexViewModel();
                var posts = feedService.GetItems(pageSize, currentPage);
                foreach (var blogPost in posts)
                {
                    blogPost.Content = null; //Prevent serialization issues
                }
                viewmodel.Posts = posts;

                return View["index", viewmodel];
            };

            Get["/{title}"] = parameters =>
            {
                var post = feedService.GetItem(parameters.title);
                if (post == null)
                {
                    return View["404"];
                }

                return View["post", post];
            };

            Get["/write-for-us"] = _ => View["writeforus"];
        }
    }
}
