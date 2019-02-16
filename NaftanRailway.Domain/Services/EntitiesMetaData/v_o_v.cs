namespace NaftanRailway.Domain.Concrete.DbContexts.OBD
{
    public partial class v_o_v
    {
        public v_o_v()
        {
            this.IsSelected = true;
        }

        public bool IsSelected { get; }

        public override string ToString()
        {
            return this.n_vag;
        }
    }
}