﻿
namespace MountIso;

using System;

public class ArgsProcessor
{
    public static AppContext Parse(string[] args)
    {
        var result = new AppContext();

        foreach (var arg in args)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                continue;
            }
            if (arg.Length == 2 && (arg[0] == '-' || arg[0] == '/'))
            {
                char option = char.ToLowerInvariant(arg[1]);

                switch (option)
                {
                    case '?':
                        result.ShowHelp = true;
                        break;
                    case 'm':
                        result.Command = Command.Mount;
                        break;
                    case 'd':
                        result.Command = Command.Dismount;
                        break;
                    case 'v':
                        result.Verbose = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                result.IsoPath = arg;
            }
        }
        return result;
    }

    public static void ShowHelp()
    {
        Console.WriteLine("MountIso [-?] [-v] -m | -d [drive:][path]filename");
        Console.WriteLine("  [drive:][path][filename]");
        Console.WriteLine("                 Specifies path to the ISO file to perform the action on.");
        Console.WriteLine();
        Console.WriteLine("  -?             Show this help information.");
        Console.WriteLine("  -v             Switches on verbose mode to display additional debug information.");
        Console.WriteLine("  -m             Mounts a previously created ISO disk image, making it appear as a normal disk.");
        Console.WriteLine("  -d             Dismounts an ISO disk image so that it can no longer be accessed as a disk.");
        Console.WriteLine();
        Console.WriteLine("If an ISO file is successfully mounted the program's exit code (ERRORLEVEL) is set");
        Console.WriteLine("to the numeric value of the driver letter e.g. 70 = F");
        Console.WriteLine("The undocumented environment variable %=ExitCodeAscii% can then be used to get the");
        Console.WriteLine("ASCII drive letter for the mounted ISO file.");
        Console.WriteLine();
        Console.WriteLine("Example usage:");
        Console.WriteLine();
        Console.WriteLine("    @SETLOCAL");
        Console.WriteLine("    @SET INPUT=LOREM_IPSUM.iso");
        Console.WriteLine();
        Console.WriteLine("    MountIso -v -m %INPUT%");
        Console.WriteLine("    @IF ERRORLEVEL 255 @GOTO :END");
        Console.WriteLine();
        Console.WriteLine("    @SET ISODRVLETTER=%=ExitCodeAscii%");
        Console.WriteLine("    @IF \"%ISODRVLETTER%\"==\"\" @GOTO :END");
        Console.WriteLine();
        Console.WriteLine("    @SET ISODRIVE=%ISODRVLETTER%:\\");
        Console.WriteLine();
        Console.WriteLine("    @ECHO.");
        Console.WriteLine("    @ECHO The ISO file %INPUT% has been mounted to drive %ISODRVLETTER%");
        Console.WriteLine("    @ECHO.");
        Console.WriteLine();
        Console.WriteLine("    DIR %ISODRIVE%");
        Console.WriteLine();
        Console.WriteLine("    @ECHO.");
        Console.WriteLine("    @ECHO.");
        Console.WriteLine("    @ECHO Ready to Dismount ISO file");
        Console.WriteLine("    @PAUSE");
        Console.WriteLine();
        Console.WriteLine("    MountIso -d %INPUT%");
        Console.WriteLine();
        Console.WriteLine("    :END");
        Console.WriteLine("    @ENDLOCAL");
        Console.WriteLine();
    }

    public static void ShowConfiguration(AppContext ctx)
    {
        Console.WriteLine($"ShowHelp: {ctx.ShowHelp}");
        Console.WriteLine($"Verbose: {ctx.Verbose}");
        Console.WriteLine($"IsoPath: \"{ctx.IsoPath}\"");
        Console.WriteLine($"Command: {ctx.Command}");
    }

    public static void ShowRunState(AppContext ctx)
    {
        string errorMessage = string.IsNullOrEmpty(ctx.ErrorMessage) ? "(Not Set)" : ctx.ErrorMessage;
        string exception = (ctx.Exception == null) ? "(Not Set)" : ctx.Exception.ToString();

        Console.WriteLine($"Status: {ctx.Status}");
        Console.WriteLine($"DriveLetter: {ctx.DriveLetter}");
        Console.WriteLine($"Error Message: {errorMessage}");
        Console.WriteLine($"Exception: {exception}");
    }
}
