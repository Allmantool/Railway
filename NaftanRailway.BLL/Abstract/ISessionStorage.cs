using System;
using System.Collections.Generic;
using NaftanRailway.BLL.DTO.Guild18;

namespace NaftanRailway.BLL.Abstract {
    public interface ISessionStorage {
        IEnumerable<ShippingInfoLineDTO> Lines { get; }
        DateTime ReportPeriod { get; set; }

        void Additem(ShippingInfoLineDTO documentPack);

        void Clear();

        void RemoveLine(ShippingInfoLineDTO shipping);

        void SaveLine(ShippingInfoLineDTO line);
    }
}
