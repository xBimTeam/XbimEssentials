using System;
using System.Collections.Concurrent;
using Xbim.Common;
using Xbim.Common.Metadata;

namespace Xbim.IO.Step21.Parser
{
    public struct StepForwardReference
    {
        public readonly int ReferenceEntityLabel;
        private readonly short _referencingPropertyId;
        private readonly IPersistEntity _referencingEntity;
        private readonly int[] _nestedIndex;

        public StepForwardReference(int referenceEntityLabel,
            short referencingProperty,
            IPersistEntity referencingEntity, int[] nestedIndex)
        {
            ReferenceEntityLabel = referenceEntityLabel;
            _referencingPropertyId = referencingProperty;
            _referencingEntity = referencingEntity;
            _nestedIndex = nestedIndex;
        }

        public bool Resolve(ConcurrentDictionary<int, IPersistEntity> references, ExpressMetaData metadata)
        {
            IPersistEntity entity;
            if (references.TryGetValue(ReferenceEntityLabel, out entity))
            {
                var pv = new PropertyValue();
                pv.Init(entity);
                try
                {
                    _referencingEntity.Parse(_referencingPropertyId, pv, _nestedIndex);
                    return true;
                }
                catch (Exception)
                {
                    var expressType = metadata.ExpressType(_referencingEntity);
                    //TODO put a logger message here
                    //EsentModel.Logger.ErrorFormat("Data Error. Cannot set the property = {0} of entity #{1} = {2} to entity #{3}, schema violation. Ignored", 
                    //    expressType.Properties[_referencingPropertyId+1].PropertyInfo.Name, 
                    //    _referencingEntity.EntityLabel,
                    //    _referencingEntity.GetType().Name,
                    //    ReferenceEntityLabel);
                    return false;
                }
            }
            else
                return false;
        }
    }
}
