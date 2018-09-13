using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kongverge.Common.DTOs;
using Kongverge.Common.Helpers;
using Serilog;

namespace Kongverge.Common.Services
{
    public class ConfigFileReader
    {
        public virtual async Task<KongvergeConfiguration> ReadConfiguration(string folderPath)
        {
            Log.Information("Reading files from {folderPath}", folderPath);

            var filePaths = Directory.EnumerateFiles(folderPath, $"*{Settings.FileExtension}", SearchOption.AllDirectories);

            var services = new List<KongService>();
            ExtendibleKongObject globalConfig = null;
            foreach (var configFilePath in filePaths)
            {
                if (configFilePath.EndsWith(Settings.GlobalConfigFileName))
                {
                    if (globalConfig != null)
                    {
                        throw new InvalidConfigurationFileException(configFilePath, $"Cannot have more than one {Settings.GlobalConfigFileName} file.");
                    }
                    globalConfig = await ParseFile<ExtendibleKongObject>(configFilePath).ConfigureAwait(false);
                }
                else
                {
                    services.Add(await ParseFile<KongService>(configFilePath).ConfigureAwait(false));
                }
            }
            
            return new KongvergeConfiguration
            {
                Services = services.AsReadOnly(),
                GlobalConfig = globalConfig ?? new ExtendibleKongObject()
            };
        }

        private static async Task<T> ParseFile<T>(string path) where T : ExtendibleKongObject
        {
            Log.Information("Reading {path}", path);
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
    }
}
