using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Ifc;

namespace Xbim.IO.Tests
{
    [TestClass]
    public class TolerantFormatConversionTests
    {
        // this test is known to fail... There's an encoded backspace in the file name inside the header that cannot be persiste to xml.
        // should the system tolerate the fault?
        // for the time being I've improved the error message so that the problem in the file is easier to resolve.
        // 
        [TestMethod]
        [DeploymentItem("TestFiles\\BackSpaceInFileName.ifc")]
        public void ForConsideration_SaveTroublesomeFile()
        {
            long count;
            int ErrFiles = 0;
            
            var sb = new StringBuilder();

            var outfile = new FileInfo("out.ifczip");
            if (outfile.Exists)
                outfile.Delete();


            var files = new List<FileInfo>();
            files.Add(new FileInfo("BackSpaceInFileName.ifc"));

            foreach (var file in files)
            {            
                try
                {
                    using (var store = IfcStore.Open(file.FullName))
                    {
                        count = store.Instances.Count;
                        store.SaveAs(
                            outfile.FullName, 
                            StorageType.IfcZip | StorageType.IfcXml
                            );
                        store.Close();
                    }
                }
                catch (Exception ex)
                {
                    ErrFiles++;
                    sb.AppendLine($"Failed for {file.FullName}");
                    var e = ex;
                    while (e!= null)
                    {
                        sb.AppendLine($"{e.Message}");
                        e = e.InnerException;
                    }
                    sb.AppendLine("===");
                }
                if (outfile.Exists)
                    outfile.Delete();                       
            }
            var errors = sb.ToString();
            if (!string.IsNullOrEmpty(errors))
            {
                var rep = $"{ErrFiles} files with problems\r\n\r\n{errors}";
                var repoutfile = new FileInfo("report.txt");
                using (var fw = repoutfile.CreateText())
                {
                    fw.WriteLine(rep);
                }
                throw new Exception(rep);
            }
        }
    }
}
