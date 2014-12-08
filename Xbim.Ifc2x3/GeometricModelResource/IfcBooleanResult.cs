#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBooleanResult.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   A Boolean result is the result of a regularized operation on two solids to create a new solid.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A Boolean result is the result of a regularized operation on two solids to create a new solid. Valid operations are regularized union, regularized intersection, and regularized difference. For purpose of Boolean operations, a solid is considered to be a regularized set of points. The final Boolean result depends upon the operation and the two operands. In the case of the difference operator the order of the operands is also significant. The operator can be either union, intersection or difference. The effect of these operators is described below: 
    ///   Union on two solids is the new solid that is the regularization of the set of all points that are in either the first operand or the second operand or in both. 
    ///   Intersection on two solids is the new solid that is the regularization of the set of all points that are in both the first operand and the second operand. 
    ///   The result of the difference operation on two solids is the regularization of the set of all points which are in the first operand, but not in the second operand. 
    ///   NOTE Corresponding STEP entity: boolean_result. The derived attribute Dim has been added at this level and was therefore demoted from the geometric_representation_item. Please refer to ISO/IS 10303-42:1994, p.175 for the final definition of the formal standard. 
    ///   HISTORY: New class in IFC Release 1.5.1.
    ///   Formal Propositions:
    ///   WR1   :   The dimensionality of the first operand shall be the same as the dimensionality of the second operand.
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public class IfcBooleanResult : IfcGeometricRepresentationItem, IfcBooleanOperand, IfcCsgSelect
    {
        #region Part 21 Step file Parse routines

        private IfcBooleanOperator _operator;
        private IfcBooleanOperand _firstOperand;
        private IfcBooleanOperand _secondOperand;

        /// <summary>
        ///   The Boolean operator used in the operation to create the result.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcBooleanOperator Operator
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _operator;
            }
            set { this.SetModelValue(this, ref _operator, value, v => Operator = v, "Operator"); }
        }

        /// <summary>
        ///   The first operand to be operated upon by the Boolean operation.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcBooleanOperand FirstOperand
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _firstOperand;
            }
            set { this.SetModelValue(this, ref _firstOperand, value, v => FirstOperand = v, "FirstOperand"); }
        }

        /// <summary>
        ///   The second operand specified for the operation.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcBooleanOperand SecondOperand
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _secondOperand;
            }
            set { this.SetModelValue(this, ref _secondOperand, value, v => SecondOperand = v, "SecondOperand"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _operator = (IfcBooleanOperator) Enum.Parse(typeof (IfcBooleanOperator), value.EnumVal, true);
                    break;
                case 1:
                    _firstOperand = (IfcBooleanOperand) value.EntityVal;
                    break;
                case 2:
                    _secondOperand = (IfcBooleanOperand) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        /// <summary>
        ///   Derived.   The space dimensionality of this entity. It is identical with the space dimensionality of the first operand. A where rule ensures that both operands have the same space dimensionality.
        /// </summary>
        public int Dim
        {
            get 
            {
                
                return _firstOperand.Dim; 
            }
        }

        #region Ifc Schema Validation Methods

        public override string WhereRule()
        {
            if (_firstOperand.Dim != _secondOperand.Dim)
                return
                    "WR1: BooleanResult: The dimensionality of the first operand shall be the same as the dimensionality of the second operand.";
            return "";
        }

        #endregion
    }
}