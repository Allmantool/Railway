using System;
using NaftanRailway.Domain.BusinessModels;

namespace NaftanRailway.Domain.Abstract {
    public interface ISessionDbRepository: IDisposable {
         void AddPreReportData(SessionStorage session);
    }
}
