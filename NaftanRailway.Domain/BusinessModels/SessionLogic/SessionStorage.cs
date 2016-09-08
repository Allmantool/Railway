using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NaftanRailway.Domain.BusinessModels.BussinesLogic;
using NaftanRailway.Domain.Concrete.DbContexts.OBD;

namespace NaftanRailway.Domain.BusinessModels.SessionLogic {
    /// <summary>
    /// Save Temp user choose. For greating Report
    /// </summary>
    public class SessionStorage {
        private readonly List<ShippingInfoLine> _linesCollection;
        /// <summary>
        /// list shipingInfo lines
        /// </summary>
        public IEnumerable<ShippingInfoLine> Lines {
            get { return _linesCollection; }
        }

        [Required]
        [Display(Name = @"Отчётный период")]
        [DataType(DataType.DateTime)]
        public DateTime ReportPeriod { get; set; }

        public SessionStorage() {
            _linesCollection = new List<ShippingInfoLine>();
            ReportPeriod = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
        public SessionStorage(DateTime reportPeriod) {
            _linesCollection = new List<ShippingInfoLine>();
            ReportPeriod = reportPeriod;
        }

        /// <summary>
        /// Add shippingInfo line
        /// </summary>
        /// <param name="documentPack"></param>
        public void Additem(ShippingInfoLine documentPack) {
            ShippingInfoLine line = _linesCollection.FirstOrDefault(sh => sh.Shipping.id == documentPack.Shipping.id);

            if (line == null) {
                _linesCollection.Add(documentPack);
            }
        }

        /// <summary>
        /// Romove line
        /// </summary>
        /// <param name="shipping"></param>
        public void RemoveLine(v_otpr shipping) {
            _linesCollection.RemoveAll(l => l.Shipping.id == shipping.id);
        }

        /// <summary>
        /// Clear storage
        /// </summary>
        public void Clear() {
            _linesCollection.Clear();
        }

        /// <summary>
        /// Save changes/ correction in line
        /// </summary>
        /// <param name="line"></param>
        public void SaveLine(ShippingInfoLine line) {
            if (_linesCollection.Any(m => m.Shipping.id == line.Shipping.id)) {
                RemoveLine(line.Shipping);
            }

            Additem(line);
        }
    }
}