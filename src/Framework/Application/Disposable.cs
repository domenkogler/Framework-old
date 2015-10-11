using System;

namespace Kogler.Framework
{
    public abstract class Disposable : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool m_Disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed) return;
            m_Disposed = true;
            if (disposing)
            {
                OnDisposing();
            }
            m_Disposed = true;
        }

        /// <summary>
        /// Free managed objects.
        /// </summary>
        protected virtual void OnDisposing() { }
    }
}