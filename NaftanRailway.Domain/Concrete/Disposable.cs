using System;

//General disposing
namespace NaftanRailway.Domain.Concrete
{
    public class Disposable : IDisposable {
        /// <summary>
        ///     (Маркер блокировки)
        ///     В прошлом для блокировки объектов очень часто применялась конструкция lock (this).
        ///     Но она пригодна только в том случае, если this является ссылкой на закрытый объект.
        ///     В связи с возможными программными и концептуальными ошибками, к которым может привести конструкция lock (this),
        ///     применять ее больше не рекомендуется.
        ///     Вместо нее лучше создать закрытый объект, чтобы затем заблокировать его.
        /// </summary>
        private readonly object _disposeLock = new object();

        private bool IsDisposed { get; set; }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     This makes sense when cleanup is not urgent and hastening it by calling Dispose is more of an optimization than a
        ///     necessity.
        /// </summary>
        ~Disposable() {
            Dispose(false);
        }

        private void Dispose(bool disposing) {
            lock (_disposeLock) {
                if (!IsDisposed && disposing) DisposeCore();

                IsDisposed = true;
            }
        }

        // Override this to dispose custom objects
        protected virtual void DisposeCore() {
        }
    }
}