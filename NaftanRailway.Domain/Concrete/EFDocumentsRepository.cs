using System;
using System.Collections.Generic;
using System.Linq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.Domain.Concrete.DbContext.Mesplan;
using MoreLinq;
using NaftanRailway.Domain.BusinessModels;


namespace NaftanRailway.Domain.Concrete {
    public class EFDocumentsRepository : IDocumentsRepository {
        private bool _disposed = false;
        private readonly OBDEntities _dbSopodContext = new OBDEntities();
        private readonly ORCEntities _dbOrcContext = new ORCEntities();
        private readonly MesplanEntities _mesplanContext = new MesplanEntities();
        private const int ShiftDate = 3;

        /// <summary>
        /// Главный запрос получения всех первичных документов по номеру отправки, с возможностью указания склада хранения
        /// </summary>
        /// <param name="shipping"></param>
        /// <param name="warehouese"></param>
        /// <returns></returns>
        public ShippingInfoLine PackDocuments(v_otpr shipping, int warehouese = 1) {
            if(shipping.date_oper == null) {
                return new ShippingInfoLine();
            }
            DateTime chooseDate = new DateTime(shipping.date_oper.Value.Year, shipping.date_oper.Value.Month, 1);
            DateTime startDate = chooseDate.AddDays(-ShiftDate);
            DateTime endDate = chooseDate.AddMonths(1).AddDays(ShiftDate);

            #region Wagons Number

            List<v_o_v> wagonsNumbers = CarriageNumbers
                .Where(v => v.id_otpr == shipping.id)
                .DistinctBy(v => v.id)
                .OrderByDescending(w => w.n_vag)
                .ToList();

            #endregion

            //(LINQ to Entity) поддерживаются только типы-примитивы и типы перечисления
            var numbersVagons = wagonsNumbers.Select(v => v.n_vag);

            #region Bills

            List<Bill> bills = (Bills)
                .Where(b => numbersVagons.Contains(b.VPamVag.nomvag) && (b.VPam.dved >= startDate && b.VPam.dved <= endDate))
                .DistinctBy(b => b.VPam.id_ved)
                .OrderByDescending(b => b.VPam.nved)
                .ToList();

            #endregion

            #region Cerificates

            List<Certificate> acts = (Certificates)
                .Where(a => numbersVagons.Contains(a.VAktVag.nomvag) && (a.VAkt.dakt >= startDate && a.VAkt.dakt <= endDate))
                .DistinctBy(a => a.VAkt.id)
                .OrderByDescending(a => a.VAkt.nakt)
                .ToList();

            #endregion

            //(LINQ to Entity) поддерживаются только типы-примитивы и типы перечисления
            var billNumbers = bills.Select(b => b.VPam.nved);
            var actNumbers = acts.Select(a => a.VAkt.nakt);

            #region Accumulative Cards

            var listUb = bills.Select(b => b.VPamVag.d_ub != null ? b.VPamVag.d_ub.Value.Date : new DateTime());
            var listUbPod = bills.Select(b => b.VPamVag.d_pod != null ? b.VPamVag.d_pod.Value.Date : new DateTime());

            //065
            List<AccumulativeCard> cards = (Cards)
                .Where(c => c.VNach.type_doc == 4 && c.VNach.cod_sbor == "065" &&
                            (listUb.Contains(c.VNach.date_raskr.Value) || listUbPod.Contains(c.VNach.date_raskr.Value)) &&
                            (c.VNach.date_raskr >= startDate && c.VNach.date_raskr <= endDate))
                .DistinctBy(c => c.VKart.id)
                .ToList();

            //Current Document
            cards.AddRange((Cards)
                .Where(c => (shipping.n_otpr == c.VNach.num_doc ||
                             numbersVagons.Contains(c.VNach.num_doc) ||
                             billNumbers.Contains(c.VNach.num_doc) ||
                             actNumbers.Contains(c.VNach.num_doc)) &&
                            (c.VNach.date_raskr >= startDate && c.VNach.date_raskr <= endDate))
                .DistinctBy(c => c.VKart.id));

            /* Advantage cards (for user option)
             * cards.AddRange(((IQueryable<AccumulativeCard>)Cards)
                .Where(c => c.VNach.type_doc == 4 && 
                    (c.VNach.date_raskr >= startDate && c.VNach.date_raskr <= endDate) &&
                     new[] { "065","067" }.Contains(c.VNach.cod_sbor))
                .DistinctBy(c => c.VKart.id)
                .OrderByDescending(c => c.VKart.num_kart));
             */

            #endregion

            //(LINQ to Entity) поддерживаются только типы-примитивы и типы перечисления
            var cardNumbers = cards.Select(c => c.VKart.num_kart);

            #region Luggage

            //Related document
            List<Luggage> luggages = (Baggage)
                .Where(l => l.OrcSbor.NOMOT == shipping.n_otpr ||
                            numbersVagons.Contains(l.OrcSbor.NOMOT) ||
                            billNumbers.Contains(l.OrcSbor.NOMOT) ||
                            actNumbers.Contains(l.OrcSbor.NOMOT) ||
                            cardNumbers.Contains(l.OrcSbor.NOMOT) &&
                            (l.OrcKrt.DTOPEN >= startDate && l.OrcKrt.DTOPEN <= endDate))
                .DistinctBy(l => l.OrcKrt.KEYKRT)
                .ToList();

            //Baggage
            luggages.AddRange((Baggage)
                .Where(l => l.OrcKrt.U_KOD == 2 && (l.OrcKrt.DTOPEN >= startDate && l.OrcKrt.DTOPEN <= endDate))
                .DistinctBy(l => l.OrcKrt.KEYKRT)
                .ToList());

            #endregion

            //don't need empty constructor
            return new ShippingInfoLine() {
                Warehouse = warehouese,
                Shipping = shipping,
                WagonsNumbers = wagonsNumbers,
                Bills = bills,
                Acts = acts,
                Cards = cards,
                Luggages = luggages
            };
        }

