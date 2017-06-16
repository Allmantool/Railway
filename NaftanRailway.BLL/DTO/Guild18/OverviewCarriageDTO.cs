﻿using NaftanRailway.Domain.Concrete.DbContexts.OBD;

namespace NaftanRailway.BLL.DTO.Guild18 {
    public class OverviewCarriageDTO {
        public string Cargo { get; set; }
        public v_OPER_ASUS Carriage { get; set; }

        public string AltCargo { get; set; }
        public v_02_podhod AltCarriage { get; set; }
    }
}
