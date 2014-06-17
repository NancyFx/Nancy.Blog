namespace Nancy.Blog
{
    using System.Collections.Generic;
    using Model;

    public static class ResponseExtensions
    {
        public static Response AsRSS(this IResponseFormatter formatter, IEnumerable<BlogPost> model, string rssTitle, string siteUrl, string feedfileName)
        {
            return new RssResponse(model, rssTitle, siteUrl, feedfileName);
        }
    }
}