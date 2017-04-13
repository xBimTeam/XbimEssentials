#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.IO
// Filename:    XbimOutputStream.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace Xbim.IO.Binary
{
    public class XbimOutputStream
    {
        private readonly XbimPackage _package = new XbimPackage();

        public void AddContent(string name, object content)
        {
            _package.AddContent(name, content);
        }

        public int Store(string fileName)
        {
            var errors = 0;
            Stream str = null;
            try
            {
                str = File.Open(fileName, FileMode.Create, FileAccess.Write);
                var formatter = new BinaryFormatter();


                formatter.Serialize(str, _package.Header);

                foreach (var item in _package.Content)
                {
                    _package.Header.SetPosition(item.Key, str.Position);
                    formatter.Serialize(str, item.Value);
                }
                str.Seek(0, SeekOrigin.Begin);
                //rewrite header with real positions
                formatter.Serialize(str, _package.Header);
                str.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error writing Xbim file", e);
            }
            finally
            {
                if (str != null) str.Close();
            }
            return errors;
        }
    }
}