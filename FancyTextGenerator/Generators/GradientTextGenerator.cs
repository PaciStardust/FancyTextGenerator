using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;

namespace FancyTextGenerator
{
    class GradientTextGenerator : GeneratorVariant
    {
        //General
        public string InputText { set; get; }
        public int PaddingW { set; get; }
        public int PaddingH { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }

        //Text related
        public int FontSize { set; get; }
        public string SelectedFont { set; get; }
        public FontStyle SelectedFontStyle { set; get; }
        public StringAlignment StringAlignment { set; get; }
        public string TextImageLink { get; set; }
        public bool TextImageNoResize { get; set; }
        public Color[] TextColors { set; get; }
        public LinearGradientMode TextGradientMode { set; get; }

        //Back related
        public string BackImageLink { get; set; }
        public ScalingMode BackScaleMode { get; set; }
        public int BackScaleW { get; set; }
        public int BackScaleH { get; set; }
        public int BackOffsetW { get; set; }
        public int BackOffsetH { get; set; }
        public Color[] BackColors { set; get; }
        public LinearGradientMode BackGradientMode { set; get; }


        public GradientTextGenerator()
        {
            InputText =         "Placeholder Text";

            FontSize =          160;
            SelectedFont =      "Arial";
            SelectedFontStyle = FontStyle.Regular;
            StringAlignment =   StringAlignment.Center;
            TextColors =        new Color[] { Color.White };
            TextImageLink =     "";
            TextImageNoResize = false;
            TextGradientMode =  LinearGradientMode.Horizontal;

            BackColors =        new Color[] { Color.Transparent };
            BackScaleMode =     ScalingMode.None;
            BackScaleW =        100;
            BackScaleH =        100;
            BackOffsetW =       50;
            BackOffsetH =       50;
            BackImageLink =     "";
            BackGradientMode =  LinearGradientMode.Horizontal;

            PaddingW =          0;
            PaddingH =          0;
            Width =             -1;
            Height =            -1;

            Filename =          Utility.r.Next(0,99999999).ToString();
            FileLocation =      Utility.defaultDirectory;
        }

        public override void ParseArgs(Dictionary<string, string> args)
        {
            InputText =         ArgParser.GetString(args,       "inputText",            InputText)          .Replace("\\n", "\n");

            FontSize =          ArgParser.GetInt(args,          "textSize",             FontSize,           1,  320);
            SelectedFont =      ArgParser.GetString(args,       "textFont",             SelectedFont);
            SelectedFontStyle = ArgParser.GetFontStyle(args,    "textStyle",            SelectedFontStyle);
            StringAlignment =   ArgParser.GetAlignment(args,    "textAlignment",        StringAlignment);

            TextColors =        ArgParser.GetColors(args,       "frontColors",          TextColors);
            TextImageLink =     ArgParser.GetString(args,       "frontImageLink",       TextImageLink);
            TextImageNoResize = ArgParser.GetBool(args,         "frontImageNoResize",   TextImageNoResize);
            TextGradientMode =  ArgParser.GetGradMode(args,     "frontGradientMode",    TextGradientMode);

            BackImageLink =     ArgParser.GetString(args,       "backImageLink",        BackImageLink);
            BackScaleMode =     ArgParser.GetScaleMode(args,    "backScaleMode",        BackScaleMode);
            BackColors =        ArgParser.GetColors(args,       "backColors",           BackColors);
            BackGradientMode =  ArgParser.GetGradMode(args,     "backGradientMode",     BackGradientMode);

            BackScaleW =        ArgParser.GetInt(args,          "backScaleW",           BackScaleW,         1,  400);
            BackScaleH =        ArgParser.GetInt(args,          "backScaleH",           BackScaleH,         1,  400);
            BackOffsetW =       ArgParser.GetInt(args,          "backOffsetW",          BackOffsetW,        0,  100);
            BackOffsetH =       ArgParser.GetInt(args,          "backOffsetH",          BackOffsetH,        0,  100);

            PaddingW =          ArgParser.GetInt(args,          "paddingW",             PaddingW,           0,  1024);
            PaddingH =          ArgParser.GetInt(args,          "paddingH",             PaddingH,           0,  1024);
            Width =             ArgParser.GetInt(args,          "width",                Width,              1,  4096);
            Height =            ArgParser.GetInt(args,          "height",               Height,             1,  4096);

            FileLocation =      ArgParser.GetString(args,       "fileLocation",         FileLocation);
            Filename =          ArgParser.GetString(args,       "fileName",             Filename);
        }

