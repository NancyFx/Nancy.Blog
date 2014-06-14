namespace NancyBlog.Services
{
    using System.Collections.Generic;
    using Model;

    public interface IFeedService
    {
        IEnumerable<BlogPost> GetItems(int feedCount = 10, int pagenum = 0);
        BlogPost GetItem(string link);
    }
}