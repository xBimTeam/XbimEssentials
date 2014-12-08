#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFaceBasedSurfaceModel.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.Ifc2x3.TopologyResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    [IfcPersistedEntityAttribute]
    public class IfcFaceBasedSurfaceModel : IfcGeometricRepresentationItem, IfcSurfaceOrFaceSurface,
                                            IFaceBasedModelCollection
    {
        public IfcFaceBasedSurfaceModel()
        {
            _fbsmFaces = new XbimSet<IfcConnectedFaceSet>(this);
        }

        #region Fields

        private XbimSet<IfcConnectedFaceSet> _fbsmFaces;

        #endregion

        /// <summary>
        ///   The set of faces arcwise connected along common edges or vertices.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public XbimSet<IfcConnectedFaceSet> FbsmFaces
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _fbsmFaces;
            }
            set { this.SetModelValue(this, ref _fbsmFaces, value, v => FbsmFaces = v, "FbsmFaces"); }
        }

        public IfcDimensionCount Dim
        {
            get { return new IfcDimensionCount(3); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
            {
                _fbsmFaces.Add((IfcConnectedFaceSet) value.EntityVal);
            }
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }


        public override string WhereRule()
        {
            return "";
        }

        #region IFaceBasedModelCollection Members

        IEnumerable<IFaceBasedModel> IFaceBasedModelCollection.FaceModels
        {
            get { return FbsmFaces.Cast<IFaceBasedModel>(); }
        }

        #endregion
    }
}