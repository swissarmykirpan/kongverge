using Serilog;
using System;

namespace Kongverge.Common.Helpers
{
    public enum ExitCodes
    {
        Success = 0,
        HostUnreachable,
        InvalidPort,
        InputFolderUnreachable,
        IncompatibleArguments
    }
    public class ExitWithCode
    {
        public static int Return(ExitCodes exitCode)
        {
            switch (exitCode)
            {
                case ExitCodes.Success:
                    Log.Information("Finished");
                    break;

                case ExitCodes.InvalidPort:
                    Log.Error("Invalid port specified");
                    break;

                case ExitCodes.HostUnreachable:
                    Log.Error("Specified host unreachable");
                    break;

                case ExitCodes.InputFolderUnreachable:
                    Log.Error("Unable to access input folder");
                    break;

                case ExitCodes.IncompatibleArguments:
                    Log.Error("Incompatible command line arguments");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(exitCode), exitCode, null);
            }

            return  (int) exitCode;
        }
    }
}
