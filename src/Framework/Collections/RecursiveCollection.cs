using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Caliburn.Micro;

namespace Kogler.Framework.Collections
{
    public class RecursiveCollectionBase<TEntity> : BindableCollection<TEntity>, IRecursiveCollection where TEntity : class//, IRecursive
    {
        public RecursiveCollectionBase(TEntity entity = null)
        {
            var type = typeof(TEntity);
            IsIPosition = type.HasInterface<IPosition>();
            IsIModifiable = type.HasInterface<IModifiable>();
            var recursiveEntity = entity as IRecursiveContainer;
            if (recursiveEntity == null) return;
            Container = recursiveEntity;
            Execute.OnUIThread(() =>
            {
                if (recursiveEntity.Collection != null) AddRange(Order(recursiveEntity.Collection.OfType<TEntity>()));
                RecalculatePositions();
            });
        }

        public bool IsIPosition { get; }
        public bool IsIModifiable { get; }

        private IEnumerable<TEntity> Order(IEnumerable<TEntity> entities)
        {
            var w = IsIModifiable ? entities.OfType<IModifiable>().Where(e => e != null && !e.Deleted).Cast<TEntity>() : entities;
            return IsIPosition ? w.OfType<IPosition>().OrderBy(o => o.Position).Cast<TEntity>() : w;
        }

        public IRecursiveContainer Container { get; }

        private void Deleteted(TEntity item)
        {
            //item.Deleted = true;
        }

        private void Added(TEntity item)
        {
            //item.Deleted = false;
        }

        protected override void InsertItemBase(int index, TEntity item)
        {
            Adopt(item);
            base.InsertItemBase(index, item);
            RecalculatePositions();
        }

        protected override void RemoveItemBase(int index)
        {
            Discard(this[index]);
            base.RemoveItemBase(index);
            RecalculatePositions();
        }

        protected override void ClearItemsBase()
        {
            foreach (var item in this) Discard(item);
            base.ClearItemsBase();
            RecalculatePositions();
        }

        protected override void SetItemBase(int index, TEntity item)
        {
            Adopt(item);
            base.SetItemBase(index, item);
            RecalculatePositions();
        }

        protected virtual void Adopt(TEntity item)
        {
            var recursive = item as IRecursiveContainer;
            if (recursive != null) recursive.Container = Container;
            Added(item);
        }

        protected virtual void Discard(TEntity item)
        {
            Deleteted(item);
            var recursive = item as IRecursiveContainer;
            if (recursive == null) return;
            recursive.RecursiveCollection = null;
        }

        public void RecalculatePositions()
        {
            if (!IsIPosition) return;
            //if (IsNotifying) return;
            for (int i = 0; i < Count; i++)
            {
                var iPosition = ((IPosition)this[i]);
                if (Container == null) iPosition.Position = AlphaNumString(i);
                else
                {
                    var oldPos = iPosition.Position;
                    iPosition.Position = (Container == null ? string.Empty : ((IPosition)Container).Position + ".") + AlphaNumString(i);
                    var recursive = this[i] as IRecursiveContainer;
                    if (recursive == null) continue;
                    if (oldPos != iPosition.Position) recursive.RecursiveCollection?.RecalculatePositions();
                }
            }
        }

        private static string AlphaNumString(int i, int significance = 2)
        {
            var i_s = i.ToString(CultureInfo.InvariantCulture);
            while (i_s.Length < significance)
            {
                i_s = "0" + i_s;
            }
            return i_s;
        }
    }


    public class RecursiveCollection<TEntity> : RecursiveCollectionBase<TEntity>, IRecursiveCollection<TEntity> where TEntity : class, IRecursive
    {
        public RecursiveCollection() { }
        public RecursiveCollection(TEntity entity) : base(entity) { }

        public new IRecursiveContainer<TEntity> Container => base.Container as IRecursiveContainer<TEntity>;
    }

