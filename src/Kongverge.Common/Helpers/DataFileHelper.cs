using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kongverge.Common.DTOs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace Kongverge.Common.Helpers
{
    public class DataFileHelper : IDataFileHelper
    {
        private readonly JsonSerializerSettings _settings;

        private readonly Settings _configuration;

        public DataFileHelper(IOptions<Settings> configuration)
        {
            _configuration = configuration.Value;
            _settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
        }

        public bool GetDataFiles(string dataPath, out IReadOnlyCollection<KongService> services, out ExtendibleKongObject globalConfig)
        {
            try
            {
                services =
                    Directory.EnumerateFiles(dataPath, $"*{Settings.FileExtension}", SearchOption.AllDirectories)
                    .Where(d => !d.EndsWith(Settings.GlobalConfigPath))
                    .Select(ParseFile<KongService>)
                    .ToList();

                var globalConfigPath = Path.Combine(dataPath, Settings.GlobalConfigPath);
                globalConfig = File.Exists(globalConfigPath) ? ParseFile<ExtendibleKongObject>(globalConfigPath) : new ExtendibleKongObject();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while reading from {path}", dataPath);
                services = Array.Empty<KongService>();
                globalConfig = new ExtendibleKongObject();
                return false;
            }
            return true;
        }

        private T ParseFile<T>(string path) where T : ExtendibleKongObject
        {
            var text = File.ReadAllText(path);
            T data;
            try
            {
                data = JsonConvert.DeserializeObject<T>(text, _settings);
            }
            catch (FormatException ex)
            {
                Log.Error(ex, "Invalid Syntax in {path}", path);
                throw;
            }
            
            return data;
        }

        public void WriteConfigFiles(IEnumerable<KongService> services, ExtendibleKongObject globalConfig)
        {
            PrepareOutputFolder();

            foreach (var service in services)
            {
                WriteConfigObject(service, $"{service.Name}{Settings.FileExtension}");
            }

            WriteConfigObject(globalConfig, $"{Settings.GlobalConfigPath}");
        }

        private void WriteConfigObject(ExtendibleKongObject configObject, string name)
        {
            configObject.StripPersistedValues();
            var json = JsonConvert.SerializeObject(configObject, _settings);
            var path = $"{_configuration.OutputFolder}\\{name}";
            Log.Information("Writing {fileName}", path);
            File.WriteAllText(path, json);
        }

        private void PrepareOutputFolder()
        {
            if (Directory.Exists(_configuration.OutputFolder))
            {
                foreach (var path in Directory.EnumerateFiles(_configuration.OutputFolder))
                {
                    File.Delete(path);
                }
            }
            else
            {
                Directory.CreateDirectory(_configuration.OutputFolder);
            }
        }
    }
}
