using System;
using System.Collections.Generic;

namespace ConsoleProgram
{
    class Program
    {
        public static List<string> ParaList = new List<string>();
        static IConsoleProgram Func;

        static int Main(string[] args)
        {
            bool result = GetArguments();
            if(!result)
                return 1;

            if (Func == null)
                return 1;

            result = Func.Run();

            // Print result
            //Console.WriteLine(result? "True":"False");
            return result ? 0 : 1;
        }

        private static bool GetArguments()
        {
            Console.WriteLine("Copyright (C) 2016 by Iverson.Hong\n");
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2)
                return false;

            FuncFactory(args[1]);
            ParaList.AddRange(args);
            ParaList.RemoveRange(0, 2);

            return true;
        }

        private static void FuncFactory(string funcCode)
        {
            switch (funcCode)
            {
                case "1":
                    Func = new CompareFile();
                    break;
                case "2":
                    Func = new AutoItReportGenerator();
                    break;
                default:
                    Func = null;
                    break;
            }
        }
    }
}
