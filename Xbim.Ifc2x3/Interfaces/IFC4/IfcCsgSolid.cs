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
namespace Xbim.Ifc2x3.GeometricModelResource
{
	public partial class @IfcCsgSolid : IIfcCsgSolid
	{
		IIfcCsgSelect IIfcCsgSolid.TreeRootExpression 
		{ 
			get
			{
				if (TreeRootExpression == null) return null;
				var ifcbooleanresult = TreeRootExpression as IfcBooleanResult;
				if (ifcbooleanresult != null) 
					return ifcbooleanresult;
				var ifccsgprimitive3d = TreeRootExpression as IfcCsgPrimitive3D;
				if (ifccsgprimitive3d != null) 
					return ifccsgprimitive3d;
				return null;
			} 
			set
			{
				if (value == null)
				{
					TreeRootExpression = null;
					return;
				}	
				var ifcbooleanresult = value as IfcBooleanResult;
				if (ifcbooleanresult != null) 
				{
					TreeRootExpression = ifcbooleanresult;
					return;
				}
				var ifccsgprimitive3d = value as IfcCsgPrimitive3D;
				if (ifccsgprimitive3d != null) 
				{
					TreeRootExpression = ifccsgprimitive3d;
					return;
				}
				
			}
		}
	//## Custom code
	//##
	}
}