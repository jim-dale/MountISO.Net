
namespace MountIso
{
    using System;
    using System.IO;

    public enum Command
    {
        NotSet,
        Mount,
        Dismount
    }

    public class AppContext
    {
        public const int INVALID_DRIVE_LETTER = 255;

        public bool ShowHelp { get; set; }
        public bool Verbose { get; set; }
        public string IsoPath { get; set; }
        public Command Command { get; set; }
        public uint Status { get; set; }
        public int DriveLetter { get; set; }
        public Exception Exception { get; set; }
        public string ErrorMessage { get; set; }

        public AppContext()
        {
            IsoPath = string.Empty;
            Command = Command.NotSet;
            DriveLetter = INVALID_DRIVE_LETTER;
        }

        public void Validate()
        {
            if (Command == Command.NotSet)
            {
                ErrorMessage = "Specify an option to either Mount or Dismount an ISO file.";
            }
            else if (string.IsNullOrWhiteSpace(IsoPath))
            {
                ErrorMessage = "Specify the path to a valid ISO file.";
            }
            else
            {
                IsoPath = Environment.ExpandEnvironmentVariables(IsoPath);
                IsoPath = Path.GetFullPath(IsoPath);

                if (File.Exists(IsoPath) == false)
                {
                    ErrorMessage = $"The ISO file \"{IsoPath}\" was not found.";
                }
            }
            if (Valid == false)
            {
                ShowHelp = true;
            }
        }

        public void SetException(Exception value)
        {
            Exception = value;
            ErrorMessage = value.Message;
        }

        public bool Valid
        {
            get
            {
                return String.IsNullOrEmpty(ErrorMessage);
            }
        }
    }
}
