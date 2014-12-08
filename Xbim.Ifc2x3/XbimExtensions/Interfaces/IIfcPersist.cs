#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IIfcPersist.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using Xbim.XbimExtensions.Parser;

#endregion

namespace Xbim.XbimExtensions
{
    public interface IPersistIfc
    {
        void IfcParse(int propIndex, IPropertyValue value);

        /// <summary>
        ///   Validates the object against the Ifc schema where rule, returns empty string if the object complies or an error string indicating the reason for compliance failure
        /// </summary>
        /// <returns></returns>
        string WhereRule();
    }

    public interface IPersistIfcEntity : IPersistIfc
    {
#if SupportActivation
        bool Activated { get; }
        void Activate(bool write);
        void Bind(IModel model, long entityLabel);
        IModel ModelOf { get; }
        long EntityLabel { get; }
#endif
    }
}