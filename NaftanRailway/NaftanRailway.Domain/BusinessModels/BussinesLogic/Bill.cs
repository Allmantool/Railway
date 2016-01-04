using NaftanRailway.Domain.Concrete.DbContext.OBD;

namespace NaftanRailway.Domain.BusinessModels {
    /// <summary>
    /// Ведомости (+ 3 сущности EF)
    /// </summary>
    public class Bill {
        public bool IsSelected { get; set; }
        public Bill() {
            IsSelected = true;
        }

        /// <summary>
        /// Шапка ведомости ф.ГУ-46
        /// </summary>
        public v_pam VPam { get; set; }
        /// <summary>
        /// Вагоны к ведомости
        /// </summary>
        public v_pam_vag VPamVag { get; set; }
        /// <summary>
        /// Сборы к ведомости
        /// </summary>
        public v_pam_sb VPamSb { get; set; }

        public override string ToString() {
            return VPam.nved;
        }

    }
}
