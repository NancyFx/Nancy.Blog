namespace NancyBlog
{
    using System;
    using System.Collections.Generic;

    public interface IFeedService
    {
        IEnumerable<BlogPost> GetItems(int feedCount = 20, int pagenum = 0);
        BlogPost GetItem(string title);
    }
}