        /// <summary>
        /// Take general information about three linq join entities (v_otpr, v_o_v, etsng (mesplan))
        /// </summary>
        public IQueryable<Shipping> ShippingInformation {
            get {
                return (from itemOtpr in ShippinNumbers
                        join itemOv in CarriageNumbers on itemOtpr.id equals itemOv.id_otpr into gOtpr
                        from otrpitemResultVov in gOtpr.DefaultIfEmpty()
                        select new Shipping() { VOtpr = itemOtpr }).AsQueryable();
            }
        }

        /// <summary>
        /// Первичная выборка ведомостей
        /// </summary>
        public IQueryable<Bill> Bills {
            get {
                return (from pv in _dbSopodContext.v_pam_vags
                        join p in _dbSopodContext.v_pams on pv.id_ved equals p.id_ved into gPams
                        from pam in gPams.DefaultIfEmpty()
                        join psb in _dbSopodContext.v_pam_sbs on pv.id_ved equals psb.id_ved into gPamsSb
                        from pamSb in gPamsSb.DefaultIfEmpty()
                        where pam.state == 32 && new[] { "3494", "349402" }.Contains(pam.kodkl)
                        select new Bill() { VPam = pam, VPamVag = pv, VPamSb = pamSb }).AsQueryable();
            }
        }
        /// <summary>
        /// Первичная выборка актов
        /// </summary>
        public IQueryable<Certificate> Certificates {
            get {
                return (from av in _dbSopodContext.v_akt_vags
                        join a in _dbSopodContext.v_akts
                            on av.id_akt equals a.id into gAkts
                        from act in gAkts.DefaultIfEmpty()
                        join aSb in _dbSopodContext.v_akt_sbs
                            on av.id_akt equals aSb.id_akt into gaSb
                        from aSb in gaSb.DefaultIfEmpty()
                        where act.state == 32 && new[] { "3494", "349402" }.Contains(act.kodkl)
                        select new Certificate { VAkt = act, VAktVag = av, VAktSb = aSb })
                    .AsQueryable();
            }
        }
        /// <summary>
        /// Первичная выборка накопительных карточек
        /// </summary>
        public IQueryable<AccumulativeCard> Cards {
            get {
                return (from k in _dbSopodContext.v_karts
                        join n in _dbSopodContext.v_nachs
                            on k.id equals n.id_kart into gNach
                        from nach in gNach.DefaultIfEmpty()
                        where
                            (new[] { "3494", "349402" }.Contains(nach.cod_kl) || new[] { "3494", "349402" }.Contains(k.cod_pl))
                        select new AccumulativeCard { VKart = k, VNach = nach }).AsQueryable();
            }
        }
        /// <summary>
        /// Первичная выборка по баггажу
        /// </summary>
        public IQueryable<Luggage> Baggage {
            get {
                return (from ok in _dbOrcContext.orc_krts
                        join os in _dbOrcContext.orc_sbors
                            on ok.KEYKRT equals os.KEYKRT into gSbor
                        from orcSbor in gSbor
                        select new Luggage { OrcKrt = ok, OrcSbor = orcSbor }).AsQueryable();
            }
        }

