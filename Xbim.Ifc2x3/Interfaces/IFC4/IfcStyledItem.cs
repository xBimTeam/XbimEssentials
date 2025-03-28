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
using Xbim.Common;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
	public partial class @IfcStyledItem : IIfcStyledItem
	{

		[CrossSchemaAttribute(typeof(IIfcStyledItem), 1)]
		IIfcRepresentationItem IIfcStyledItem.Item 
		{ 
			get
			{
				return Item;
			} 
			set
			{
				Item = value as GeometryResource.IfcRepresentationItem;
				
			}
		}

		[CrossSchemaAttribute(typeof(IIfcStyledItem), 2)]
		IItemSet<IIfcStyleAssignmentSelect> IIfcStyledItem.Styles 
		{ 
			get
			{
			
				return _stylesIfc4 ?? (_stylesIfc4 = new Common.Collections.ExtendedItemSet<IfcPresentationStyleAssignment, IIfcStyleAssignmentSelect>(
                    Styles, 
                    new ItemSet<IIfcStyleAssignmentSelect>(this, 0, -2), 
					StylesToIfc4, 
                    StylesToIfc2X3));
			} 
		}

		//private field to hold any extended data
		private IItemSet<IIfcStyleAssignmentSelect> _stylesIfc4;
		//transformation function to convert/cast IFC2x3 data to appear as IFC4
		private static IIfcStyleAssignmentSelect StylesToIfc4 (IfcPresentationStyleAssignment member)
		{
			return member;
		}

		//transformation function to convert/cast IFC4 data to appear as IFC2x3 if possible
		private IfcPresentationStyleAssignment StylesToIfc2X3 (IIfcStyleAssignmentSelect member){

			if(member is IIfcPresentationStyleSelect ps && Model.CurrentTransaction != null)
			{
				return Model.Instances.New<IfcPresentationStyleAssignment>(pa => ((IIfcPresentationStyleAssignment)pa).Styles.Add(ps));
			}
			return member as IfcPresentationStyleAssignment;
		}

		[CrossSchemaAttribute(typeof(IIfcStyledItem), 3)]
		Ifc4.MeasureResource.IfcLabel? IIfcStyledItem.Name 
		{ 
			get
			{
				if (!Name.HasValue) return null;
				return new Ifc4.MeasureResource.IfcLabel(Name.Value);
			} 
			set
			{
				Name = value.HasValue ? 
					new MeasureResource.IfcLabel(value.Value) :  
					 new MeasureResource.IfcLabel?() ;
				
			}
		}
	//## Custom code
	//##
	}
}