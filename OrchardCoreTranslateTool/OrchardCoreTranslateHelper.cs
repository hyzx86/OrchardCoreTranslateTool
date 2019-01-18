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
            if (!projectPath.EndsWith('\\'))
            {
                projectPath = projectPath + "\\";
            }
            var projectRootName = projectPath.Split(new char[] { '\\' },
                                        StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            string sourceFileName = projectRootName + "." + filePath.Replace(projectPath, string.Empty);
            var msgctxt = sourceFileName.Replace('\\', '.');
            var poList = new List<PoModel>();
            var fileContent = File.ReadAllText(filePath);
            if (filePath.ToLower().EndsWith("cshtml"))
            {
                msgctxt = msgctxt.Replace(".cshtml", string.Empty);
                Regex regex = new Regex(@"(?:(@?T[[])"")(?'KeyName'.+?)(?:(""[],]))",
                    RegexOptions.Singleline | RegexOptions.Multiline |
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.ExplicitCapture);
                foreach (Match match in regex.Matches(fileContent))
                {
                    GroupCollection groups = match.Groups;
                    var strKey = groups["KeyName"].Value;
                    var po = new PoModel
                    {
                        FullFilePath = sourceFileName,
                        Msgctxt = msgctxt,
                        MsgId = strKey,
                        MsgStr = strKey
                    };
                    WriteLog(po.ToString());
                    poList.Add(po);

                }
            }
            else if (filePath.ToLower().EndsWith("cs"))
            {
                msgctxt = msgctxt.Replace(".cs", string.Empty);
                Regex regex = new Regex(@"([HTS]|TS)[[]""(?<KeyName>.+?)((,.* ""[]])|(""[]]))");


                Regex ns = new Regex(@"(?:namespace\s)(?'namespace'(\w|\d|[.])*)(?:\b)", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                var mc = ns.Match(fileContent);
                var nsName = mc.Groups["namespace"].Value;

                Regex clnm = new Regex(@"(?:\sclass\s)(?'class'(\w|[.])+?)(?:\b)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var clMatch = clnm.Match(fileContent);
                var clname = clMatch.Groups["class"].Value;


                foreach (Match match in regex.Matches(fileContent))
                {
                    GroupCollection groups = match.Groups;

                    var strKey = groups["KeyName"].Value;
                    var po = new PoModel
                    {
                        FullFilePath = sourceFileName,
                        Msgctxt = nsName + "." + clname,
                        MsgId = strKey,
                        MsgStr = strKey
                    };
                    poList.Add(po);
                }
            }

            return poList;
        }

        public static List<PoModel> SearchKeysFromProjectFolder(string projectPath)
        {
            var poList = new List<PoModel>();
            var allFiles = Directory.GetFiles(projectPath, "*.cshtml", SearchOption.AllDirectories).ToList();
            allFiles.AddRange(Directory.GetFiles(projectPath, "*.cs", SearchOption.AllDirectories));

            WriteLog($"Finding in ProjectPath : {projectPath}");

            foreach (var file in allFiles)
            {
                if (file.ToLower().IndexOf("\\obj\\") != -1
                    || file.ToLower().IndexOf("\\debug\\") != -1
                    )
                {
                    continue;
                }
                poList.AddRange(SearchKeysFromFile(projectPath, file));
            }

            return poList;
        }


        public static List<string> GetAllProjectPath(string solutionPath)
        {
            var allFiles = Directory.GetFiles(solutionPath, "*.csproj", SearchOption.AllDirectories);
            var projectPathList = new List<string>();

            foreach (var file in allFiles)
            {
                projectPathList.Add(Path.GetDirectoryName(file));
                WriteLog($"Project Path : {file}");
            }
            WriteLog($"Total projects : {projectPathList.Count}");
            return projectPathList;
        }


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
    }
}
