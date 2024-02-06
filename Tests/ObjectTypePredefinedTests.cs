using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ObjectTypePredefinedTests
    {
        [TestMethod]
        public void CanGetObjectsPredefinedTypeInIfc4()
        {
            using (var model = new MemoryModel(new Ifc4.EntityFactoryIfc4()))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    var pile = model.Instances.New<Xbim.Ifc4.StructuralElementsDomain.IfcPile>(p =>
                    {
                        p.PredefinedType = Ifc4.Interfaces.IfcPileTypeEnum.FRICTION;
                    });

                    var obj = pile as IIfcObject;

                    Assert.AreEqual("FRICTION", obj.GetPredefinedTypeValue());
                }
            }
        }

        [TestMethod]
        public void CanGetTypesPredefinedTypeInIfc4()
        {
            using (var model = new MemoryModel(new Ifc4.EntityFactoryIfc4()))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    var pile = model.Instances.New<Xbim.Ifc4.StructuralElementsDomain.IfcPileType>(p =>
                    {
                        p.PredefinedType = Ifc4.Interfaces.IfcPileTypeEnum.FRICTION;
                    });

                    var obj = pile as IIfcTypeObject;

                    Assert.AreEqual("FRICTION", obj.GetPredefinedTypeValue());
                }
            }
        }

        [TestMethod]
        public void MissingAttributeHandled()
        {
            using (var model = new MemoryModel(new Ifc4.EntityFactoryIfc4()))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    var person = model.Instances.New<Xbim.Ifc4.Kernel.IfcActor>();

                    var obj = person as IIfcObject;

                    Assert.IsNull(obj.GetPredefinedTypeValue());
                }
            }
        }

        [TestMethod]
        public void NullPredefinedTypeHandled()
        {
            using (var model = new MemoryModel(new Ifc4.EntityFactoryIfc4()))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    var pile = model.Instances.New<Xbim.Ifc4.BuildingControlsDomain.IfcActuator>(p =>
                    {
                        p.PredefinedType = null;
                    });

                    var obj = pile as IIfcObject;

                    Assert.IsNull(obj.GetPredefinedTypeValue());
                }
            }
        }

        [TestMethod]
        public void CanGetObjectsPredefinedTypeInIfc2x3()
        {
            using (var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3()))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    var pile = model.Instances.New<Xbim.Ifc2x3.StructuralElementsDomain.IfcPile>(p =>
                    {
                        p.PredefinedType = Xbim.Ifc2x3.StructuralElementsDomain.IfcPileTypeEnum.FRICTION;
                    });

                    var obj = pile as IIfcObject;

                    Assert.AreEqual("FRICTION", obj.GetPredefinedTypeValue());
                }
            }
        }

        [TestMethod]
        public void CanGetTypesPredefinedTypeInIfc2x3()
        {
            using (var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3()))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    var pile = model.Instances.New<Xbim.Ifc2x3.SharedBldgElements.IfcBeamType>(p =>
                    {
                        p.PredefinedType = Xbim.Ifc2x3.SharedBldgElements.IfcBeamTypeEnum.JOIST;
                    });

                    var obj = pile as IIfcTypeObject;

                    Assert.AreEqual("JOIST", obj.GetPredefinedTypeValue());
                }
            }
        }


        [TestMethod]
        public void CanGetObjectsPredefinedTypeInIfc4x3()
        {
            using (var model = new MemoryModel(new Ifc4x3.EntityFactoryIfc4x3Add1()))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    var pile = model.Instances.New<Xbim.Ifc4x3.StructuralElementsDomain.IfcPile>(p =>
                    {
                        p.PredefinedType = Xbim.Ifc4x3.StructuralElementsDomain.IfcPileTypeEnum.BORED;
                    });

                    var obj = pile as IIfcObject;

                    Assert.AreEqual("BORED", obj.GetPredefinedTypeValue());
                }
            }
        }


        [TestMethod]
        public void CanGetTypesPredefinedTypeInIfc4x3()
        {
            using (var model = new MemoryModel(new Ifc4x3.EntityFactoryIfc4x3Add1()))
            {
                using (var txn = model.BeginTransaction("Test"))
                {
                    var pile = model.Instances.New<Xbim.Ifc4x3.StructuralElementsDomain.IfcPileType>(p =>
                    {
                        p.PredefinedType = Xbim.Ifc4x3.StructuralElementsDomain.IfcPileTypeEnum.BORED;
                    });

                    var obj = pile as IIfcTypeObject;

                    Assert.AreEqual("BORED", obj.GetPredefinedTypeValue());
                }
            }
        }
    }
}
