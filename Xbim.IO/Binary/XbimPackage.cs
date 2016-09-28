#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.IO
// Filename:    XbimPackage.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Reflection;
using Xbim.Common.Step21;
using Xbim.IO.Parser;

#endregion

namespace Xbim.IO.Binary
{
    [Serializable]
    internal class XbimHeader
    {
        public StepFileDescription StepFileDescriptor = new StepFileDescription("2;1");

        public StepFileName StepFileName = new StepFileName(DateTime.Now)
                                        {
                                            PreprocessorVersion =
                                                string.Format("Xbim.Ifc File Processor version {0}",
                                                              Assembly.GetAssembly(typeof (P21Parser)).GetName().Version),
                                            OriginatingSystem =
                                                string.Format("Xbim version {0}",
                                                              Assembly.GetExecutingAssembly().GetName().Version),
                                        };

        public StepFileSchema StepFileSchema = new StepFileSchema("IFC2X3");

        public Dictionary<string, long> Contents = new Dictionary<string, long>();

        public void SetPosition(string contentName, long pos)
        {
            Contents[contentName] = pos;
        }
    }

    [Serializable]
    public class XbimPackage
    {
        [NonSerialized] private readonly XbimHeader _header = new XbimHeader();

        internal XbimHeader Header
        {
            get { return _header; }
        }

        private readonly Dictionary<string, object> _content = new Dictionary<string, object>();

        public Dictionary<string, object> Content
        {
            get { return _content; }
        }

        public void AddContent(string name, object content)
        {
            _header.Contents.Add(name, 0);
            _content.Add(name, content);
        }
    }
}