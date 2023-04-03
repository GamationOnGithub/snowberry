﻿using System;
using Microsoft.Xna.Framework;
using Monocle;

namespace Snowberry;

public static class Util {
    public static class Colors {
        public static readonly Color White = Calc.HexToColor("f0f0f0");
        public static readonly Color Cyan = Calc.HexToColor("1fc4bc");
        public static readonly Color DarkCyan = Calc.HexToColor("1da4ad");
        public static readonly Color Red = Calc.HexToColor("db2323");
        public static readonly Color DarkRed = Calc.HexToColor("b02020");
        public static readonly Color Blue = Calc.HexToColor("2877de");
        public static readonly Color DarkBlue = Calc.HexToColor("2365cf");
        public static readonly Color DarkGray = Calc.HexToColor("060607");
        public static readonly Color CloudGray = Calc.HexToColor("293036");
        public static readonly Color CloudLightGray = Calc.HexToColor("5c646b");
    }

    public static int Bit(this bool b) {
        return b ? 1 : 0;
    }

    public static object Default(Type t) {
        if (t.IsEnum)
            return t.GetEnumValues().GetValue(0);
        if (t == typeof(string))
            return "";
        if (t == typeof(int))
            return 0;
        if (t == typeof(short))
            return (short)0;
        if (t == typeof(byte))
            return (byte)0;
        if (t == typeof(long))
            return (long)0;
        if (t == typeof(float))
            return (float)0;
        if (t == typeof(double))
            return (double)0;
        if (t == typeof(bool))
            return false;
        return null;
    }
}