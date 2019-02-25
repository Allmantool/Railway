﻿namespace NaftanRailway.Domain.Abstract.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IUpdateRepository<T>
        where T : class, new()
    {
        void Update(
            T entity,
            IEnumerable<Action<T>> operations,
            bool enableDetectChanges = true);

        void Update(
            IQueryable<T> entities,
            IEnumerable<Action<T>> operations,
            bool enableDetectChanges = true);

        void Update(
            Expression<Func<T, bool>> predicate,
            IEnumerable<Action<T>> operations,
            bool enableDetectChanges = true);
    }
}