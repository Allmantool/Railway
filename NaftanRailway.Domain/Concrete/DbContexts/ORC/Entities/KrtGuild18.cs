namespace NaftanRailway.Domain.Concrete.DbContexts.ORC.Entities
{
    using System;

    public class KrtGuild18
    {
        public DateTime reportPeriod { get; set; }

        public int? warehouse { get; set; }

        public int? idDeliviryNote { get; set; }

        public byte type_doc { get; set; }

        public int? idSrcDocument { get; set; }

        public bool codeType { get; set; }

        public int code { get; set; }

        public decimal sum { get; set; }

        public decimal rateVAT { get; set; }

        public long? idScroll { get; set; }

        public int? idCard { get; set; }

        public int? idSapod { get; set; }

        public long? scrollColl { get; set; }

        public int Id { get; set; }
    }
}