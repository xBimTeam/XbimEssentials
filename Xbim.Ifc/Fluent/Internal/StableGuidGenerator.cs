using System;
using Xbim.Common;

namespace Xbim.Ifc.Fluent.Internal
{
    /// <summary>
    /// A Guid generator that creates reproducable identifiers based on an IFC element's EntityId
    /// </summary>
    /// <remarks>These Guids are only guaranteed to be unique within a file! The purpose is to allow test files to 
    /// be regenerated altering the UniqueIdentifier/Guids on every entity.</remarks>
    internal class StableGuidGenerator : IGuidGenerator
    {
        private readonly Guid baseGuid;

        public StableGuidGenerator(Guid baseGuid)
        {
            this.baseGuid = baseGuid;
        }

        /// <summary>
        /// Generates a 'GUID' based off the entity's <see cref="IPersistEntity.EntityLabel"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Guid GenerateForEntity(IPersistEntity entity)
        {
            if(entity.EntityLabel == 0)
            {
                // Should never happen?
                return Guid.NewGuid();
            }
            var guidBytes = baseGuid.ToByteArray();
            var counterBytes = BitConverter.GetBytes((long)entity.EntityLabel);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(counterBytes);
            }

            for (int i = 0; i < 8; i++)
            {
                guidBytes[15 - i] = counterBytes[i];
            }

            return new Guid(guidBytes);

        }
    }
}
