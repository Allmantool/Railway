using System.Collections.Generic;

namespace NaftanRailway.Domain.Concrete.DbContexts.OBD
{
    public partial class v_pam_vag
    {
        public ICollection<v_o_v> Vag { get; set; }
    }
}