namespace Nancy.Blog.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel.Syndication;
    using System.Xml;
    using Model;
    using ServiceStack;


    public class CachedFeedService : IFeedService
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly int cacheMinutes;
        private IEnumerable<BlogPost> cachedItems = Enumerable.Empty<BlogPost>();
        private DateTime cacheDateTime;
        private readonly IEnumerable<string> nancyCategories;

        public CachedFeedService(IConfigSettings configSettings, IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
            cacheMinutes = configSettings.GetAppSetting<int>("cacheminutes");
            nancyCategories = configSettings.GetAppSetting("nancycategories")
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public IEnumerable<BlogPost> GetItems(int feedCount = 10, int pagenum = 0)
        {
            if ((DateTime.Now - cacheDateTime).TotalMinutes > cacheMinutes)
            {
                cachedItems = this.GetItemsForCache(feedCount, pagenum);
                cacheDateTime = DateTime.Now;
            }
            return cachedItems;
        }

        private IEnumerable<BlogPost> GetItemsForCache(int feedCount, int pagenum)
        {
            string json = File.ReadAllText(rootPathProvider.GetRootPath() + "feeddata.json");
            var metadataEntries = json.FromJson<MetaData[]>()
                                      .Where(m => m != null)
                                      .ToArray();

            var syndicationFeeds = GetSyndicationFeeds(metadataEntries);

            var data = syndicationFeeds
                .SelectMany(pair => pair.Value.Items, (pair, item) => new { Id = pair.Key, Item = item })
                .Where(x => x.Item.Categories.Any(FeedCategoryInNancyCategories))
                .Select(x =>
                {
                    var metaauthor = metadataEntries.First(y => y.Id == x.Id);
                    var authorname = metaauthor.Author;
                    var authoremail = metaauthor.AuthorEmail;

                    var link = x.Item.Links.FirstOrDefault(y => y.RelationshipType == "alternate");
                    var locallink = string.Empty;
                    if (link != null)
                    {
                        locallink = link.Uri.Segments.Last();
                        if (locallink.Contains("."))
                        {
                            locallink = locallink.Substring(0, locallink.IndexOf(".", System.StringComparison.Ordinal));
                        }
                    }

                    var originallink = link == null ? string.Empty : link.Uri.AbsoluteUri;

                    var summary = x.Item.Summary == null
                        ? ((TextSyndicationContent)x.Item.Content).Text.TruncateHtml(700, "")
                        : x.Item.Summary.Text;

                    var encodedcontent = x.Item.ElementExtensions.ReadElementExtensions<string>("encoded",
                        "http://purl.org/rss/1.0/modules/content/");

                    var content = string.Empty;

                    if (encodedcontent.Any())
                    {
                        content = encodedcontent.First();
                    }
                    else if (x.Item.Content != null)
                    {
                        content = ((TextSyndicationContent)x.Item.Content).Text;
                    }
                    else
                    {
                        content = summary;
                    }

                    return new BlogPost
                    {
                        Title = x.Item.Title.Text,
                        Summary = summary,
                        Author = authorname,
                        AuthorEmail = authoremail,
                        Localink = locallink,
                        OriginalLink = originallink,
                        PublishedDate = x.Item.PublishDate.DateTime,
                        Content = content
                    };

                })
                .OrderByDescending(x => x.PublishedDate);


            return data;
        }

        private bool FeedCategoryInNancyCategories(SyndicationCategory category)
        {
            return nancyCategories.Any(x => x.Equals(category.Name, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<KeyValuePair<string, SyndicationFeed>> GetSyndicationFeeds(IEnumerable<MetaData> metadataEntries)
        {
            var syndicationFeeds = new List<KeyValuePair<string, SyndicationFeed>>();
            foreach (var metadata in metadataEntries)
            {
                GetFeed(metadata.FeedUrl, metadata.Id, syndicationFeeds);
            }

            return syndicationFeeds;
        }

        private static void GetFeed(string url, string id, List<KeyValuePair<string, SyndicationFeed>> syndicationFeeds)
        {
            try
            {
                SyndicationFeed feed = null;
                using(var reader = XmlReader.Create(url))
                {
                  feed = SyndicationFeed.Load(reader);
                }

                if (feed != null)
                {
                    syndicationFeeds.Add(new KeyValuePair<string, SyndicationFeed>(id, feed));
                    if (feed.Links.Any(x => x.RelationshipType == "next"))
                    {
                        foreach (var pagingLink in feed.Links.Where(x => x.RelationshipType == "next"))
                        {
                            GetFeed(pagingLink.Uri.AbsoluteUri, id, syndicationFeeds);
                        }
                    }
                }
            }
            catch (WebException exception)
            {
                //Unable to load RSS feed
            }
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