    public static class RecursiveExtensions
    {
        //public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> childSelector)
        //{
        //    var stack = new Stack<T>(items);
        //    while (stack.Any())
        //    {
        //        var next = stack.Pop();
        //        foreach (var child in childSelector(next)) stack.Push(child);
        //        yield return next;
        //    }
        //}

        public static IEnumerable<TEntity> Deep<TEntity>(this TEntity entity) where TEntity : IRecursiveBase<TEntity>
        {
            return entity.WithDeep().Skip(1);
        }

        public static IEnumerable<TEntity> WithDeep<TEntity>(this TEntity entity) where TEntity : IRecursiveBase<TEntity>
        {
            if (Equals(entity, null)) yield break;
            yield return entity;
            foreach (var child in entity.ChildrenCollection.SelectMany(s => s.WithDeep()))
            {
                yield return child;
            }
        }

        public static IEnumerable<TEntity> WithSiblings<TEntity>(this TEntity entity) where TEntity : IRecursiveContainer
        {
            if (Equals(entity, null)) return Enumerable.Empty<TEntity>();
            var parent = entity.Container;
            return Equals(parent, null) ? Enumerable.Empty<TEntity>() : parent.Collection.OfType<TEntity>();
        }

        public static IEnumerable<TEntity> Siblings<TEntity>(this TEntity entity) where TEntity : IRecursiveContainer
        {
            if (Equals(entity, null)) return Enumerable.Empty<TEntity>();
            return entity.WithSiblings().Where(e => !Equals(e, entity));
        }

        public static IEnumerable<TEntity> Parents<TEntity>(this TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (Equals(entity, null)) yield break;
            var parent = entity.Parent;
            while (!Equals(parent, default(TEntity)))
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        public static IEnumerable<TEntity> WithParents<TEntity>(this TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (Equals(entity, null)) return Enumerable.Empty<TEntity>();
            return entity.Parents().Union(new[] { entity });
        }

        public static bool IsParentOf<TEntity>(this TEntity parent, TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (parent == null || entity == null) return false;
            return entity.Parents().Contains(parent);
        }

        public static bool IsChildOf<TEntity>(this TEntity entity, TEntity parent) where TEntity : IRecursiveBase<TEntity>
        {
            if (Equals(parent, null) || Equals(entity, null)) return false;
            return parent.Deep().Contains(entity);
        }

        /// <summary>
        /// Filter items by child presence - return only items without children (remove parents)
        /// </summary>
        /// <typeparam name="TEntity">original IEnumerable</typeparam>
        /// <param name="dics"></param>
        /// <returns>original IEnumerable with only items that have no children</returns>
        public static IEnumerable<TEntity> Flat<TEntity>(this IEnumerable<TEntity> dics) where TEntity : IRecursiveBase<TEntity>
        {
            return dics.Where(d => !d.ChildrenCollection.Any());
        }

        /// <summary>
        /// Filter items by child presence - return parents only (remove items with no children)
        /// </summary>
        /// <typeparam name="TEntity">original IEnumerable</typeparam>
        /// <param name="dics"></param>
        /// <returns>original IEnumerable with child nodes removed</returns>
        public static IEnumerable<TEntity> Roots<TEntity>(this IEnumerable<TEntity> dics) where TEntity : IRecursiveBase<TEntity>
        {
            return dics.Where(d => d.ChildrenCollection.Any());
        }

        public static IEnumerable<TEntity> InLine<TEntity>(this TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (entity == null) return Enumerable.Empty<TEntity>();
            return entity.WithDeep().Union(entity.Parents());
        }

        public static IEnumerable<TEntity> InLineFlat<TEntity>(this TEntity entity) where TEntity : class, IRecursive<TEntity>
        {
            if (entity == null) return Enumerable.Empty<TEntity>();
            return entity.InLine().Union(entity.Parents().SelectMany(p => p.ChildrenCollection)).Distinct().Flat();
        }
    }
}