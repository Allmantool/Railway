using NaftanRailway.Domain.Concrete.DbContext.OBD;

namespace NaftanRailway.Domain.BusinessModels {
    /// <summary>
    /// Карточки (2 сущности EF)
    /// </summary>
    public class AccumulativeCard {
        public bool IsSelected { get; set; }

        public AccumulativeCard() {
            IsSelected = true;
        }

        /// <summary>
        /// Таблица начислений по карточке (сборов)
        /// </summary>
        public v_nach VNach { get; set; }

        /// <summary>
        /// Карточки с платежами
        /// </summary>
        public v_kart VKart { get; set; }

        public override string ToString() {
            return VKart.num_kart;
        }
    }
}
