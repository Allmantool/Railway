using System;

//General disposing
public class Disposable : IDisposable {
    private bool isDisposed;
    /// <summary>
    /// (Маркер блокировки)
    /// В прошлом для блокировки объектов очень часто применялась конструкция lock (this). 
    /// Но она пригодна только в том случае, если this является ссылкой на закрытый объект. 
    /// В связи с возможными программными и концептуальными ошибками, к которым может привести конструкция lock (this), применять ее больше не рекомендуется. 
    /// Вместо нее лучше создать закрытый объект, чтобы затем заблокировать его.
    /// </summary>
    private readonly object _disposeLock = new object();

    ~Disposable() {
        Dispose(false);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    private void Dispose(bool disposing) {
        lock (_disposeLock) {
            if (!isDisposed && disposing) {
                DisposeCore();
            }

            isDisposed = true;
        }
    }

    // Ovveride this to dispose custom objects
    protected virtual void DisposeCore() {
    }
}