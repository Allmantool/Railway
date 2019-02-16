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
        public IDictionary<string, string> ValuesDictionary { get; set; }

        public IEnumerable<string> AllAvailableValues { get; set; }
        public IEnumerable<string> CheckedValues { get; set; }

        /// <summary>
        /// It requeres a real name of entity/ table for work with reflaction mechanisms principals
        /// </summary>
        public string FieldName { get; set; }
        //another variant add <T> general type for filter and with reflection return metadata propeprty from entity model
        public string NameDescription { get; set; }
        public bool ActiveFilter { get; set; }

        public CheckListFilter() {

        }

        /// <summary>
        /// By default check all values on list box
        /// </summary>
        public CheckListFilter(IEnumerable<string> allAvailableValues) :
            this(allAvailableValues.GroupBy(x => x).ToDictionary(x => x.First(), x => x.First())) { }

        /// <summary>
        /// Для данного объекта не определено беспараметрических конструкторов. (from .js)
        /// </summary>
        public CheckListFilter(IDictionary<string, string> keyVal) {
            this.ValuesDictionary = keyVal;

            this.AllAvailableValues = this.ValuesDictionary.Values.Select(x => x);
            this.CheckedValues = this.ValuesDictionary.Keys.Select(x => x);
        }

        /// <summary>
        /// Filter by concrete(current) fields
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> FilterByField<T>() where T : class {
            var predicate = this.CheckedValues.Aggregate(
                PredicateBuilder.New<T>(false).DefaultExpression,
                (current, innerItem) => current.Or(PredicateExtensions.FilterByName<T>(this.FieldName, innerItem))
            );

            return predicate;
        }
    }
}