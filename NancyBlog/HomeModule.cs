namespace NancyBlog
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using Nancy;

    public class HomeModule : NancyModule
    {
        public HomeModule(IFeedService feedService)
        {
            Get["/"] = _  => Response.AsJson(feedService.GetItems());
        }
    }
}
