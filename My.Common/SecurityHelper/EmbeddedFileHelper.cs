using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace My.Common
{
    public static class EmbeddedFileHelper
    {
        public static byte[] GetEmbeddedFileBytes(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly item in from x in assemblies
                                      where x.FullName != null && !x.FullName.StartsWith("Anonymously")
                                      select x)
            {
                try
                {
                    using (Stream stream = item.GetManifestResourceStream(name))
                    {
                        byte[] bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, (int)stream.Length);
                        return bytes;
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        public static List<string> GetEmbeddedFileContents(string name)
        {
            List<string> lines = new List<string>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly item in from x in assemblies
                                      where x.FullName != null && !x.FullName.StartsWith("Anonymously")
                                      select x)
            {
                try
                {
                    using (Stream stream = item.GetManifestResourceStream(name))
                    {
                        if (stream != null)
                        {
                            StreamReader textStreamReader = new StreamReader(stream, new UTF8Encoding());
                            for (string line = textStreamReader.ReadLine(); line != null; line = textStreamReader.ReadLine())
                            {
                                if (line.Trim().Length > 0)
                                {
                                    lines.Add(line);
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }
            return lines;
        }

        public static Stream GetEmbeddedFileStream(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly item in from x in assemblies
                                      where x.FullName != null && !x.FullName.StartsWith("Anonymously")
                                      select x)
            {
                try
                {
                    return item.GetManifestResourceStream(name);
                }
                catch
                {
                }
            }
            return null;
        }

        public static string GetEmbeddedFileText(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly item in from x in assemblies
                                      where x.FullName != null && !x.FullName.StartsWith("Anonymously")
                                      select x)
            {
                try
                {
                    using (Stream stream = item.GetManifestResourceStream(name))
                    {
                        StreamReader textStreamReader = new StreamReader(stream, new UTF8Encoding());
                        return textStreamReader.ReadToEnd();
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        public static List<string> ToStatements(List<string> sqlContentLines)
        {
            StringComparison noCase = StringComparison.OrdinalIgnoreCase;
            List<string> statements = new List<string>();
            StringBuilder statementBuilder = new StringBuilder();
            int i = 0;
            for (int count = sqlContentLines.Count; i < count; i++)
            {
                string line = sqlContentLines[i];
                if (line.Trim().Equals("GO", noCase))
                {
                    statements.Add(statementBuilder.ToString());
                    statementBuilder.Clear();
                }
                else
                {
                    statementBuilder.AppendLine(line);
                }
            }
            if (statementBuilder.Length > 0)
            {
                statements.Add(statementBuilder.ToString());
            }
            return statements;
        }
    }

}
