using System;
using System.IO;
using System.Text;
using System.Linq;
namespace OrchardCoreTranslateTool
{
    class Program
    {
        static void Main(string[] args)
        {
        start:
            var path = string.Empty;
            var fileName = "zh-CN.po";
            if (args.Length == 2)
            {
                fileName = args[0];
                path = args[1];
            }
            else if (args.Length < 2 && args.Length != 0)
            {
                Console.WriteLine("args error!");
                Console.ReadKey();
                goto start;
            }
            else
            {
//#if DEBUG
//                fileName = "zh-CN.po";
//                path = @"C:\Users\hyzx8\Source\Repos\OrchardCore\OrchardCore";
//#else
                Console.WriteLine("Please input po file name（jst like zh-CN.po）:");
                fileName = Console.ReadLine();
                if (File.Exists(fileName))
                {
                    Console.WriteLine("The file is allready exists ,if continue will override. input y to continue. or input other content to  exit.");

                    if (Console.ReadLine().ToLower() != "y")
                    {
                        return;
                    }
                }

                Console.WriteLine("Please input Orchard Core solution root path");
                path = Console.ReadLine();

//#endif

            }



            var poList = OrchardCoreTranslateHelper.SearchKeysFromSolutionRoot(path);

            var sb = new StringBuilder();
            foreach (var item in poList)
            {
                sb.Append(item.ToString());
                sb.AppendLine();
            }

            File.WriteAllText(fileName, sb.ToString());
            Console.WriteLine("All Done.");
            Console.WriteLine("Please open the tool folder to find your po file.");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
