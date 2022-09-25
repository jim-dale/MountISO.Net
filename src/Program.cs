
namespace MountIso;

using System;
using System.Linq;
using System.Management;

partial class Program
{
    const string NamespacePath = @"\\.\ROOT\Microsoft\Windows\Storage";

    const string DiskImageClassName = "MSFT_DiskImage";
    const string MountMethodName = "Mount";
    const string DismountMethodName = "Dismount";

    const string VolumeClassName = "MSFT_Volume";
    const string DriveLetterPropertName = "DriveLetter";

    public static int Main(string[] args)
    {
        var ctx = ArgsProcessor.Parse(args);

        ctx.Validate();

        if (ctx.ShowHelp)
        {
            ArgsProcessor.ShowHelp();
        }

        if (ctx.Valid)
        {
            if (ctx.Verbose)
            {
                ArgsProcessor.ShowConfiguration(ctx);
            }

            try
            {
                switch (ctx.Command)
                {
                    case Command.NotSet:
                        break;
                    case Command.Mount:
                        MountDiskImage(ctx);
                        break;
                    case Command.Dismount:
                        DismountDiskImage(ctx);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ctx.SetException(ex);
            }
            if (ctx.Verbose)
            {
                ArgsProcessor.ShowRunState(ctx);
            }
        }

        if (ctx.Valid == false)
        {
            ctx.DriveLetter = AppContext.INVALID_DRIVE_LETTER;
            Console.WriteLine(ctx.ErrorMessage);
        }
        return ctx.DriveLetter;
    }

    private static void MountDiskImage(AppContext ctx)
    {
        var path = BuildPath(ctx.IsoPath);
        var diskImage = GetDiskImage(ctx, path);

        ctx.Status = (uint)diskImage.InvokeMethod(MountMethodName, new object[] { (int)Access.ReadOnly, false });
        if (ctx.Status == (uint)ManagementStatus.NoError)
        {
            ctx.DriveLetter = GetDriveLetter(ctx, diskImage);
        }
    }

    private static void DismountDiskImage(AppContext ctx)
    {
        var path = BuildPath(ctx.IsoPath);
        var diskImage = GetDiskImage(ctx, path);

        ctx.Status = (uint)diskImage.InvokeMethod(DismountMethodName, null);
    }

    private static ManagementObject GetDiskImage(AppContext ctx, ManagementPath path)
    {
        ManagementObject result = new(path);

        result.Get();

        if (ctx.Verbose)
        {
            ShowProperties(result);
        }
        return result;
    }

    private static ManagementPath BuildPath(string imagePath)
    {
        imagePath = imagePath.Replace("\\", "\\\\");

        var result = new ManagementPath($"{NamespacePath}:{DiskImageClassName}.ImagePath=\"{imagePath}\",StorageType={(int)StorageType.Iso}");

        return result;
    }

    private static char GetDriveLetter(AppContext ctx, ManagementObject diskImage)
    {
        char result = char.MinValue;

        using (ManagementObjectCollection items = diskImage.GetRelated(VolumeClassName))
        {
            var volume = items.Cast<ManagementObject>().FirstOrDefault();
            if (volume != null)
            {
                result = (char)volume[DriveLetterPropertName];

                if (ctx.Verbose)
                {
                    ShowProperties(volume);
                }
            }
        }
        return result;
    }

    private static void ShowProperties(ManagementObject obj)
    {
        Console.WriteLine();
        Console.WriteLine($"Properties for \"{obj}\"");

        var properties = obj.Properties;
        foreach (PropertyData property in properties)
        {
            string value = GetPropertyValueAsString(property.Value);
            Console.WriteLine($"{property.Name}={value} ({property.Type})");
        }
        Console.WriteLine();
    }

    private static string GetPropertyValueAsString(object value)
    {
        string result = (value == null) ? "{Not Set}" : value.ToString();
        return result;
    }

    private enum Access
    {
        Unknown = 0,
        ReadWrite = 2,
        ReadOnly = 3
    }

    private enum StorageType
    {
        Unknown = 0,
        Iso = 1,
        Vhd = 2,
        Vhdx = 3,
        VhdSet = 4
    }
}
