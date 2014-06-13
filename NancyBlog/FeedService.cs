namespace NancyBlog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Xml;
    using ServiceStack.Text;

    public class FeedService : IFeedService
    {
        public IEnumerable<BlogPost> GetItems(int feedCount = 20, int pagenum = 0)
        {
            string json = File.ReadAllText("feeddata.json");
            var metadataEntries = json.FromJson<MetaData[]>();

            var syndicationFeeds = new Dictionary<string, SyndicationFeed>();

            foreach (var metadata in metadataEntries)
            {
                var reader = XmlReader.Create(metadata.FeedUrl);
                var feed = SyndicationFeed.Load(reader);

                reader.Close();
                if (feed != null)
                {
                    syndicationFeeds.Add(metadata.Id, feed);
                }
            }

            var data = syndicationFeeds
                .SelectMany(pair => pair.Value.Items, (pair, item) => new { Id = pair.Key, Item = item })
                .Where(x => x.Item.Categories.Any(y => y.Name.ToLower() == "nancy" || y.Name.ToLower() == "nancyfx"))
                .Select(x =>
                {
                    var metaauthor = metadataEntries.FirstOrDefault(y => y.Id == x.Id);
                    var authorname = metaauthor.Author;
                    var authoremail = metaauthor.AuthorEmail;

                    var link = x.Item.Links.FirstOrDefault(y=>y.RelationshipType == "alternate");
                    var locallink = string.Empty;
                    if (link != null)
                    {
                        locallink = link.Uri.Segments.LastOrDefault();
                        if (locallink.Contains("."))
                        {
                            locallink = link.Uri.Segments.LastOrDefault()
                                        .Substring(0,link.Uri.Segments.LastOrDefault().IndexOf(".", System.StringComparison.Ordinal));
                        }
                    }

                    var originallink = link == null ? string.Empty : link.Uri.AbsoluteUri;

                    var summary = x.Item.Summary == null
                        ? ((TextSyndicationContent)x.Item.Content).Text.TruncateHtml(700,"")
                        : x.Item.Summary.Text;

                    var encodedcontent = x.Item.ElementExtensions.ReadElementExtensions<string>("encoded",
                        "http://purl.org/rss/1.0/modules/content/");

                    var content = encodedcontent.Any()
                        ? encodedcontent.FirstOrDefault()
                        : ((TextSyndicationContent) x.Item.Content).Text;

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
                .Skip(feedCount * pagenum)
                .Take(feedCount)
                .OrderByDescending(x => x.PublishedDate)
                ;

            return data;
        }

        public BlogPost GetItem(string link)
        {
            return new BlogPost();
        }
    }
}