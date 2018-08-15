using Serilog;
using System;

namespace Kongverge.Common.Helpers
{
    public enum ExitCode
    {
        Success = 0,
        HostUnreachable,
        InvalidPort,
        InputFolderUnreachable,
        IncompatibleArguments
    }
    public class ExitWithCode
    {
        public static int Return(ExitCode exitCode)
        {
            switch (exitCode)
            {
                case ExitCode.Success:
                    Log.Information("Finished");
                    break;

                case ExitCode.InvalidPort:
                    Log.Error("Invalid port specified");
                    break;

                case ExitCode.HostUnreachable:
                    Log.Error("Specified host unreachable");
                    break;

                case ExitCode.InputFolderUnreachable:
                    Log.Error("Unable to access input folder");
                    break;

                case ExitCode.IncompatibleArguments:
                    Log.Error("Incompatible command line arguments");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(exitCode), exitCode, null);
            }

            return  (int)exitCode;
        }
    }
}
