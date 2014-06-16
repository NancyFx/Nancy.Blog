namespace Nancy.Blog
{
    using Nancy;
    using Nancy.TinyIoc;
    using Services;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<IConfigSettings, ConfigSettings>();
            container.Register<IFeedService, CachedFeedService>();
            container.Register<IPostService, PostService>();
        }
    }
}
