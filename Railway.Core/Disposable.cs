namespace Railway.Core
{
    using System;

    public class Disposable : IDisposable
    {
        private readonly object disposeLockMarker = new object();

        ~Disposable()
        {
            Dispose(false);
        }

        private bool IsDisposed { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void ExtenstionDispose()
        {
        }

        private void Dispose(bool disposing)
        {
            lock (disposeLockMarker)
            {
                if (!IsDisposed && disposing)
                {
                    ExtenstionDispose();
                }

                IsDisposed = true;

                GC.SuppressFinalize(this);
            }
        }
    }
}