#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.IO
// Filename:    XbimInputStream.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#endregion

namespace Xbim.IO.Binary
{
    public class XbimInputStream
    {
        private XbimHeader _header;
        private string _fileName;

        public Dictionary<string, long>.KeyCollection Content
        {
            get
            {
                if (_header != null)
                    return _header.Contents.Keys;
                else return null;
            }
        }

        public int Open(string fileName)
        {
            var errors = 0;
            _fileName = fileName;
            Stream str = null;
            try
            {
                str = File.OpenRead(_fileName);
                var formatter = new BinaryFormatter();
                _header = (XbimHeader) formatter.Deserialize(str);
                str.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error reading Xbim file", e);
            }
            finally
            {
                if (str != null) str.Close();
            }
            return errors;
        }

        public T Load<T>(string contentName) where T : class
        {
            if (_header != null)
            {
                Stream str = null;
                try
                {
                    var pos = _header.Contents[contentName];
                    str = File.OpenRead(_fileName);
                    var formatter = new BinaryFormatter();
                    str.Seek(pos, SeekOrigin.Begin);
                    var res = (T) formatter.Deserialize(str);
                    str.Close();
                    return res;
                }
                catch (Exception e)
                {
                    throw new Exception("Error reading Xbim file", e);
                }
                finally
                {
                    if (str != null) str.Close();
                }
            }
            return null;
        }
    }
}