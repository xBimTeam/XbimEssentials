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

using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.XbimExtensions.Interfaces
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
        bool Activated { get; }
        void Activate(bool write);
        void Bind(IModel model, int entityLabel, bool activated);
        IModel ModelOf { get; }
        int EntityLabel { get; }
    }
}