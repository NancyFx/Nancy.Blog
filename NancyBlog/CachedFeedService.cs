namespace NancyBlog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CachedFeedService : IFeedService
    {
        private readonly IFeedService feedService;
        private readonly int cacheMinutes;
        private IEnumerable<BlogPost> cachedItems = Enumerable.Empty<BlogPost>();
        private DateTime cacheDateTime;

        public CachedFeedService(IFeedService feedService, IConfigSettings configManager)
        {
            this.feedService = feedService;
            cacheMinutes = configManager.GetAppSetting<int>("cacheminutes");
        }

        public IEnumerable<BlogPost> GetItems(int feedCount = 20, int pagenum = 0)
        {
            if ((DateTime.Now - cacheDateTime).TotalMinutes > cacheMinutes)
            {
                cachedItems = this.feedService.GetItems(feedCount, pagenum);
                cacheDateTime = DateTime.Now;
            }
            return cachedItems;
        }

        public BlogPost GetItem(string link)
        {
            if (!cachedItems.Any())
            {
                GetItems();
            }

            return cachedItems.FirstOrDefault(x => x.Localink.Trim('/') == link);
        }
    }
}