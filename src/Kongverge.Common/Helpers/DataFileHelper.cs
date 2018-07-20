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

        public bool GetDataFiles(string dataPath, out List<KongDataFile> dataFiles)
        {
            try
            {
                dataFiles = Directory.EnumerateFiles(dataPath, $"*{Settings.FileExtension}", SearchOption.AllDirectories)
                    .Select(ParseFile)
                    .ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while reading from {path}", dataPath);
                dataFiles = new List<KongDataFile>();
                return false;
            }
            return true;
        }

        public void WriteConfigFiles(List<KongService> existingServices)
        {
            if (!Directory.Exists(_configuration.OutputFolder))
            {
                Directory.CreateDirectory(_configuration.OutputFolder);
            }

            foreach (var service in existingServices)
            {
                var dataFile = new KongDataFile
                {
                    Service = service,
                };

                var yamlOut = JsonConvert.SerializeObject(dataFile, _settings);

                var fileName = $"{_configuration.OutputFolder}\\{service.Name}{Settings.FileExtension}";
                Log.Information("Writing {fileName}", fileName);
                File.WriteAllText(fileName, yamlOut);
            }
        }
    }
}