        /// <summary>
        /// Первичная выборка номеров отправок
        /// </summary>
        public IQueryable<v_otpr> ShippinNumbers {
            get {
                return (_dbSopodContext.v_otprs
                    .Where(x => x.state == 32 &&
                                ((new[] { "3494", "349402" }.Contains(x.cod_kl_otpr) && x.oper == 1) ||
                                 (new[] { "3494", "349402" }.Contains(x.cod_klient_pol) && x.oper == 2)))
                    .OrderByDescending(x => x.date_oper)
                    .AsQueryable());
            }
        }
        /// <summary>
        /// Первичная выборка вагонов
        /// </summary>
        public IQueryable<v_o_v> CarriageNumbers {
            get {
                return _dbSopodContext.v_o_vs
                    .AsQueryable();
            }
        }

        public IQueryable<v_pam_vag> PamVags {
            get { return _dbSopodContext.v_pam_vags.AsQueryable(); }
        }

        public IQueryable<v_pam_sb> PamSbs {
            get { return _dbSopodContext.v_pam_sbs.AsQueryable(); }
        }

        public IQueryable<v_pam> Pams {
            get {
                _dbSopodContext.v_pam_sbs.AsQueryable();
                return null;
            }
        }

        public IQueryable<v_akt> Akts {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_akt_sb> AktSbs {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_akt_vag> AktVags {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_kart> Karts {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<v_nach> Naches {
            get { throw new NotImplementedException(); }
        }

        public IQueryable<orc_krt> OrcKrts {
            get { return _dbOrcContext.orc_krts; }
        }

        public IQueryable<orc_sbor> OrcSbors {
            get { return _dbOrcContext.orc_sbors; }
        }

        /// <summary>
        /// Return etsng table
        /// </summary>
        public IQueryable<etsng> Etsngs {
            get { return _mesplanContext.etsngs.AsQueryable(); }
        }

        public IQueryable<krt_Naftan> KrtNaftans {
            get { return _dbOrcContext.krt_Naftans; }
        }

        public IQueryable<krt_Naftan_orc_sapod> KrtNaftanOrcSapods {
            get { return _dbOrcContext.krt_Naftan_orc_sapods; }
        }

        protected virtual void Dispose(bool disposing) {
            if(!_disposed) {
                if(disposing) {
                    _dbSopodContext.Dispose();
                    _dbOrcContext.Dispose();
                    _mesplanContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AddKrtNaftan(krt_Naftan record) {
            _dbOrcContext.krt_Naftans.Add(record);
            _dbOrcContext.SaveChanges();
        }
    }
}

