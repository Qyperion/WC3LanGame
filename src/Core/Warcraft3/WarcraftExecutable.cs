using Microsoft.Win32;

using System.ComponentModel;
using System.Diagnostics;

using WC3LanGame.Core.Warcraft3.Types;

namespace WC3LanGame.Core.Warcraft3
{
    public static class WarcraftExecutable
    {
        private const string Warcraft3RegistryKey = @"HKEY_CURRENT_USER\Software\Blizzard Entertainment\Warcraft III";
        private const string Warcraft3ProcessName = "war3";

        public static string RunWC3(WarcraftType warcraftType)
        {
            string programKey = warcraftType == WarcraftType.TheFrozenThrone ? "ProgramX" : "Program";

            string program = (string)Registry.GetValue(Warcraft3RegistryKey, programKey, null);

            if (program == null)
            {
                return "Unable to locate Warcraft 3 executable";
            }

            try
            {
                Process.Start(program);
                return "Warcraft3 process started!";
            }
            catch (Win32Exception ex)
            {
                return $"Unable to launch WC3 ({program}).\nException: {ex.Message}";
            }
            catch (FileNotFoundException ex)
            {
                return $"Unable to launch WC3 ({program}).\nException: {ex.Message}";
            }
        }

        public static string GetInstalledWC3Version()
        {
            string program = Registry.GetValue(Warcraft3RegistryKey, "InstallPath", null) + "\\war3.exe";
            if (!File.Exists(program))
                return "";

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(program);
            return $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}";
        }

        public static bool IsWC3ProcessRunning()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(Warcraft3ProcessName);
                try
                {
                    return processes.Length > 0;
                }
                finally
                {
                    foreach (Process process in processes)
                        process.Dispose();
                }
            }
            catch (Win32Exception)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public static string StopWC3ProcessRunning()
        {
            Process[] processes = Process.GetProcessesByName(Warcraft3ProcessName);
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
    }
}
