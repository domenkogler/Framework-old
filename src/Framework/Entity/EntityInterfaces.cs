using System;

namespace Kogler.Framework
{
    public interface IEntity
    {
        long ID { get; }
        bool IsTransient { get; }
    }

    public interface IKeyEntity : IEntity<string> { }

    public interface IEntity<out TKey> : IEntity
    {
        TKey Key { get; }
    }

    public interface IModifiable
    {
        bool Deleted { get; set; }
        string UserName { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
        byte[] Version { get; set; }
    }

    public interface IPosition
    {
        string Position { get; set; }
    }

    public interface IParentID
    {
        long? ParentID { get; set; }
    }
}