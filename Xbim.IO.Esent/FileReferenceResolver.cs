using System;
using System.Collections.Generic;
using System.IO;

namespace Xbim.IO
{
    class FileReferenceResolver
    {
        internal static string EvaluateRelativePath(string mainDirPath, string absoluteFilePath)
        {
            var
            firstPathParts = mainDirPath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            var
            secondPathParts = absoluteFilePath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

            var sameCounter = 0;
            for (var i = 0; i < Math.Min(firstPathParts.Length,
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

            var newPath = String.Empty;
            for (var i = sameCounter; i < firstPathParts.Length; i++)
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
            for (var i = sameCounter; i < secondPathParts.Length; i++)
            {
                newPath += Path.DirectorySeparatorChar;
                newPath += secondPathParts[i];
            }
            return newPath;
        }

        internal static List<string> ResourceAlternatives(string ResFileName, string ProjFilePrev, string ProjFileCurr)
        {
            var _ret = new List<string>();
            if (File.Exists(ResFileName))
                _ret.Add(ResFileName);
            try
            {
                var ProjFolderCurr = Path.GetDirectoryName(ProjFileCurr);
                var ProjFolderPrev = Path.GetDirectoryName(ProjFilePrev);

                // if project folder has not changed then just return the first option
                if (ProjFolderCurr == ProjFolderPrev)
                {
                    return _ret;
                }
                var RelativeToPrev = EvaluateRelativePath(ProjFolderPrev, ResFileName);
                var AbsoluteToCurr = Path.GetFullPath(Path.Combine(ProjFolderCurr, RelativeToPrev));
                
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
