using System.Security.Cryptography;
using System.Text;

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
        private static readonly string[] NancyCategories = { "nancy", "nancyfx" };

        public IEnumerable<BlogPost> GetItems(int feedCount = 20, int pagenum = 0)
        {
            return File.ReadAllText("feeddata.json")
                .FromJson<IEnumerable<BlogMetadata>>()
                .SelectMany(GetBlogs)
                .SelectMany(GetBlogPosts)
                .Skip(feedCount * pagenum)
                .Take(feedCount)
                .OrderByDescending(x => x.PublishedDate);
        }

        public BlogPost GetItem(string link)
        {
            return new BlogPost();
        }

        private static IEnumerable<BlogPost> GetBlogPosts(Blog blog)
        {
            return blog.Feed.Items
                .Where(item => item.Categories.Any(IsNancyCategory))
                .Select(item => GetBlogPost(item, blog));
        }

        private static BlogPost GetBlogPost(SyndicationItem item, Blog blog)
        {
            var link = item.Links.FirstOrDefault(y => y.RelationshipType == "alternate");

            return new BlogPost
            {
                Title = item.Title.Text,
                Summary = GetSummary(item),
                Author = blog.Metadata.Author.Name,
                AuthorEmail = blog.Metadata.Author.Email,
                AuthorAvatarUrl = GetGravatarUrl(blog.Metadata.Author.Email),
                Localink = GetLocalLink(link),
                OriginalLink = GetOriginalLink(link),
                PublishedDate = item.PublishDate.DateTime,
                Content = GetContent(item)
            };
        }

        private static string GetSummary(SyndicationItem item)
        {
            if (item.Summary != null)
            {
                return item.Summary.Text;
            }

            var syndicationContent = item.Content as TextSyndicationContent;
            if (syndicationContent != null)
            {
                return syndicationContent.Text.TruncateHtml(350, string.Empty);
            }

            return string.Empty;
        }

        private static Uri GetGravatarUrl(string authorEmail)
        {
            return new Uri(string.Format("http://s.gravatar.com/{0}?s=128", GetMd5Hash(authorEmail)));
        }

        private static string GetMd5Hash(string authorEmail)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(authorEmail);
                var hash = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();

                foreach (var hexValue in hash.Select(x => x.ToString("X2")))
                {
                    sb.Append(hexValue);
                }

                return sb.ToString();
            }
        }

        private static string GetLocalLink(SyndicationLink link)
        {
            if (link == null)
            {
                return string.Empty;
            }

            var localLink = link.Uri.Segments.Last();
            if (localLink.Contains("."))
            {
                return link.Uri.Segments.Last()
                    .Substring(0, link.Uri.Segments.Last().IndexOf(".", StringComparison.Ordinal));
            }

            return localLink;
        }

        private static string GetOriginalLink(SyndicationLink link)
        {
            return link == null ? string.Empty : link.Uri.AbsoluteUri;
        }

        private static string GetContent(SyndicationItem item)
        {
            var encodedcontent = item.ElementExtensions
                .ReadElementExtensions<string>("encoded", "http://purl.org/rss/1.0/modules/content/");

            if (encodedcontent.Any())
            {
                return encodedcontent.FirstOrDefault();
            }

            var syndicationContent = item.Content as TextSyndicationContent;
            if (syndicationContent != null)
            {
                return syndicationContent.Text;
            }

            return string.Empty;
        }

        private static bool IsNancyCategory(SyndicationCategory category)
        {
            return NancyCategories.Any(x => x.Equals(category.Name, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<Blog> GetBlogs(BlogMetadata metadata)
        {
            using (var reader = XmlReader.Create(metadata.FeedUrl))
            {
                var feed = SyndicationFeed.Load(reader);
                if (feed != null)
                {
                    yield return new Blog(feed, metadata);
                }
            }
        }

        private class Blog
        {
            public Blog(SyndicationFeed feed, BlogMetadata metadata)
            {
                Feed = feed;
                Metadata = metadata;
            }

            public BlogMetadata Metadata { get; private set; }

            public SyndicationFeed Feed { get; private set; }
        }
    }
}