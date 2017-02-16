using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace NaftanRailway.BLL.Services {
    /// <summary>
    /// Operation type on cargo (Sending or Arrivals)
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EnumOperationType : short {
        All = 0,
        Sending = 1,
        Arrivals = 2,
    }

    /// <summary>
    /// Type of opertion in quik menu (details nomenclature)
    /// </summary>
    [Serializable]
    public enum EnumMenuOperation : short {
        Join = 0,
        Edit = 1,
        Delete = 2,
    }

    ///// <summary>
    ///// Detect in how mode we are working (for generalization purpose)
    ///// </summary>
    //[Serializable]
    //public enum EnumNomeclatureMode : short {
    //    General = 0,
    //    Detail = 1
    //}

    [Serializable]
    public enum EnumTypeFilterMenu : short {
        Nomenclature = 0,
        NomenclatureDetail = 1
    }
}