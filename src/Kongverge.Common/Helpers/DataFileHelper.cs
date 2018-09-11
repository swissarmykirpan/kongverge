using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Serilog;

namespace Kongverge.Common.Helpers
{
    public class DataFileHelper : IDataFileHelper
    {
        public async Task<KongvergeConfiguration> ReadConfiguration(string folderPath)
        {
            var filePaths = Directory.EnumerateFiles(folderPath, $"*{Settings.FileExtension}", SearchOption.AllDirectories);

            var services = new List<KongService>();
            foreach (var serviceFilePath in filePaths)
            {
                services.Add(await ParseFile<KongService>(serviceFilePath).ConfigureAwait(false));
            }

            var globalConfigFilePath = Path.Combine(folderPath, Settings.GlobalConfigPath);
            var globalConfiguration = File.Exists(globalConfigFilePath)
                ? await ParseFile<ExtendibleKongObject>(globalConfigFilePath).ConfigureAwait(false)
                : new ExtendibleKongObject();
            
            return new KongvergeConfiguration
            {
                Services = services.AsReadOnly(),
                GlobalConfig = globalConfiguration
            };
        }

        private static async Task<T> ParseFile<T>(string path) where T : ExtendibleKongObject
        {
            string text;
            using (var reader = File.OpenText(path))
            {
                text = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            T data;
            var errorMessages = new List<string>();
            try
            {
                data = text.ToKongObject<T>();
                await data.Validate(errorMessages).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new InvalidConfigurationFileException(path, ex.Message, ex);
            }

            if (errorMessages.Any())
            {
                throw new InvalidConfigurationFileException(path, string.Join(Environment.NewLine, errorMessages));
            }
            
            return data;
        }

        public async Task WriteConfiguration(KongvergeConfiguration configuration, string folderPath)
        {
            PrepareOutputFolder(folderPath);

            foreach (var service in configuration.Services)
            {
                await WriteConfigObject(service, folderPath, $"{service.Name}{Settings.FileExtension}").ConfigureAwait(false);
            }

            if (configuration.GlobalConfig.Plugins.Any())
            {
                await WriteConfigObject(configuration.GlobalConfig, folderPath, $"{Settings.GlobalConfigPath}").ConfigureAwait(false);
            }
        }

        private static async Task WriteConfigObject(ExtendibleKongObject configObject, string folderPath, string fileName)
        {
            var json = configObject.ToConfigJson();
            var path = $"{folderPath}\\{fileName}";
            Log.Information("Writing {path}", path);
            using (var stream = File.OpenWrite(path))
            using (var writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(json).ConfigureAwait(false);
            }
        }

        private void PrepareOutputFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                foreach (var path in Directory.EnumerateFiles(folderPath))
                {
                    File.Delete(path);
                }
            }
            else
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }
}
