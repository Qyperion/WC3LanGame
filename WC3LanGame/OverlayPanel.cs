using System.Drawing;
using System.Windows.Forms;

namespace WC3LanGame
{
    /// <summary>
    /// A Panel that draws a semi-transparent dark overlay on top of
    /// whatever the parent's BackgroundImage shows through.
    /// Child controls with BackColor = Transparent render correctly.
    /// </summary>
    internal class OverlayPanel : Panel
    {
        private static readonly Color OverlayColor = Color.FromArgb(185, 12, 12, 28);

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

            // Then overlay a semi-transparent dark tint
            using var brush = new SolidBrush(OverlayColor);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }
    }
}
