namespace Nancy.Blog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.ServiceModel.Syndication;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Nancy;
    using Nancy.Blog.Model;
    using Nancy.IO;

    public class RssResponse : Response
    {
        private const string UrlRegex = @"(?<=<(a|img)\s+[^>]*?(href|src)=(?<q>['""]))(?!https?://)(?<url>/?.+?)(?=\k<q>)";
        private readonly string siteUrl;
        private readonly string feedfileName;
        private string RssTitle { get; set; }

        public RssResponse(IEnumerable<BlogPost> model, string rssTitle, string siteUrl, string feedfileName)
        {
            this.siteUrl = siteUrl;
            this.feedfileName = feedfileName;

            RssTitle = rssTitle;
            Contents = GetXmlContents(model);
            ContentType = "application/rss+xml";
            StatusCode = HttpStatusCode.OK;
        }

        private Action<Stream> GetXmlContents(IEnumerable<BlogPost> model)
        {
            var items = new List<SyndicationItem>();

            foreach (var post in model)
            {
                // Replace all relative urls with full urls.
                var contentHtml = Regex.Replace(post.Content, UrlRegex, m => siteUrl.TrimEnd('/') + "/" + m.Value.TrimStart('/'));
                var excerptHtml = Regex.Replace(post.Summary, UrlRegex, m => siteUrl.TrimEnd('/') + "/" + m.Value.TrimStart('/'));

                var item = new SyndicationItem(
                    post.Title,
                    contentHtml,
                    new Uri(siteUrl + post.Localink)
                    )
                {
                    Id = siteUrl + post.Localink,
                    LastUpdatedTime = post.PublishedDate.ToUniversalTime(),
                    PublishDate = post.PublishedDate.ToUniversalTime(),
                    Content = new TextSyndicationContent(contentHtml, TextSyndicationContentKind.Html),
                    Summary = new TextSyndicationContent(excerptHtml, TextSyndicationContentKind.Html),

                };
                item.Authors.Add(new SyndicationPerson(post.AuthorEmail, post.Author, string.Empty));
                item.Categories.Add(new SyndicationCategory("nancy"));

                items.Add(item);
            }

            var feed = new SyndicationFeed(
                RssTitle,
                RssTitle, /* Using Title also as Description */
                new Uri(siteUrl  + feedfileName),
                items);

            var formatter = new Rss20FeedFormatter(feed);

            return stream =>
            {
                var encoding = new UTF8Encoding(false);
                var streamWrapper = new UnclosableStreamWrapper(stream);

                using (var writer = new XmlTextWriter(streamWrapper, encoding))
                {
                    formatter.WriteTo(writer);
                }
            };
        }
    }
}