namespace NancyBlog.Model
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<BlogPost> Posts { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int NextPage { get; set; }
        public int PreviousPage { get; set; }
    }
}
