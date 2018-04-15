using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests.Utilities
{
    public delegate IModel ModelProvider(string path);

    public class ModelFactory : IDisposable, IEnumerable<IModel>
    {
        public static List<ModelProvider> Providers = new List<ModelProvider>();

        private List<IModel> _models = new List<IModel>();

        static ModelFactory()
        {
            Providers.Add((path) =>
            {
                return MemoryModel.OpenRead(path);
            });
        }

        public ModelFactory(string file)
        {
            _models = Providers.Select(p => p(file)).ToList();
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
            var f = schema == XbimSchemaVersion.Ifc4 ?
                new Ifc4.EntityFactory() as IEntityFactory :
                new Ifc2x3.EntityFactory();
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
            var f = schema == XbimSchemaVersion.Ifc4 ?
                new Ifc4.EntityFactory() as IEntityFactory :
                new Ifc2x3.EntityFactory();
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
