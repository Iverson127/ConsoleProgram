﻿using System;
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

            Console.WriteLine("Done");
            return Func.Run();
        }

        private static bool GetArguments()
        {
            Console.WriteLine("ConsoleProgram v.0.3.0");
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
                case "3":
                    Func = new GetAutoItReportResult();
                    break;
                default:
                    Func = null;
                    break;
            }
        }
    }
}
