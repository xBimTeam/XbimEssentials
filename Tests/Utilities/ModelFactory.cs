using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.IO.Esent;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests.Utilities
{
    public delegate IModel FileProvider(string path);
    public delegate IModel ModelProvider(IEntityFactory factory);

    public class ModelFactory : IDisposable, IEnumerable<IModel>
    {
        private static List<FileProvider> FileProviders = new List<FileProvider>();
        private static List<ModelProvider> ModelProviders = new List<ModelProvider>();

        private List<IModel> _models = new List<IModel>();

        static ModelFactory()
        {
            FileProviders.Add((path) =>
            {
                return MemoryModel.OpenRead(path);
            });
            FileProviders.Add((path) =>
            {
                // return Esent DB
                return Ifc.IfcStore.Open(path, null, 0, (pct, o) => { });
            });

            FileProviders.Add((path) =>
            {
                // return Memory DB
                return Ifc.IfcStore.Open(path, null, null, (pct, o) => { });
            });

            ModelProviders.Add((factory) =>
            {
                return new MemoryModel(factory);
            });
            ModelProviders.Add((factory) =>
            {
                return EsentModel.CreateTemporaryModel(factory);
            });
        }

        public ModelFactory(string file)
        {
            _models = FileProviders.Select(p => p(file)).ToList();
        }

        public ModelFactory(XbimSchemaVersion schema)
        {
            IEntityFactory factory = null;
            switch (schema)
            {
                case XbimSchemaVersion.Ifc4:
                    factory = new Xbim.Ifc4.EntityFactoryIfc4();
                    break;
                case XbimSchemaVersion.Ifc4x1:
                    factory = new Xbim.Ifc4.EntityFactoryIfc4x1();
                    break;
                case XbimSchemaVersion.Ifc2X3:
                    factory = new Xbim.Ifc2x3.EntityFactoryIfc2x3();
                    break;
                case XbimSchemaVersion.Cobie2X4:
                case XbimSchemaVersion.Unsupported:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (factory != null)
            {
                _models = ModelProviders.Select(p => p(factory)).ToList();
            }
        }


        /// <summary>
        /// Executes the action. Any transaction handling has to be done in the user code
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ModelFactory Do(Action<IModel> action)
        {
            foreach (var model in _models)
            {
                action(model);
            }
            return this;
        }

        /// <summary>
        /// Executes the action in transaction and commits the transaction
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ModelFactory DoInTransaction(Action<IModel> action)
        {
            foreach (var model in _models)
            {
                using (var txn = model.BeginTransaction("Transaction"))
                {
                    action(model);
                    txn.Commit();
                }
            }
            return this;
        }

        /// <summary>
        /// Executes the action and rolls back the transaction
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ModelFactory DoAndRollBack(Action<IModel> action)
        {
            foreach (var model in _models)
            {
                using (var txn = model.BeginTransaction("Transaction"))
                {
                    action(model);
                    txn.RollBack();
                }
            }
            return this;
        }

        public void Dispose()
        {
            _models.ForEach(m => m.Dispose());
            _models.Clear();
        }

        public IEnumerator<IModel> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        public static void Create(string path, XbimSchemaVersion schema, Action<IModel> creation)
        {
            var f = MemoryModel.GetFactory(schema);
            using (var model = new MemoryModel(f))
            {
                using (var txn = model.BeginTransaction("Creation"))
                {
                    creation(model);
                    txn.Commit();
                }

                using (var file = File.Create(path))
                {
                    if (path.ToLowerInvariant().EndsWith(".ifc"))
                    {
                        model.SaveAsStep21(file);
                    }
                    else if (path.ToLowerInvariant().EndsWith(".ifcxml"))
                    {
                        model.SaveAsXml(file, new System.Xml.XmlWriterSettings { Indent = true, IndentChars = "  " });
                    }
                    else
                    {
                        throw new Exception("Unexpected extension");
                    }

                    file.Close();
                }
            }

        }

        public static IModel Create(XbimSchemaVersion schema, Action<IModel> creation)
        {
            var f = MemoryModel.GetFactory(schema);
            var model = new MemoryModel(f);
            using (var txn = model.BeginTransaction("Creation"))
            {
                creation(model);
                txn.Commit();
            }
            return model;
        }
    }
}
