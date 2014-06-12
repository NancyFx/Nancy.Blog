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
                var model = feedService.GetItems();
                foreach (var blogPost in model)
                {
                    blogPost.Content = null;
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
