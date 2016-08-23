using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete;

namespace NaftanRailway.Domain.BusinessModels.BussinesLogic {
    /// <summary>
    /// Класс отвечающий за формирование безнесс объектов (содержащий бизнес логику приложения)
    /// </summary>
    public class BussinesEngage : IBussinesEngage {
        private bool _disposed;
        public IUnitOfWork Uow { get; set; }
        public BussinesEngage(IUnitOfWork unitOfWork) {
            Uow = unitOfWork;
            _disposed = false;
        }

        /// <summary>
        /// Get General table with predicate ( load in memory)
        /// </summary>
        public IEnumerable<T> GetTable<T, TKey>(Expression<Func<T, bool>> predicate = null, Expression<Func<T, TKey>> orderPredicate = null, bool caсhe = false, bool tracking = false) where T : class {
            using (Uow = new UnitOfWork()) {
                return (orderPredicate == null) ? Uow.Repository<T>().Get_all(predicate, caсhe, tracking).ToList() : Uow.Repository<T>().Get_all(predicate, caсhe, tracking).OrderByDescending(orderPredicate).ToList();
            }
        }
        public long GetCountRows<T>(Expression<Func<T, bool>> predicate = null) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(predicate, false, false).Count();
            }
        }
        /// <summary>
        /// Return pagging part of table and general count of rows
        /// </summary>
        /// <typeparam name="T">Current enity</typeparam>
        /// <typeparam name="TKey">Type for ordering</typeparam>
        /// <param name="page">Number page</param>
        /// <param name="size">Count row per one page</param>
        /// <param name="recordCount"></param>
        /// <param name="orderPredicate">Condition for ordering</param>
        /// <param name="filterPredicate">Condition for filtering</param>
        /// <param name="caсhe"></param>
        /// <returns>Return definition count rows of specific entity</returns>
        public IEnumerable<T> GetSkipRows<T, TKey>(int page, int size, out long recordCount, Expression<Func<T, TKey>> orderPredicate, Expression<Func<T, bool>> filterPredicate = null, bool caсhe = false) where T : class {
            recordCount = GetCountRows(filterPredicate);
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(filterPredicate, caсhe).OrderByDescending(orderPredicate).Skip((page - 1) * size).Take(size).ToList();
            }
        }
        public IEnumerable<T> GetSkipRows<T, TKey>(int page, int size, Expression<Func<T, TKey>> orderPredicate, Expression<Func<T, bool>> filterPredicate = null, bool caсhe = false) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(filterPredicate, caсhe).OrderByDescending(orderPredicate).Skip((page - 1) * size).Take(size).ToList();
            }
        }
        public IEnumerable<TKey> GetGroup<T, TKey>(Expression<Func<T, TKey>> groupPredicate, Expression<Func<T, bool>> predicate = null, bool caсhe = false) where T : class {
            using (Uow = new UnitOfWork()) {
                return Uow.Repository<T>().Get_all(predicate, caсhe).GroupBy(groupPredicate).OrderBy(x => x.Key).Select(x => x.Key).ToList();
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing)
                    Uow.Dispose();
            }
            _disposed = true;
        }
    }
}