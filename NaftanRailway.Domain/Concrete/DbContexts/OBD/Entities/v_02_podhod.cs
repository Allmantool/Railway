using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Railway.Domain.Concrete.DbContexts.OBD.Entities
{
    public class v_02_podhod
    {
        public string n_vag { get; set; }
        public Nullable<int> massa_t { get; set; }
        public string st_nazn { get; set; }
        public string kod_etsng { get; set; }
        public string kod_pol { get; set; }
        public string prim { get; set; }
        public Nullable<System.DateTime> date_oper_v { get; set; }
        public Nullable<short> oper { get; set; }
        public string n_otpr { get; set; }
        public short pr_v { get; set; }
        public string kod_stan_oper { get; set; }
        public System.DateTime date_oper_t { get; set; }
    }
}
