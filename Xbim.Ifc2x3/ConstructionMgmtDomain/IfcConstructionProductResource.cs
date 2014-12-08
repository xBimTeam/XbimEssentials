#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConstructionProductResource.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.ConstructionMgmtDomain
{
    [IfcPersistedEntityAttribute]
    public class IfcConstructionProductResource : IfcConstructionResource
    {
        public override string WhereRule()
        {
            string baseErr = base.WhereRule();
            int cnt = this.ResourceOf.Count();
            if (cnt > 1)
                baseErr +=
                    "WR1 IfcConstructionProductResource : There should only be a single relationship, assigning products to the product resource.\n";
            if (cnt == 1 &&
                (((!this.ResourceOf.First().RelatedObjectsType.HasValue) ||
                  (this.ResourceOf.First().RelatedObjectsType.HasValue &&
                   this.ResourceOf.First().RelatedObjectsType.Value != IfcObjectType.Product))))
                baseErr +=
                    "WR2 IfcConstructionProductResource :  	 If a reference to a resource is given, then through the IfcRelAssignsToResource relationship with the RelatedObjectType PRODUCT.\n";
            return baseErr;
        }
    }
}