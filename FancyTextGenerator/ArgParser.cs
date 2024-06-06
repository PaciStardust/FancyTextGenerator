using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace FancyTextGenerator
{
    static class ArgParser
    {
        public static readonly Dictionary<string, string[]> Args = new Dictionary<string, string[]>()
        {
            {"-tsi",    new string[] {"textSize",           "s",    "TEXT MODE (Integer)\nDetermines the size of text"} },
            {"-tf",     new string[] {"textFont",           "m",    "TEXT MODE (String +)\nDetermines the font of the text"} },
            {"-tst",    new string[] {"textStyle",          "s",    "TEXT MODE (Options)\nDetermines style of the font\n> b/bold, i/italics, u/underline"} },
            {"-ta",     new string[] {"textAlignment",      "s",    "TEXT MODE (Options)\nDetermines alignment of font\n> n/l/near/left, f/r/far/right"} },

            {"-fil",    new string[] {"frontImageLink",     "m",    "TEXT / IMAGE / COLOR MODE (ImgFile)\nImage for text instead of color\nNOTE: Overrides Colors"} },
            {"-finr",   new string[] {"frontImageNoResize", "n",    "TEXT MODE (None)\nSets textImage bounds to image bounds instead of text bounds"} },
            {"-fcol",   new string[] {"frontColors",        "m",    "TEXT / IMAGE / COLOR MODE (Hex/RGB/Name +)\nSets colors for front gradient\nNOTE: Gets overriden by imageLink"} },
            {"-fgm",    new string[] {"frontGradientMode",  "s",    "TEXT / IMAGE / COLOR MODE (Options)\nSets direction for front gradient\n> v/vertical, b/backward, f/forward"} },

            {"-bil",    new string[] {"backImageLink",      "m",    "TEXT MODE (ImgFile)\nImage for text instead of color\nNOTE: Overrides colors"} },
            {"-bsm",    new string[] {"backScaleMode",      "s",    "TEXT MODE (Options)\nScaling image across axis (Will keep aspect ratio if used)\n> h/height, w/width, b/both"} },
            {"-bsw",    new string[] {"backScaleW",         "s",    "TEXT MODE (Integer)\nImage scale width in percent"} },
            {"-bsh",    new string[] {"backScaleH",         "s",    "TEXT MODE (Integer)\nImage scale height in percent"} },
            {"-bow",    new string[] {"backOffsetW",        "s",    "TEXT MODE (Integer)\nImage offset width in percent"} },
            {"-boh",    new string[] {"backOffsetH",        "s",    "TEXT MODE (Integer)\nImage offset height in percent"} },
            {"-bcol",   new string[] {"backColors",         "m",    "TEXT MODE (Hex/RGB/Name +)\n Sets colors for background gradient\nNOTE: Gets overriden by imageLink"} },
            {"-bgm",    new string[] {"backGradMode",       "s",    "TEXT MODE (Options)\nSets direction for background gradient\n> v/vertical, b/backward, f/forward" } },

            {"-cit",    new string[] {"colorIntensity",     "s",    "COLOR MODE (Integer)\nColor Blending value in percent" } },

            {"-gr",     new string[] {"grayscaleRate",      "s",    "IMAGE MODE (Integer)\nRatio between black and white in percent when making image grayscale" } },
            {"-igs",    new string[] {"invertGrayscale",    "n",    "IMAGE MODE (None)\nInverts grayscale" } },

            {"-pw",     new string[] {"paddingW",           "s",    "TEXT MODE (Integer)\nPads width by value"} },
            {"-ph",     new string[] {"paddingH",           "s",    "TEXT MODE (Integer)\nPads height by value"} },
            {"-w",      new string[] {"width",              "s",    "TEXT MODE (Integer)\nSets width to value"} },
            {"-h",      new string[] {"height",             "s",    "TEXT MODE (Integer)\nSets height to value"} },

            {"-sdir",   new string[] {"fileLocation",       "m",    "ALL MODES (Directory)\nSets storage location for file\nNOTE: Generates folder in pictures dir. as fallback"} },
            {"-snm",    new string[] {"fileName",           "m",    "ALL MODES (String +)\nSets filename for file"} },
        };

        public static string GetString(Dictionary<string, string> args, string argName, string def)
        {
            if (!args.ContainsKey(argName))
                return def;

            return args[argName];
        }

        public static bool GetBool(Dictionary<string, string> args, string argName, bool def)
        {
            if (!args.ContainsKey(argName))
                return def;

            return !def;
        }

        public static Color[] GetColors(Dictionary<string, string> args, string argName, Color[] def)
        {
            if (!args.ContainsKey(argName))
                return def;

            return Utility.GetColorsFromString(args[argName]);
        }

        public static LinearGradientMode GetGradMode(Dictionary<string,string> args, string argName, LinearGradientMode def)
        {
            if (!args.ContainsKey(argName))
                return def;

            return args[argName].ToLower() switch
            {
                "v" => LinearGradientMode.Vertical,
                "vertical" => LinearGradientMode.Vertical,

                "f" => LinearGradientMode.ForwardDiagonal,
                "forward" => LinearGradientMode.ForwardDiagonal,
                "fd" => LinearGradientMode.ForwardDiagonal,
                "forwarddiagonal" => LinearGradientMode.ForwardDiagonal,

                "b" => LinearGradientMode.BackwardDiagonal,
                "backward" => LinearGradientMode.BackwardDiagonal,
                "bd" => LinearGradientMode.BackwardDiagonal,
                "backwarddiagonal" => LinearGradientMode.BackwardDiagonal,

                _ => LinearGradientMode.Horizontal
            };
        }

        public static int GetInt(Dictionary<string, string> args, string argName, int def, int min, int max)
        {
            if (!args.ContainsKey(argName))
                return def;

            if (Int32.TryParse(args[argName], out int parsedArg))
                return Math.Max(min, Math.Min(max, parsedArg));

            return def;
        }

        public static FontStyle GetFontStyle(Dictionary<string, string> args, string argName, FontStyle def)
        {
            if (!args.ContainsKey(argName))
                return def;

            return args[argName].ToLower() switch
            {
                "b" => FontStyle.Bold,
                "bold" => FontStyle.Bold,

                "i" => FontStyle.Italic,
                "italic" => FontStyle.Italic,

                "u" => FontStyle.Underline,
                "underline" => FontStyle.Underline,

                _ => FontStyle.Regular
            };
        }

        public static StringAlignment GetAlignment(Dictionary<string, string> args, string argName, StringAlignment def)
        {
            if (!args.ContainsKey(argName))
                return def;

            return args[argName].ToLower() switch
            {
                "n" => StringAlignment.Near,
                "near" => StringAlignment.Near,
                "left" => StringAlignment.Near,
                "l" => StringAlignment.Near,

                "f" => StringAlignment.Far,
                "far" => StringAlignment.Far,
                "right" => StringAlignment.Far,
                "r" => StringAlignment.Far,

                _ => StringAlignment.Center
            };
        }

        public static ScalingMode GetScaleMode(Dictionary<string, string> args, string argName, ScalingMode def)
        {
            if (!args.ContainsKey(argName))
                return def;

            return args[argName].ToLower() switch
            {
                "h" => ScalingMode.Height,
                "height" => ScalingMode.Height,

                "w" => ScalingMode.Width,
                "width" => ScalingMode.Width,

                "b" => ScalingMode.Both,
                "both" => ScalingMode.Both,

                _ => ScalingMode.None
            };
        }
    }
}
