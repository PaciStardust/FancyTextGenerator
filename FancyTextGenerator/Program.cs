using System;
using System.Collections.Generic;

namespace FancyTextGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Gradient Text Generator by Paci");

            //Debug
            Dictionary<string, string> CompiledArguments = CompileArguments(args);
            foreach (string arg in CompiledArguments.Keys)
            {
                Console.WriteLine($"{arg}: {CompiledArguments[arg]}");
            }

            if (args.Length == 0)
                args = new string[] { "manual" };

            ProcessingMode selectedMode = GetMode(args[0]);
            switch (selectedMode)
            {
                case ProcessingMode.None:
                    return;

                case ProcessingMode.Invalid:
                    Console.WriteLine("Invalid mode, please see \"help\" for more infos");
                    return;

                default: //Should only happen when a valid mode is picked
                    GeneratorVariant gen = selectedMode switch
                    {
                        ProcessingMode.GradientImage    => new GradientImageGenerator(),
                        ProcessingMode.Colorizer        => new ColorizedGenerator(),
                        _                               => new GradientTextGenerator(),
                    };
                    gen.ParseArgs(CompileArguments(args));
                    gen.Generate();
                    return;
            }
        }

        private static ProcessingMode GetMode(string firstArg)
        {
            switch (firstArg)
            {
                case "text":
                    return ProcessingMode.GradientText;

                case "image":
                    return ProcessingMode.GradientImage;

                case "color":
                    return ProcessingMode.Colorizer;

                case "help":
                    ShowHelp();
                    return ProcessingMode.None;

                case "manual":
                    RunManualMode();
                    return ProcessingMode.None;

                case "fontlist":
                    Utility.ListFonts();
                    return ProcessingMode.None;

                default:
                    return ProcessingMode.Invalid;
            }
        }

        private static Dictionary<string, string> CompileArguments(string[] args)
        {
            Dictionary<string, string> compiled = new Dictionary<string, string>();

            for (int a = 1; a < args.Length; a++)
            {
                if (!args[a].StartsWith('-'))
                {
                    if (a == 1)
                        compiled["inputText"] = Utility.GetAllArgs(args, a, out a);
                    continue;
                }

                string argLower = args[a].ToLower();

                if (ArgParser.Args.ContainsKey(argLower))
                {
                    string[] argValues = ArgParser.Args[argLower];

                    string output = argValues[1] switch
                    {
                        "n" => "true",
                        "s" => Utility.GetNextArg(args, a + 1, out a),
                        "m" => Utility.GetAllArgs(args, a + 1, out a),
                        _ => ""
                    };

                    if (output != "")
                        compiled[argValues[0]] = output;
                }
            }

            return compiled;
        }

        private static void RunManualMode()
        {
            string mode = Utility.AskQuestion("What would you like to do?\n H: Help\n FL: Fontlist\n GT: GradientText\n GI: GradientImage").ToLower();

            if (mode == "fl")
            {
                Utility.ListFonts();
                return;
            }
            else if(mode == "gt" || mode == "gi" || mode == "cl")
            {
                Dictionary<string, string> args = new Dictionary<string, string>();

                string answer = Utility.AskQuestion("text?");
                if (answer != "")
                    args["inputText"] = answer;

                foreach (string possibleArg in ArgParser.Args.Keys)
                {
                    answer = Utility.AskQuestion(ArgParser.Args[possibleArg][0] + "?");
                    if (answer != "")
                        args[ArgParser.Args[possibleArg][0]] = answer;
                }

                GeneratorVariant gen = mode switch
                {
                    "gi" => new GradientImageGenerator(),
                    "cl" => new ColorizedGenerator(),
                    _    => new GradientTextGenerator()
                };
                gen.ParseArgs(args);
                gen.Generate();
            } 
            else if(mode == "h")
            {
                ShowHelp();
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine
                ("\n" +
                "--Help--\n\n" +
                "How a command is formed:\n" +
                " [mode] [direct mode param?] [cmd] [cmd param]\n" +
                " Ex: text Hello World -f Comic Sans MS -tcol Red 0,255,0 0000FF -fst B -snm test\n\n" +
                " Res: Text \"Hello World\" in a gradient from Red to Blue to Green in Bold Comic Sans, filename is \"test\"\n" +
                " Prm:      (dir. mode param)               (-tcol)                 (-fst, -f)                    (-snm)\n\n" +
                "Modes:\n" +
                " manual - Manual Input Mode\n" +
                " fontlist - List all available fonts\n" +
                " image - Generate Gradient Image\n" +
                " color - Colorize an Image\n" +
                " text - Generate Gradient Text\n\n" +
                "Direct Mode Params:\n" +
                " TEXT MODE (String +) - Text to be printed out\n" +
                " COLOR MODE (ImgFile) - Link for image to color\n" +
                " IMAGE MODE (ImgFile) - Link for image to gradient\n\n" +
                "Args: (Most only work for some modes)\n"
                );

            foreach (string possibleArg in ArgParser.Args.Keys)
            {
                string[] infos = ArgParser.Args[possibleArg];
                Console.WriteLine($"{infos[0]} ({possibleArg})\n{infos[2]}\n");
            }
        }
    }
}
