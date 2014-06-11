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

            var feedSetting = System.Configuration.ConfigurationManager.AppSettings.Get("blogfeeds");
            var feedurls = feedSetting.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var feedService = new FeedService(feedurls);

            var cachedService = new CachedFeedService(feedService, container.Resolve<IConfigSettings>());

            container.Register<IFeedService>(cachedService);

        }
    }
}
