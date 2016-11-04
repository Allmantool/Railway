using System;

//General disposing
public class Disposable : IDisposable {
    public bool isDisposed { get; private set; }
    /// <summary>
    /// (Маркер блокировки)
    /// В прошлом для блокировки объектов очень часто применялась конструкция lock (this). 
    /// Но она пригодна только в том случае, если this является ссылкой на закрытый объект. 
    /// В связи с возможными программными и концептуальными ошибками, к которым может привести конструкция lock (this), применять ее больше не рекомендуется. 
    /// Вместо нее лучше создать закрытый объект, чтобы затем заблокировать его.
    /// </summary>
    private readonly object _disposeLock = new object();

    /// <summary>
    /// This makes sense when cleanup is not urgent and hastening it by calling Dispose is more of an optimization than a necessity.
    /// </summary>
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