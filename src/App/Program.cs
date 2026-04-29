using WC3LanGame.Core;

namespace WC3LanGame.App;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) =>
        {
            MessageBox.Show(
                $"An unexpected error occurred:\n{e.Exception.Message}\n\n{e.Exception.StackTrace}",
                "WC3 Lan Game — Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        };
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show(
                    $"A fatal error occurred:\n{ex.Message}\n\n{ex.StackTrace}",
                    "WC3 Lan Game — Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        };

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        var settings = AppSettings.Load();
        ThemePalette.ApplyColorMode(settings.ThemeMode);
        Application.Run(new MainForm(settings));
    }
}
