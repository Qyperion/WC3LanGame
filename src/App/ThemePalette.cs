namespace WC3LanGame.App
{
    internal sealed class ThemePalette
    {
        private ThemePalette(
            Color overlayTint,
            Color primaryText,
            Color secondaryText,
            Color inactiveStatus,
            Color statusSuccess,
            Color statusAttention,
            Color logSuccess,
            Color logWarning,
            Color logError,
            Color trayIconBorder)
        {
            OverlayTint = overlayTint;
            PrimaryText = primaryText;
            SecondaryText = secondaryText;
            InactiveStatus = inactiveStatus;
            StatusSuccess = statusSuccess;
            StatusAttention = statusAttention;
            LogSuccess = logSuccess;
            LogWarning = logWarning;
            LogError = logError;
            TrayIconBorder = trayIconBorder;
        }

        public static ThemePalette Current => Application.IsDarkModeEnabled ? Dark : Light;

        public Color OverlayTint { get; }
        public Color PrimaryText { get; }
        public Color SecondaryText { get; }
        public Color InactiveStatus { get; }
        public Color StatusSuccess { get; }
        public Color StatusAttention { get; }
        public Color LogSuccess { get; }
        public Color LogWarning { get; }
        public Color LogError { get; }
        public Color TrayIconBorder { get; }

        private static ThemePalette Dark { get; } = new(
            overlayTint: Color.FromArgb(185, 12, 12, 28),
            primaryText: Color.White,
            secondaryText: Color.FromArgb(180, 180, 180),
            inactiveStatus: Color.Gray,
            statusSuccess: Color.LimeGreen,
            statusAttention: Color.FromArgb(255, 180, 0),
            logSuccess: Color.FromArgb(80, 220, 80),
            logWarning: Color.FromArgb(230, 190, 50),
            logError: Color.FromArgb(240, 80, 80),
            trayIconBorder: Color.FromArgb(30, 30, 30));

        private static ThemePalette Light { get; } = new(
            overlayTint: Color.FromArgb(120, 255, 255, 255),
            primaryText: Color.FromArgb(30, 30, 30),
            secondaryText: Color.FromArgb(80, 80, 80),
            inactiveStatus: Color.FromArgb(40, 40, 40),
            statusSuccess: Color.LimeGreen,
            statusAttention: Color.FromArgb(255, 180, 0),
            logSuccess: Color.FromArgb(0, 120, 0),
            logWarning: Color.FromArgb(160, 120, 0),
            logError: Color.FromArgb(200, 30, 30),
            trayIconBorder: Color.FromArgb(30, 30, 30));
    }
}
