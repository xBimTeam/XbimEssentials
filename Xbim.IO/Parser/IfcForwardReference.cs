using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common.Exceptions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;


namespace Xbim.IO.Parser
{
    public struct IfcForwardReference
    {
        public readonly int ReferenceEntityLabel;
        private readonly short ReferencingPropertyId;
        private readonly IPersistIfcEntity ReferencingEntity;

        public IfcForwardReference(int referenceEntityLabel,
            short referencingProperty,
            IPersistIfcEntity referencingEntity)
        {
            ReferenceEntityLabel = referenceEntityLabel;
            ReferencingPropertyId = referencingProperty;
            ReferencingEntity = referencingEntity;
        }

        public bool Resolve(ConcurrentDictionary<int, IPersistIfcEntity> references)
        {
            IPersistIfcEntity entity;
            if (references.TryGetValue(ReferenceEntityLabel, out entity))
            {
                PropertyValue pv = new PropertyValue();
                pv.Init(entity);
                try
                {
                    ReferencingEntity.IfcParse(ReferencingPropertyId, pv);
                    return true;
                }
                catch (Exception)
                {
                    IfcType ifcType = IfcMetaData.IfcType(ReferencingEntity);
                    
                    XbimModel.Logger.ErrorFormat("Data Error. Cannot set the property = {0} of entity #{1} = {2} to entity #{3}, schema violation. Ignored", 
                        ifcType.IfcProperties[ReferencingPropertyId+1].PropertyInfo.Name, 
                        ReferencingEntity.EntityLabel,
                        ReferencingEntity.GetType().Name,
                        ReferenceEntityLabel);
                    return false;
                }
            }
            else
                return false;
        }
    }
}
