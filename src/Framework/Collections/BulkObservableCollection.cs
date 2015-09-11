using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Kogler.Framework
{
    
    public interface IBulkObservableCollection : IList
    {
        void AddRange(IEnumerable items);
        void RemoveRange(IEnumerable items);
    }
    public interface IBulkObservableCollection<T> : IBulkObservableCollection, IList<T>
    {
        void AddRange(IEnumerable<T> items);
        void RemoveRange(IEnumerable<T> items);
    }

    /// <summary>
    /// Represents an <see cref="ObservableCollection{T}"/> that has ability to suspend
    /// change notification events.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public class BulkObservableCollection<T> : ObservableCollection<T>, IBulkObservableCollection<T>
    {
        protected bool m_SuspendCollectionChangeNotification;

        #region << Constructor >>

        public BulkObservableCollection() { }
        public BulkObservableCollection(IEnumerable<T> items) : base(items) { }

        public object SyncRootObject => ((ICollection)this).SyncRoot;

        #endregion

        #region << NotifyChange >>

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (m_SuspendCollectionChangeNotification) return;
            Dispatcher.RunInUI(() =>
            {
                try
                {
                    base.OnCollectionChanged(e);
                }
                catch (InvalidOperationException)
                {
                    Dispatcher.DoEvents();
                    base.OnCollectionChanged(e);
                }
            });
        }

        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dispatcher.RunInUI(() =>
            {
                try
                {
                    base.OnPropertyChanged(e);
                }
                catch (InvalidOperationException)
                {
                    Dispatcher.DoEvents();
                    base.OnPropertyChanged(e);
                }
            });
        }

        public void RaiseCollectionChanged()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region << Public >>

        public void SuspendCollectionChangeNotification()
        {
            m_SuspendCollectionChangeNotification = true;
        }

        public void ResumeCollectionChangeNotification()
        {
            m_SuspendCollectionChangeNotification = false;
        }

        public void AddRange(IEnumerable items)
        {
            lock (SyncRootObject)
            {
                if (items == null) return;
                AddRange(items.OfType<T>());
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            lock (SyncRootObject)
            {
                if (items == null) return;
                if (!(items is T[])) items = items.ToArrayLock();
                if (!items.Any()) return;
                bool collectionChanged = false;
                SuspendCollectionChangeNotification();
                try
                {
                    foreach (T item in items) Add(item);
                    collectionChanged = true;
                }
                catch (InvalidOperationException)
                {
                    Dispatcher.DoEvents();
                    AddRange(items);
                }
                finally
                {
                    ResumeCollectionChangeNotification();
                    if (collectionChanged) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        public void ReplaceItems(IEnumerable<T> items)
        {
            lock (SyncRootObject)
            {
                Clear();
                AddRange(items);
            }
        }

        public void RemoveRange(IEnumerable items)
        {
            lock (SyncRootObject)
            {
                RemoveRange(items.OfType<T>());
            }
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            lock (SyncRootObject)
            {
                if (null == items) throw new ArgumentNullException(nameof(items));
                if (!(items is T[])) items = items.ToArrayLock();
                if (!items.Any()) return;
                bool collectionChanged = false;
                SuspendCollectionChangeNotification();
                try
                {
                    foreach (T item in items) Remove(item);
                    collectionChanged = true;
                }
                catch (InvalidOperationException)
                {
                    Dispatcher.DoEvents();
                    RemoveRange(items);
                }
                finally
                {
                    ResumeCollectionChangeNotification();
                    if (collectionChanged) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        public void RemoveRange(Func<T, bool> condition)
        {
            lock (SyncRootObject)
            {
                if (null == condition) throw new ArgumentNullException(nameof(condition));
                var items = Items.Where(condition).ToArrayLock();
                if (!items.Any()) return;
                bool collectionChanged = false;
                SuspendCollectionChangeNotification();
                try
                {
                    foreach (T item in items) Remove(item);
                    collectionChanged = true;
                }
                catch (InvalidOperationException)
                {
                    Dispatcher.DoEvents();
                    RemoveRange(condition);
                }
                finally
                {
                    ResumeCollectionChangeNotification();
                    if (collectionChanged) OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        #endregion

        #region << Protected >>

        private static void RunAsyncCatchAllExceptions(Action a)
        {
            Dispatcher.RunInUI(() =>
            {
                try { a(); }
                catch (Exception) { }
            });
        }

        protected override void InsertItem(int index, T item)
        {
            lock (SyncRootObject)
            {
                try
                {
                    base.InsertItem(index, item);
                }
                catch (InvalidOperationException)
                {
                    RunAsyncCatchAllExceptions(() => base.InsertItem(index, item));
                }
            }
        }

        protected override void ClearItems()
        {
            lock (SyncRootObject)
            {
                try
                {
                    base.ClearItems();
                }
                catch (InvalidOperationException)
                {
                    RunAsyncCatchAllExceptions(() => base.ClearItems());
                }
            }
        }

        protected override void SetItem(int index, T item)
        {
            lock (SyncRootObject)
            {
                try
                {
                    base.SetItem(index, item);
                }
                catch (InvalidOperationException)
                {
                    RunAsyncCatchAllExceptions(() => base.SetItem(index, item));
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (SyncRootObject)
            {
                try
                {
                    base.RemoveItem(index);
                }
                catch (InvalidOperationException)
                {
                    RunAsyncCatchAllExceptions(() => base.RemoveItem(index));
                }
            }
        }

        #endregion
    }
}
