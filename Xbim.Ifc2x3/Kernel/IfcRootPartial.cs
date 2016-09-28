using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Ifc2x3.Kernel
{
    public partial class IfcRoot
    {
        public string FriendlyName
        {
            get
            {
            string name;
            var rel = this as IfcRelDecomposes;
            if (rel != null)
                name = rel.RelatingObject.FriendlyName;
            else
                name = Name;

            if (string.IsNullOrEmpty(name))
                name = ToString();

            return name;    
            }
            
        }
    }
}
