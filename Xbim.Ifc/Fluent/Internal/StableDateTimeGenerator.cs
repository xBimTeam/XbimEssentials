using System;

namespace Xbim.Ifc.Fluent.Internal
{

    /// <summary>
    /// Generates a DateTime using the user's provided value
    /// </summary>
    /// <remarks>Used to ensure test IFC files have a stable timestamp and ownerHistory dates</remarks>
    internal class StableDateTimeGenerator : IDateTimeGenerator
    {
        private readonly DateTime baseDate;

        public StableDateTimeGenerator(DateTime baseDate)
        {
            this.baseDate = baseDate;
        }

        public DateTime Generate()
        {
            return baseDate;
        }
    }
}
