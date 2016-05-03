using System;
using System.IO;
using Shouldly;
using Xunit;

namespace Kogler.Framework.Test
{
    public class ConfigurationServiceTests
    {
        private class ConfigurationService : ConfigurationServiceBase
        {
            public ConfigurationService() : base(new FileInfo(new Uri(typeof(ConfigurationService).Assembly.CodeBase).LocalPath).DirectoryName, "config1.json", "config2.json") { }
        }

        [Fact]
        public void GetSectionTest()
        {
            var cs = new ConfigurationService();
            var api = cs.GetSection("Url:api");
            var breeze = cs.GetSection("Url:api_breeze");
            
            api.ShouldBe("https://localhost:44332/");
            breeze.ShouldBe("/breeze/");
            
        }

        [Fact]
        public void JoinSectionTest()
        {
            var cs = new ConfigurationService();
            var breeze_api = cs.JoinSection("Url:api", "Url:api_breeze");
            breeze_api.ShouldBe("https://localhost:44332//breeze/");
        }
    }
}