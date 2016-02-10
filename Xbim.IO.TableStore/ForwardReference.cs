using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using Xbim.Common;

namespace Xbim.IO.TableStore
{
    /// <summary>
    /// This class is used to resolve forward references in the model. It uses configuration from mapping and
    /// row as a data context. Forward reference doesn't keep the reference to the entity so it is possible for 
    /// the IModel to use memory optimizations while this reference exists. It will load the entity only when it is to
    /// be resolved.
    /// </summary>
    internal class ForwardReference
    {
        public ReferenceContext Context { get; private set; }
        public TableStore Store { get; set; }

        /// <summary>
        /// Handle to the object which will be resolved
        /// </summary>
        private readonly XbimInstanceHandle _handle;

        /// <summary>
        /// Row context of the referenced value
        /// </summary>
        public IRow Row { get; private set; }

        /// <summary>
        /// Model of the entity
        /// </summary>
        public IModel Model { get { return _handle.Model; } }

        private IPersistEntity Entity { get; set; }


        public ForwardReference(XbimInstanceHandle handle, ReferenceContext context, TableStore store)
        {
            Store = store;
            _handle = handle;
            Row = context.CurrentRow;
            Context = context;
        }

        public ForwardReference(IPersistEntity entity, ReferenceContext context, TableStore store)
        {
            Store = store;
            _handle = new XbimInstanceHandle(entity);
            Row = context.CurrentRow;
            Context = context;
        }

        /// <summary>
        /// Resolves all references for the entity using configuration from class mapping and data from the row
        /// </summary>
        public void Resolve()
        {
            //load context data
            Context.LoadData(Row, false);

            //load entity from the model
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            Entity = _handle.GetEntity();

            //resolve parent if this is a parent context
            if (Context.ContextType == ReferenceContextType.Parent)
                ResolveParent();
            //resolve all other kinds of references
            else
                ResolveMember();
        }

        private void ResolveMember()
        {
            if (Context.ContextType == ReferenceContextType.Parent)
                return;

            var children = Store.GetReferencedEntities(Context).ToList();
            
            //if no children was found but context contains data for creation of the object, create one
            if(!children.Any() && Context.HasData)
                children.Add(Store.ResolveContext(Context, -1, false));

            foreach (var child in children)
                Store.AssignEntity(Entity, child, Context);
        }

        private void ResolveParent()
        {
            if (Context.ContextType != ReferenceContextType.Parent)
                return;

            var parents = Store.GetReferencedEntities(Context).ToList();
            if (!parents.Any())
            {
                Store.Log.WriteLine("There is no parent of type {0} for type {1}", Context.SegmentType.ExpressName,
                    Entity.ExpressType.ExpressName);
                return;
            }
            if (parents.Count > 1)
            {
                Store.Log.WriteLine("There is more than one parent of type {0} for type {1}. All parents will be used.", Context.SegmentType.ExpressName,
                    Entity.ExpressType.ExpressName);
            }

            var destination =
                Context.AllChildren.FirstOrDefault(
                    c =>
                        !c.AllChildren.Any() &&
                        (c.ContextType == ReferenceContextType.Entity ||
                         c.ContextType == ReferenceContextType.EntityList));
            if (destination == null)
            {
                Store.Log.WriteLine("There is destination path for type {1} in type {0}, table {2}.",
                    Context.SegmentType.ExpressName,
                    Entity.ExpressType.ExpressName, Context.CMapping.TableName);
                return;
            }
            foreach (var parent in parents)
            {
                AddToPath(destination, parent, Entity);
            }
        }

