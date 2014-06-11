namespace NancyBlog
{
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Xml;

    public class FeedService : IFeedService
    {
        private readonly string[] feedUrls;

        public FeedService(params string[] feedUrls)
        {
            this.feedUrls = feedUrls;
        }

        public IEnumerable<BlogPost> GetItems(int feedCount = 20, int pagenum = 0)
        {
            var syndicationFeeds = new List<SyndicationFeed>();

            foreach (var feedUrl in feedUrls)
            {
                var reader = XmlReader.Create(feedUrl);
                var feed = SyndicationFeed.Load(reader);

                reader.Close();
                if (feed != null)
                {
                    syndicationFeeds.Add(feed);
                }
            }

            var data = syndicationFeeds
                .SelectMany(feed => feed.Items)
                .Where(x => x.Categories.Any(y => y.Name.ToLower().Contains("nancy")))
                .Select(x => new BlogPost { Title = x.Title.Text, Summary = x.Summary.Text, PublishedDate = x.PublishDate.DateTime, Localink = x.Links.FirstOrDefault().Uri.PathAndQuery, OriginalLink = x.Links.FirstOrDefault().Uri.AbsoluteUri })
                .Skip(feedCount * pagenum)
                .Take(feedCount)
                .OrderByDescending(x => x.PublishedDate);

            return data;
        }

        public BlogPost GetItem(string title)
        {
            //Content = ((TextSyndicationContent)x.Content).Text,
            return new BlogPost();
        }
    }
}