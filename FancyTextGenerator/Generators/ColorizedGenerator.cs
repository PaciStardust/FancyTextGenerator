using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace FancyTextGenerator
{
    class ColorizedGenerator : GeneratorVariant
    {
        public Color[] PickedColors { set; get; }
        public int ColorIntensity { get; set; }
        public string ImageLink { get; set; }
        public string BlendImageLink { get; set; }
        public LinearGradientMode GradientMode { get; set; }

        public ColorizedGenerator()
        {
            PickedColors =      new Color[] { Color.White };
            GradientMode =      LinearGradientMode.Horizontal;
            ColorIntensity =    50;
            ImageLink =         "";
            BlendImageLink =    "";

            Filename =          Utility.r.Next(0, 99999999).ToString();
            FileLocation =      Utility.defaultDirectory;
        }

        public override void ParseArgs(Dictionary<string, string> args)
        {
            ImageLink =         ArgParser.GetString(args,   "inputText",        ImageLink)          .Replace("\\n", "\n");
            PickedColors =      ArgParser.GetColors(args,   "frontColors",      PickedColors);
            ColorIntensity =    ArgParser.GetInt(args,      "colorIntensity",   ColorIntensity,     0,  100);
            BlendImageLink =    ArgParser.GetString(args,   "frontImageLink",   BlendImageLink);
            GradientMode =      ArgParser.GetGradMode(args, "frontGradMode",    GradientMode);

            FileLocation =      ArgParser.GetString(args,   "fileLocation",     FileLocation);
            Filename =          ArgParser.GetString(args,   "fileName",         Filename);
        }

        public override void Generate()
        {
            //Preparing image and graphics
            Bitmap image =      new Bitmap(Image.FromFile(ImageLink));
            Rectangle rect =    new Rectangle(0, 0, image.Width, image.Height);
            Graphics graphics = Graphics.FromImage(image);

            //Draw image on top if link provided, else use gradient
            if (BlendImageLink != "" && File.Exists(BlendImageLink))
            {
                //Loading image, setting up matrix
                Image mask = Image.FromFile(BlendImageLink);

                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = ColorIntensity / 100f;

                //Applying matrix
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //Drawing
                graphics.DrawImage(mask, rect, 0, 0, mask.Width, mask.Height, GraphicsUnit.Pixel, attributes);
            }
            else
            {
                //Preparing colors and brush
                Color[] actualColors =  new Color[PickedColors.Length];
                int opacity =           (int)(255 * ColorIntensity / 100f);

                for (int i = 0; i < PickedColors.Length; i++)
                    actualColors[i] = Color.FromArgb(opacity, PickedColors[i]);
                LinearGradientBrush brush = Utility.PrepareGradientBrush(rect, actualColors, GradientMode);

                //Drawing
                graphics.FillRectangle(brush, rect);
            }

            //Saving file
            SaveFile(image);
        }
    }
}
