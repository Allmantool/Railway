using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete;
using NaftanRailway.BLL.Abstract;
using log4net;

namespace NaftanRailway.BLL.Concrete.BussinesLogic {
    /// <summary>
    /// Класс отвечающий за формирование безнесс объектов (содержащий бизнес логику приложения)
    /// </summary>
    public sealed class BussinesEngage : Disposable, IBussinesEngage {
        public ILog Log { get; }
        public IUnitOfWork Uow { get; set; }
        public BussinesEngage(IUnitOfWork unitOfWork, ILog log) {
            Uow = unitOfWork;
            Log = log;
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
        /// <typeparam name="T">Current entity</typeparam>
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

        /// <summary>
        /// Custom group function
        /// </summary>
        /// <typeparam name="T">A element type (entity, table, object)</typeparam>
        /// <typeparam name="TKey">It's key by which grouping sequence of type T</typeparam>
        /// <param name="groupPredicate">It's predicate for grouping</param>
        /// <param name="predicate">It's predicate for filtering</param>
        /// <param name="caсhe"></param>
        /// <returns>It returns IEnumerable'IGrouping' </returns>
        public IEnumerable<IGrouping<TKey, T>> GetGroup<T, TKey>(Expression<Func<T, TKey>> groupPredicate, Expression<Func<T, bool>> predicate = null, bool caсhe = false) where T : class {
            using (Uow = new UnitOfWork()) {
                IList<IGrouping<TKey, T>> result;
                try {
                    result = Uow.Repository<T>().Get_all(predicate, caсhe).GroupBy(groupPredicate).ToList();
                } catch (Exception ex) {
                    Log.DebugFormat($"GetGroup LINQ custom method throws exception: {ex.Message}.");
                    throw;
                }

                return result;
            }
        }

        protected override void DisposeCore() {
            Uow?.Dispose();
        }
    }
}