        private void AddToPath(ReferenceContext targetContext, IPersistEntity parent, IPersistEntity child)
        {
            //get context path from root entity
            var ctxStack = new Stack<ReferenceContext>();
            var entityStack = new Stack<IPersistEntity>();
            var context = targetContext;
            while (!context.IsRoot && context.ContextType != ReferenceContextType.Parent)
            {
                ctxStack.Push(context);
                context = context.ParentContext;
            }

            var entity = parent;
            while (ctxStack.Count != 0)
            {
                context = ctxStack.Pop();
                entityStack.Push(entity);
                //browse to the level of the bottom context and call ResolveContext there
                var index = context.Index != null ? new[] { context.Index } : null;
                var value = context.PropertyInfo.GetValue(entity, index);
                if (context.ContextType == ReferenceContextType.Entity)
                {
                    var e = value as IPersistEntity;
                    //if it is null, create a new one or assign the child
                    if (e == null)
                    {
                        e = context == targetContext ? child : Store.ResolveContext(context, -1, true);
                        Store.AssignEntity(entity, e, context);
                        entity = e;
                        continue;
                    }

                    //verify that this is the desired one by the values. If not, create a new one on this level and higher
                    if (TableStore.IsValidEntity(context, e))
                    {
                        entity = e;
                        continue;
                    }

                    //create a new one and assign it higher
                    e = context == targetContext ? child : Store.ResolveContext(context, -1, true);
                    Join(e, context, entityStack);
                    entity = e;
                    continue;
                }

                //it should be enumerable
                var entities = value as IEnumerable;
                if (entities == null)
                {
                    Store.Log.WriteLine("It wasn't possible to browse to the data entry point.");
                    return;
                }
                
                if (context == targetContext)
                {
                    Store.AssignEntity(entity, child, context);
                    return;
                }
                
                //get first valid entity
                entity = GetFirstValid(entities.Cast<object>(), context);
                //entity = entities.Cast<object>().FirstOrDefault(e => TableStore.IsValidEntity(context, e)) as IPersistEntity;
                if (entity != null) continue;

                //create new entity and assign it on a higher level
                entity = Store.ResolveContext(context, -1, true);
                AddToPath(context, parent, entity);
            }
        }

        private IPersistEntity GetFirstValid(IEnumerable<object> entities, ReferenceContext context)
        {
            var enumerable = entities as IList<object> ?? entities.ToList();

            //if there is only one in the list, retun that one
            if (enumerable.Count == 1)
                return enumerable.First() as IPersistEntity;

            if (enumerable.Count == 0)
                return null;

            //if context doesn't have any data at all on any level down
            if (!context.HasData)
            {
                return null;
            }

            //this context has scalar data on its level
            if (context.ScalarChildren.Any(c => c.Values != null && c.Values.Any()))
            {
                enumerable = enumerable.Where(e => TableStore.IsValidEntity(context, e)).ToList();
                //if there is only one in the list, retun that one
                if (enumerable.Count == 1)
                    return enumerable.First() as IPersistEntity;

                if (enumerable.Count == 0)
                    return null;
            }

            return enumerable.FirstOrDefault(e =>
                context.EntityChildren.All(c =>
                {
                    var index = c.Index != null ? new[] {c.Index} : null;
                    var value = c.PropertyInfo.GetValue(e, index);
                    //this is equivalent of contains any valid
                    if (c.ContextType == ReferenceContextType.EntityList)
                    {
                        var enu = value as IEnumerable<IPersistEntity>;
                        if (enu == null)
                            return false;
                        return GetFirstValid(enu, c) != null;
                    }

                    var ent = value as IPersistEntity;
                    return ent != null && TableStore.IsValidEntity(c, ent);
                })) as IPersistEntity;
        }

        private void Join(IPersistEntity entity, ReferenceContext context,
            Stack<IPersistEntity> parents)
        {
            var temp = new Stack<IPersistEntity>();
            IPersistEntity parent;
            while (parents.Count != 0)
            {
                parent = parents.Pop();
                if (context.ContextType == ReferenceContextType.EntityList)
                {
                    Store.AssignEntity(parent, entity, context);
                    temp.Push(parent);
                    break;
                }

                var e = Store.ResolveContext(context.ParentContext, -1, true);
                Store.AssignEntity(e, entity, context);
                context = context.ParentContext;
                entity = e;
                temp.Push(e);
            }

            //fill parents with the new stuff
            while (temp.Count != 0)
            {
                parent = temp.Pop();
                parents.Push(parent);
            }
        }

       

       
    }
}
