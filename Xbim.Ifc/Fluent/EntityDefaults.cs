using System;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc.Fluent
{
#nullable enable
    public record EntityDefaults()
    {
        public IfcLabel? Name { get; set; }
        public IfcText? Description { get; set; }
        public IIfcOwnerHistory? OwnerHistory { get; set; }
        public Guid? GlobalId { get; set; }
        public string? PredefinedType { get; set; }
    }
}
