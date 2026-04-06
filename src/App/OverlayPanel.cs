namespace WC3LanGame.App
{
    /// <summary>
    /// A Panel that draws a theme-aware overlay on top of whatever the parent's BackgroundImage shows through.
    /// Child controls with BackColor = Transparent render correctly.
    /// </summary>
    internal class OverlayPanel : Panel
    {
        public OverlayPanel()
        {
            SetStyle(
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true);

            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Let the base paint the parent's background image region first
            base.OnPaintBackground(e);

            using var brush = new SolidBrush(ThemePalette.Current.OverlayTint);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }
}
