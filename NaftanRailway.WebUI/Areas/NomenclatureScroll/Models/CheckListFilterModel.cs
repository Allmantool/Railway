using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using NaftanRailway.Domain.ExpressionTreeExtensions;

namespace NaftanRailway.WebUI.Areas.NomenclatureScroll.Models {
    /// <summary>
    /// General model for drop down checkbox list (per one field)
    /// </summary>
    public class CheckListFilterModel {
        public IEnumerable<string> AllAvailableValues { get; set; }
        public IEnumerable<string> CheckedValues { get; set; }
        public string SortFieldName { get; set; }
        //another variant add <T> general type for filter and with reflection return metadata propeprty from entity model
        public string NameDescription { get; set; }
        public bool ActiveFilter { get; set; }
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
        /// Filter by concrete(current) fields
        /// </summary>
        /// <returns></returns>
        public Expression<Func<T, bool>> FilterByField<T>() where T : class{
            var predicate = PredicateBuilder.False<T>();
            predicate = CheckedValues.Aggregate(predicate, (current, innerItem) => current.Or(EtExtensions.FilterByName<T>(SortFieldName, innerItem)));
            
            return predicate;
        } 
    }
}