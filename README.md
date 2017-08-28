# MountISO.Net

The MountISO command mounts a previously created ISO disk image, making it appear as a normal disk.

The command returns the drive letter in numeric format in the ERRORLEVEL variable.
This value can be converted to an ASCII character using the undocumented `%=ExitCodeAscii%` environment variable.

See `src\example-usage.cmd` for usage.
