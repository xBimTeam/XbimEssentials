using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.MeasureResource
{
    public partial struct IfcCompoundPlaneAngleMeasure
    {
        public double AsDouble
        {
            get
            {
                var components = _value.Count;
                double measure1 = 0; double measure2 = 0; double measure3 = 0; double millionthsOfaSecond = 0;
                if (components > 3) millionthsOfaSecond = _value[3];
                if (components > 2) measure3 = _value[2];
                if (components > 1) measure2 = _value[1];
                if (components > 0) measure1 = _value[0];
                double main = measure1 + (measure2 / 60.0) + (measure3 / 3600.0)+ (millionthsOfaSecond/3600e6) ;
                return main;
            }
        }

        public static IfcCompoundPlaneAngleMeasure FromDouble(double degreeAngle)
        {
            int measure1; int measure2; int measure3; int measure4;
            measure1 = (int)degreeAngle;       //round to int value                               
            measure2 = (int)(degreeAngle - measure1) * 60;
            measure3 = (int)((degreeAngle - measure1) * 60 - measure2) * 60;
            measure4 = (int)((((degreeAngle - measure1) * 60 - measure2) * 60 - measure3) * 1e6);
            var vals = new List<long>() { measure1, measure2, measure3, measure4 };
            return new IfcCompoundPlaneAngleMeasure(vals);
        }

        public long ItemAt(int index)
        {
            return ToArray().ItemAt(index);
        }

        public long[] ToArray()
        {
            return _value.ToArray();
        }
    }
}
