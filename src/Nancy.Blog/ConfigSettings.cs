namespace Nancy.Blog
{
    using System;
    using System.ComponentModel;

    public class ConfigSettings : IConfigSettings
    {
        public string GetAppSetting(string propertyName)
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get(propertyName);
        }

        public T GetAppSetting<T>(string propertyName) where T : struct
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            var value = GetAppSetting(propertyName);

            T returnVal = default(T);
            if (String.IsNullOrEmpty(value))
                return returnVal;

            returnVal = TryParse<T>(value);
            return returnVal;
        }

        private T TryParse<T>(string s)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            try
            {
                return (T)converter.ConvertFromString(s);
            }
            catch
            {
                return default(T);
            }
        }
    }
}