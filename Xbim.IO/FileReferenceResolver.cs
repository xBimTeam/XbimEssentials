using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xbim.IO
{
    class FileReferenceResolver
    {
        internal static string EvaluateRelativePath(string mainDirPath, string absoluteFilePath)
        {
            string[]
            firstPathParts = mainDirPath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string[]
            secondPathParts = absoluteFilePath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

            int sameCounter = 0;
            for (int i = 0; i < Math.Min(firstPathParts.Length,
            secondPathParts.Length); i++)
            {
                if (
                !firstPathParts[i].ToLower().Equals(secondPathParts[i].ToLower()))
                {
                    break;
                }
                sameCounter++;
            }

            if (sameCounter == 0)
            {
                return absoluteFilePath;
            }

            string newPath = String.Empty;
            for (int i = sameCounter; i < firstPathParts.Length; i++)
            {
                if (i > sameCounter)
                {
                    newPath += Path.DirectorySeparatorChar;
                }
                newPath += "..";
            }
            if (newPath.Length == 0)
            {
                newPath = ".";
            }
            for (int i = sameCounter; i < secondPathParts.Length; i++)
            {
                newPath += Path.DirectorySeparatorChar;
                newPath += secondPathParts[i];
            }
            return newPath;
        }

        internal static List<string> ResourceAlternatives(string ResFileName, string ProjFilePrev, string ProjFileCurr)
        {
            List<string> _ret = new List<string>();
            if (File.Exists(ResFileName))
                _ret.Add(ResFileName);
            try
            {
                string ProjFolderCurr = Path.GetDirectoryName(ProjFileCurr);
                string ProjFolderPrev = Path.GetDirectoryName(ProjFilePrev);

                // if project folder has not changed then just return the first option
                if (ProjFolderCurr == ProjFolderPrev)
                {
                    return _ret;
                }
                string RelativeToPrev = EvaluateRelativePath(ProjFolderPrev, ResFileName);
                string AbsoluteToCurr = Path.GetFullPath(Path.Combine(ProjFolderCurr, RelativeToPrev));
                
                if (AbsoluteToCurr == ResFileName || !File.Exists(AbsoluteToCurr))
                    return _ret;

                _ret.Add(AbsoluteToCurr);
            }
            catch
            {
                // nothing to do
            }
            return _ret;
        }
    }
}
