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

                var model = feedService.GetItems(pageSize, currentPage);
                foreach (var blogPost in model)
                {
                    blogPost.Content = null; //Prevent serialization issues
                }
                return View["index", model];
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
        }
    }
}
