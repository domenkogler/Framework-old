using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.Configuration.Json;

namespace Kogler.Framework
{
    public interface IConfigurationService
    {
        string GetSection(string key);
        string JoinSection(params string[] keys);
        IEnumerable<string> GetValues(string key);
        IConfigurationRoot Config { get; }
    }

    [InheritedExport(typeof(IConfigurationService))]
    public abstract class ConfigurationServiceBase : IConfigurationService
    {
        protected ConfigurationServiceBase(string basePath, params string[] paths)
        {
            var cb = new ConfigurationBuilder();
            foreach (var provider in paths.Select(p => new JsonConfigurationProvider(basePath + p)))
            {
                cb.Add(provider);
            }
            Config = cb.Build();
        }

        public string GetSection(string key)
        {
            return Config.GetSection(key).Value;
        }

        public string JoinSection(params string[] keys)
        {
            return string.Join("", keys.Select(GetSection));
        }

        public IEnumerable<string> GetValues(string key)
        {
            return Config.GetSection(key).GetChildren().Select(s => s.Value);
        }

        public IConfigurationRoot Config { get; }
    }
}