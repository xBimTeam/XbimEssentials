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
namespace Xbim.Ifc2x3.ProductExtension
{
	public partial class @IfcRelAssociatesMaterial : IIfcRelAssociatesMaterial
	{

		[CrossSchemaAttribute(typeof(IIfcRelAssociatesMaterial), 6)]
		IIfcMaterialSelect IIfcRelAssociatesMaterial.RelatingMaterial 
		{ 
			get
			{
				if (RelatingMaterial == null) return null;
				var ifcmaterial = RelatingMaterial as MaterialResource.IfcMaterial;
				if (ifcmaterial != null) 
					//## Handle entity IfcMaterial which is not a part of the target select interface IIfcMaterialSelect in property RelatingMaterial
			        return ifcmaterial;
					//##
				var ifcmateriallist = RelatingMaterial as MaterialResource.IfcMaterialList;
				if (ifcmateriallist != null) 
					return ifcmateriallist;
				var ifcmateriallayersetusage = RelatingMaterial as MaterialResource.IfcMaterialLayerSetUsage;
				if (ifcmateriallayersetusage != null) 
					//## Handle entity IfcMaterialLayerSetUsage which is not a part of the target select interface IIfcMaterialSelect in property RelatingMaterial
			        return ifcmateriallayersetusage;
					//##
				var ifcmateriallayerset = RelatingMaterial as MaterialResource.IfcMaterialLayerSet;
				if (ifcmateriallayerset != null) 
					//## Handle entity IfcMaterialLayerSet which is not a part of the target select interface IIfcMaterialSelect in property RelatingMaterial
                    return ifcmateriallayerset;
					//##
				var ifcmateriallayer = RelatingMaterial as MaterialResource.IfcMaterialLayer;
				if (ifcmateriallayer != null) 
					//## Handle entity IfcMaterialLayer which is not a part of the target select interface IIfcMaterialSelect in property RelatingMaterial
			        return ifcmateriallayer;
					//##
				return null;
			} 
			set
			{
				if (value == null)
				{
					RelatingMaterial = null;
					return;
				}	
				var ifcmaterial = value as MaterialResource.IfcMaterial;
				if (ifcmaterial != null) 
				{
					RelatingMaterial = ifcmaterial;
					return;
				}
				var ifcmateriallayer = value as MaterialResource.IfcMaterialLayer;
				if (ifcmateriallayer != null) 
				{
					RelatingMaterial = ifcmateriallayer;
					return;
				}
				var ifcmateriallayerset = value as MaterialResource.IfcMaterialLayerSet;
				if (ifcmateriallayerset != null) 
				{
					RelatingMaterial = ifcmateriallayerset;
					return;
				}
				var ifcmateriallayersetusage = value as MaterialResource.IfcMaterialLayerSetUsage;
				if (ifcmateriallayersetusage != null) 
				{
					RelatingMaterial = ifcmateriallayersetusage;
					return;
				}
				var ifcmateriallist = value as MaterialResource.IfcMaterialList;
				if (ifcmateriallist != null) 
				{
					RelatingMaterial = ifcmateriallist;
					return;
				}
				
			}
		}
	//## Custom code
	//##
	}
}