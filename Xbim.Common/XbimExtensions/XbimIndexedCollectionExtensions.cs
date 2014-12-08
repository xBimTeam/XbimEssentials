#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    XbimIndexedCollectionExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Ex = System.Linq.Expressions.Expression;

#endregion

namespace Xbim.XbimExtensions
{
    public static class XbimIndexedCollectionExtensions
    {
        private static MemberExpression GetIndexablePropertyOnLeft<T>(Expression leftSide,
                                                                      XbimIndexedCollection<T> sourceCollection)
        {
            MemberExpression mex = leftSide as MemberExpression;
            if (leftSide.NodeType == ExpressionType.Call)
            {
                MethodCallExpression call = leftSide as MethodCallExpression;
                if (call.Method.Name == "CompareString")
                {
                    mex = call.Arguments[0] as MemberExpression;
                }
            }

            return mex;
        }


        private static int? GetHashRight(Expression leftSide, Expression rightSide)
        {
            if (leftSide.NodeType == ExpressionType.Call)
            {
                MethodCallExpression call = leftSide as MethodCallExpression;
                if (call.Method.Name == "CompareString")
                {
                    LambdaExpression evalRight = Ex.Lambda(call.Arguments[1], null);
                    //Compile it, invoke it, and get the resulting hash
                    return (evalRight.Compile().DynamicInvoke(null).GetHashCode());
                }
            }
            //rightside is where we get our hash...
            switch (rightSide.NodeType)
            {
                    //shortcut constants, dont eval, will be faster
                case ExpressionType.Constant:
                    ConstantExpression constExp
                        = (ConstantExpression) rightSide;
                    return (constExp.Value.GetHashCode());

                    //if not constant (which is provably terminal in a tree), convert back to Lambda and eval to get the hash.
                default:
                    //Lambdas can be created from expressions... yay
                    LambdaExpression evalRight = Ex.Lambda(rightSide, null);
                    //Compile that mutherf-ker, invoke it, and get the resulting hash
                    return (evalRight.Compile().DynamicInvoke(null).GetHashCode());
            }
        }

        //extend the where when we are working with indexable collections! 
        public static IEnumerable<T> Where<T>
            (
            this XbimIndexedCollection<T> sourceCollection,
            Expression<Func<T, bool>> expr
            )
        {
            //our indexes work from the hash values of that which is indexed, regardless of type
            int? hashRight = null;
            bool noIndex = true;

            //indexes only work on equality expressions here
            if (expr.Body.NodeType == ExpressionType.Equal)
            {
                //Equality is a binary expression
                BinaryExpression binExp = (BinaryExpression) expr.Body;
                //Get some aliases for either side
                Expression leftSide = binExp.Left;
                Expression rightSide = binExp.Right;

                hashRight = GetHashRight(leftSide, rightSide);

                //if we were able to create a hash from the right side (likely)
                MemberExpression returnedEx = GetIndexablePropertyOnLeft(leftSide, sourceCollection);
                if (hashRight.HasValue && returnedEx != null)
                {
                    //cast to MemberExpression - it allows us to get the property
                    MemberExpression propExp = returnedEx;
                    string property = propExp.Member.Name;
                    IEnumerable<T> values = sourceCollection.GetValues(property, hashRight.Value);
                    if (values != null)
                    {
                        foreach (T item in values)
                        {
                            yield return item;
                        }
                        noIndex = false; //we found an index, whether it had values or not is another matter
                    }
                }
            }
            if (noIndex) //no index?  just do it the normal slow way then...
            {
                IEnumerable<T> sourceEnum = sourceCollection.AsEnumerable();
                IEnumerable<T> result = sourceEnum.Where(expr.Compile());
                foreach (T resultItem in result)
                    yield return resultItem;
            }
        }
    }
}