using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kongverge.Common.Helpers;

namespace Kongverge.Integration.Tests.ProgramTests
{
    public abstract class ProgramSteps
    {
        protected const string Plus = "+";
        protected const string NonExistentInputFolder = "NonExistent";
        protected const string InputFolderInvalidData = @"ProgramTests\InputFolderInvalidData";
        protected const string InputFolderBadFormat = @"ProgramTests\InputFolderBadFormat";
        protected const string InputFolder0 = @"ProgramTests\InputFolder0";
        protected const string InputFolder1 = @"ProgramTests\InputFolder1";
        protected const string InputFolder2 = @"ProgramTests\InputFolder2";
        protected const string OutputFolder = @"ProgramTests\OutputFolder";

        protected CommandLineArguments Arguments = new CommandLineArguments();
        protected string InputFolder;
        protected ExitCode ExitCode;

        protected void InvokingMain() => ExitCode = (ExitCode)Program.Main(Arguments.ToArray());

        protected void InvokingMainAgainForExport()
        {
            TheExitCodeIs(ExitCode.Success);
            Arguments = new CommandLineArguments();
            AValidHost();
            AValidPort();
            OutputFolderIs(OutputFolder);
            ExitCode = (ExitCode)Program.Main(Arguments.ToArray());
        }

        protected void NoArguments() { }

        protected void AnInvalidPort() => Arguments.AddPair("--port", 1);

        protected void AValidHost() => Arguments.AddPair("--host", KongvergeTestFixture.Host);

        protected void AValidPort() => Arguments.AddPair("--port", KongvergeTestFixture.Port);

        protected void NoInputOrOutputFolder() { }

        protected void InputAndOutputFolders()
        {
            InputFolderIs(Guid.NewGuid().ToString());
            OutputFolderIs(Guid.NewGuid().ToString());
        }

        protected void InputFolderIs(string folder)
        {
            InputFolder = folder;
            Arguments.AddPair("--input", folder);
        }

        protected void KongIsInABlankState()
        {
            KongIsInAStateMatchingInputFolder(InputFolder0);
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

        protected void OutputFolderIs(string folder) => Arguments.AddPair("--output", folder);

        protected void TheExitCodeIs(ExitCode exitCode) => ExitCode.Should().Be(exitCode);

        protected async Task OutputFolderContentsMatchInputFolderContents()
        {
            Debug.WriteLine(Directory.GetCurrentDirectory());

            var dataFileHelper = new DataFileHelper();
            var inputConfiguration = await dataFileHelper.ReadConfiguration(InputFolder);
            var outputConfiguration = await dataFileHelper.ReadConfiguration(OutputFolder);

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
