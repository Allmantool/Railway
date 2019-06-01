namespace Railway.DeliveryCargo.Data.EF.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// A generic repository.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public interface IRepository<T>
        where T : class
    {
        /// <summary>
        /// Retrieve a queryable object.
        /// </summary>
        /// <returns>A queryable object.</returns>
        IQueryable<T> Get();

        /// <summary>
        /// Retrieve a queryable object with disabled tracking.
        /// </summary>
        /// <returns>A queryable object.</returns>
        IQueryable<T> GetAsNoTracking();

        /// <summary>
        /// Retrieve a queryable object with included navigations.
        /// </summary>
        /// <param name="navigations">Expressions specifying the child objects to include.</param>
        /// <returns>A queryable object.</returns>
        IQueryable<T> GetWith(params Expression<Func<T, object>>[] navigations);

        /// <summary>
        /// Retrieve a queryable object with included navigations and disabled tracking.
        /// </summary>
        /// <param name="navigations">Expressions specifying the child objects to include.</param>
        /// <returns>A queryable object.</returns>
        IQueryable<T> GetAsNoTrackingWith(params Expression<Func<T, object>>[] navigations);

        /// <summary>
        /// Add an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity type.</param>
        void Add(T entity);

        /// <summary>
        /// Add a list of entities to the repository.
        /// </summary>
        /// <param name="entities">The entity type.</param>
        void AddRange(IList<T> entities);

        /// <summary>
        /// Delete an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity type.</param>
        void Delete(T entity);

        /// <summary>
        /// Delete a list of entities form the repository.
        /// </summary>
        /// <param name="entities">The entity type.</param>
        void DeleteRange(IList<T> entities);

        /// <summary>
        /// Attach an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity type.</param>
        void Attach(T entity);

        /// <summary>
        /// Attach a list of entities to the repository.
        /// </summary>
        /// <param name="entities">The entity type.</param>
        void AttachRange(IList<T> entities);

        /// <summary>
        /// Detach an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity type.</param>
        void Detach(T entity);
    }
}
