using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.Fluent
{
#nullable enable
    public record PropertyDefault
    {
        public string? PropertySet { get; set; }
        public string? Name { get; set; }
        public IIfcValue? Value { get; set; }
    }
}
