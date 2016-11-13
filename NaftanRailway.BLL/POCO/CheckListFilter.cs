using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using NaftanRailway.BLL.Services.ExpressionTreeExtensions;

namespace NaftanRailway.BLL.POCO {
    /// <summary>
    /// General model for drop down checkbox list (per one field)
    /// </summary>
    public class CheckListFilter {
        public IEnumerable<string> AllAvailableValues { get; set; }
        public IEnumerable<string> CheckedValues { get; set; }
        public string FieldName { get; set; }
        //another variant add <T> general type for filter and with reflection return metadata propeprty from entity model
        public string NameDescription { get; set; }
        public bool ActiveFilter { get; set; }

        /// <summary>
        /// By default check all values on list box
        /// </summary>
        public CheckListFilter(IEnumerable<string> allAvailableValues) {
            var availableValues = allAvailableValues as string[] ?? allAvailableValues.ToArray();

            AllAvailableValues = availableValues;
            CheckedValues = availableValues;
        }

        /// <summary>
        /// Для данного объекта не определено беспараметрических конструкторов. (from .js)
        /// </summary>
        public CheckListFilter(){}

        /// <summary>
        /// Filter by concrete(current) fields
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> FilterByField<T>() where T : class {
            var predicate = CheckedValues.Aggregate(
                PredicateBuilder.False<T>(),
                (current, innerItem) => current.Or(PredicateExtensions.FilterByName<T>(FieldName, innerItem))
            );

            return predicate;
        }
    }   
}