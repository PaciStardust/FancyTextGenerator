using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;

namespace FancyTextGenerator
{
    class GradientImageGenerator : GeneratorVariant
    {
        public string InputLink { set; get; }
        public string ImageLink { get; set; }
        public Color[] Colors { set; get; }
        public LinearGradientMode GradientMode { set; get; }

        public int GrayscaleRate { get; set; }
        public bool InvertGrayscale { get; set; }

        public GradientImageGenerator()
        {
            InputLink =         "";

            ImageLink =         "";
            Colors =            new Color[] { Color.White };
            GradientMode =      LinearGradientMode.Horizontal;

            GrayscaleRate =     50;
            InvertGrayscale =   false;

            Filename =          Utility.r.Next(0, 99999999).ToString();
            FileLocation =      Utility.defaultDirectory;
        }

        public override void ParseArgs(Dictionary<string, string> args)
        {
            InputLink =     ArgParser.GetString(args, "inputText", InputLink).Replace("\\n", "\n");

            Colors =            ArgParser.GetColors(args,   "frontColors",          Colors);
            ImageLink =         ArgParser.GetString(args,   "frontImageLink",       ImageLink);
            GradientMode =      ArgParser.GetGradMode(args, "frontGradientMode",    GradientMode);

            GrayscaleRate =     ArgParser.GetInt(args,      "grayscaleRate",        GrayscaleRate,      0,  100);
            InvertGrayscale =   ArgParser.GetBool(args,     "invertGrayscale",      InvertGrayscale);

            FileLocation =      ArgParser.GetString(args,   "fileLocation",         FileLocation);
            Filename =          ArgParser.GetString(args,   "fileName",             Filename);
        }

        public override void Generate()
        {
            //Setting up mask
            Bitmap mask =       new Bitmap(Image.FromFile(InputLink));
            Graphics graphics = Graphics.FromImage(mask);

            //Gradient matrix
            float[][] grayMatrix = new float[][] {
            new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
            new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
            new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
            new float[] { 0,      0,      0,      1, 0 },
            new float[] { 0,      0,      0,      0, 1 }
            };

            var attributes = new System.Drawing.Imaging.ImageAttributes();
            attributes.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(grayMatrix));
            attributes.SetThreshold(GrayscaleRate / 100f);

            Rectangle rect = new Rectangle(0, 0, mask.Width, mask.Height);
            graphics.DrawImage(mask, rect, 0, 0, mask.Width, mask.Height, GraphicsUnit.Pixel, attributes);

            //Preparing image
            Bitmap image = new Bitmap(mask.Width, mask.Height);
            graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Transparent);

            //Filling image with gradient or picture
            if (ImageLink != "" && File.Exists(ImageLink))
            {
                graphics.DrawImage(Image.FromFile(ImageLink), rect);
            }
            else
            {
                LinearGradientBrush brush = Utility.PrepareGradientBrush(rect, Colors, GradientMode);
                graphics.FillRectangle(brush, rect);
            }

            //Applying mask
            for (int y = 0; y < rect.Height; y++)
            {
                for (int x = 0; x < rect.Width; x++)
                {
                    if ((mask.GetPixel(x, y) == Color.FromArgb(255,255,255,255) && !InvertGrayscale) || (mask.GetPixel(x, y) == Color.FromArgb(255, 0, 0, 0) && InvertGrayscale))
                        image.SetPixel(x, y, Color.Transparent);
                }
            }

            //Saving file
            SaveFile(image);
        }
    }
}