        public override void Generate()
        {
            Console.WriteLine("Setting up various things...");
            Font font = new Font(SelectedFont, FontSize, SelectedFontStyle);

            //We generate this just so we can calculate the size of the text, because c# does not allow me to otherwise
            Bitmap bitmap =     new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(bitmap);

            //Setting up Sizes
            Size textSizes =    graphics.MeasureString(InputText, font).ToSize();
            Size sizes =        new Size(Width == -1 ? textSizes.Width : Width, Height == -1 ? textSizes.Height : Height);
            sizes +=            new Size(PaddingW * 2, PaddingH * 2);

            //Setting up actual bitmap and graphics
            bitmap =    new Bitmap(sizes.Width, sizes.Height);
            graphics =  Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);

            //Preparing Rect and String Alignment
            Rectangle rect =    new Rectangle(0, 0, sizes.Width, sizes.Height);
            StringFormat sf =   new StringFormat();
            sf.LineAlignment = StringAlignment;
            sf.Alignment = StringAlignment;

            //Setting up gradient for background, drawing it
            Console.WriteLine("Drawing Background...");
            LinearGradientBrush brush = Utility.PrepareGradientBrush(rect, BackColors, BackGradientMode);
            graphics.FillRectangle(brush, rect);

            //Check if theres an image link and the file exist, try drawing it
            if (BackImageLink != "" && File.Exists(BackImageLink))
            {
                Image image = Image.FromFile(BackImageLink);
                Size drawSize = new Size(image.Width, image.Height);

                switch(BackScaleMode)
                {
                    case ScalingMode.Both:
                        drawSize = new Size(rect.Width, rect.Height);
                        break;

                    case ScalingMode.Height:
                        drawSize = new Size(image.Width * rect.Height / image.Height, rect.Height);
                        break;

                    case ScalingMode.Width:
                        drawSize = new Size(rect.Width, image.Height * rect.Width / image.Width);
                        break;
                }

                drawSize = new Size((int)(drawSize.Width * BackScaleW * 0.01), (int)(drawSize.Height * BackScaleH * 0.01));
                Rectangle drawRect = new Rectangle(new Point((int)((rect.Width - drawSize.Width) * BackOffsetH * 0.01), (int)((rect.Height - drawSize.Height) * BackOffsetW * 0.01)), drawSize);
                graphics.DrawImage(image, drawRect);
            }

            //Setting up gradient for text, drawing it, kinda ugly repetitive code
            Console.WriteLine("Drawing Text...");

            PointF offset = StringAlignment switch
            {
                StringAlignment.Center => new PointF(rect.Width / 2, rect.Height / 2),
                StringAlignment.Far => new PointF(rect.Width - PaddingW, rect.Height - PaddingH),
                _ => new PointF(PaddingW, PaddingH)
            };

            //Check if theres an image link and the file exist, try drawing it, else draw gradient text
            if (TextImageLink != "" && File.Exists(TextImageLink))
            {
                Bitmap tempBitmap = new Bitmap(sizes.Width, sizes.Height);
                Graphics tempGraphics = Graphics.FromImage(tempBitmap);

                tempGraphics.DrawImage(Image.FromFile(TextImageLink), TextImageNoResize ? rect : new Rectangle(new Point(PaddingW, PaddingH), textSizes));

                graphics.DrawString(InputText, font, new TextureBrush(tempBitmap), offset, sf);
            }
            else
            {
                brush = Utility.PrepareGradientBrush(rect, TextColors, TextGradientMode);
                graphics.DrawString(InputText, font, brush, offset, sf);
            }

            //Saving file
            SaveFile(bitmap);
        }
    }
}
