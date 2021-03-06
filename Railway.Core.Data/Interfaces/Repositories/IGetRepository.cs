﻿namespace Railway.Core.Data.Interfaces.Repositories
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IGetRepository<T>
        where T : class
    {
        IQueryable<T> GetAll(
            Expression<Func<T, bool>> predicate = null,
            bool enableDetectChanges = true,
            bool enableTracking = true);

        T Get(
            Expression<Func<T, bool>> predicate = null,
            bool enableDetectChanges = true,
            bool enableTracking = true);

        T Find<TK>(TK key, bool enableDetectChanges = true);
    }
}