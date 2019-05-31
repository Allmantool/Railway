namespace Railway.DeliveryCargo.Data.EF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;
    using Railway.DeliveryCargo.Data.EF.Interfaces;

    public class Repository<T> : IRepository<T>
        where T : class
    {
        public Repository(DbContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        protected DbContext Context { get; }

        protected DbSet<T> DbSet { get; }

        public IQueryable<T> Get()
        {
            return DbSet;
        }

        public IQueryable<T> GetAsNoTracking()
        {
            return DbSet.AsNoTracking();
        }

        public IQueryable<T> GetWith(params Expression<Func<T, object>>[] navigations)
        {
            IQueryable<T> query = DbSet;

            if (navigations == null || navigations.Length == 0)
            {
                return query;
            }

            return navigations.Aggregate(query, (current, navigation) => current.Include(navigation));
        }

        public IQueryable<T> GetAsNoTrackingWith(params Expression<Func<T, object>>[] navigations)
        {
            IQueryable<T> query = DbSet.AsNoTracking();

            if (navigations == null || navigations.Length == 0)
            {
                return query;
            }

            return navigations.Aggregate(query, (current, navigation) => current.Include(navigation));
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void AddRange(IList<T> entities)
        {
            DbSet.AddRange(entities);
        }

        public void Delete(T entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            DbSet.Remove(entity);
        }

        public void DeleteRange(IList<T> entities)
        {
            var detached = entities.Where(entity => Context.Entry(entity).State == EntityState.Detached);
            foreach (var item in detached)
            {
                DbSet.Attach(item);
            }

            DbSet.RemoveRange(entities);
        }

        public void Attach(T entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Detach(T entity)
        {
            Context.Entry(entity).State = EntityState.Detached;
        }

        public void AttachRange(IList<T> entities)
        {
            foreach (var item in entities)
            {
                DbSet.Attach(item);
                Context.Entry(item).State = EntityState.Modified;
            }
        }
    }
}
