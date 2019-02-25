namespace NaftanRailway.Domain.Concrete
{
    using System;

    public class Disposable : IDisposable
    {
        private readonly object disposeLockМarker = new object();

        ~Disposable()
        {
            this.Dispose(false);
        }

        private bool IsDisposed { get; set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void ExtenstionDispose()
        {
        }

        private void Dispose(bool disposing)
        {
            lock (this.disposeLockМarker)
            {
                if (!this.IsDisposed && disposing)
                {
                    this.ExtenstionDispose();
                }

                this.IsDisposed = true;

                GC.SuppressFinalize(this);
            }
        }
    }
}