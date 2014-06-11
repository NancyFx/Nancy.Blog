namespace NancyBlog
{
    using Nancy;

    public class HomeModule : NancyModule
    {
        public HomeModule(IFeedService feedService)
        {
            Get["/"] = _  => Response.AsJson(feedService.GetItems());
        }
    }
}
