using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace FancyTextGenerator
{
    abstract class GeneratorVariant
    {
        public string Filename { set; get; }
        public string FileLocation { set; get; }

        abstract public void Generate();
        abstract public void ParseArgs(Dictionary<string, string> args);

        protected void SaveFile(Bitmap bitmap)
        {
            if (!Directory.Exists(FileLocation))
            {
                Console.WriteLine("Directory does not exist!");
                FileLocation = Utility.defaultDirectory;
            }

            if (FileLocation == Utility.defaultDirectory && !Directory.Exists(Utility.defaultDirectory))
            {
                Console.WriteLine("Creating default directory...");
                Directory.CreateDirectory(Utility.defaultDirectory);
            }

            string saveDirectory = $"{FileLocation}{Filename}.jpeg";
            bitmap.Save(saveDirectory);
            Console.WriteLine("File saved as " + saveDirectory);
        }
    }
}
