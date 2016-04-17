using System;
using System.IO;
using System.Linq;

namespace ConsoleProgram
{
    class CompareFile : IConsoleProgram
    {
        public int Run()
        {
            if (Program.ParaList.Count < 2)
                return -1;

            string filePath1 = Program.ParaList[0];
            string filePath2 = Program.ParaList[1];

            if (!File.Exists(filePath1) || !File.Exists(filePath2))
                return -2;
            try
            {
                var byteAry1 = File.ReadAllBytes(filePath1);
                var byteAry2 = File.ReadAllBytes(filePath2);

                if (byteAry1.Length != byteAry2.Length)
                    return -3;

                if (!byteAry1.SequenceEqual(byteAry2))
                    return -4;

                return 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return -5;
            }
        }
    }
}
