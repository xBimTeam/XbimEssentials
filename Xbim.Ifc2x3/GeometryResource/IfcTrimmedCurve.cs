#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTrimmedCurve.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;


#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    [IfcPersistedEntityAttribute]
    public class TrimmingSelectList : XbimListSet<IfcTrimmingSelect>
    {
        internal TrimmingSelectList(IPersistIfcEntity owner)
            : base(owner, 2)
        {
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            foreach (IfcTrimmingSelect item in this)
            {
                if (str.Length > 0)
                    str.Append(",");
                IfcCartesianPoint pt = item as IfcCartesianPoint;
                if (pt != null)
                    str.AppendFormat("C{0}", pt);
                else
                    str.AppendFormat("P{0}", item);
            }
            return str.ToString();
        }
    }


    [IfcPersistedEntityAttribute]
    public class IfcTrimmedCurve : IfcBoundedCurve
    {
        public IfcTrimmedCurve()
        {
            _trim1 = new TrimmingSelectList(this);
            _trim2 = new TrimmingSelectList(this);
        }

        #region Fields

        private IfcCurve _basisCurve;
        private TrimmingSelectList _trim1;
        private TrimmingSelectList _trim2;
        private IfcBoolean _senseAgreement = true;
        private IfcTrimmingPreference _masterRepresentation;

        #endregion

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   The curve to be trimmed. For curves with multiple representations any parameter values given as Trim1 or Trim2 refer to the master representation of the BasisCurve only.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcCurve BasisCurve
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _basisCurve;
            }
            set { this.SetModelValue(this, ref _basisCurve, value, v => BasisCurve = v, "BasisCurve"); }
        }


        /// <summary>
        ///   The first trimming point which may be specified as a Cartesian point, as a real parameter or both. Only really used for Ifc compatibility
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1, 2)]
        public TrimmingSelectList Trim1
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _trim1;
            }
            set { this.SetModelValue(this, ref _trim1, value, v => Trim1 = v, "Trim1"); }
        }


        /// <summary>
        ///   The second trimming point which may be specified as a Cartesian point, as a real parameter or both. Only really used for Ifc compatibility
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1, 2)]
        public TrimmingSelectList Trim2
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _trim2;
            }
            set { this.SetModelValue(this, ref _trim2, value, v => Trim2 = v, "Trim2"); }
        }


        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public IfcBoolean SenseAgreement
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _senseAgreement;
            }
            set { this.SetModelValue(this, ref _senseAgreement, value, v => SenseAgreement = v, "SenseAgreement"); }
        }


        [IfcAttribute(5, IfcAttributeState.Mandatory)]
        public IfcTrimmingPreference MasterRepresentation
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _masterRepresentation;
            }
            set
            {
                this.SetModelValue(this, ref _masterRepresentation, value, v => MasterRepresentation = v,
                                           "MasterRepresentation");
            }
        }

        public override IfcDimensionCount Dim
        {
            get { return BasisCurve.Dim; }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _basisCurve = (IfcCurve) value.EntityVal;
                    break;
                case 1:
                    _trim1.Add((IfcTrimmingSelect)value.EntityVal);
                    break;
                case 2:
                    _trim2.Add((IfcTrimmingSelect)value.EntityVal);
                    break;
                case 3:
                    _senseAgreement = value.BooleanVal;
                    break;
                case 4:
                    _masterRepresentation =
                        (IfcTrimmingPreference) Enum.Parse(typeof (IfcTrimmingPreference), value.EnumVal, true);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion



        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            string err = "";

            if (Trim1.Count > 1 && Trim1[0].GetType() == Trim1[1].GetType())
                err +=
                    "WR41: TrimmedCurve: Either a single value is specified for Trim1, or the two trimming values are of different type (point and parameter).";
            if (Trim2.Count > 1 && Trim2[0].GetType() == Trim2[1].GetType())
                err +=
                    "WR42: TrimmedCurve: Either a single value is specified for Trim2, or the two trimming values are of different type (point and parameter).";
            if (!(BasisCurve is IfcLine || BasisCurve is IfcConic))
                err +=
                    "WR43: TrimmedCurve:   Only line and conic curves should be trimmed, not other bounded curves. NOTE: This is an additional constraint of IFC.";
            return err;
        }

        #endregion
    }
}