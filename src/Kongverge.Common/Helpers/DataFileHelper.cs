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

        public DataFileHelper(IOptions<Settings> configuration, PluginConverter converter)
        {
            _configuration = configuration.Value;
            _settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new[] { converter },
                Formatting = Formatting.Indented
            };
        }

        public KongDataFile ParseFile(string filename)
        {
            var text = File.ReadAllText(filename);
            KongDataFile data;
            try
            {
                data = JsonConvert.DeserializeObject<KongDataFile>(text, _settings);
            }
            catch (FormatException ex)
            {
                Log.Error(ex, "Invalid Syntax in {filename}", filename);
                throw;
            }
            
            return data;
        }

        public bool GetDataFiles(string dataPath, out IReadOnlyCollection<KongDataFile> dataFiles, out GlobalConfig globalConfig)
        {
            try
            {
                dataFiles =
                    Directory.EnumerateFiles(dataPath, $"*{Settings.FileExtension}", SearchOption.AllDirectories)
                    .Where(d => !d.EndsWith(Settings.GlobalConfigPath))
                    .Select(ParseFile)
                    .ToList();

                var globalPluginsFile = Path.Combine(dataPath, Settings.GlobalConfigPath);
                globalConfig = File.Exists(globalPluginsFile) ? ParseGlobalConfig(globalPluginsFile) : new GlobalConfig();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while reading from {path}", dataPath);
                dataFiles = new List<KongDataFile>();
                globalConfig = new GlobalConfig();
                return false;
            }
            return true;
        }

        private GlobalConfig ParseGlobalConfig(string path)
        {
            var text = File.ReadAllText(path);
            try
            {
                return JsonConvert.DeserializeObject<GlobalConfig>(text, _settings);
            }
            catch (FormatException ex)
            {
                Log.Error(ex, "Invalid Syntax in global config file: {filename}", path);
                throw;
            }
        }

        public void WriteConfigFiles(IEnumerable<KongService> existingServices)
        {
            if (Directory.Exists(_configuration.OutputFolder))
            {
                foreach (var enumerateFile in Directory.EnumerateFiles(_configuration.OutputFolder))
                {
                    File.Delete(enumerateFile);
                }
            }
            else
            {
                Directory.CreateDirectory(_configuration.OutputFolder);
            }

            foreach (var service in existingServices)
            {
                var dataFile = new KongDataFile
                {
                    Service = service,
                };

                var json = JsonConvert.SerializeObject(dataFile, _settings);

                var fileName = $"{_configuration.OutputFolder}\\{service.Name}{Settings.FileExtension}";
                Log.Information("Writing {fileName}", fileName);
                File.WriteAllText(fileName, json);
            }
        }
    }
}
