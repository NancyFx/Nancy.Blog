namespace NancyBlog
{
    using Nancy;

    public class HomeModule : NancyModule
    {
        public HomeModule(IFeedService feedService)
        {
            Get["/"] = _ => View["index", feedService.GetItems()];
        }
    }
}
