using System;

namespace NaftanRailway.Domain.BusinessModels.BussinesLogic {
    /// <summary>
    /// Operation type on cargo (Sending or Arrivals)
    /// </summary>
    [Serializable]
    public enum EnumOperationType : short {
        All = 0,
        Sending = 1,
        Arrivals = 2,
    }
    /// <summary>
    /// Type of opertion in menu
    /// </summary>
    [Serializable]
    public enum EnumMenuOperation : short {
        Join = 0,
        Edit = 1,
        Delete = 2,
    }
}