namespace NancyBlog
{
    using System;
    using Nancy;
    using Nancy.TinyIoc;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            container.Register<IConfigSettings, ConfigSettings>();

            var feedService = new FeedService();

            var cachedService = new CachedFeedService(feedService, container.Resolve<IConfigSettings>());

            container.Register<IFeedService>(cachedService);

        }
    }
}
