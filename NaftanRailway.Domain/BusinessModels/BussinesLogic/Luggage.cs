using NaftanRailway.Domain.Concrete.DbContext.ORC;

namespace NaftanRailway.Domain.BusinessModels {
    public class Luggage {
        /// <summary>
        /// Таблица карточек ОРЦ
        /// </summary>
        public orc_krt OrcKrt { get; set; }
        /// <summary>
        /// Сборы к карточка (ОРЦ)
        /// </summary>
        public orc_sbor OrcSbor { get; set; }

        public override string ToString() {
            return OrcSbor.NAMESBR;
        }
    }
}
