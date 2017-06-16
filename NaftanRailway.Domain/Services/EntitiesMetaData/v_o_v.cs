namespace NaftanRailway.Domain.Concrete.DbContexts.OBD
{
    public partial class v_o_v
    {
        public v_o_v()
        {
            IsSelected = true;
        }

        public bool IsSelected { get; }

        public override string ToString()
        {
            return n_vag;
        }
    }
}