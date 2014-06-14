namespace NancyBlog.Modules
{
    using System;
    using System.Linq;
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
                    return View["404"];
                }

                return View["post", post];
            };

            Get["/write-for-us"] = _ =>
            {
                var model = configSettings.GetAppSetting("nancycategories")
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                ViewBag.NancyCategories = string.Join(" or ", model);

                return View["writeforus"];
            };
        }
    }
}
