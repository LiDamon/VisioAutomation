﻿namespace VisioAutomation.Drawing
{
    public struct ColorHSL
    {
        // HSL http://msdn.microsoft.com/en-us/library/ms406705(v=office.12).aspx
        // HUE http://msdn.microsoft.com/en-us/library/ms406706(v=office.12).aspx
        // SAT http://msdn.microsoft.com/en-us/library/ms425560(office.12).aspx
        // LUM http://office.microsoft.com/en-us/visio-help/HV080400509.aspx

        public byte H { get; }
        public byte S { get; }
        public byte L { get; }

        public ColorHSL(byte h, byte s, byte l)
        {
            this.H = h;
            this.S = s;
            this.L = l;
        }

        private void CheckValidVisioHSL()
        {
            CheckValidVisioHSL(this.H,this.S,this.L);
        }

        private static void CheckValidVisioHSL(byte h, byte s, byte l)
        {
            if (h > 255)
            {
                throw new System.ArgumentOutOfRangeException(nameof(h), "Visio Hue value must be <=255");
            }
            if (s > 240)
            {
                throw new System.ArgumentOutOfRangeException(nameof(s), "Visio saturation value must be <=240");
            }
            if (l > 240)
            {
                throw new System.ArgumentOutOfRangeException(nameof(l), "Visio lumincance value must be <=240");
            }
        }

        public ColorHSL(short h, short s, short l) :
            this((byte)h, (byte)s, (byte)l)
        {
        }

        public override string ToString()
        {
            var s = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}({1},{2},{3})", nameof(ColorHSL), this.H, this.S, this.L);
            return s;
        }

        public override bool Equals(object other)
        {
            return other is ColorHSL && this.Equals((ColorHSL)other);
        }

        public static bool operator ==(ColorHSL lhs, ColorHSL rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ColorHSL lhs, ColorHSL rhs)
        {
            return !lhs.Equals(rhs);
        }

        private bool Equals(ColorHSL other)
        {
            return (this.H == other.H && this.S == other.S && this.L == other.L);
        }

        public override int GetHashCode()
        {
            return this.ToHSLBytes();
        }

        private int ToHSLBytes()
        {
            return (this.H << 16) | (this.S << 8) | (this.L);
        }

        public string ToFormula()
        {
            this.CheckValidVisioHSL();
            string formula = string.Format("{0}({1},{2},{3})", "HSL",this.H, this.S, this.L);
            return formula;
        }
    }
}