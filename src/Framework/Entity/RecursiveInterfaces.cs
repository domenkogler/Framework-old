using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.Micro;

namespace Kogler.Framework
{
    #region << Reccursive >>

    #region << Reccursive Entity >>

    public interface IRecursive
    {
        IRecursiveContainer Container { get; set; }
    }

    public interface IRecursiveContainer : IRecursive
    {
        IEnumerable Collection { get; }
        IRecursiveCollection RecursiveCollection { get; set; }
    }

    //public interface IParent<TEntity> where TEntity : IRecursive
    //{
    //    TEntity Parent { get; set; }
    //}

    public interface IRecursiveBase<out TEntity> : IRecursive
    {
        IEnumerable<TEntity> ChildrenCollection { get; }
    }

    //public interface IRecursiveParent<out TEntity> : IRecursiveBase<TEntity> where TEntity : IRecursive
    //{
    //}

    public interface IRecursiveContainer<TEntity> : IRecursiveBase<TEntity>, IRecursiveContainer where TEntity : IRecursive//, IPosition
    {
        new IRecursiveCollection<TEntity> RecursiveCollection { get; }
    }

    public interface IRecursive<TEntity> : IRecursiveContainer<TEntity> where TEntity : IRecursive//, IPosition
    {
        TEntity Parent { get; }
    }

    //public interface IRecursiveEntity<TEntity> : IRecursive<TEntity> where TEntity : class, IRecursive<TEntity>, IParentID, IModifiable, IEntity, IPosition
    //{
    //}


    #endregion

    #region << Reccursive Collection >>

    public interface IRecursiveCollection : ICollection
    {
        void RecalculatePositions();
        IRecursiveContainer Container { get; }
    }

    public interface IRecursiveCollection<TEntity> : IObservableCollection<TEntity>, IRecursiveCollection, IEnumerable<TEntity> where TEntity : IRecursive//, IPosition
    {
        new IRecursiveContainer<TEntity> Container { get; }
    }

    #endregion

    #endregion
}
