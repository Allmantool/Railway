//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NaftanRailway.Domain.Concrete.DbContext.SessionState
{
    using System;
    using System.Collections.Generic;
    
    public partial class SessionShipping
    {
        public SessionShipping()
        {
            this.SessionBills = new HashSet<SessionBill>();
        }
    
        public System.DateTime reportPeriod { get; set; }
        public int id_otpr { get; set; }
        public long id_wag { get; set; }
        public string n_otpr { get; set; }
        public string n_wag { get; set; }
        public int warehouse { get; set; }
        public string cargo { get; set; }
        public string address { get; set; }
        public string executor { get; set; }
    
        public virtual ICollection<SessionBill> SessionBills { get; set; }
    }
}