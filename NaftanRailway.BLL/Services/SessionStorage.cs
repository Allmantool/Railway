using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NaftanRailway.BLL.DTO.Guild18;
using NaftanRailway.BLL.Abstract;

namespace NaftanRailway.BLL.Services {
    /// <summary>
    /// Save Temp user choose. For greating Report
    /// This tool use session mechanism (throught binding 'StorageTableModelBinder') to persist
    /// </summary>
    public class SessionStorage : ISessionStorage {
        private readonly List<ShippingInfoLineDTO> _linesCollection;
        /// <summary>
        /// list shipingInfo lines
        /// </summary>
        public IEnumerable<ShippingInfoLineDTO> Lines {
            get { return this._linesCollection; }
        }

        [Required]
        [Display(Name = @"Отчётный период")]
        [DataType(DataType.DateTime)]
        public DateTime ReportPeriod { get; set; }

        public SessionStorage() {
            this._linesCollection = new List<ShippingInfoLineDTO>();
            this.ReportPeriod = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }
        public SessionStorage(DateTime reportPeriod) {
            this._linesCollection = new List<ShippingInfoLineDTO>();
            this.ReportPeriod = reportPeriod;
        }

        /// <summary>
        /// Add shippingInfo line
        /// </summary>
        /// <param name="documentPack"></param>
        public void Additem(ShippingInfoLineDTO documentPack) {
            ShippingInfoLineDTO line = this._linesCollection.FirstOrDefault(sh => sh.Shipping.id == documentPack.Shipping.id);

            if (line == null) {
                this._linesCollection.Add(documentPack);
            }
        }

        /// <summary>
        /// Clear storage
        /// </summary>
        public void Clear() {
            this._linesCollection.Clear();
        }

        /// <summary>
        /// Save changes/ correction in line
        /// </summary>
        /// <param name="line"></param>
        public void SaveLine(ShippingInfoLineDTO line) {
            if (this._linesCollection.Any(m => m.Shipping.id == line.Shipping.id)) {
                this.RemoveLine(line);
            }

            this.Additem(line);
        }

        public void RemoveLine(ShippingInfoLineDTO shipping) {
            throw new NotImplementedException();
        }
    }
}