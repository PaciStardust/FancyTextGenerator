using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace FancyTextGenerator
{
    enum ProcessingMode
    {
        None,
        Invalid,
        GradientText,
        GradientImage,
        Colorizer
    }

    enum ScalingMode
    {
        Both,
        Height,
        Width,
        None
    }

    static class Utility
    {
        public static readonly Random r = new Random();

        public static readonly string defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\GradientGenerator\\";

        private static readonly Regex HexColorRegEX = new Regex("^[0-9a-fA-F]{6}$");
        private static readonly Regex RGBColorRegEX = new Regex("^\\d{1,3},\\d{1,3},\\d{1,3}$");

        #region Color Related
        public static Color GetRandomColor()
        {
            return Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256));
        }

        public static Color GetColor(string input)
        {
            if (HexColorRegEX.IsMatch(input))
            {
                return ColorTranslator.FromHtml('#' + input);
            }
            else if (RGBColorRegEX.IsMatch(input))
            {
                string[] splitColors = input.Split(',');
                int[] colorsParsed = new int[3];

                for (int a = 0; a < 3; a++)
                {
                    int num = Int32.Parse(splitColors[a]);
                    colorsParsed[a] = num > 255 ? 255 : num;
                }

                return Color.FromArgb(colorsParsed[0], colorsParsed[1], colorsParsed[2]);
            }
            else if (input == "Transparent")
            {
                return Color.FromArgb(0, 0, 0, 0);
            }
            else
            {
                Color color = Color.FromName(input);
                if (color.A == 0)
                    color = GetRandomColor();
                return color;
            }
        }

        public static Color[] GetColorsFromString(string input)
        {
            string[] inputParsed = input.Split(" ");

            if (inputParsed.Length == 0)
                return new Color[] { GetRandomColor() };

            Color[] colors = new Color[inputParsed.Length];

            for (int i = 0; i < inputParsed.Length; i++)
            {
                colors[i] = GetColor(inputParsed[i]);
            }

            return colors;
        }
        #endregion

        #region Drawing Related
        public static LinearGradientBrush PrepareGradientBrush(Rectangle rect, Color[] colors, LinearGradientMode mode)
        {
            int colorsLength = colors.Length;
            LinearGradientBrush brush = new LinearGradientBrush(rect, colors[0], colors[0], mode);

            if (colorsLength > 1)
            {
                ColorBlend blend = new ColorBlend();

                float[] positions = new float[colorsLength];
                for (int i = 0; i < colorsLength; i++)
                    positions[i] = i / (colorsLength - 1f);

                blend.Positions = positions;
                blend.Colors = colors;
                brush.InterpolationColors = blend;
            }

            return brush;
        }
        #endregion

        #region Command Related
        public static void ListFonts()
        {
            InstalledFontCollection fonts = new InstalledFontCollection();
            foreach (FontFamily family in fonts.Families)
                Console.WriteLine(family.Name);
        }

        public static int FindNextCommand(string[] args, int start)
        {
            for (int i = start; i < args.Length; i++)
            {
                if (args[i].StartsWith('-'))
                    return i;
            }
            return args.Length;
        }

        public static string GetAllArgs(string[] args, int start, out int newoffset)
        {
            newoffset = start+1;
            string returnstring = "";
            for (int i = start; i < args.Length; i++)
            {
                if (args[i].StartsWith('-'))
                    break;
                else
                    returnstring += args[i] + " ";

                newoffset = i;
            }
            return returnstring.Trim();
        }

        public static string GetNextArg(string[] args, int index, out int newoffset)
        {
            newoffset = index;
            return args[index];
        }

        public static string AskQuestion(string question, bool trim)
        {
            Console.Write(question + "\n> ");
            if (trim)
                return Console.ReadLine().Trim();
            else
                return Console.ReadLine().Trim();
        }
        public static string AskQuestion(string question)
        {
            return AskQuestion(question, true);
        }
        #endregion
    }
}
