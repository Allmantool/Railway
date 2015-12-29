using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
namespace NaftanRailway.UnitTest.Tests {
    [TestClass]
    public class LinqTests {
        [TestMethod]
        public void LeftOuterJoin() {
            //Arrange
            List<v_pam> pams = new List<v_pam>() {
                new v_pam(){id_ved =1 ,nved = "000001" ,state = 32,kodkl = "3494"},
                new v_pam(){id_ved =2 ,nved = "000002" ,state = 32,kodkl = "3494"},
                new v_pam(){id_ved =3 ,nved = "000003",state = 32,kodkl = "349402"},
                new v_pam(){id_ved =4 ,nved = "000004",state = 32},
                new v_pam(){id_ved =5 ,nved = "000005",state = 30,kodkl = "3494"},
            };

            List<v_pam_vag> pamVags = new List<v_pam_vag>() {
                new v_pam_vag(){id_ved = 1,nomvag = "00000001"},
                new v_pam_vag(){id_ved = 2,nomvag = "00000002"},
                new v_pam_vag(){id_ved = 3,nomvag = "00000003"},
                new v_pam_vag(){id_ved = 4,nomvag = "00000004"},
                new v_pam_vag(){id_ved = 5,nomvag = "00000005"},
                new v_pam_vag(){id_ved = 4,nomvag = "00000006"},
                new v_pam_vag(){id_ved = 3,nomvag = "00000007"},
            };

            List<v_pam_sb> pamSbs = new List<v_pam_sb>() {
                new v_pam_sb(){id_ved = 1},
                new v_pam_sb(){id_ved = 2},
                new v_pam_sb(){id_ved = 3},
                new v_pam_sb(){id_ved = 7},
                new v_pam_sb(){id_ved = 6},
            };

            List<v_o_v> wagonsNumbers = new List<v_o_v>() {
                new v_o_v(){n_vag = "00000001"},
                new v_o_v(){n_vag = "00000002"},
                new v_o_v(){n_vag = "00000002"},
                new v_o_v(){n_vag = "00000003"},
            };

            IQueryable bills = (from pv in pamVags
                                join p in pams on pv.id_ved equals p.id_ved into g1
                                from y in g1.DefaultIfEmpty()
                                join psb in pamSbs on pv.id_ved equals psb.id_ved into g2
                                from x in g2.DefaultIfEmpty()
                                select new Bill() { VPam = y, VPamVag = pv, VPamSb = x }).AsQueryable();

            List<Bill> linq = ((IQueryable<Bill>)bills)
                .Where(b => wagonsNumbers.Exists(ws => ws.n_vag == b.VPamVag.nomvag))
                .DistinctBy(b => b.VPam.id_ved)
                .OrderByDescending(b => b.VPam.nved).ToList();

            //Assert
            Assert.AreEqual(3, linq.Count());
            Assert.AreEqual(3, linq.Count(c => c.VPam != null));
            Assert.AreEqual(3, linq.Count(c => c.VPamSb != null));
            Assert.AreEqual("000003", linq.First().VPam.nved);
        }
    }
}
