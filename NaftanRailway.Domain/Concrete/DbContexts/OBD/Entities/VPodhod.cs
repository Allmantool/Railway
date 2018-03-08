namespace NaftanRailway.Domain.Concrete.DbContexts.OBD.Entities
{
    using System;

    public class VPodhod
    {
        public string n_vag { get; set; }

        public string kod_etsng { get; set; }

        public string kod_pol { get; set; }

        public DateTime? date_oper_v { get; set; }
    }
}