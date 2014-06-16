namespace Nancy.Blog.Services
{
    using Model;

    public interface IPostService
    {
        IndexViewModel GetViewModel(int feedCount, int pagenum);
    }
}