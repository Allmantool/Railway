using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.Ajax.Utilities;
using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {
    /// <summary>
    /// General model for drop down checkbox list (per one field)
    /// </summary>
    public class CheckListFilterModel {
        public IEnumerable<string> AllAvailableValues { get; set; }
        public IEnumerable<string> CheckedValues { get; set; }
        public string SortFieldName { get; set; }
        /// <summary>
        /// By default check all values on list box
        /// </summary>
        public CheckListFilterModel(IEnumerable<string> allAvailableValues) {
            var availableValues = allAvailableValues as string[] ?? allAvailableValues.ToArray();

            AllAvailableValues = availableValues;
            CheckedValues = availableValues;
        }
        public CheckListFilterModel() { }
        /// <summary>
        /// Filter by concrete fields
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //public Expression<Func<T, bool>> FilterByField(T entity){
        //     var predicate = PredicateBuilder.False<T>();
        //     predicate = this.CheckedValues.Aggregate(predicate, (current, innerItem) => current.Or(f => innerItem.Contains(f.nkrt)));
        //    return predicate;
        //} 
    }
}