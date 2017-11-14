#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    Parser.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.IO;
using System.Linq;
using QUT.Gppg;
using Xbim.Common;
using Xbim.Common.Logging;

#endregion

namespace Xbim.IO.Parser
{
    public sealed partial class Scanner : ScanBase
    {
        private readonly ILogger Logger = LoggerFactory.GetLogger();
        public override void yyerror(string format, params object[] args)
        {
            var errmsg = string.Format(format, args);
            Logger.ErrorFormat("Illegal character found at line {0}, column {1}\n{2}", this.yyline, this.yycol, errmsg);
        }
    }
    
    public delegate IPersist CreateEntityEventHandler(string className, long? label, bool headerEntity, out int[] i);

    public delegate long EntityStoreHandler(IPersistEntity ent);

    //public delegate void EntitySelectChangedHandler(StepP21Entity entity);

    public delegate void ParameterSetter(int propIndex, IPropertyValue value, int[] nestedIndex);

   

    public class Part21Entity
    {
        public Part21Entity(string label)
        {
            EntityLabel = Convert.ToInt64(label.TrimStart('#'));
        }

        public Part21Entity(string label, IPersist ent)
            : this(Convert.ToInt64(label.TrimStart('#')), ent)
        {
        }

        public Part21Entity(IPersist ent)
            : this(-1, ent)
        {
        }

        public Part21Entity(long label, IPersist ent)
        {
            EntityLabel = label;
            Entity = ent;
        }

        public IPersist Entity { get; set; }

        public long EntityLabel;


        public int CurrentParamIndex = -1;
        public int[] RequiredParameters;

       
        public ParameterSetter ParameterSetter
        {
            get
            {
                if (RequiredParameters == null || RequiredParameters.Contains(CurrentParamIndex))
                    return (Entity).Parse;
                else
                    return ParameterEater;
            }
        }

        private void ParameterEater(int i, IPropertyValue v, int[] nestedIndex)
        {
        }

        private Type GetItemTypeFromGenericType(Type genericType)
        {
            if (genericType.IsGenericType || genericType.IsInterface)
            {
                var genericTypes = genericType.GetGenericArguments();
                if (genericTypes.GetUpperBound(0) >= 0)
                {
                    return genericTypes[genericTypes.GetUpperBound(0)];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (genericType.BaseType != null)
                {
                    return GetItemTypeFromGenericType(genericType.BaseType);
                }
                else
                {
                    return null;
                }
            }
        }
    }

    abstract partial class P21Parser
    {
        protected P21Parser(Stream strm)
            : base(new Scanner(strm))
        {
        }

        protected P21Parser()
            : base(new Scanner())
        {
        }

        internal virtual void SetErrorMessage()
        {
        }

        protected abstract void CharacterError();
        protected abstract void BeginParse();
        protected abstract void EndParse();
        protected abstract void BeginHeader();
        protected abstract void EndHeader();
        protected abstract void BeginScope();
        protected abstract void EndScope();
        protected abstract void EndSec();
        protected abstract void BeginList();
        protected abstract void EndList();
        protected abstract void BeginComplex();
        protected abstract void EndComplex();
        protected abstract void SetType(string entityTypeName);
        protected abstract void NewEntity(string entityLabel);
        protected abstract void EndEntity();
        protected abstract void EndHeaderEntity();
        protected abstract void SetIntegerValue(string value);
        protected abstract void SetHexValue(string value);
        protected abstract void SetFloatValue(string value);
        protected abstract void SetStringValue(string value);
        protected abstract void SetEnumValue(string value);
        protected abstract void SetBooleanValue(string value);
        protected abstract void SetNonDefinedValue();
        protected abstract void SetOverrideValue();
        protected abstract void SetObjectValue(string value);
        protected abstract void EndNestedType(string value);
        protected abstract void BeginNestedType(string value);
    }
}