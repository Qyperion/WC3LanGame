using Microsoft.Win32;

using System.ComponentModel;
using System.Diagnostics;
using System.Security;

using WC3LanGame.Core.Warcraft3.Types;

namespace WC3LanGame.Core.Warcraft3
{
    public static class WarcraftExecutable
    {
        // Registry sub-keys where Warcraft III installations may be registered.
        private static readonly string[] RegistrySubKeys =
        [
            @"SOFTWARE\Blizzard Entertainment\Warcraft III",
            @"SOFTWARE\WOW6432Node\Blizzard Entertainment\Warcraft III",
        ];

        private static readonly string[] ExecutableNames =
        [
            "Warcraft III.exe",
            "war3.exe",
        ];

        // Known process names (without .exe) for detecting a running instance
        private static readonly string[] ProcessNames =
        [
            "Warcraft III",
            "war3",
        ];

        public static string ResolveExecutablePath(string preferredPath = null, WarcraftType? warcraftType = null)
        {
            string resolved = NormalizeExecutablePath(preferredPath);
            if (resolved != null)
                return resolved;

            foreach (string valueName in GetRegistryValueNames(warcraftType))
            {
                foreach (string subKey in RegistrySubKeys)
                {
                    resolved = TryReadRegistryPath(RegistryHive.CurrentUser, subKey, valueName)
                            ?? TryReadRegistryPath(RegistryHive.LocalMachine, subKey, valueName);
                    if (resolved != null)
                        return resolved;
                }
            }

            return null;
        }

        public static string RunWC3(string executablePath)
        {
            if (string.IsNullOrWhiteSpace(executablePath))
                return "Unable to locate Warcraft III executable";

            try
            {
                ProcessStartInfo startInfo = new(executablePath);
                string workingDirectory = Path.GetDirectoryName(executablePath);
                if (!string.IsNullOrWhiteSpace(workingDirectory))
                    startInfo.WorkingDirectory = workingDirectory;

                Process.Start(startInfo);
                return "Warcraft III process started!";
            }
            catch (Exception ex) when (ex is Win32Exception or InvalidOperationException or FileNotFoundException)
            {
                return $"Unable to launch WC3 ({executablePath}).\nException: {ex.Message}";
            }
        }

        public static string GetInstalledWC3Version(string preferredPath = null)
        {
            string executablePath = ResolveExecutablePath(preferredPath);
            if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath))
                return "";

            try
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(executablePath);
                return $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}";
            }
            catch (Exception e) when (e is FileNotFoundException or Win32Exception)
            {
                return "";
            }
        }

        public static bool IsWC3ProcessRunning(string preferredPath = null)
        {
            List<Process> processes = FindWarcraftProcesses(preferredPath);
            try
            {
                return processes.Count > 0;
            }
            finally
            {
                foreach (Process process in processes)
                    process.Dispose();
            }
        }

        public static string StopWC3ProcessRunning(string preferredPath = null)
        {
            List<Process> processes = FindWarcraftProcesses(preferredPath);
            try
            {
                Process wc3Process = processes.FirstOrDefault();
                if (wc3Process == null)
                    return "WC3 isn't running.";

                wc3Process.CloseMainWindow();
                return "WC3 process was stopped.";
            }
            finally
            {
                foreach (Process process in processes)
                    process.Dispose();
            }
        }

        private static string[] GetRegistryValueNames(WarcraftType? warcraftType) => warcraftType switch
        {
            WarcraftType.ReignOfChaos => ["GamePath", "Program", "ProgramX", "InstallPath"],
            _ => ["GamePath", "ProgramX", "Program", "InstallPath"],
        };

        private static string TryReadRegistryPath(RegistryHive hive, string subKey, string valueName)
        {
            try
            {
                using RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
                using RegistryKey key = baseKey.OpenSubKey(subKey);

                return NormalizeExecutablePath(key?.GetValue(valueName) as string);
            }
            catch (Exception e) when (e is ArgumentException or IOException or UnauthorizedAccessException or SecurityException)
            {
                return null;
            }
        }

        private static string NormalizeExecutablePath(string candidatePath)
        {
            if (string.IsNullOrWhiteSpace(candidatePath))
                return null;

            try
            {
                string path = Environment.ExpandEnvironmentVariables(candidatePath.Trim().Trim('"'));

                if (File.Exists(path))
                    return Path.GetFullPath(path);

                return Directory.Exists(path)
                    ? FindExecutableInDirectory(path)
                    : null;
            }
            catch (Exception e) when (e is ArgumentException or IOException or NotSupportedException)
            {
                return null;
            }
        }

        private static string FindExecutableInDirectory(string directoryPath)
        {
            foreach (string name in ExecutableNames)
            {
                string path = Path.Combine(directoryPath, name);
                if (File.Exists(path))
                    return Path.GetFullPath(path);
            }

            return null;
        }

        private static List<Process> FindWarcraftProcesses(string preferredPath)
        {
            HashSet<int> seen = [];
            List<Process> result = [];

            foreach (string processName in CollectProcessNames(preferredPath))
            {
                try
                {
                    foreach (Process process in Process.GetProcessesByName(processName))
                    {
                        if (seen.Add(process.Id))
                        {
                            result.Add(process);
                            continue;
                        }

                        process.Dispose();
                    }
                }
                catch (Exception e) when (e is Win32Exception or InvalidOperationException) { }
            }

            return result;
        }

        // Builds a deduplicated set of process names from known names + user-configured path.
        private static HashSet<string> CollectProcessNames(string preferredPath)
        {
            HashSet<string> names = new(ProcessNames, StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(preferredPath))
                names.Add(Path.GetFileNameWithoutExtension(preferredPath));

            return names;
        }
    }
}
