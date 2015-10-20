namespace Xbim.Ifc2x3.Kernel
{
    public partial class IfcTypeObject
    {
        public void AddPropertySet(IfcPropertySetDefinition pSetDefinition)
        {
            HasPropertySets.Add(pSetDefinition);
        }
    }
}
