using System;
using Xbim.Common;
using Xbim.Common.Metadata;

namespace Xbim.IO.Esent
{
    /// <summary>
    /// A lightweight structure for obtaining a handle to an Ifc Instance, the instance is not loaded into memory unless the GetInstance function is called
    /// IfcInstanceHandle are specific to the model they were generated from
    /// </summary>
    public struct XbimInstanceHandle
    {
        public readonly int EntityLabel;
        public short EntityTypeId;
        public readonly EsentModel Model;
       
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
            return Model == ((XbimInstanceHandle)b).Model && 
                   EntityLabel  == ((XbimInstanceHandle)b).EntityLabel && 
                   EntityTypeId == ((XbimInstanceHandle)b).EntityTypeId;
        }

        public Type EntityType
        {
            get
            {
                return Model.Metadata.GetType(EntityTypeId);
            }
        }

        public ExpressType EntityExpressType
        {
            get
            {
                return Model.Metadata.ExpressType(EntityTypeId);
            }
        }
        
        public bool IsEmpty
        {
            get
            {
                return (Model == null);
            }
        }

        
        public XbimInstanceHandle(EsentModel model, int entityLabel, short type = 0)
        {
            Model = model;
            EntityLabel = entityLabel;
            EntityTypeId= type;
        }

        public XbimInstanceHandle(EsentModel model, int entityLabel, Type type)
        {
            Model = model;
            EntityLabel = entityLabel;
            EntityTypeId = Model.Metadata.ExpressTypeId(type);
        }

        public XbimInstanceHandle(EsentModel model, int? label, short? type)
        {
            Model = model;
            EntityLabel = label ?? 0;
            EntityTypeId = type ?? 0;  
        }

        public XbimInstanceHandle(IPersistEntity entity)
        {
            Model = entity.Model as EsentModel;
            if(Model == null) throw new NullReferenceException("Entity must be in an Esent model");
            EntityLabel = entity.EntityLabel;
            EntityTypeId = Model.Metadata.ExpressTypeId(entity);
        }

        public IPersistEntity GetEntity()
        {
            return Model.Instances[EntityLabel];
        }
       
        internal ExpressType ExpressType()
        {
            return Model.Metadata.ExpressType(EntityTypeId);
        }




       
    }
}
