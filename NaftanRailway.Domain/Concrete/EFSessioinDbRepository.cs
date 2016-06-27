using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.SessionState;

namespace NaftanRailway.Domain.Concrete.DbContext {
    public class EFSessioinDbRepository : ISessionDbRepository {
        private bool _disposed = false;
        private readonly SessionDbContext _dbContext = new SessionDbContext();

        /// <summary>
        /// Save data from SubReport in db
        /// Method triggered before display Report
        /// Alter data is possible throughtout merge tsqloperator or alternative linq to sql
        /// Save PreReport data add some benefit as view prior report and work later with select data even some trouble in server
        /// </summary>
        /// <param name="session"></param>
        public void AddPreReportData(SessionStorage session) {
            int i = 0;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
            try {
                //shipping
                foreach(ShippingInfoLine line in session.Lines) {
                    //Wagons
                    foreach(v_o_v wagonsNumber in line.WagonsNumbers) {
                        _dbContext.SessionShippings.Add(new SessionShipping() {
                            id_otpr = line.Shipping.id,
                            id_wag = wagonsNumber.id,
                            reportPeriod = new DateTime(session.ReportPeriod.Year, session.ReportPeriod.Month, 1),
                            n_otpr = line.Shipping.n_otpr,
                            n_wag = wagonsNumber.n_vag,
                            //warehouse = line.Warehouse,
                            cargo = line.Shipping.g11,
                            address = line.Shipping.g8,
                            executor = line.Shipping.fio_tk,
                            SessionBills = new List<SessionBill>() {
                                new SessionBill() {
                                    id = i,
                                    id_otpr = line.Shipping.id,
                                    id_wag = wagonsNumber.id,
                                    reportPeriod =
                                        new DateTime(session.ReportPeriod.Year, session.ReportPeriod.Month, 1),
                                    n_bill = "000001"
                                }
                            }
                        });
                    }
                }
                _dbContext.ChangeTracker.DetectChanges();
                _dbContext.SaveChanges();
            }
            catch(DbEntityValidationException dbEx) {
                foreach(var validationErrors in dbEx.EntityValidationErrors) {
                    foreach(var validationError in validationErrors.ValidationErrors) {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch(Exception exception) {

            }


        }

        protected virtual void Dispose(bool disposing) {
            if(!_disposed) {
                if(disposing) {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
