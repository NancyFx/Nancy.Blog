namespace Nancy.Blog
{
    public interface IConfigSettings
    {
        string GetAppSetting(string propertyName);
        T GetAppSetting<T>(string propertyName) where T : struct;
    }
}