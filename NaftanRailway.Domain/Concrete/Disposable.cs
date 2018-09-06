namespace NaftanRailway.Domain.Concrete
{
    using System;

    public class Disposable : IDisposable
    {
        private readonly object disposeLockМarker = new object();

        /// <summary>
        /// Finalizes an instance of the <see cref="Disposable"/> class.
        /// This makes sense when cleanup is not urgent and hastening it
        /// by calling Dispose is more of an optimization than a necessity.
        /// </summary>
        ~Disposable()
        {
            // Значение false указывает на то, что очистка была инициирована сборщиком мусора.
            this.Dispose(false);
        }

        private bool IsDisposed { get; set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Override this to dispose custom objects
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

                // Подавление финализации.
                GC.SuppressFinalize(this);
            }
        }
    }
}