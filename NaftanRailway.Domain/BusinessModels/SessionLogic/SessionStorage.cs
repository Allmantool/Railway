using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NaftanRailway.Domain.Concrete.DbContext.OBD;

namespace NaftanRailway.Domain.BusinessModels {
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

        public SessionStorage() {
            _linesCollection = new List<ShippingInfoLine>();
            ReportPeriod = DateTime.Today;
        }

        [Required]
        [Display(Name = @"Отчётный период")]
        [DataType(DataType.DateTime)]
        public DateTime ReportPeriod { get; set; }

        public SessionStorage(DateTime reportPeriod) {
            _linesCollection = new List<ShippingInfoLine>();
            ReportPeriod = reportPeriod;
        }

        /// <summary>
        /// Add shippingInfo line
        /// </summary>
        /// <param name="documentPack"></param>
        public void AddItem(ShippingInfoLine documentPack) {
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

            AddItem(line);
        }

        /// <summary>
        /// apply changes (check/unchecked and etc)
        /// </summary>
        public void Update(ShippingInfoLine line) {
            //wagons
            if (line.WagonsNumbers != null) {
                List<v_o_v> fixLine = line.WagonsNumbers.ToList().FindAll(w => w.IsSelected);
                //remove all don't work
                line.WagonsNumbers.Clear();
                line.WagonsNumbers = fixLine;
            }
            //bills
            if (line.Bills != null) {
                List<Bill> fixLine = line.Bills.ToList().FindAll(b => b.IsSelected);
                //remove all don't work
                line.Bills.Clear();
                line.Bills = fixLine;
            }
            //acts
            if (line.Acts != null) {
                List<Certificate> fixLine = line.Acts.ToList().FindAll(b => b.IsSelected);
                //remove all don't work
                line.Acts.Clear();
                line.Acts = fixLine;
            }
            //cards
            if (line.Cards != null) {
                List<AccumulativeCard> fixLine = line.Cards.ToList().FindAll(b => b.IsSelected);
                //remove all don't work
                line.Cards.Clear();
                line.Cards = fixLine;
            }
        }

        /// <summary>
        /// Подготовка модели для:
        /// 1)записи в БД (также необходимы доп. запросы к бд ORC для получения сумм)
        /// 2)средствами Report Server отобразить данные записанные в БД
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ReportModel> ToReport() {
            List<ReportModel> result = new List<ReportModel>();

            foreach (var line in _linesCollection) {
                result.Add(new ReportModel() {
                    n_otpr = line.Shipping.n_otpr,
                    n_vag = String.Join(",\n",line.WagonsNumbers),
                    date_oper = line.Shipping.date_oper.Value,
                    cod_sb = "300",
                    nameSb = "Тариф по отправке БЧ",
                    sum_no_nds = 2930988,
                    nds = 20,
                    sum_nds = 586198,
                    sum_with_nds = 3517186,
                    n_per_list = "25795",
                    n_kart = "ЖТ2922",
                    note = "160303",
                    warehouse = line.Warehouse.ToString()
                });
            }

            return result;
        }
    }
}
