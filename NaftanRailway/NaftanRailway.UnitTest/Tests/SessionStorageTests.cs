using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NaftanRailway.Domain.Abstract;
using NaftanRailway.Domain.BusinessModels;
using NaftanRailway.Domain.Concrete.DbContext.OBD;
using NaftanRailway.Domain.Concrete.DbContext.ORC;
using NaftanRailway.WebUI.Controllers;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.UnitTest.Tests {
    [TestClass]
    public class SessionStorageTests {
        [TestMethod]
        public void Add_New_Lines() {
            #region Arrange
            //Arrange fill info new Dispatch
            v_otpr i1 = new v_otpr() { id = 1,n_otpr = "09882881" };
            v_otpr i2 = new v_otpr() { id = 2,n_otpr = "09882877" };
            v_otpr i3 = new v_otpr() { id = 4,n_otpr = "09882881" };
            v_otpr i4 = new v_otpr() { id = 3,n_otpr = "09864171" };

            //mock 
            Mock<IDocumentsRepository> mock = new Mock<IDocumentsRepository>();

            mock.Setup(m => m.PackDocuments(i1,1)).Returns(new ShippingInfoLine() {
                Shipping = (new[] { 
                    new v_otpr() { id = 1,n_otpr = "09882881" },
                    new v_otpr() { id = 2,n_otpr = "09882877" },
                    new v_otpr() { id = 4,n_otpr = "09882881" },
                    new v_otpr() { id = 3,n_otpr = "09864171" },
            }).FirstOrDefault(),
                WagonsNumbers = (new[] {
                new v_o_v() {id =1, id_otpr = 2, n_vag = "000000001"},
                new v_o_v() {id =1, id_otpr = 2, n_vag = "000000001"},
                new v_o_v() {id =2, id_otpr = 3, n_vag = "000000002"},
                new v_o_v() {id =3, id_otpr = 3, n_vag = "000000003"},
                new v_o_v() {id =3, id_otpr = 2, n_vag = "000000003"},
                new v_o_v() {id =3, id_otpr = 1, n_vag = "000000003"},
            }).ToList(),
                Bills = (new[] {
                new Bill() {VPam = new v_pam(){id_ved = 1,nved = "000001",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000001",d_pod = new DateTime(2015,1,10),d_ub = new DateTime(2015,1,12)}},

                new Bill() {VPam = new v_pam(){id_ved = 1,nved = "000001",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000001",d_pod = new DateTime(2015,1,10),d_ub = new DateTime(2015,1,12)}},

                new Bill() {VPam = new v_pam(){id_ved = 3,nved = "000003",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000002",d_pod = new DateTime(2015,2,20),d_ub = new DateTime(2015,2,25)}},

                new Bill() {VPam = new v_pam(){id_ved = 4,nved = "000004",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,8),d_ub = new DateTime(2015,3,10)}},

                new Bill() {VPam = new v_pam(){id_ved = 5,nved = "000005",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,10),d_ub = new DateTime(2015,3,15)}},

                new Bill() {VPam = new v_pam(){id_ved = 6,nved = "000006",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,13),d_ub = new DateTime(2015,3,16)}},
            }).ToList(),
                Acts = (new[] {
                new Certificate() {VAkt = new v_akt(){id = 1,nakt = "001",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000001"}},
                new Certificate() {VAkt = new v_akt(){id = 1,nakt = "001",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000001"}},
                new Certificate() {VAkt = new v_akt(){id = 3,nakt = "003",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000003"}},
                new Certificate() {VAkt = new v_akt(){id = 4,nakt = "004",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                new Certificate() {VAkt = new v_akt(){id = 5,nakt = "005",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                new Certificate() {VAkt = new v_akt(){id = 6,nakt = "006",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
            }).ToList(),
                Cards = (new[] {
                new AccumulativeCard() {VKart = new v_kart(){id=1}, 
                    VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                new AccumulativeCard() {VKart = new v_kart(){id=1}, 
                    VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                new AccumulativeCard() {VKart = new v_kart(){id=2}, 
                    VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                new AccumulativeCard() {VKart = new v_kart(){id=3}, 
                    VNach = new v_nach(){num_doc = "09882881",date_raskr = DateTime.Today}},
                new AccumulativeCard() {VKart = new v_kart(){id=4}, 
                    VNach = new v_nach(){type_doc = 4,date_raskr = DateTime.Today}},
                new AccumulativeCard() {VKart = new v_kart(){id=3}, 
                    VNach = new v_nach(){date_raskr = DateTime.Today}},
            }).ToList(),
                Luggages = (new[] {
                new Luggage() {OrcKrt = new orc_krt(){DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="000000003"}},
                new Luggage() {OrcKrt = new orc_krt(){DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="000005"}},
                new Luggage() {OrcKrt = new orc_krt(){DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="005"}},
                new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 1,U_KOD = 2,DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="09882881"}},
                new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 1,U_KOD = 2,DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="09882881"}},
                new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 2,U_KOD = 2,DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor()},
            }).ToList()
            });

            //Arrange new storage
            SessionStorage storage = new SessionStorage();
            #endregion

            #region Act
            //Act
            storage.AddItem(mock.Object.PackDocuments(i1,1));

            ShippingInfoLine[] results = storage.Lines.ToArray();
            #endregion

            #region Assert
            #region General
            Assert.AreEqual(1,results.Length);
            Assert.AreEqual(1,results[0].Shipping.id);
            #endregion

            #region Shipping
            //id
            Assert.AreEqual(1,results[0].Shipping.id);
            //number
            Assert.AreEqual("09882881",results[0].Shipping.n_otpr);
            #endregion

            #region Wagons
            //distinctBy(id)
            Assert.AreEqual(6,results[0].WagonsNumbers.Count());
            //Total
            Assert.AreEqual(6,storage.Lines.Sum(l => l.WagonsNumbers.Count));
            #endregion

            #region  Bills
            Assert.AreEqual(6,results[0].Bills.Count);
            #endregion

            #region Certificates
            Assert.AreEqual(6,results[0].Acts.Count);
            #endregion

            #region Accumulative Cards
            Assert.AreEqual(6,results[0].Cards.Count);
            #endregion

            #region Luggage
            Assert.AreEqual(6,results[0].Luggages.Count);
            #endregion
            #endregion
        }

        [TestMethod]
        public void StorageController_Add_New_Lines() {
            Mock<IBussinesEngage> mock = new Mock<IBussinesEngage>();

            #region Arrange

            mock.Setup(m => m.PackDocuments(new v_otpr(),1)).Returns(
                new ShippingInfoLine() { Shipping = new v_otpr() { id = 1 } });

            //Arrange fill info new Dispatch
            mock.Setup(m => m.ShippinNumbers).Returns((new[] {
                new v_otpr() { id = 1,n_otpr = "09882881",date_oper = DateTime.Today},
            }.AsQueryable()));

            //Wagons
            mock.Setup(m => m.CarriageNumbers).Returns((new[] {
                new v_o_v() {id =1, id_otpr = 2, n_vag = "000000001"},
                new v_o_v() {id =1, id_otpr = 2, n_vag = "000000001"},
                new v_o_v() {id =2, id_otpr = 3, n_vag = "000000002"},
                new v_o_v() {id =3, id_otpr = 3, n_vag = "000000003"},
                new v_o_v() {id =3, id_otpr = 2, n_vag = "000000003"},
                new v_o_v() {id =3, id_otpr = 1, n_vag = "000000003"},
            }).AsQueryable());

            //Bills
            mock.Setup(m => m.Bills).Returns((new[] {
                new Bill() {VPam = new v_pam(){id_ved = 1,nved = "000001",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000001",d_pod = new DateTime(2015,1,10),d_ub = new DateTime(2015,1,12)}},

                new Bill() {VPam = new v_pam(){id_ved = 1,nved = "000001",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000001",d_pod = new DateTime(2015,1,10),d_ub = new DateTime(2015,1,12)}},

                new Bill() {VPam = new v_pam(){id_ved = 3,nved = "000003",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000002",d_pod = new DateTime(2015,2,20),d_ub = new DateTime(2015,2,25)}},

                new Bill() {VPam = new v_pam(){id_ved = 4,nved = "000004",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,8),d_ub = new DateTime(2015,3,10)}},

                new Bill() {VPam = new v_pam(){id_ved = 5,nved = "000005",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,10),d_ub = new DateTime(2015,3,15)}},

                new Bill() {VPam = new v_pam(){id_ved = 6,nved = "000006",dved = DateTime.Today},
                            VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,13),d_ub = new DateTime(2015,3,16)}},
            }).AsQueryable());

            //Certificates
            mock.Setup(m => m.Certificates).Returns((new[] {
                new Certificate() {VAkt = new v_akt(){id = 1,nakt = "001",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000001"}},
                new Certificate() {VAkt = new v_akt(){id = 1,nakt = "001",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000001"}},
                new Certificate() {VAkt = new v_akt(){id = 3,nakt = "003",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000003"}},
                new Certificate() {VAkt = new v_akt(){id = 4,nakt = "004",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                new Certificate() {VAkt = new v_akt(){id = 5,nakt = "005",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                new Certificate() {VAkt = new v_akt(){id = 6,nakt = "006",dakt = DateTime.Today},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
            }).AsQueryable());

            //Accumulative Cards
            mock.Setup(m => m.Cards).Returns((new[] {
                new AccumulativeCard() {VKart = new v_kart(){id=1}, 
                    VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                new AccumulativeCard() {VKart = new v_kart(){id=1}, 
                    VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                new AccumulativeCard() {VKart = new v_kart(){id=2}, 
                    VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                new AccumulativeCard() {VKart = new v_kart(){id=3}, 
                    VNach = new v_nach(){num_doc = "09882881",date_raskr = DateTime.Today}},
                new AccumulativeCard() {VKart = new v_kart(){id=4}, 
                    VNach = new v_nach(){type_doc = 4,date_raskr = DateTime.Today}},
                new AccumulativeCard() {VKart = new v_kart(){id=3}, 
                    VNach = new v_nach(){date_raskr = DateTime.Today}},
            }).AsQueryable());

            //Luggage
            mock.Setup(m => m.Baggage).Returns((new[] {
                new Luggage() {OrcKrt = new orc_krt(){DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="000000003"}},
                new Luggage() {OrcKrt = new orc_krt(){DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="000005"}},
                new Luggage() {OrcKrt = new orc_krt(){DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="005"}},
                new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 1,U_KOD = 2,DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="09882881"}},
                new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 1,U_KOD = 2,DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor(){NOMOT ="09882881"}},
                new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 2,U_KOD = 2,DTOPEN = DateTime.Today}, OrcSbor = new orc_sbor()},
            }).AsQueryable());

            #endregion

            SessionStorage storage = new SessionStorage();
            
            StorageController target = new StorageController(mock.Object);
            storage.AddItem(new ShippingInfoLine(){Shipping = new v_otpr(){id=1}});

            //act
            target.AddRow(storage,1,null);

            //assert
            Assert.AreEqual(1,storage.Lines.Count());
            Assert.AreEqual(1,storage.Lines.ToArray()[0].Shipping.id);
        }

        [TestMethod]
        public void Remove_Line() {
            #region Arrange
            //Arrange fill info new Dispatch
            v_otpr i1 = new v_otpr() { id = 1,n_otpr = "09882881" };
            v_otpr i2 = new v_otpr() { id = 2,n_otpr = "09882877" };
            v_otpr i3 = new v_otpr() { id = 4,n_otpr = "09882881" };
            v_otpr i4 = new v_otpr() { id = 3,n_otpr = "09864171" };

            //mock 
            Mock<IDocumentsRepository> mock = new Mock<IDocumentsRepository>();

            //Wagons
            mock.Setup(m => m.CarriageNumbers).Returns((new[] {
                    new v_o_v() {id =1, id_otpr = 2, n_vag = "000000001"},
                    new v_o_v() {id =1, id_otpr = 2, n_vag = "000000001"},
                    new v_o_v() {id =2, id_otpr = 3, n_vag = "000000002"},
                    new v_o_v() {id =3, id_otpr = 3, n_vag = "000000003"},
                    new v_o_v() {id =3, id_otpr = 2, n_vag = "000000003"},
                    new v_o_v() {id =3, id_otpr = 1, n_vag = "000000003"},
                }).AsQueryable());

            //Bills
            mock.Setup(m => m.Bills).Returns((new[] {
                    new Bill() {VPam = new v_pam(){id_ved = 1,nved = "000001"},
                                VPamVag = new v_pam_vag(){nomvag = "000000001",d_pod = new DateTime(2015,1,10),d_ub = new DateTime(2015,1,12)}},

                    new Bill() {VPam = new v_pam(){id_ved = 1,nved = "000001"},
                                VPamVag = new v_pam_vag(){nomvag = "000000001",d_pod = new DateTime(2015,1,10),d_ub = new DateTime(2015,1,12)}},

                    new Bill() {VPam = new v_pam(){id_ved = 3,nved = "000003"},
                                VPamVag = new v_pam_vag(){nomvag = "000000002",d_pod = new DateTime(2015,2,20),d_ub = new DateTime(2015,2,25)}},

                    new Bill() {VPam = new v_pam(){id_ved = 4,nved = "000004"},
                                VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,8),d_ub = new DateTime(2015,3,10)}},

                    new Bill() {VPam = new v_pam(){id_ved = 5,nved = "000005"},
                                VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,10),d_ub = new DateTime(2015,3,15)}},

                    new Bill() {VPam = new v_pam(){id_ved = 6,nved = "000006"},
                                VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,13),d_ub = new DateTime(2015,3,16)}},
                }).AsQueryable());

            //Certificates
            mock.Setup(m => m.Certificates).Returns((new[] {
                    new Certificate() {VAkt = new v_akt(){id = 1,nakt = "001"},VAktVag = new v_akt_vag(){nomvag = "000000001"}},
                    new Certificate() {VAkt = new v_akt(){id = 1,nakt = "001"},VAktVag = new v_akt_vag(){nomvag = "000000001"}},
                    new Certificate() {VAkt = new v_akt(){id = 3,nakt = "003"},VAktVag = new v_akt_vag(){nomvag = "000000003"}},
                    new Certificate() {VAkt = new v_akt(){id = 4,nakt = "004"},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                    new Certificate() {VAkt = new v_akt(){id = 5,nakt = "005"},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                    new Certificate() {VAkt = new v_akt(){id = 6,nakt = "006"},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                }).AsQueryable());

            //Accumulative Cards
            mock.Setup(m => m.Cards).Returns((new[] {
                    new AccumulativeCard() {VKart = new v_kart(){id=1}, VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                    new AccumulativeCard() {VKart = new v_kart(){id=1}, VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                    new AccumulativeCard() {VKart = new v_kart(){id=2}, VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                    new AccumulativeCard() {VKart = new v_kart(){id=3}, VNach = new v_nach(){num_doc = "09882881"}},
                    new AccumulativeCard() {VKart = new v_kart(){id=4}, VNach = new v_nach(){type_doc = 4}},
                    new AccumulativeCard() {VKart = new v_kart(){id=3}, VNach = new v_nach()},
                }).AsQueryable());

            //Luggage
            mock.Setup(m => m.Baggage).Returns((new[] {
                    new Luggage() {OrcKrt = new orc_krt(), OrcSbor = new orc_sbor(){NOMOT ="000000003"}},
                    new Luggage() {OrcKrt = new orc_krt(), OrcSbor = new orc_sbor(){NOMOT ="000005"}},
                    new Luggage() {OrcKrt = new orc_krt(), OrcSbor = new orc_sbor(){NOMOT ="005"}},
                    new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 1,U_KOD = 2}, OrcSbor = new orc_sbor(){NOMOT ="09882881"}},
                    new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 1,U_KOD = 2}, OrcSbor = new orc_sbor(){NOMOT ="09882881"}},
                    new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 2,U_KOD = 2}, OrcSbor = new orc_sbor()},
                }).AsQueryable());

            //Arrange new storage
            SessionStorage storage = new SessionStorage();
            #endregion

            storage.AddItem(new ShippingInfoLine() { Shipping = i1 });
            storage.AddItem(new ShippingInfoLine() { Shipping = i1 });
            storage.AddItem(new ShippingInfoLine() { Shipping = i1 });
            storage.AddItem(new ShippingInfoLine() { Shipping = i1 });

            //Act
            storage.RemoveLine(i3);

            ShippingInfoLine[] results = storage.Lines.ToArray();

            //Assert
            Assert.AreEqual(0,results.Count(r => r.Shipping == i3));
            Assert.AreEqual(1,results.Count());
        }

        [TestMethod]
        public void Clear_Content_Storage() {
            #region Arrange
            //Arrange fill info new Dispatch
            v_otpr i1 = new v_otpr() { id = 1,n_otpr = "09882881" };
            v_otpr i2 = new v_otpr() { id = 2,n_otpr = "09882877" };
            v_otpr i3 = new v_otpr() { id = 4,n_otpr = "09882881" };
            v_otpr i4 = new v_otpr() { id = 3,n_otpr = "09864171" };

            //mock 
            Mock<IDocumentsRepository> mock = new Mock<IDocumentsRepository>();

            //Wagons
            mock.Setup(m => m.CarriageNumbers).Returns((new[] {
                    new v_o_v() {id =1, id_otpr = 2, n_vag = "000000001"},
                    new v_o_v() {id =1, id_otpr = 2, n_vag = "000000001"},
                    new v_o_v() {id =2, id_otpr = 3, n_vag = "000000002"},
                    new v_o_v() {id =3, id_otpr = 3, n_vag = "000000003"},
                    new v_o_v() {id =3, id_otpr = 2, n_vag = "000000003"},
                    new v_o_v() {id =3, id_otpr = 1, n_vag = "000000003"},
                }).AsQueryable());

            //Bills
            mock.Setup(m => m.Bills).Returns((new[] {
                    new Bill() {VPam = new v_pam(){id_ved = 1,nved = "000001"},
                                VPamVag = new v_pam_vag(){nomvag = "000000001",d_pod = new DateTime(2015,1,10),d_ub = new DateTime(2015,1,12)}},

                    new Bill() {VPam = new v_pam(){id_ved = 1,nved = "000001"},
                                VPamVag = new v_pam_vag(){nomvag = "000000001",d_pod = new DateTime(2015,1,10),d_ub = new DateTime(2015,1,12)}},

                    new Bill() {VPam = new v_pam(){id_ved = 3,nved = "000003"},
                                VPamVag = new v_pam_vag(){nomvag = "000000002",d_pod = new DateTime(2015,2,20),d_ub = new DateTime(2015,2,25)}},

                    new Bill() {VPam = new v_pam(){id_ved = 4,nved = "000004"},
                                VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,8),d_ub = new DateTime(2015,3,10)}},

                    new Bill() {VPam = new v_pam(){id_ved = 5,nved = "000005"},
                                VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,10),d_ub = new DateTime(2015,3,15)}},

                    new Bill() {VPam = new v_pam(){id_ved = 6,nved = "000006"},
                                VPamVag = new v_pam_vag(){nomvag = "000000003",d_pod = new DateTime(2015,3,13),d_ub = new DateTime(2015,3,16)}},
                }).AsQueryable());

            //Certificates
            mock.Setup(m => m.Certificates).Returns((new[] {
                    new Certificate() {VAkt = new v_akt(){id = 1,nakt = "001"},VAktVag = new v_akt_vag(){nomvag = "000000001"}},
                    new Certificate() {VAkt = new v_akt(){id = 1,nakt = "001"},VAktVag = new v_akt_vag(){nomvag = "000000001"}},
                    new Certificate() {VAkt = new v_akt(){id = 3,nakt = "003"},VAktVag = new v_akt_vag(){nomvag = "000000003"}},
                    new Certificate() {VAkt = new v_akt(){id = 4,nakt = "004"},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                    new Certificate() {VAkt = new v_akt(){id = 5,nakt = "005"},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                    new Certificate() {VAkt = new v_akt(){id = 6,nakt = "006"},VAktVag = new v_akt_vag(){nomvag = "000000002"}},
                }).AsQueryable());

            //Accumulative Cards
            mock.Setup(m => m.Cards).Returns((new[] {
                    new AccumulativeCard() {VKart = new v_kart(){id=1}, VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                    new AccumulativeCard() {VKart = new v_kart(){id=1}, VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                    new AccumulativeCard() {VKart = new v_kart(){id=2}, VNach = new v_nach(){cod_sbor = "065",type_doc = 4,date_raskr = new DateTime(2015,3,16)}},
                    new AccumulativeCard() {VKart = new v_kart(){id=3}, VNach = new v_nach(){num_doc = "09882881"}},
                    new AccumulativeCard() {VKart = new v_kart(){id=4}, VNach = new v_nach(){type_doc = 4}},
                    new AccumulativeCard() {VKart = new v_kart(){id=3}, VNach = new v_nach()},
                }).AsQueryable());

            //Luggage
            mock.Setup(m => m.Baggage).Returns((new[] {
                    new Luggage() {OrcKrt = new orc_krt(), OrcSbor = new orc_sbor(){NOMOT ="000000003"}},
                    new Luggage() {OrcKrt = new orc_krt(), OrcSbor = new orc_sbor(){NOMOT ="000005"}},
                    new Luggage() {OrcKrt = new orc_krt(), OrcSbor = new orc_sbor(){NOMOT ="005"}},
                    new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 1,U_KOD = 2}, OrcSbor = new orc_sbor(){NOMOT ="09882881"}},
                    new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 1,U_KOD = 2}, OrcSbor = new orc_sbor(){NOMOT ="09882881"}},
                    new Luggage() {OrcKrt = new orc_krt(){KEYKRT = 2,U_KOD = 2}, OrcSbor = new orc_sbor()},
                }).AsQueryable());

            //Arrange new storage
            SessionStorage storage = new SessionStorage();
            #endregion

            storage.AddItem(new ShippingInfoLine() { Shipping = i1 });
            storage.AddItem(new ShippingInfoLine() { Shipping = i1 });
            storage.AddItem(new ShippingInfoLine() { Shipping = i1 });
            storage.AddItem(new ShippingInfoLine() { Shipping = i1 });

            //Act
            storage.Clear();

            ShippingInfoLine[] results = storage.Lines.ToArray();

            //Assert
            Assert.AreEqual(0,results.Count());
        }

        [TestMethod]
        public void Can_View_SessionStorageContent_Contents() {

            //Arrange
            SessionStorage storage = new SessionStorage();

            StorageController target = new StorageController(null);

            //Act
            SessionStorageViewModel resutl = (SessionStorageViewModel)target.Index(storage,"myUrl").ViewData.Model;

            //Assert
            Assert.AreEqual(storage,resutl.Storage);
            Assert.AreEqual("myUrl",resutl.ReturnUrl);
        }

        [TestMethod]
        public void Can_Edit_StorageLine() {
            // Arrange - create the mock repository
            Mock<IBussinesEngage> mock = new Mock<IBussinesEngage>();

            // Arrange - create the controller
            StorageController target = new StorageController(mock.Object);
            SessionStorage storage = new SessionStorage();
            storage.AddItem(new ShippingInfoLine{ 
                Shipping = new v_otpr() {id = 1, n_otpr = "00000001",date_oper = DateTime.Today},
            });

            // Act
            var infoLineViewModel = target.EditRow(storage,1,null).ViewData.Model as InfoLineViewModel;
            if (infoLineViewModel != null) {
                ShippingInfoLine l1 = infoLineViewModel.DocumentPackLine;

                // Assert
                Assert.AreEqual(1,l1.Shipping.id);
            }
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_StorageLine() {
            // Arrange - create the mock repository
            Mock<IBussinesEngage> mock = new Mock<IBussinesEngage>();

            mock.Setup(m => m.ShippinNumbers).Returns((new[] {
        new v_otpr() {id = 1, n_otpr = "00000001"},
        new v_otpr() {id = 2, n_otpr = "00000002"},
        new v_otpr() {id = 3, n_otpr = "00000003"},
        }).AsQueryable());

            // Arrange - create the controller
            StorageController target = new StorageController(mock.Object);
            SessionStorage storage = new SessionStorage();

            // Act
            ShippingInfoLine l1 = target.EditRow(storage,1,null).ViewData.Model as ShippingInfoLine;

            // Assert
            Assert.IsNull(l1);
        }

        [TestMethod]
        public void Edit_Post() {
            // Arrange - create the mock repository
            Mock<IBussinesEngage> mock = new Mock<IBussinesEngage>();

            Mock<SessionStorage> sessionMock = new Mock<SessionStorage>();

            StorageController controller = new StorageController(mock.Object);

            SessionStorage storage = new SessionStorage();
            InfoLineViewModel line = new InfoLineViewModel() {
                DocumentPackLine = new ShippingInfoLine(){Shipping = new v_otpr()}
            };

            //Act
            var result = controller.EditRow(storage,line);

            // Assert - check that the repository was called
            sessionMock.Verify();

            // Assert - check the method result type
            Assert.IsNotInstanceOfType(result,typeof(ViewResult));
        }
    }
}
