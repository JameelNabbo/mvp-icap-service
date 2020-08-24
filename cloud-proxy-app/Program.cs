﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Glasswall.IcapServer.CloudProxyApp
{
    class Program
    {
        const string InputConfigurationKey = "input";
        const string OutputConfigurationKey = "output";
        const string ConfigurationConfigurationKey = "configuration";

        static int Main(string[] args)
        {
            var configuration = GetConfiguration(args);
            if (!CheckConfigurationIsValid(configuration))
            {
                return (int)ReturnOutcome.GW_ERROR;
            }

            return CopyFile(configuration[InputConfigurationKey], configuration[OutputConfigurationKey]);
        }

        private static int CopyFile(string sourceFilepath, string destinationFilepath)
        {
            try
            {
                var destinationFolder = System.IO.Path.GetDirectoryName(destinationFilepath);
                System.IO.Directory.CreateDirectory(destinationFolder);
                System.IO.File.Copy(sourceFilepath, destinationFilepath, true);
                return (int)ReturnOutcome.GW_REBUILT;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error Processing 'input' {sourceFilepath}, {ex.Message}");
                return (int)ReturnOutcome.GW_ERROR;
            }
        }

        static bool CheckConfigurationIsValid(IConfiguration configuration)
        {
            var configurationErrors = new List<string>();

            if (string.IsNullOrEmpty(configuration[InputConfigurationKey]))
                configurationErrors.Add($"'{InputConfigurationKey}' is missing from command line");

            if (string.IsNullOrEmpty(configuration[OutputConfigurationKey]))
                configurationErrors.Add($"'{OutputConfigurationKey}' is missing from command line");

            if (configurationErrors.Any())
            {
                Console.WriteLine($"Error Processing Command {Environment.NewLine} \t{string.Join(',', configurationErrors)}");
            }

            return !configurationErrors.Any();
        }

        static IConfiguration GetConfiguration(string[] args)
        {
            var switchMappings = new Dictionary<string, string>()
            {
                { "-i", InputConfigurationKey },
                { "-o", OutputConfigurationKey },
                { "-c", ConfigurationConfigurationKey },
            };
            var builder = new ConfigurationBuilder().AddCommandLine(args, switchMappings);
            return builder.Build();
        }
    }
}
