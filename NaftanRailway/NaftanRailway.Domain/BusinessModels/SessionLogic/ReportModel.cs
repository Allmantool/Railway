using System;

namespace NaftanRailway.Domain.BusinessModels {
    /// <summary>
    /// Report view model 
    /// Also it's model will be used for store in sessionDb 
    /// and then Reposrt Server can have normal DataSet
    /// </summary>
    public class ReportModel {
        /// <summary>
        /// Номер отправки
        /// </summary>
        public string n_otpr { get; set; }
        /// <summary>
        /// Номер вагона
        /// </summary>
        public string n_vag { get; set; }
        /// <summary>
        /// Дата накладной
        /// </summary>
        public DateTime date_oper { get; set; }
        /// <summary>
        /// Код сбора
        /// </summary>
        public string cod_sb { get; set; }
        /// <summary>
        /// Наименование сбора
        /// </summary>
        public string nameSb { get; set; }
        /// <summary>
        /// Стоимость без НДС
        /// </summary>
        public decimal sum_no_nds { get; set; }
        /// <summary>
        /// НДС (процент)
        /// </summary>
        public decimal nds { get; set; }
        /// <summary>
        ///  НДС
        /// </summary>
        public decimal sum_nds { get; set; }
        /// <summary>
        /// Стоимость с НДС
        /// </summary>
        public decimal sum_with_nds { get; set; }
        /// <summary>
        /// Номер перечня
        /// </summary>
        public string n_per_list { get; set; }
        /// <summary>
        /// Номер накопительной карты
        /// </summary>
        public string n_kart { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// Склад (по нему идет группировка)
        /// </summary>
        public string warehouse { get; set; }
    }
}
