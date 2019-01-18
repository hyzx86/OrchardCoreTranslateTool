using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrchardCoreTranslateTool
{
    public class OrchardCoreTranslateHelper
    {
        public static void WriteLog(string text, params object[] args)
        {
            if (args.Length != 0)
            {
                Console.WriteLine(text, args);
            }
            else
            {
                Console.WriteLine(text);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectPath">
        /// C:\Users\hyzx8\Source\Repos\OrchardCore\OrchardCore\src\OrchardCore.Modules\OrchardCore.Liquid\
        /// </param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<PoModel> SearchKeysFromFile(string projectPath, string filePath)
        {
            if (!projectPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                projectPath = projectPath + Path.AltDirectorySeparatorChar;
            }
            var projectRootName = projectPath.Split(new char[] { Path.AltDirectorySeparatorChar },
                                        StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            var msgctxt = filePath.Replace(projectPath, string.Empty);

            var poList = new List<PoModel>();
            var fileContent = File.ReadAllText(filePath);
            if (filePath.ToLower().EndsWith("cshtml"))
            {
                msgctxt = msgctxt.Replace(".cshtml", string.Empty);
                Regex regex = new Regex(@"(([@.*T])|(@T))[[]""(?<KeyName>.*)""(,.*)?[]]");

                foreach (Match match in regex.Matches(fileContent))
                {
                    GroupCollection groups = match.Groups;
                    var strKey = groups["KeyName"].Value;
                    var po = new PoModel { Msgctxt = msgctxt, MsgId = strKey, MsgStr = strKey };
                    WriteLog(po.ToString());
                    poList.Add(po);

                }
            }
            else if (filePath.ToLower().EndsWith("cs"))
            {
                msgctxt = msgctxt.Replace(".cs", string.Empty);
                Regex regex = new Regex(@"([HTS]|TS)[[]""(?<KeyName>.*)((,.* ""[]])|(""[]]))");

                foreach (Match match in regex.Matches(fileContent))
                {
                    GroupCollection groups = match.Groups;

                    var strKey = groups["KeyName"].Value;
                    var po = new PoModel { Msgctxt = msgctxt, MsgId = strKey, MsgStr = strKey };

                    poList.Add(po);
                }
            }

            return poList;
        }

        public static List<PoModel> SearchKeysFromProjectFolder(string projectPath)
        {
            var poList = new List<PoModel>();
            var allFiles = Directory.GetFiles(projectPath, "*.cshtml|*.cs", SearchOption.AllDirectories);
            WriteLog($"Finding in ProjectPath : {projectPath}");
            foreach (var file in allFiles)
            {
                poList.AddRange(SearchKeysFromFile(projectPath, file));
            }

            return poList;
        }

        public static List<PoModel> SearchKeysFromSolutionRoot(string solutionPath)
        {
            var poList = new List<PoModel>();

            WriteLog($"{nameof(solutionPath)} : {solutionPath}");

            var projectPathList = GetAllProjectPath(solutionPath);

            foreach (var projectPath in projectPathList)
            {
                poList.AddRange(SearchKeysFromProjectFolder(projectPath));
            }
            return poList;
        }

        public static List<string> GetAllProjectPath(string solutionPath)
        {
            var allFiles = Directory.GetFiles(solutionPath, "*.csproj|*.cs", SearchOption.AllDirectories);
            var projectPathList = new List<string>();

            foreach (var file in allFiles)
            {
                projectPathList.Add(Path.GetDirectoryName(file));
                WriteLog($"Project Path : {solutionPath}");
            }
            WriteLog($"Total projects : {projectPathList.Count}");
            return projectPathList;
        }
    }
}
