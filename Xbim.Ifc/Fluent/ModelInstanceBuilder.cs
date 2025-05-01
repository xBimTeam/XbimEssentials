using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.Fluent
{
    internal class ModelInstanceBuilder : IModelInstanceBuilder
    {
        public IModel Model { get => FileBuilder.Model; }

        public EntityCreator Factory { get => FileBuilder.Factory; }
        public IEntityCollection Instances { get => Model.Instances; }

        protected IModelFileBuilder FileBuilder { get; }


        public ModelInstanceBuilder(IModelFileBuilder fileBuilder)
        {
            FileBuilder = fileBuilder;
        }
    }
}
