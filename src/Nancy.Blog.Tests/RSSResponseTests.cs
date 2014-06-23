namespace NancyBlog.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Xml;
    using Nancy;
    using Nancy.Blog;
    using Nancy.Blog.Model;
    using Nancy.Testing;
    using Xunit;

    public class RssResponseTests
    {
        [Fact]
        public void Response_Should_Be_RSS_Content_Type_And_OK_Status()
        {
            //Given & When
            var rssResponse = GetResponse();

            //Then
            Assert.Equal("application/rss+xml", rssResponse.ContentType);
            Assert.Equal(HttpStatusCode.OK,rssResponse.StatusCode);
        }

        [Fact]
        public void Response_Should_Make_BlogPost_Content_And_Summary_RelativeURLs_Full_Paths()
        {
            //Given & When
            var rssResponse = GetResponse();

            var feed = GetFeedFromResponse(rssResponse);

            //Then
            Assert.Equal("<p><a href=\"http://blog.nancyfx.org/bob\">Bob</a></p>", ((TextSyndicationContent)feed.Items.First().Content).Text);
            Assert.Equal("<p>Read <a href=\"http://blog.nancyfx.org/blog\">this</a>, its awesome<p>", feed.Items.First().Summary.Text);
        }

        [Fact]
        public void Response_Should_Have_Items_Populated()
        {
            //Given & When
            var rssResponse = GetResponse();
            var feed = GetFeedFromResponse(rssResponse);

            //Then
            Assert.Equal(1, feed.Items.Count());
        }

        private RssResponse GetResponse()
        {
            return new RssResponse(new[]
            {
                new BlogPost
                {
                    Title = "Awesome Post",
                    Author = "Vincent Vega",
                    AuthorEmail = "vincentvega@home.com",
                    Content = "<p><a href=\"/bob\">Bob</a></p>",
                    Localink = "awesome-post",
                    OriginalLink = "http://vincentvega.com/2014/06/01/awesome-post",
                    PublishedDate = DateTime.UtcNow.AddDays(-7),
                    Summary = "<p>Read <a href=\"/blog\">this</a>, its awesome<p>"
                }
            }, "Blog Title", "http://blog.nancyfx.org/", "rss");
        }

        private SyndicationFeed GetFeedFromResponse(RssResponse rssResponse)
        {
            var wrapper = new BrowserResponseBodyWrapper(rssResponse);
            var rss = wrapper.AsString();

            var reader = XmlReader.Create(new StringReader(rss));
            var feed = SyndicationFeed.Load(reader);

            return feed;
        }
    }
}
