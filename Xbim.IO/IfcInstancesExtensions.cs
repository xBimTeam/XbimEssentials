using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions;

namespace Xbim.IO
{
    public static class IfcInstancesExtensions
    {
        public static IEnumerable<T> Where<T>(this IfcInstances instances, Expression<Func<T, bool>> expr)
        {
            Type type = typeof(T);
            IfcType ifcType = IfcInstances.IfcEntities[type];
            foreach (Type itemType in ifcType.NonAbstractSubTypes)
            {
                ICollection<long> entities;

                if (instances.TryGetValue(itemType, out entities))
                {
                    bool noIndex = true;
                    XbimIndexedCollection<long> indexColl =
                        entities as XbimIndexedCollection<long>;
                    if (indexColl != null)
                    {
                        //our indexes work from the hash values of that which is indexed, regardless of type
                        object hashRight = null;

                        //indexes only work on equality expressions here
                        if (expr.Body.NodeType == ExpressionType.Equal)
                        {
                            //Equality is a binary expression
                            BinaryExpression binExp = (BinaryExpression)expr.Body;
                            //Get some aliases for either side
                            Expression leftSide = binExp.Left;
                            Expression rightSide = binExp.Right;

                            hashRight = GetRight(leftSide, rightSide);

                            //if we were able to create a hash from the right side (likely)
                            MemberExpression returnedEx = GetIndexablePropertyOnLeft<T>(leftSide);
                            if (returnedEx != null)
                            {
                                //cast to MemberExpression - it allows us to get the property
                                MemberExpression propExp = returnedEx;
                                string property = propExp.Member.Name;
                                if (indexColl.HasIndex(property))
                                {
                                    IEnumerable<long> values = indexColl.GetValues(property, hashRight);
                                    if (values != null)
                                    {
                                        foreach (T item in values.Cast<T>())
                                        {
                                            yield return item;
                                        }
                                        noIndex = false;
                                    }
                                }
                            }
                        }
                        else if (expr.Body.NodeType == ExpressionType.Call)
                        {
                            MethodCallExpression callExp = (MethodCallExpression)expr.Body;
                            if (callExp.Method.Name == "Contains")
                            {
                                Expression keyExpr = callExp.Arguments[0];
                                if (keyExpr.NodeType == ExpressionType.Constant)
                                {
                                    ConstantExpression constExp = (ConstantExpression)keyExpr;
                                    object key = constExp.Value;
                                    if (callExp.Object.NodeType == ExpressionType.MemberAccess)
                                    {
                                        MemberExpression memExp = (MemberExpression)callExp.Object;

                                        string property = memExp.Member.Name;
                                        if (indexColl.HasIndex(property))
                                        {
                                            IEnumerable<long> values = indexColl.GetValues(property, key);
                                            if (values != null)
                                            {
                                                foreach (T item in values.Cast<T>())
                                                {
                                                    yield return item;
                                                }
                                                noIndex = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (noIndex)
                    {
                        
                        Func<T, bool> predicate = expr.Compile();
                        // IEnumerable<T> result = sourceEnum.Where<T>(expr.Compile());
                        foreach (long handle in entities)
                        {
                            T resultItem = (T)instances.GetOrCreateEntity(handle);
                            if (predicate(resultItem)) yield return resultItem;
                        }
                    }
                }
            }
        }


        private static MemberExpression GetIndexablePropertyOnLeft<T>(Expression leftSide)
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


        private static object GetRight(Expression leftSide, Expression rightSide)
        {
            if (leftSide.NodeType == ExpressionType.Call)
            {
                MethodCallExpression call = leftSide as MethodCallExpression;
                if (call.Method.Name == "CompareString")
                {
                    LambdaExpression evalRight = Expression.Lambda(call.Arguments[1], null);
                    //Compile it, invoke it, and get the resulting hash
                    return (evalRight.Compile().DynamicInvoke(null));
                }
            }
            //rightside is where we get our hash...
            switch (rightSide.NodeType)
            {
                //shortcut constants, dont eval, will be faster
                case ExpressionType.Constant:
                    ConstantExpression constExp
                        = (ConstantExpression)rightSide;
                    return (constExp.Value);

                //if not constant (which is provably terminal in a tree), convert back to Lambda and eval to get the hash.
                default:
                    //Lambdas can be created from expressions... yay
                    LambdaExpression evalRight = Expression.Lambda(rightSide, null);
                    //Compile and invoke it, and get the resulting hash
                    return (evalRight.Compile().DynamicInvoke(null));
            }
        }
    }

}
