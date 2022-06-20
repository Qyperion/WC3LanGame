using System.ComponentModel;
using WC3LanGame.Extensions;

namespace WC3LanGame.Warcraft3.Types
{
    internal enum WarcraftVersion : byte
    {
        [Description("1.21")] V1_21 = 0x15,
        [Description("1.22")] V1_22 = 0x16,
        [Description("1.23")] V1_23 = 0x17,
        [Description("1.24")] V1_24 = 0x18,
        [Description("1.25")] V1_25 = 0x19,
        [Description("1.26")] V1_26 = 0x1A,
        [Description("1.27")] V1_27 = 0x1B,
        [Description("1.28")] V1_28 = 0x1C,
        [Description("1.29")] V1_29 = 0x1D,
    }

    internal class WarcraftVersionWrapper
    {
        public WarcraftVersion Version { get; }

        public WarcraftVersionWrapper(WarcraftVersion version)
        {
            Version = version;
        }

        public override string ToString()
        {
            return Version.Version();
        }
    }
}
