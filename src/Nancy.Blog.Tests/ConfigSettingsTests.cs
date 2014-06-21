namespace NancyBlog.Tests
{
    using System;
    using Nancy.Blog;
    using Xunit;

    public class ConfigSettingsTests
    {
        [Fact]
        public void GetAppSetting_Should_Return_Value()
        {
            //Given
            var configsettings = GetConfigSettings();

            //When
            var result = configsettings.GetAppSetting("gun");

            //Then
            Assert.Equal("bang", result);
        }

        [Fact]
        public void GetGenericAppSetting_Should_Throw_If_Null_Key()
        {
            //Given
            var configsettings = GetConfigSettings();

            //When
            var result = Record.Exception(() => configsettings.GetAppSetting<int>(null));

            //Then
            Assert.IsType<ArgumentNullException>(result);
        }

        [Fact]
        public void GetGenericAppSetting_Should_Return_DefaultValue()
        {
            //Given
            var configsettings = GetConfigSettings();

            //When
            var result = configsettings.GetAppSetting<int>("thiskeydoesnotexist");

            //Then
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetGenericAppSetting_Should_Parse_Result_Correctly()
        {
            //Given
            var configsettings = GetConfigSettings();

            //When
            var result = configsettings.GetAppSetting<int>("whatrankisnancyinwebframeworks");

            //Then
            Assert.Equal(1, result);
        }

        private ConfigSettings GetConfigSettings()
        {
            return new ConfigSettings();
        }
    }
}
