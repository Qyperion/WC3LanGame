using System.Runtime.InteropServices;

namespace WC3LanGame.App
{
    internal static class DarkMode
    {
        private static readonly Color ControlBackground = Color.FromArgb(45, 45, 48);
        private static readonly Color ControlForeground = Color.FromArgb(220, 220, 220);
        private static readonly Color ButtonBackground = Color.FromArgb(55, 55, 58);
        private static readonly Color ButtonBorder = Color.FromArgb(80, 80, 85);
        private static readonly Color ButtonHover = Color.FromArgb(70, 70, 75);
        private static readonly Color ButtonPressed = Color.FromArgb(80, 80, 90);
        private static readonly Color StripBackground = Color.FromArgb(30, 30, 30);
        private static readonly Color StripForeground = Color.FromArgb(180, 180, 180);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        public static void Apply(Form form)
        {
            if (form.IsHandleCreated)
                EnableDarkTitleBar(form.Handle);
            else
                form.HandleCreated += (_, _) => EnableDarkTitleBar(form.Handle);

            ApplyToControl(form);
        }

        public static void Apply(ToolStrip toolStrip)
        {
            toolStrip.BackColor = StripBackground;
            toolStrip.ForeColor = StripForeground;
            toolStrip.Renderer = new DarkToolStripRenderer();
            foreach (ToolStripItem item in toolStrip.Items)
                item.ForeColor = StripForeground;
        }

        private static void EnableDarkTitleBar(IntPtr handle)
        {
            int value = 1;
            DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, sizeof(int));
        }

        private static void ApplyToControl(Control control)
        {
            switch (control)
            {
                case StatusStrip strip:
                    strip.BackColor = StripBackground;
                    strip.ForeColor = StripForeground;
                    strip.Renderer = new DarkToolStripRenderer();
                    foreach (ToolStripItem item in strip.Items)
                        item.ForeColor = StripForeground;
                    break;

                case Button button:
                    button.FlatStyle = FlatStyle.Flat;
                    button.BackColor = ButtonBackground;
                    button.ForeColor = ControlForeground;
                    button.FlatAppearance.BorderColor = ButtonBorder;
                    button.FlatAppearance.MouseOverBackColor = ButtonHover;
                    button.FlatAppearance.MouseDownBackColor = ButtonPressed;
                    break;

                case ComboBox comboBox:
                    comboBox.BackColor = ControlBackground;
                    comboBox.ForeColor = ControlForeground;
                    break;
            }

            foreach (Control child in control.Controls)
                ApplyToControl(child);
        }

        private sealed class DarkToolStripRenderer : ToolStripProfessionalRenderer
        {
            public DarkToolStripRenderer() : base(new DarkColorTable()) { }

            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                using var brush = new SolidBrush(StripBackground);
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected || e.Item.Pressed)
                {
                    using var brush = new SolidBrush(Color.FromArgb(60, 60, 65));
                    e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
                }
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                using var bgBrush = new SolidBrush(StripBackground);
                e.Graphics.FillRectangle(bgBrush, 0, 0, e.Item.Width, e.Item.Height);
                int y = e.Item.Height / 2;
                using var pen = new Pen(Color.FromArgb(70, 70, 75));
                e.Graphics.DrawLine(pen, 4, y, e.Item.Width - 4, y);
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                using var brush = new SolidBrush(StripBackground);
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (e.ToolStrip is ContextMenuStrip or ToolStripDropDownMenu)
                {
                    using var pen = new Pen(Color.FromArgb(70, 70, 75));
                    var bounds = e.AffectedBounds;
                    e.Graphics.DrawRectangle(pen, 0, 0, bounds.Width - 1, bounds.Height - 1);
                    return;
                }
                base.OnRenderToolStripBorder(e);
            }
        }

        private sealed class DarkColorTable : ProfessionalColorTable
        {
            public override Color StatusStripGradientBegin => StripBackground;
            public override Color StatusStripGradientEnd => StripBackground;
        }
    }
}
