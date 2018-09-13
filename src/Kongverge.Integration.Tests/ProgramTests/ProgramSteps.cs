using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.Helpers;
using Kongverge.Common.Services;

namespace Kongverge.Integration.Tests.ProgramTests
{
    public abstract class ProgramSteps
    {
        protected const string And = "_";
        protected const string NonExistent = "NonExistent";
        protected const string InvalidData1 = "InvalidData1";
        protected const string InvalidData2 = "InvalidData2";
        protected const string BadFormat = "BadFormat";
        protected const string BlankState = "BlankState";
        protected const string A = "A";
        protected const string B = "B";
        protected const string Output = "Output";

        protected CommandLineArguments Arguments = new CommandLineArguments();
        protected string InputFolder;
        protected string OutputFolder;
        protected ExitCode ExitCode;

        private static string MakeFolderName(string name) => $@"ProgramTests\Folder{name}";

        protected void InvokingMain() => ExitCode = (ExitCode)Program.Main(Arguments.ToArray());

        protected void InvokingMainAgainForExport()
        {
            TheExitCodeIs(ExitCode.Success);
            Arguments = new CommandLineArguments();
            AValidHost();
            AValidPort();
            OutputFolderIs(MakeFolderName(Output));
            ExitCode = (ExitCode)Program.Main(Arguments.ToArray());
        }

        protected void NoArguments() { }

        protected void AnInvalidPort() => Arguments.AddPair("--port", 1);

        protected void AValidHost() => Arguments.AddPair("--host", KongvergeTestFixture.Host);

        protected void AValidPort() => Arguments.AddPair("--port", KongvergeTestFixture.Port);

        protected void NoPort() { }

        protected void NoInputOrOutputFolder() { }

        protected void InputAndOutputFolders()
        {
            InputFolderIs(Guid.NewGuid().ToString());
            OutputFolderIs(Guid.NewGuid().ToString());
        }

        protected void InputFolderIs(string name)
        {
            InputFolder = MakeFolderName(name);
            Arguments.AddPair("--input", InputFolder);
        }

        protected void OutputFolderIs(string name)
        {
            OutputFolder = MakeFolderName(name);
            Arguments.AddPair("--output", OutputFolder);
        }

        protected void KongIsInABlankState()
        {
            KongIsInAStateMatchingInputFolder(BlankState);
        }

        protected void KongIsInAStateMatchingInputFolder(string folder)
        {
            Arguments = new CommandLineArguments();
            AValidHost();
            AValidPort();
            InputFolderIs(folder);
            ExitCode = (ExitCode)Program.Main(Arguments.ToArray());
            TheExitCodeIs(ExitCode.Success);
            Arguments = new CommandLineArguments();
        }

        protected void TheExitCodeIs(ExitCode exitCode) => ExitCode.Should().Be(exitCode);

        protected async Task OutputFolderContentsMatchInputFolderContents()
        {
            Debug.WriteLine(Directory.GetCurrentDirectory());

            var configReader = new ConfigFileReader();
            var inputConfiguration = await configReader.ReadConfiguration(InputFolder);
            var outputConfiguration = await configReader.ReadConfiguration(OutputFolder);

            inputConfiguration.GlobalConfig.Plugins.Should().NotBeEmpty();
            inputConfiguration.Services.Count.Should().Be(4);

            outputConfiguration.GlobalConfig.Plugins.Should().BeEquivalentTo(inputConfiguration.GlobalConfig.Plugins);
            outputConfiguration.Services.Should().BeEquivalentTo(inputConfiguration.Services);
            foreach (var outputService in outputConfiguration.Services)
            {
                var inputService = inputConfiguration.Services.Single(x => x.Name == outputService.Name);
                outputService.Plugins.Should().BeEquivalentTo(inputService.Plugins);
                outputService.Routes.Should().BeEquivalentTo(inputService.Routes);
                foreach (var outputServiceRoute in outputService.Routes)
                {
                    var inputServiceRoute = inputService.Routes.Single(x => x.Equals(outputServiceRoute));
                    outputServiceRoute.Plugins.Should().BeEquivalentTo(inputServiceRoute.Plugins);
                }
            }
        }
    }

    public class CommandLineArguments : List<string>
    {
        public void AddPair(string name, object value)
        {
            Add(name);
            Add(value.ToString());
        }
    }
}
