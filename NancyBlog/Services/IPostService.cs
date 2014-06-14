namespace NancyBlog.Services
{
    using Model;

    public interface IPostService
    {
        IndexViewModel GetViewModel(int feedCount, int pagenum);
    }
}