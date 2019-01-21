using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergCrowdinFiles
{
    class Program
    {
        static void Main(string[] args)
        {

            string newFileName = "zh-CN.po";

            var files = Directory.EnumerateFiles(@"C:\Users\hyzx8\Documents\Visual Studio 2017\Projects\OrchardCoreTranslateTool\MergCrowdinFiles\zh-CN"
             , "*.po", System.IO.SearchOption.AllDirectories);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in files)
            {
                stringBuilder.Append(File.ReadAllText(item));
                stringBuilder.AppendLine("# From File:" + item);
            }

            File.WriteAllText(newFileName, stringBuilder.ToString());
        }
    }
}
