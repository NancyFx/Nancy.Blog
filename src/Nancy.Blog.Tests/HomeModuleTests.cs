namespace NancyBlog.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.ServiceModel.Syndication;
    using System.Xml;
    using FakeItEasy;
    using Nancy;
    using Nancy.Blog;
    using Nancy.Blog.Model;
    using Nancy.Blog.Modules;
    using Nancy.Blog.Services;
    using Nancy.Testing;
    using Xunit;

    public class HomeModuleTests
    {
        [Fact]
        public void Get_About_Should_Return_About_View()
        {
            //Given
            var browser = new Browser(GetBootstrapper());

            //When
            var result = browser.Get("/about");

            //Then
            result.Body["h1 a"].First().ShouldContain("About");
        }

        [Fact]
        public void Get_About_Should_Return_Populated_ViewBag()
        {
            //Given
            var fakeconfigSettings = GetConfigSettings();
            A.CallTo(() => fakeconfigSettings.GetAppSetting("nancycategories")).Returns("nancy,nancyfx");

            var browser = new Browser(GetBootstrapper(configSettings: fakeconfigSettings));

            //When
            var result = browser.Get("/about");

            //Then
            result.Body["p"].First().ShouldContain("nancy or nancyfx");
        }

        [Fact]
        public void Get_RSS_Returns_RSS_Data()
        {
            //Given
            var fakefeedService = GetFeedService();
            A.CallTo(() => fakefeedService.GetItems(10, 0))
                .Returns(new[]
                {
                    new BlogPost
                    {
                        Title = "Awesome Post",
                        Author = "Vincent Vega",
                        AuthorEmail = "vincentvega@home.com",
                        Content = "<p>My post</p>",
                        Localink = "awesome-post",
                        OriginalLink = "http://vincentvega.com/2014/06/01/awesome-post",
                        PublishedDate = DateTime.UtcNow.AddDays(-7),
                        Summary = "Read this, its awesome"
                    }
                });

            var browser = new Browser(GetBootstrapper(feedService: fakefeedService));

            //When
            var result = browser.Get("/rss");

            var reader = XmlReader.Create(new StringReader(result.Body.AsString()));
            var feed = SyndicationFeed.Load(reader);

            //Then
            Assert.Equal("<p>My post</p>", ((TextSyndicationContent)feed.Items.First().Content).Text);
        }

        [Fact]
        public void Get_Blog_Post_Returns_404_If_Not_Found()
        {
            //Given
            var fakefeedService = GetFeedService();
            A.CallTo(() => fakefeedService.GetItem(A<string>.Ignored)).Returns(null);

            var browser = new Browser(GetBootstrapper(feedService: fakefeedService));

            //When
            var result = browser.Get("/my-awesome-post");

            //Then
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public void Get_Blog_Post_Returns_Post_Page()
        {
            //Given
            var fakefeedService = GetFeedService();
            A.CallTo(() => fakefeedService.GetItem(A<string>.Ignored)).Returns(new BlogPost
            {
                Title = "Awesome Post",
                Author = "Vincent Vega",
                AuthorEmail = "vincentvega@home.com",
                Content = "<p>My post</p>",
                Localink = "awesome-post",
                OriginalLink = "http://vincentvega.com/2014/06/01/awesome-post",
                PublishedDate = DateTime.UtcNow.AddDays(-7),
                Summary = "Read this, its awesome"
            });

            var browser = new Browser(GetBootstrapper(feedService: fakefeedService));

            //When
            var result = browser.Get("/my-awesome-post");

            //Then
            result.Body["head title"].AllShouldContain("Awesome Post");
        }

        [Fact]
        public void Get_Root_Returns_View_With_Data()
        {
            //Given
            var fakepostService = GetPostService();
            A.CallTo(() => fakepostService.GetViewModel(10, 1)).Returns(new IndexViewModel
            {
                Posts = new[]
                {
                    new BlogPost
                    {
                        Title = "Awesome Post",
                        Author = "Vincent Vega",
                        AuthorEmail = "vincentvega@home.com",
                        Content = "<p>My post</p>",
                        Localink = "awesome-post",
                        OriginalLink = "http://vincentvega.com/2014/06/01/awesome-post",
                        PublishedDate = DateTime.UtcNow.AddDays(-7),
                        Summary = "Read this, its awesome"
                    }
                }
            });

            var browser = new Browser(GetBootstrapper(postService: fakepostService));

            //When
            var result = browser.Get("/");

            //Then
            result.Body["a.readmore"].ShouldExist();
        }

        private ConfigurableBootstrapper GetBootstrapper(IPostService postService = null,
            IFeedService feedService = null, IConfigSettings configSettings = null)
        {
            postService = postService ?? GetPostService();
            feedService = feedService ?? GetFeedService();
            configSettings = configSettings ?? GetConfigSettings();

            return new ConfigurableBootstrapper(with =>
            {
                with.Module<HomeModule>();
                with.Dependency<IPostService>(postService);
                with.Dependency<IFeedService>(feedService);
                with.Dependency<IConfigSettings>(configSettings);
                with.RootPathProvider<HomeFakeRootPathProvider>();
            });
        }

        private IConfigSettings GetConfigSettings()
        {
            return A.Fake<IConfigSettings>();
        }

        private IFeedService GetFeedService()
        {
            return A.Fake<IFeedService>();
        }

        private IPostService GetPostService()
        {
            return A.Fake<IPostService>();
        }

        public class HomeFakeRootPathProvider : IRootPathProvider
        {
            private static readonly string RootPath = System.IO.Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName + "\\Nancy.Blog";
            public string GetRootPath()
            {
                return RootPath;
            }
        }
    }
}
