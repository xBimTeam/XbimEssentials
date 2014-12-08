#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    StepP21Token.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.XbimExtensions
{
    public class StepP21Token
    {
        public StepP21Token(string val)
        {
            Value = val;
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(StepP21Token obj)
        {
            return (obj.Value);
        }
    }
}