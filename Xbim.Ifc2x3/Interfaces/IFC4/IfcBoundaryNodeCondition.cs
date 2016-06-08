// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool Xbim.CodeGeneration 
//  
//     Changes to this file may cause incorrect behaviour and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using Xbim.Ifc4.Interfaces;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.StructuralLoadResource
{
	public partial class @IfcBoundaryNodeCondition : IIfcBoundaryNodeCondition
	{
		IIfcTranslationalStiffnessSelect IIfcBoundaryNodeCondition.TranslationalStiffnessX 
		{ 
			get
			{
				//## Handle return of TranslationalStiffnessX for which no match was found
                if (_translationalStiffnessX != null)
                    return _translationalStiffnessX;

                var value = LinearStiffnessX;
                if (!value.HasValue) return null;
                return new Ifc4.MeasureResource.IfcLinearStiffnessMeasure(value.Value);
				//##
			} 
			set
			{
				//## Handle setting of TranslationalStiffnessX for which no match was found
                if (value == null)
                {
                    LinearStiffnessX = null;
                    if (_translationalStiffnessX != null)
                        SetValue(v => _translationalStiffnessX = v, _translationalStiffnessX, null, "TranslationalStiffnessX", byte.MaxValue);
                    return;

                }
                if (value is Ifc4.MeasureResource.IfcLinearStiffnessMeasure)
                {
                    LinearStiffnessX = new MeasureResource.IfcLinearStiffnessMeasure((Ifc4.MeasureResource.IfcLinearStiffnessMeasure)value);
                    if (_translationalStiffnessX != null)
                        SetValue(v => _translationalStiffnessX = v, _translationalStiffnessX, null, "TranslationalStiffnessX", byte.MaxValue);
                    return;
                }

                SetValue(v => _translationalStiffnessX = v, _translationalStiffnessX, value, "TranslationalStiffnessX", byte.MaxValue);
                return;
				//##
				NotifyPropertyChanged("TranslationalStiffnessX");
				
			}
		}
		IIfcTranslationalStiffnessSelect IIfcBoundaryNodeCondition.TranslationalStiffnessY 
		{ 
			get
			{
				//## Handle return of TranslationalStiffnessY for which no match was found
                if (_translationalStiffnessY != null)
                    return _translationalStiffnessY;

                var value = LinearStiffnessY;
                if (!value.HasValue) return null;
                return new Ifc4.MeasureResource.IfcLinearStiffnessMeasure(value.Value);
                //##
			} 
			set
			{
				//## Handle setting of TranslationalStiffnessY for which no match was found
                if (value == null)
                {
                    LinearStiffnessY = null;
                    if (_translationalStiffnessY != null)
                        SetValue(v => _translationalStiffnessY = v, _translationalStiffnessY, null, "TranslationalStiffnessY", byte.MaxValue);
                    return;

                }
                if (value is Ifc4.MeasureResource.IfcLinearStiffnessMeasure)
                {
                    LinearStiffnessY = new MeasureResource.IfcLinearStiffnessMeasure((Ifc4.MeasureResource.IfcLinearStiffnessMeasure)value);
                    if (_translationalStiffnessY != null)
                        SetValue(v => _translationalStiffnessY = v, _translationalStiffnessY, null, "TranslationalStiffnessY", byte.MaxValue);
                    return;
                }

                SetValue(v => _translationalStiffnessY = v, _translationalStiffnessY, value, "TranslationalStiffnessY", byte.MaxValue);
                return;
                //##
				NotifyPropertyChanged("TranslationalStiffnessY");
				
			}
		}
		IIfcTranslationalStiffnessSelect IIfcBoundaryNodeCondition.TranslationalStiffnessZ 
		{ 
			get
			{
				//## Handle return of TranslationalStiffnessZ for which no match was found
                if (_translationalStiffnessZ != null)
                    return _translationalStiffnessZ;

                var value = LinearStiffnessZ;
                if (!value.HasValue) return null;
                return new Ifc4.MeasureResource.IfcLinearStiffnessMeasure(value.Value);
                //##
			} 
			set
			{
				//## Handle setting of TranslationalStiffnessZ for which no match was found
                if (value == null)
                {
                    LinearStiffnessZ = null;
                    if (_translationalStiffnessZ != null)
                        SetValue(v => _translationalStiffnessZ = v, _translationalStiffnessZ, null, "TranslationalStiffnessZ", byte.MaxValue);
                    return;

                }
                if (value is Ifc4.MeasureResource.IfcLinearStiffnessMeasure)
                {
                    LinearStiffnessZ = new MeasureResource.IfcLinearStiffnessMeasure((Ifc4.MeasureResource.IfcLinearStiffnessMeasure)value);
                    SetValue(v => _translationalStiffnessZ = v, _translationalStiffnessZ, null, "TranslationalStiffnessZ", byte.MaxValue);
                    return;
                }

                SetValue(v => _translationalStiffnessZ = v, _translationalStiffnessZ, value, "TranslationalStiffnessZ", byte.MaxValue);
                return;
                //##
				NotifyPropertyChanged("TranslationalStiffnessZ");
				
			}
		}
		IIfcRotationalStiffnessSelect IIfcBoundaryNodeCondition.RotationalStiffnessX 
		{ 
			get
			{
				//## Handle return of RotationalStiffnessX for which no match was found
                if (_rotationalStiffnessX4 != null)
                    return _rotationalStiffnessX4;

                var value = RotationalStiffnessX;
                if (!value.HasValue) return null;
                return new Ifc4.MeasureResource.IfcRotationalStiffnessMeasure(value.Value);
				//##
			} 
			set
			{
				//## Handle setting of RotationalStiffnessX for which no match was found
                if (value == null)
                {
                    RotationalStiffnessX = null;
                    if (_rotationalStiffnessX4 != null)
                        SetValue(v => _rotationalStiffnessX4 = v, _rotationalStiffnessX4, null, "RotationalStiffnessX", byte.MaxValue);
                    return;

                }
                if (value is Ifc4.MeasureResource.IfcRotationalStiffnessMeasure)
                {
                    RotationalStiffnessX = new MeasureResource.IfcRotationalStiffnessMeasure((Ifc4.MeasureResource.IfcRotationalStiffnessMeasure)value);
                    if (_rotationalStiffnessX4 != null)
                        SetValue(v => _rotationalStiffnessX4 = v, _rotationalStiffnessX4, null, "RotationalStiffnessX", byte.MaxValue);
                    return;
                }

                SetValue(v => _rotationalStiffnessX4 = v, _rotationalStiffnessX4, value, "RotationalStiffnessX", byte.MaxValue);
				//##
				
			}
		}
		IIfcRotationalStiffnessSelect IIfcBoundaryNodeCondition.RotationalStiffnessY 
		{ 
			get
			{
				//## Handle return of RotationalStiffnessY for which no match was found
                if (_rotationalStiffnessY4 != null)
                    return _rotationalStiffnessY4;

                var value = RotationalStiffnessY;
                if (!value.HasValue) return null;
                return new Ifc4.MeasureResource.IfcRotationalStiffnessMeasure(value.Value);
                //##
			} 
			set
			{
				//## Handle setting of RotationalStiffnessY for which no match was found
                if (value == null)
                {
                    RotationalStiffnessY = null;
                    if (_rotationalStiffnessY4 != null)
                        SetValue(v => _rotationalStiffnessY4 = v, _rotationalStiffnessY4, null, "RotationalStiffnessY", byte.MaxValue);
                    return;

                }
                if (value is Ifc4.MeasureResource.IfcRotationalStiffnessMeasure)
                {
                    RotationalStiffnessY = new MeasureResource.IfcRotationalStiffnessMeasure((Ifc4.MeasureResource.IfcRotationalStiffnessMeasure)value);
                    if (_rotationalStiffnessY4 != null)
                        SetValue(v => _rotationalStiffnessY4 = v, _rotationalStiffnessY4, null, "RotationalStiffnessY", byte.MaxValue);
                    return;
                }

                SetValue(v => _rotationalStiffnessY4 = v, _rotationalStiffnessY4, value, "RotationalStiffnessY", byte.MaxValue);
                //##
				
			}
		}
		IIfcRotationalStiffnessSelect IIfcBoundaryNodeCondition.RotationalStiffnessZ 
		{ 
			get
			{
				//## Handle return of RotationalStiffnessZ for which no match was found
                if (_rotationalStiffnessZ4 != null)
                    return _rotationalStiffnessZ4;

                var value = RotationalStiffnessZ;
                if (!value.HasValue) return null;
                return new Ifc4.MeasureResource.IfcRotationalStiffnessMeasure(value.Value);
                //##
			} 
			set
			{
				//## Handle setting of RotationalStiffnessZ for which no match was found
                if (value == null)
                {
                    RotationalStiffnessZ = null;
                    if (_rotationalStiffnessZ4 != null)
                        SetValue(v => _rotationalStiffnessZ4 = v, _rotationalStiffnessZ4, null, "RotationalStiffnessZ", byte.MaxValue);
                    return;

                }
                if (value is Ifc4.MeasureResource.IfcRotationalStiffnessMeasure)
                {
                    RotationalStiffnessZ = new MeasureResource.IfcRotationalStiffnessMeasure((Ifc4.MeasureResource.IfcRotationalStiffnessMeasure)value);
                    if (_rotationalStiffnessZ4 != null)
                        SetValue(v => _rotationalStiffnessZ4 = v, _rotationalStiffnessZ4, null, "RotationalStiffnessZ", byte.MaxValue);
                    return;
                }

                SetValue(v => _rotationalStiffnessZ4 = v, _rotationalStiffnessZ4, value, "RotationalStiffnessZ", byte.MaxValue);
                //##
				
			}
		}
	//## Custom code
        private IIfcTranslationalStiffnessSelect _translationalStiffnessX;
        private IIfcTranslationalStiffnessSelect _translationalStiffnessY;
        private IIfcTranslationalStiffnessSelect _translationalStiffnessZ;

        private IIfcRotationalStiffnessSelect _rotationalStiffnessX4;
        private IIfcRotationalStiffnessSelect _rotationalStiffnessY4;
        private IIfcRotationalStiffnessSelect _rotationalStiffnessZ4;
	    //##
	}
}