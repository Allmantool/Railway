using NaftanRailway.Domain.Concrete.DbContext.OBD;

namespace NaftanRailway.Domain.BusinessModels {
    public class Certificate {
        public bool IsSelected { get; set; }

        public Certificate() {
            IsSelected = true;
        }
        /// <summary>
        /// Таблица актов
        /// </summary>
        public v_akt VAkt { get; set; }
        /// <summary>
        /// Вагоны к актам
        /// </summary>
        public v_akt_vag VAktVag { get; set; }
        /// <summary>
        /// Сборы по актам
        /// </summary>
        public v_akt_sb VAktSb { get; set; }

        public override string ToString() {
            return VAkt.nakt;
        }
    }
}
