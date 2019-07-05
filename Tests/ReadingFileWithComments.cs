using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc4;
using Xbim.IO.Parser;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ReadingFileWithComments
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\mapped-shape-with-transformation.ifc")]
        public void ScanningTheFile()
        {
            const string file = "mapped-shape-with-transformation.ifc";
            using (var stream = File.OpenRead(file) )
            {
                var s = new Scanner(stream);
                int t;
                do
                {
                    t = s.yylex();
                    var v = s.yytext;
                    Console.WriteLine(@"{0}: {1}", (Tokens)t, v);
                } while (t != (int)Tokens.EOF);
            }

            using (var model = new IO.Memory.MemoryModel(new EntityFactoryIfc4()))
            {
                var errs = model.LoadStep21(file);
                Assert.AreEqual(0, errs);
            }
        }
    }
}
