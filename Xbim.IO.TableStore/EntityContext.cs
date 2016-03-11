using System.Collections.Generic;
using System.Linq;
using Xbim.Common;

namespace Xbim.IO.TableStore
{
    public class EntityContext
    {
        public IPersistEntity Entity { get; private set; }

        public EntityContext Parent { get; set; }

        public List<EntityContext> Children { get; private set; } 

        public EntityContext(IPersistEntity entity)
        {
            Entity = entity;
            Children = new List<EntityContext>();
        }

        public EntityContext(IEnumerable<IPersistEntity> entities)
        {
            Entity = null;
            Children = new List<EntityContext>();
            Add(entities);
        }

        public int Level { get { return Parent == null ? 0 : Parent.Level + 1; } }

        private int _leaveDepth;

        public int LeavesDepth
        {
            get { return Root._leaveDepth; }
            set { Root._leaveDepth = value; }
        }

        public EntityContext Add(EntityContext child)
        {
            child.Parent = this;
            Children.Add(child);
            return child;
        }

        /// <summary>
        /// Adds entity as a child of this context and sets up the relations
        /// </summary>
        /// <param name="child">Child entity</param>
        /// <returns>Child context</returns>
        public EntityContext Add(IPersistEntity child)
        {
            return Add(new EntityContext(child));
        }

        /// <summary>
        /// Adds entity as a child of this context and sets up the relations
        /// </summary>
        /// <param name="children"></param>
        public void Add(IEnumerable<IPersistEntity> children)
        {
            foreach (var child in children)
            {
                Add(new EntityContext(child));
            }
        }

        /// <summary>
        /// Root context of the hierarchy
        /// </summary>
        public EntityContext Root
        {
            get
            {
                return Parent == null ? this : Parent.Root;
            }
        }

        /// <summary>
        /// Root entity of the hierarchy
        /// </summary>
        public IPersistEntity RootEntity
        {
            get { return Root.Entity; }
        }

        /// <summary>
        /// Leaves on any level of context hierarchy
        /// </summary>
        public IEnumerable<EntityContext> Leaves
        {
            get { return (Children == null || !Children.Any()) && LeavesDepth == Level ? 
                new []{this} : 
                AllChildren.Where(c => c.Level == LeavesDepth); }
        }

        public IEnumerable<EntityContext> AllChildren
        {
            get
            {
                if(Children == null)
                    yield break;
                foreach (var child in Children)
                {
                    yield return child;
                }
                foreach (var child in Children.SelectMany(c => c.AllChildren))
                {
                    yield return child;
                }
            }
        } 
    }
}
