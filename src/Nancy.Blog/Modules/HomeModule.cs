namespace Nancy.Blog.Modules
{
    using System;
    using Nancy;
    using Services;

    public class HomeModule : NancyModule
    {
        public HomeModule(IPostService postService, IFeedService feedService, IConfigSettings configSettings)
        {
            Get["/"] = _ =>
            {
                int currentPage;
                int pageSize;

                if (!int.TryParse(Request.Query.currentpage.ToString(), out currentPage))
                {
                    currentPage = 1;
                }

                if (!int.TryParse(Request.Query.pagesize.ToString(), out pageSize))
                {
                    pageSize = 10;
                }

                var viewmodel = postService.GetViewModel(pageSize, currentPage);

                return View["index", viewmodel];
            };

            Get["/{title}"] = parameters =>
            {
                var post = feedService.GetItem(parameters.title);
                if (post == null)
                {
                    return View["404"].WithStatusCode(404);
                }

                return View["post", post];
            };

            Get["/about"] = _ =>
            {
                var model = configSettings.GetAppSetting("nancycategories")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                ViewBag.NancyCategories = string.Join(" or ", model);

                return View["about"];
            };

            Get["/rss"] = x => Response.AsRSS(feedService.GetItems(), "Nancy Blog", "http://blog.nancyfx.org/", "rss");
        }
    }
}
