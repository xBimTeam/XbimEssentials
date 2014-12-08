using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.IO
{
    /// <summary>
    /// A lightweight structure for obtaining a handle to an Ifc Instance, the instance is not loaded into memory unless the GetInstance function is called
    /// IfcInstanceHandle are specific to the model they were generated from
    /// </summary>
    public struct XbimInstanceHandle
    {
        public int EntityLabel;
        public short EntityTypeId;
        public XbimModel Model;
       
        public static bool operator ==(XbimInstanceHandle a, XbimInstanceHandle b)
        {
            return a.Model == b.Model && a.EntityLabel == b.EntityLabel && a.EntityTypeId == b.EntityTypeId;
        }

        public static bool operator !=(XbimInstanceHandle a, XbimInstanceHandle b)
        {
            return a.Model != b.Model || a.EntityLabel != b.EntityLabel || a.EntityTypeId == b.EntityTypeId;
        }

        public override int GetHashCode()
        {
            return EntityLabel.GetHashCode() ^ Model.GetHashCode();
        }

        public override bool Equals(object b)
        {
            return this.Model == ((XbimInstanceHandle)b).Model && 
                   this.EntityLabel  == ((XbimInstanceHandle)b).EntityLabel && 
                   this.EntityTypeId == ((XbimInstanceHandle)b).EntityTypeId;
        }

        public Type EntityType
        {
            get
            {
                return IfcMetaData.GetType(EntityTypeId);
            }
        }

        public IfcType EntityIfcType
        {
            get
            {
                return  IfcMetaData.IfcType(EntityTypeId);
            }
        }
        
        public bool IsEmpty
        {
            get
            {
                return (Model == null);
            }
        }

        
        public XbimInstanceHandle(XbimModel model, int entityLabel, short type = 0)
        {
            Model = model;
            EntityLabel = entityLabel;
            EntityTypeId= type;
        }

        public XbimInstanceHandle(XbimModel model, int entityLabel, Type type)
        {
            Model = model;
            EntityLabel = entityLabel;
            EntityTypeId = IfcMetaData.IfcTypeId(type);
        }

        public XbimInstanceHandle(XbimModel model, int? label, short? type)
        {
            Model = model;
            this.EntityLabel = label ?? 0;
            this.EntityTypeId = type ?? 0;  
        }

        public XbimInstanceHandle(IPersistIfcEntity entity)
        {
            Model = (XbimModel)entity.ModelOf;
            this.EntityLabel = entity.EntityLabel;
            this.EntityTypeId = IfcMetaData.IfcTypeId(entity);
        }

        public IPersistIfcEntity GetEntity()
        {
            return Model.Instances[EntityLabel];
        }
       
        internal IfcType IfcType()
        {
            return IfcMetaData.IfcType(EntityTypeId);
        }




       
    }
}
