using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Wantsan.Models;

namespace Wantsan.Controllers
{
    public class ViewModel
    {
        public IEnumerable<teslim_yerleri_tipleri> teslims { get; set; }
        public List<buyukluk_tipleri> buyukluks { get; set; }

        public List<buyukluk_values> buyuklukv { get; set; }
        public List<teklif> teklifs { get; set; }
        public List<Pozisyonlar> pozisyonlars { get; set; }
    }
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Class.Mails mail = new Class.Mails();
            //Response.Redirect(mail.Gonder("Test Maili", "Selam", "muhammedseker@hotmail.com"));
            return View();
        }
        [HttpPost]
        public ActionResult Index(users u)
        {
            vantsanEntities ent = new vantsanEntities();
            if (ent.users.Where(x => x.username == u.username && x.password == u.password).ToList().Count > 0)
            {
                Session.Add("Login", true);
                return RedirectToAction("Teklifler");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Main()
        {
            return View();
        }
        public ActionResult TeslimYerleriTipleri()
        {
            vantsanEntities ent = new vantsanEntities();

            return View(ent.teslim_yerleri_tipleri.ToList());
        }
        [HttpPost]
        public ActionResult TeslimYerleriTipleri(teslim_yerleri_tipleri t)
        {
            vantsanEntities ent = new vantsanEntities();
            teslim_yerleri_tipleri teslim = new teslim_yerleri_tipleri();
            teslim.name = t.name;
            ent.teslim_yerleri_tipleri.Add(teslim);
            ent.SaveChanges();
            return View(ent.teslim_yerleri_tipleri.ToList());
        }
        public ActionResult TeslimTipiSil(int id)
        {
            vantsanEntities ent = new vantsanEntities();
            teslim_yerleri_tipleri teslim = ent.teslim_yerleri_tipleri.Where(x => x.id == id).FirstOrDefault();
            ent.teslim_yerleri_tipleri.Remove(teslim);
            ent.SaveChanges();
            return RedirectToAction("TeslimYerleriTipleri");
        }
        public ActionResult BuyuklukTipleri()
        {
            vantsanEntities ent = new vantsanEntities();
            return View(ent.buyukluk_tipleri.ToList());
        }
        [HttpPost]
        public ActionResult BuyuklukTipleri(buyukluk_tipleri byk)
        {
            vantsanEntities ent = new vantsanEntities();
            buyukluk_tipleri buyukluk = new buyukluk_tipleri();
            buyukluk.name = byk.name;
            ent.buyukluk_tipleri.Add(buyukluk);
            ent.SaveChanges();
            return View(ent.buyukluk_tipleri.ToList());
        }
        public ActionResult TeklifEkle()
        {
            vantsanEntities ent = new vantsanEntities();
            ViewBag.teslims = ent.teslim_yerleri_tipleri.ToList();
            ViewBag.buyukluks = ent.buyukluk_tipleri.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult TeklifEkle(teklif t, FormCollection f1)
        {
            vantsanEntities ent = new vantsanEntities();
            teklif tklf = new teklif();
            tklf.date = t.date;
            tklf.garanti = t.garanti;
            tklf.ilgi = t.ilgi;
            tklf.isin_adi = t.isin_adi;
            tklf.obsiyon = t.obsiyon;
            tklf.odemesarti = t.odemesarti;
            tklf.sayin = t.sayin;
            tklf.teslimsuresi = t.teslimsuresi;
            tklf.teslim_yeri = t.teslim_yeri;
            ent.teklif.Add(tklf);
            ent.SaveChanges();
            return RedirectToAction("PozisyonEkle/" + tklf.id);
            //return RedirectToAction("Teklifler");
        }
        public ActionResult PozisyonEkle(int id)
        {
            vantsanEntities ent = new vantsanEntities();

            if (ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList().Count > 0)
            {
                ViewBag.id = id;
                ViewBag.count = ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList().Count;
                ViewBag.poztutartoplam = ent.teklif.Where(x => x.id == id).FirstOrDefault().poz_tutar_toplam;
                ViewBag.pozkdv = ent.teklif.Where(x => x.id == id).FirstOrDefault().poz_kdv;
                ViewBag.poztoplam = ent.teklif.Where(x => x.id == id).FirstOrDefault().poz_toplam;
                ViewBag.id = id;
                return View(ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList());
            }
            else
            {
                ViewBag.id = id;
                ViewBag.count = ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList().Count;
                return View();
            }
        }

        [HttpPost]
        public ActionResult PozisyonEkle(int id, Pozisyonlar p)
        {
            ViewBag.id = id;
            vantsanEntities ent = new vantsanEntities();
            Pozisyonlar pozisyon = new Pozisyonlar();
            pozisyon.Teklif_ID = id;
            pozisyon.pozNo = p.pozNo;
            pozisyon.Cinsi = p.Cinsi;
            pozisyon.Miktar = p.Miktar;
            pozisyon.BirimFiyatı = p.BirimFiyatı;
            pozisyon.Tutarı = p.Tutarı;
            ent.Pozisyonlar.Add(pozisyon);
            ent.SaveChanges();
            teklif tek = new teklif();
            tek = ent.teklif.Where(x => x.id == id).FirstOrDefault();
            int tutartoplam = 0;
            int kdv = 0;
            int toplam = 0;
            foreach (var item in ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList())
            {
                tutartoplam += (int)item.Tutarı;
            }
            kdv = Convert.ToInt32(tutartoplam * 0.18);
            toplam = tutartoplam + kdv;
            tek.poz_kdv = kdv;
            tek.poz_tutar_toplam = tutartoplam;
            tek.poz_toplam = toplam;
            tek.toplambedel = toplam;
            ent.SaveChanges();
            return RedirectToAction("PozisyonEkle");
            //Pozisyon silindiği zaman teklif tablosundan toplam bedeli ayarları ona göre yap tutartoplam kdv poz toplam vs...
        }
        public ActionResult PozisyonSil(int id)
        {
            vantsanEntities ent = new vantsanEntities();
            Pozisyonlar pz = new Pozisyonlar();
            pz = ent.Pozisyonlar.Where(x => x.id == id).FirstOrDefault();
            int pztklfid = pz.Teklif_ID;
            ViewBag.id = pztklfid;
            ent.Pozisyonlar.Remove(pz);
            teklif tklf = new teklif();
            tklf = ent.teklif.Where(x => x.id == pztklfid).FirstOrDefault();
            tklf.poz_tutar_toplam = tklf.poz_tutar_toplam - pz.Tutarı;
            tklf.poz_kdv = tklf.poz_kdv - Convert.ToInt32((pz.Tutarı * 0.18));
            tklf.poz_toplam = tklf.poz_toplam - (pz.Tutarı + Convert.ToInt32(pz.Tutarı * 0.18));
            tklf.toplambedel = tklf.poz_toplam;
            ent.SaveChanges();
            return RedirectToAction("PozisyonDuzenles/" + pztklfid);
        }
        public ActionResult Teklifler()
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                return View(ent.teklif.ToList());
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        public ActionResult TeklifDuzenle(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                ViewBag.teslims = ent.teslim_yerleri_tipleri.ToList();
                return View(ent.teklif.Where(x => x.id == id).FirstOrDefault());
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult TeklifDuzenle(teklif t)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                teklif tklf = new teklif();
                tklf = ent.teklif.Where(x => x.id == t.id).FirstOrDefault();
                tklf.date = t.date;
                tklf.sayin = t.sayin;
                tklf.isin_adi = t.isin_adi;
                tklf.ilgi = t.ilgi;
                tklf.teslimsuresi = t.teslimsuresi;
                tklf.teslim_yeri = t.teslim_yeri;
                tklf.odemesarti = t.odemesarti;
                tklf.garanti = t.garanti;
                tklf.obsiyon = t.obsiyon;
                ent.SaveChanges();
                return RedirectToAction("Teklifler");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult TeklifSil(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                List<Pozisyonlar> pz = ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList();
                foreach (var item in pz)
                {
                    ent.Pozisyonlar.Remove(ent.Pozisyonlar.FirstOrDefault(x => x.id == item.id));
                }
                List<buyukluk_values> buyuk = ent.buyukluk_values.Where(x => x.teklif_id == id).ToList();
                foreach (var item in buyuk)
                {
                    ent.buyukluk_values.Remove(ent.buyukluk_values.FirstOrDefault(x => x.id == item.id));
                }
                ent.SaveChanges();
                ent.teklif.Remove(ent.teklif.FirstOrDefault(x => x.id == id));
                ent.SaveChanges();
                return RedirectToAction("Teklifler");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult PozisyonDuzenle(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();

                if (ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList().Count > 0)
                {
                    ViewBag.id = id;
                    ViewBag.count = ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList().Count;
                    ViewBag.poztutartoplam = ent.teklif.Where(x => x.id == id).FirstOrDefault().poz_tutar_toplam;
                    ViewBag.pozkdv = ent.teklif.Where(x => x.id == id).FirstOrDefault().poz_kdv;
                    ViewBag.poztoplam = ent.teklif.Where(x => x.id == id).FirstOrDefault().poz_toplam;
                    return View(ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList());
                }
                else
                {
                    ViewBag.id = id;
                    ViewBag.count = ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList().Count;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult PozisyonDuzenles(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();

                if (ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList().Count > 0)
                {
                    ViewBag.id = id;
                    ViewBag.count = ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList().Count;
                    ViewBag.poztutartoplam = ent.teklif.Where(x => x.id == id).FirstOrDefault().poz_tutar_toplam;
                    ViewBag.pozkdv = ent.teklif.Where(x => x.id == id).FirstOrDefault().poz_kdv;
                    ViewBag.poztoplam = ent.teklif.Where(x => x.id == id).FirstOrDefault().poz_toplam;
                    return View(ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList());
                }
                else
                {
                    ViewBag.id = id;
                    ViewBag.count = ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList().Count;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult PozisyonDuzenles(int id, Pozisyonlar p)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                Pozisyonlar pozisyon = new Pozisyonlar();
                pozisyon.Teklif_ID = id;
                pozisyon.pozNo = p.pozNo;
                pozisyon.Cinsi = p.Cinsi;
                pozisyon.Miktar = p.Miktar;
                pozisyon.BirimFiyatı = p.BirimFiyatı;
                pozisyon.Tutarı = p.Tutarı;
                ent.Pozisyonlar.Add(pozisyon);
                ent.SaveChanges();
                teklif tek = new teklif();
                tek = ent.teklif.Where(x => x.id == id).FirstOrDefault();
                int tutartoplam = 0;
                int kdv = 0;
                int toplam = 0;
                foreach (var item in ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList())
                {
                    tutartoplam += (int)item.Tutarı;
                }
                kdv = Convert.ToInt32(tutartoplam * 0.18);
                toplam = tutartoplam + kdv;
                tek.poz_kdv = kdv;
                tek.poz_tutar_toplam = tutartoplam;
                tek.poz_toplam = toplam;
                tek.toplambedel = toplam;
                ent.SaveChanges();
                return RedirectToAction("PozisyonDuzenle/" + id);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult TeknikOzellikEkle(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                ViewBag.buyukluks = ent.buyukluk_tipleri.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult TeknikOzellikEkle(int id, FormCollection f1)
        {
            vantsanEntities ent = new vantsanEntities();
            buyukluk_values buyukluk = new buyukluk_values();
            teklif tklf = new teklif();

            if (f1["txtbuyukluk"].ToString() != "" && f1["txtbuyukluk"] != null)
            {
                var xs = f1["slcbuyukluk"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                //ent.buyukluk_tipleri.Where(x => x.name == f1["slcbuyukluk"].ToString()).FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Fan Tipi";
                buyukluk.value2 = "Büyüklük";
                buyukluk.value3 = f1["txtbuyukluk"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }

            if (f1["txtakiskan"].ToString() != "" && f1["txtakiskan"] != null)
            {
                var xs = f1["slcakiskan"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "V";
                buyukluk.value2 = "Akışgan Debisi";
                buyukluk.value3 = f1["txtakiskan"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }

            if (f1["txttoplambasinc"].ToString() != "" && f1["txttoplambasinc"] != null)
            {
                var xs = f1["slctoplambasinc"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Pt";
                buyukluk.value2 = "Toplam Basınç";
                buyukluk.value3 = f1["txttoplambasinc"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtdinamikbasinc"].ToString() != "" && f1["txtdinamikbasinc"] != null)
            {
                var xs = f1["slcdinamikbasinc"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Pd 2";
                buyukluk.value2 = "Çıkış Ağzındaki dinamik basınç";
                buyukluk.value3 = f1["txtdinamikbasinc"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtakiskanhiz"].ToString() != "" && f1["txtakiskanhiz"] != null)
            {
                var xs = f1["slcakiskanhiz"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "C 2";
                buyukluk.value2 = "Çıkış Ağzındaki akışgan hız";
                buyukluk.value3 = f1["txtakiskanhiz"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtmotorgucu"].ToString() != "" && f1["txtmotorgucu"] != null)
            {
                var xs = f1["slcmotorgucu"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Nm";
                buyukluk.value2 = "Motor gücü";
                buyukluk.value3 = f1["txtmotorgucu"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtfancalismadevri"].ToString() != "" && f1["txtfancalismadevri"] != null)
            {
                var xs = f1["slcfancalismadevri"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "n";
                buyukluk.value2 = "Fan çalışma devri";
                buyukluk.value3 = f1["txtfancalismadevri"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }

            if (f1["txttoplamverim"].ToString() != "" && f1["txttoplamverim"] != null)
            {
                var xs = f1["slctoplamverim"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "n";
                buyukluk.value2 = "Toplam verim";
                buyukluk.value3 = f1["txttoplamverim"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtgurultusiddeti"].ToString() != "" && f1["txtgurultusiddeti"] != null)
            {
                var xs = f1["slcgurultusiddeti"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Ls";
                buyukluk.value2 = "Özgül gürültü şiddeti";
                buyukluk.value3 = f1["txtgurultusiddeti"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtanagurultusiddeti"].ToString() != "" && f1["txtanagurultusiddeti"] != null)
            {
                var xs = f1["slcanagurultusiddeti"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Lu";
                buyukluk.value2 = "Ana gürültü şiddeti";
                buyukluk.value3 = f1["txtanagurultusiddeti"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }

            if (f1["txtsalyangoz"].ToString() != "" && f1["txtsalyangoz"] != null)
            {
                var xs = f1["slcsalyangoz"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Fan Tipi";
                buyukluk.value2 = "Salyangoz Boyutu";
                buyukluk.value3 = f1["txtsalyangoz"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtemis"].ToString() != "" && f1["txtemis"] != null)
            {
                var xs = f1["slcemis"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "V";
                buyukluk.value2 = "Emiş Ağzı";
                buyukluk.value3 = f1["txtemis"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }

            if (f1["txtcikisagzi"].ToString() != "" && f1["txtcikisagzi"] != null)
            {
                var xs = f1["slccikisagzi"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Pt";
                buyukluk.value2 = "Çıkış Ağzı";
                buyukluk.value3 = f1["txtcikisagzi"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtrotor"].ToString() != "" && f1["txtrotor"] != null)
            {
                var xs = f1["slcrotor"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Pd 2";
                buyukluk.value2 = "Rotor çapı";
                buyukluk.value3 = f1["txtrotor"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtagirlik"].ToString() != "" && f1["txtagirlik"] != null)
            {
                var xs = f1["slcagirlik"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "C 2";
                buyukluk.value2 = "Ağırlık";
                buyukluk.value3 = f1["txtagirlik"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txttahrik"].ToString() != "" && f1["txttahrik"] != null)
            {
                var xs = f1["slctahrik"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Nm";
                buyukluk.value2 = "Tahrik şekli";
                buyukluk.value3 = f1["txttahrik"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }

            if (f1["txtnotasyon"].ToString() != "" && f1["txtnotasyon"] != null)
            {
                var xs = f1["slcnotasyon"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "n";
                buyukluk.value2 = "Konum notasyonu";
                buyukluk.value3 = f1["txtnotasyon"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }

            if (f1["txtsicaklik"].ToString() != "" && f1["txtsicaklik"] != null)
            {
                var xs = f1["slcsicaklik"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "n";
                buyukluk.value2 = "İşletme sıcaklığı";
                buyukluk.value3 = f1["txtsicaklik"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtkanatadet"].ToString() != "" && f1["txtkanatadet"] != null)
            {
                var xs = f1["slckanatadet"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Ls";
                buyukluk.value2 = "Kanat adedi";
                buyukluk.value3 = f1["txtkanatadet"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            if (f1["txtkanatpozisyon"].ToString() != "" && f1["txtkanatpozisyon"] != null)
            {
                var xs = f1["slckanatpozisyon"].ToString();
                var listCLientResult = (from c in ent.buyukluk_tipleri
                                        where (c.name == xs)
                                        select c);
                buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                buyukluk.teklif_id = id;
                buyukluk.value1 = "Lu";
                buyukluk.value2 = "Kanat pozisyonu";
                buyukluk.value3 = f1["txtkanatpozisyon"].ToString();
                ent.buyukluk_values.Add(buyukluk);
                ent.SaveChanges();
            }
            return RedirectToAction("SuccessResult/" + id);

        }
        public ActionResult SuccessResult(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                ViewBag.id = id;
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult GoruntuleYazdir(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult PozisyonDuzenle(int id, Pozisyonlar p)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                Pozisyonlar pozisyon = new Pozisyonlar();
                pozisyon.Teklif_ID = id;
                pozisyon.pozNo = p.pozNo;
                pozisyon.Cinsi = p.Cinsi;
                pozisyon.Miktar = p.Miktar;
                pozisyon.BirimFiyatı = p.BirimFiyatı;
                pozisyon.Tutarı = p.Tutarı;
                ent.Pozisyonlar.Add(pozisyon);
                ent.SaveChanges();
                teklif tek = new teklif();
                tek = ent.teklif.Where(x => x.id == id).FirstOrDefault();
                int tutartoplam = 0;
                int kdv = 0;
                int toplam = 0;
                foreach (var item in ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList())
                {
                    tutartoplam += (int)item.Tutarı;
                }
                kdv = Convert.ToInt32(tutartoplam * 0.18);
                toplam = tutartoplam + kdv;
                tek.poz_kdv = kdv;
                tek.poz_tutar_toplam = tutartoplam;
                tek.poz_toplam = toplam;
                tek.toplambedel = toplam;
                ent.SaveChanges();
                return RedirectToAction("PozisyonDuzenle");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult TeknikDuzenle(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                ViewBag.buyukluks = ent.buyukluk_tipleri.ToList();
                ViewBag.id = id;

                ViewModel vm = new ViewModel();
                vm.buyukluks = ent.buyukluk_tipleri.ToList();
                vm.buyuklukv = ent.buyukluk_values.Where(x => x.teklif_id == id).ToList();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult TeknikDuzenle(FormCollection f1, int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                buyukluk_values buyukluk = new buyukluk_values();
                teklif tklf = new teklif();

                if (f1["txtbuyukluk"].ToString() != "" && f1["txtbuyukluk"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Büyüklük").FirstOrDefault();
                    byk.value1 = "Fan Tipi";
                    byk.value2 = "Büyüklük";
                    byk.value3 = f1["txtbuyukluk"].ToString();

                    if (f1["slcbuyukluk"].ToString() != null && f1["slcbuyukluk"].ToString() != "")
                    {
                        //BURADA KALDI
                        string xs = f1["slcbuyukluk"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }

                if (f1["txtakiskan"].ToString() != "" && f1["txtakiskan"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Akışgan Debisi").FirstOrDefault();
                    byk.value1 = "V";
                    byk.value2 = "Akışgan Debisi";
                    byk.value3 = f1["txtakiskan"].ToString();
                    if (f1["slcakiskan"].ToString() != null && f1["slcakiskan"].ToString() != "")
                    {
                        string xs = f1["slcakiskan"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }

                if (f1["txttoplambasinc"].ToString() != "" && f1["txttoplambasinc"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    if (ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Toplam Basınç").FirstOrDefault() != null)
                    {
                        byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Toplam Basınç").FirstOrDefault();
                        byk.value1 = "Pt";
                        byk.value2 = "Toplam Basınç";
                        byk.value3 = f1["txttoplambasinc"].ToString();
                        if (f1["slctoplambasinc"].ToString() != null && f1["slctoplambasinc"].ToString() != "")
                        {
                            string xs = f1["slctoplambasinc"].ToString();
                            byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                        }
                        ent.SaveChanges();
                    }
                    else
                    {
                        byk.teklif_id = id;
                        byk.value1 = "Pt";
                        byk.value2 = "Toplam Basınç";
                        byk.value3 = f1["txttoplambasinc"].ToString();
                        var xs = f1["slctoplambasinc"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                        ent.buyukluk_values.Add(byk);
                        ent.SaveChanges();
                    }

                }
                if (f1["txtdinamikbasinc"].ToString() != "" && f1["txtdinamikbasinc"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Çıkış Ağzındaki dinamik basınç").FirstOrDefault();
                    byk.value1 = "Pd 2";
                    byk.value2 = "Çıkış Ağzındaki Dinamik Basınç";
                    byk.value3 = f1["txtdinamikbasinc"].ToString();
                    if (f1["slcdinamikbasinc"].ToString() != null && f1["slcdinamikbasinc"].ToString() != "")
                    {
                        string xs = f1["slcdinamikbasinc"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtakiskanhiz"].ToString() != "" && f1["txtakiskanhiz"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Çıkış Ağzındaki akışgan hız").FirstOrDefault();
                    byk.value1 = "C 2";
                    byk.value2 = "Çıkış Ağzındaki Akışgan Hız";
                    byk.value3 = f1["txtakiskanhiz"].ToString();
                    if (f1["slcakiskanhiz"].ToString() != null && f1["slcakiskanhiz"].ToString() != "")
                    {
                        string xs = f1["slcakiskanhiz"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtmotorgucu"].ToString() != "" && f1["txtmotorgucu"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Motor Gücü").FirstOrDefault();
                    byk.value1 = "Nm";
                    byk.value2 = "Motor Gücü";
                    byk.value3 = f1["txtmotorgucu"].ToString();
                    if (f1["slcmotorgucu"].ToString() != null && f1["slcmotorgucu"].ToString() != "")
                    {
                        string xs = f1["slcmotorgucu"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtfancalismadevri"].ToString() != "" && f1["txtfancalismadevri"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Fan Çalışma Devri").FirstOrDefault();
                    byk.value1 = "n";
                    byk.value2 = "Fan Çalışma Devri";
                    byk.value3 = f1["txtfancalismadevri"].ToString();
                    if (f1["slcfancalismadevri"].ToString() != null && f1["slcfancalismadevri"].ToString() != "")
                    {
                        string xs = f1["slcfancalismadevri"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }

                if (f1["txttoplamverim"].ToString() != "" && f1["txttoplamverim"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Toplam Verim").FirstOrDefault();
                    byk.value1 = "n";
                    byk.value2 = "Toplam Verim";
                    byk.value3 = f1["txttoplamverim"].ToString();
                    if (f1["slctoplamverim"].ToString() != null && f1["slctoplamverim"].ToString() != "")
                    {
                        string xs = f1["slctoplamverim"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtgurultusiddeti"].ToString() != "" && f1["txtgurultusiddeti"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Özgül gürültü şiddeti").FirstOrDefault();
                    byk.value1 = "Ls";
                    byk.value2 = "Özgül gürültü şiddeti";
                    byk.value3 = f1["txtgurultusiddeti"].ToString();
                    if (f1["slcgurultusiddeti"].ToString() != null && f1["slcgurultusiddeti"].ToString() != "")
                    {
                        string xs = f1["slcgurultusiddeti"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtanagurultusiddeti"].ToString() != "" && f1["txtanagurultusiddeti"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Ana gürültü şiddeti").FirstOrDefault();
                    byk.value1 = "Lu";
                    byk.value2 = "Ana gürültü şiddeti";
                    byk.value3 = f1["txtanagurultusiddeti"].ToString();
                    if (f1["slcanagurultusiddeti"].ToString() != null && f1["slcanagurultusiddeti"].ToString() != "")
                    {
                        string xs = f1["slcanagurultusiddeti"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }

                if (f1["txtsalyangoz"].ToString() != "" && f1["txtsalyangoz"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Salyangoz Boyutu").FirstOrDefault();
                    byk.value1 = "Fan Tipi";
                    byk.value2 = "Salyangoz Boyutu";
                    byk.value3 = f1["txtsalyangoz"].ToString();
                    if (f1["slcsalyangoz"].ToString() != null && f1["slcsalyangoz"].ToString() != "")
                    {
                        string xs = f1["slcsalyangoz"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtemis"].ToString() != "" && f1["txtemis"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Emiş Ağzı").FirstOrDefault();
                    byk.value1 = "V";
                    byk.value2 = "Emiş Ağzı";
                    byk.value3 = f1["txtemis"].ToString();
                    if (f1["slcemis"].ToString() != null && f1["slcemis"].ToString() != "")
                    {
                        string xs = f1["slcemis"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }

                if (f1["txtcikisagzi"].ToString() != "" && f1["txtcikisagzi"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Çıkış ağzı").FirstOrDefault();
                    byk.value1 = "Pt";
                    byk.value2 = "Çıkış ağzı";
                    byk.value3 = f1["txtcikisagzi"].ToString();
                    if (f1["slccikisagzi"].ToString() != null && f1["slccikisagzi"].ToString() != "")
                    {
                        string xs = f1["slccikisagzi"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtrotor"].ToString() != "" && f1["txtrotor"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Rotor çapı").FirstOrDefault();
                    byk.value1 = "Pd 2";
                    byk.value2 = "Rotor çapı";
                    byk.value3 = f1["txtrotor"].ToString();
                    if (f1["slcrotor"].ToString() != null && f1["slcrotor"].ToString() != "")
                    {
                        string xs = f1["slcrotor"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtagirlik"].ToString() != "" && f1["txtagirlik"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Ağırlık").FirstOrDefault();
                    byk.value1 = "C 2";
                    byk.value2 = "Ağırlık";
                    byk.value3 = f1["txtagirlik"].ToString();
                    if (f1["slcagirlik"].ToString() != null && f1["slcagirlik"].ToString() != "")
                    {
                        string xs = f1["slcagirlik"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txttahrik"].ToString() != "" && f1["txttahrik"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Tahrik şekli").FirstOrDefault();
                    byk.value1 = "Nm";
                    byk.value2 = "Tahrik şekli";
                    byk.value3 = f1["txttahrik"].ToString();
                    if (f1["slctahrik"].ToString() != null && f1["slctahrik"].ToString() != "")
                    {
                        string xs = f1["slctahrik"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }

                if (f1["txtnotasyon"].ToString() != "" && f1["txtnotasyon"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Konum notasyonu").FirstOrDefault();
                    byk.value1 = "n";
                    byk.value2 = "Konum notasyonu";
                    byk.value3 = f1["txtnotasyon"].ToString();
                    if (f1["slcnotasyon"].ToString() != null && f1["slcnotasyon"].ToString() != "")
                    {
                        string xs = f1["slcnotasyon"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }

                if (f1["txtsicaklik"].ToString() != "" && f1["txtsicaklik"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "İşletme Sıcaklığı").FirstOrDefault();
                    byk.value1 = "n";
                    byk.value2 = "İşletme sıcaklığı";
                    byk.value3 = f1["txtsicaklik"].ToString();
                    if (f1["slcsicaklik"].ToString() != null && f1["slcsicaklik"].ToString() != "")
                    {
                        string xs = f1["slcsicaklik"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtkanatadet"].ToString() != "" && f1["txtkanatadet"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Kanat adedi").FirstOrDefault();
                    byk.value1 = "Ls";
                    byk.value2 = "Kanat adedi";
                    byk.value3 = f1["txtkanatadet"].ToString();
                    if (f1["slckanatadet"].ToString() != null && f1["slckanatadet"].ToString() != "")
                    {
                        string xs = f1["slckanatadet"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                if (f1["txtkanatpozisyon"].ToString() != "" && f1["txtkanatpozisyon"] != null)
                {
                    buyukluk_values byk = new buyukluk_values();
                    byk = ent.buyukluk_values.Where(x => x.teklif_id == id && x.value2 == "Kanat pozisyonu").FirstOrDefault();
                    byk.value1 = "Lu";
                    byk.value2 = "Kanat pozisyonu";
                    byk.value3 = f1["txtkanatpozisyon"].ToString();
                    if (f1["slckanatpozisyon"].ToString() != null && f1["slckanatpozisyon"].ToString() != "")
                    {
                        string xs = f1["slckanatpozisyon"].ToString();
                        byk.buyukluk_id = ent.buyukluk_tipleri.Where(x => x.name == xs).FirstOrDefault().id;
                    }
                    ent.SaveChanges();
                }
                return RedirectToAction("Teklifler");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult TeknikOzellikGuncelleiki(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                ViewBag.id = id;
                ViewBag.buyukluks = ent.buyukluk_tipleri.ToList();
                ViewModel vm = new ViewModel();
                vm.buyukluks = ent.buyukluk_tipleri.ToList();
                vm.buyuklukv = ent.buyukluk_values.Where(x => x.teklif_id == id).ToList();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult TeknikOzellikGuncelleiki(int id, FormCollection f1)
        {
            vantsanEntities ent = new vantsanEntities();
            buyukluk_values buyukluk = new buyukluk_values();
            buyukluk.teklif_id = id;
            var xs = f1["buyukluktip"].ToString();
            string val1;

            if (xs == "Büyüklük" || xs == "Salyangoz boyutu")
            {
                val1 = "Fan Tipi";
            }
            else if (xs == "Akışgan Debisi" || xs == "Emiş ağzı")
            {
                val1 = "V";
            }
            else if (xs == "Toplam Basınç" || xs == "Çıkış ağzı")
            {
                val1 = "Pt";
            }
            else if (xs == "Çıkış ağzındaki dinamik basınç" || xs == "Rotor çapı")
            {
                val1 = "Pd 2";
            }
            else if (xs == "Çıkış ağzındaki akışgan hız" || xs == "Ağırlık")
            {
                val1 = "C 2";
            }
            else if (xs == "Motor gücü" || xs == "Tahrik şekli")
            {
                val1 = "Nm";
            }
            else if (xs == "Fan çalışma devri" || xs == "Konum notasyonu")
            {
                val1 = "n";
            }
            else if (xs == "Toplam verim" || xs == "İşletme sıcaklığı")
            {
                val1 = "n";
            }
            else if (xs == "Özgül gürültü şiddeti" || xs == "Kanat adedi")
            {
                val1 = "Ls";
            }
            else if (xs == "Ana Gürültü Şiddeti" || xs == "Kanat pozisyonu")
            {
                val1 = "Lu";
            }
            else
            {
                val1 = " ";
            }
            var xx = f1["byks"].ToString();
            buyukluk.value1 = val1;
            buyukluk.value2 = xs;
            buyukluk.value3 = f1["deger"].ToString();
            buyukluk.buyukluk_id = ent.buyukluk_tipleri.FirstOrDefault(x => x.name == xx).id;
            ent.buyukluk_values.Add(buyukluk);
            ent.SaveChanges();
            return RedirectToAction("TeknikOzellikGuncelleiki");
        }
        public ActionResult TeknikSil(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                buyukluk_values buyuk = ent.buyukluk_values.FirstOrDefault(x => x.id == id);
                int tekid = buyuk.teklif_id;
                ent.buyukluk_values.Remove(buyuk);
                ent.SaveChanges();
                return RedirectToAction("TeknikOzellikGuncelleiki/" + tekid);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult TeknikGuncelleSolo(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                ViewBag.buyukluks = ent.buyukluk_tipleri.ToList();
                var modal = ent.buyukluk_values.FirstOrDefault(x => x.id == id);
                return View(modal);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult TeknikGuncelleSolo(int id, buyukluk_values val, FormCollection f1)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                buyukluk_values buyuk = new buyukluk_values();
                buyuk = ent.buyukluk_values.Where(x => x.id == val.id).FirstOrDefault();
                buyuk.value3 = val.value3;
                var xs = f1["byks"].ToString();
                buyuk.buyukluk_id = ent.buyukluk_tipleri.FirstOrDefault(x => x.name == xs).id;
                ent.SaveChanges();
                return RedirectToAction("TeknikOzellikGuncelleiki/" + buyuk.teklif_id);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult TeklifGoruntuleTR(int id)
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                ViewModel vm = new ViewModel();
                vm.buyukluks = ent.buyukluk_tipleri.ToList();
                vm.buyuklukv = ent.buyukluk_values.Where(x => x.teklif_id == id).ToList();
                vm.pozisyonlars = ent.Pozisyonlar.Where(x => x.Teklif_ID == id).ToList();
                vm.teklifs = ent.teklif.Where(x => x.id == id).ToList();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult BuyuklukTipSil(int id)
        {
            vantsanEntities ent = new vantsanEntities();
            ent.buyukluk_tipleri.Remove(ent.buyukluk_tipleri.FirstOrDefault(x => x.id == id));
            ent.SaveChanges();
            return RedirectToAction("BuyuklukTipleri");
        }
        public ActionResult YeniTeklif()
        {
            if (Session["Login"] != null && Convert.ToBoolean(Session["Login"]) == true)
            {
                vantsanEntities ent = new vantsanEntities();
                ViewModel vm = new ViewModel();
                vm.teslims = ent.teslim_yerleri_tipleri.ToList();
                vm.buyukluks = ent.buyukluk_tipleri.ToList();
                return View(vm);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult YeniTeklif(FormCollection f1)
        {


            //Teklif Ekleme


            vantsanEntities ent = new vantsanEntities();
            teklif tklf = new teklif();
            string c = Convert.ToDateTime(f1["date"]).ToString("yyyy-MM-dd");
            tklf.date = Convert.ToDateTime(c);
            tklf.garanti = f1["garanti"].ToString();
            tklf.ilgi = f1["ilgi"].ToString();
            tklf.isin_adi = f1["isin_adi"].ToString();
            tklf.obsiyon = f1["obsiyon"].ToString();
            tklf.odemesarti = f1["odemesarti"].ToString();
            tklf.sayin = f1["sayin"].ToString();
            tklf.teslimsuresi = f1["teslimsuresi"].ToString();
            tklf.teslim_yeri = f1["teslim_yeri"].ToString();
            tklf.iskonto_orani = Convert.ToInt32(f1["iskonto"].ToString());
            ent.teklif.Add(tklf);
            ent.SaveChanges();

            //Pozisyon Ekleme

            try
            {
                if (f1["miktar[]"] != null && f1["miktar"] != "")
                {

                    ViewBag.id = tklf.id;
                    Pozisyonlar pozisyon = new Pozisyonlar();
                    pozisyon.Teklif_ID = tklf.id;
                    pozisyon.Cinsi = f1["cinsi[]"].ToString();
                    pozisyon.Miktar = Int32.Parse(f1["miktar[]"].ToString());
                    pozisyon.BirimFiyatı = Int32.Parse(f1["birimfiyatı[]"].ToString());
                    pozisyon.Tutarı = Int32.Parse(f1["tutarı[]"].ToString());
                    ent.Pozisyonlar.Add(pozisyon);
                    ent.SaveChanges();
                    
                    try
                    {
                        if (f1["miktars[]"] != null && f1["miktars"] != "")
                        {
                            string[] cinsdizi;
                            string[] miktardizi;
                            string[] birimdizi;
                            string[] tutardizi;
                            string cinsler = "";
                            string miktarlar = "";
                            string birimler = "";
                            string tutarlar = "";
                            for (int i = 0; i < f1["cinsis[]"].ToList().Count; i++)
                            {
                                cinsler += f1["cinsis[]"][i].ToString();
                            }
                            for (int i = 0; i < f1["miktars[]"].ToList().Count; i++)
                            {
                                miktarlar += f1["miktars[]"][i].ToString();
                            }
                            for (int i = 0; i < f1["birimfiyatıs[]"].ToList().Count; i++)
                            {
                                birimler += f1["birimfiyatıs[]"][i].ToString();
                            }
                            for (int i = 0; i < f1["tutarıs[]"].ToList().Count; i++)
                            {
                                tutarlar += f1["tutarıs[]"][i].ToString();
                            }
                            cinsdizi = cinsler.Split(',');
                            birimdizi = birimler.Split(',');
                            miktardizi = miktarlar.Split(',');
                            tutardizi = tutarlar.Split(',');
                            Pozisyonlar pzs = new Pozisyonlar();
                            for (int i = 0; i < tutardizi.Count() - 1; i++)
                            {
                                pzs.BirimFiyatı = Convert.ToInt32(birimdizi[i].ToString());
                                pzs.Cinsi = cinsdizi[i].ToString();
                                pzs.Miktar = Convert.ToInt32(miktardizi[i].ToString());
                                pzs.Teklif_ID = tklf.id;
                                pzs.Tutarı = Convert.ToInt32(tutardizi[i].ToString());
                                ent.Pozisyonlar.Add(pzs);
                                ent.SaveChanges();
                            }

                            teklif tek = new teklif();
                            tek = ent.teklif.Where(x => x.id == tklf.id).FirstOrDefault();
                            int tutartoplam = 0;
                            int kdv = 0;
                            int toplam = 0;
                            foreach (var item in ent.Pozisyonlar.Where(x => x.Teklif_ID == tklf.id).ToList())
                            {
                                tutartoplam += (int)item.Tutarı;
                            }
                            double indirim = Convert.ToDouble(f1["iskonto"])/100;
                            tek.iskonto_tutari = Convert.ToInt32(Convert.ToDouble(tutartoplam) * indirim);
                            kdv = Convert.ToInt32((tutartoplam-tek.iskonto_tutari)*0.18);
                            toplam = (int)((tutartoplam-tek.iskonto_tutari) + kdv);
                            tek.poz_kdv = kdv;
                            tek.poz_tutar_toplam = tutartoplam;
                            tek.poz_toplam = toplam;
                            tek.toplambedel = toplam;
                            ent.SaveChanges();
                        }
                    }
                    catch (Exception)
                    {

                    }

                }

                //TEKNİK ÖZELLİK EKLEME

                buyukluk_values buyukluk = new buyukluk_values();


                if (f1["txtbuyukluk"].ToString() != "" && f1["txtbuyukluk"] != null)
                {
                    var xs = f1["slcbuyukluk"].ToString();
                    var listCLientResult = (from a in ent.buyukluk_tipleri
                                            where (a.name == xs)
                                            select a);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    //ent.buyukluk_tipleri.Where(x => x.name == f1["slcbuyukluk"].ToString()).FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Fan Tipi";
                    buyukluk.value2 = "Büyüklük";
                    buyukluk.value3 = f1["txtbuyukluk"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }

                if (f1["txtakiskan"].ToString() != "" && f1["txtakiskan"] != null)
                {
                    var xs = f1["slcakiskan"].ToString();
                    var listCLientResult = (from b in ent.buyukluk_tipleri
                                            where (b.name == xs)
                                            select b);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "V";
                    buyukluk.value2 = "Akışgan Debisi";
                    buyukluk.value3 = f1["txtakiskan"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }

                if (f1["txttoplambasinc"].ToString() != "" && f1["txttoplambasinc"] != null)
                {
                    var xs = f1["slctoplambasinc"].ToString();
                    var listCLientResult = (from y in ent.buyukluk_tipleri
                                            where (y.name == xs)
                                            select y);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Pt";
                    buyukluk.value2 = "Toplam Basınç";
                    buyukluk.value3 = f1["txttoplambasinc"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtdinamikbasinc"].ToString() != "" && f1["txtdinamikbasinc"] != null)
                {
                    var xs = f1["slcdinamikbasinc"].ToString();
                    var listCLientResult = (from d in ent.buyukluk_tipleri
                                            where (d.name == xs)
                                            select d);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Pd 2";
                    buyukluk.value2 = "Çıkış Ağzındaki dinamik basınç";
                    buyukluk.value3 = f1["txtdinamikbasinc"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtakiskanhiz"].ToString() != "" && f1["txtakiskanhiz"] != null)
                {
                    var xs = f1["slcakiskanhiz"].ToString();
                    var listCLientResult = (from e in ent.buyukluk_tipleri
                                            where (e.name == xs)
                                            select e);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "C 2";
                    buyukluk.value2 = "Çıkış Ağzındaki akışgan hız";
                    buyukluk.value3 = f1["txtakiskanhiz"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtmotorgucu"].ToString() != "" && f1["txtmotorgucu"] != null)
                {
                    var xs = f1["slcmotorgucu"].ToString();
                    var listCLientResult = (from f in ent.buyukluk_tipleri
                                            where (f.name == xs)
                                            select f);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Nm";
                    buyukluk.value2 = "Motor gücü";
                    buyukluk.value3 = f1["txtmotorgucu"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtfancalismadevri"].ToString() != "" && f1["txtfancalismadevri"] != null)
                {
                    var xs = f1["slcfancalismadevri"].ToString();
                    var listCLientResult = (from g in ent.buyukluk_tipleri
                                            where (g.name == xs)
                                            select g);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "n";
                    buyukluk.value2 = "Fan çalışma devri";
                    buyukluk.value3 = f1["txtfancalismadevri"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }

                if (f1["txttoplamverim"].ToString() != "" && f1["txttoplamverim"] != null)
                {
                    var xs = f1["slctoplamverim"].ToString();
                    var listCLientResult = (from h in ent.buyukluk_tipleri
                                            where (h.name == xs)
                                            select h);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "n";
                    buyukluk.value2 = "Toplam verim";
                    buyukluk.value3 = f1["txttoplamverim"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtgurultusiddeti"].ToString() != "" && f1["txtgurultusiddeti"] != null)
                {
                    var xs = f1["slcgurultusiddeti"].ToString();
                    var listCLientResult = (from i in ent.buyukluk_tipleri
                                            where (i.name == xs)
                                            select i);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Ls";
                    buyukluk.value2 = "Özgül gürültü şiddeti";
                    buyukluk.value3 = f1["txtgurultusiddeti"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtanagurultusiddeti"].ToString() != "" && f1["txtanagurultusiddeti"] != null)
                {
                    var xs = f1["slcanagurultusiddeti"].ToString();
                    var listCLientResult = (from j in ent.buyukluk_tipleri
                                            where (j.name == xs)
                                            select j);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Lu";
                    buyukluk.value2 = "Ana gürültü şiddeti";
                    buyukluk.value3 = f1["txtanagurultusiddeti"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }

                if (f1["txtsalyangoz"].ToString() != "" && f1["txtsalyangoz"] != null)
                {
                    var xs = f1["slcsalyangoz"].ToString();
                    var listCLientResult = (from l in ent.buyukluk_tipleri
                                            where (l.name == xs)
                                            select l);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Fan Tipi";
                    buyukluk.value2 = "Salyangoz Boyutu";
                    buyukluk.value3 = f1["txtsalyangoz"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                
                if (f1["txtemis"].ToString() != "" && f1["txtemis"] != null)
                {
                    var xs = f1["slcemis"].ToString();
                    var listCLientResult = (from m in ent.buyukluk_tipleri
                                            where (m.name == xs)
                                            select m);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "V";
                    buyukluk.value2 = "Emiş Ağzı";
                    buyukluk.value3 = f1["txtemis"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }

                if (f1["txtcikisagzi"].ToString() != "" && f1["txtcikisagzi"] != null)
                {
                    var xs = f1["slccikisagzi"].ToString();
                    var listCLientResult = (from n in ent.buyukluk_tipleri
                                            where (n.name == xs)
                                            select n);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Pt";
                    buyukluk.value2 = "Çıkış Ağzı";
                    buyukluk.value3 = f1["txtcikisagzi"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtrotor"].ToString() != "" && f1["txtrotor"] != null)
                {
                    var xs = f1["slcrotor"].ToString();
                    var listCLientResult = (from o in ent.buyukluk_tipleri
                                            where (o.name == xs)
                                            select o);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Pd 2";
                    buyukluk.value2 = "Rotor çapı";
                    buyukluk.value3 = f1["txtrotor"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtagirlik"].ToString() != "" && f1["txtagirlik"] != null)
                {
                    var xs = f1["slcagirlik"].ToString();
                    var listCLientResult = (from p in ent.buyukluk_tipleri
                                            where (p.name == xs)
                                            select p);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "C 2";
                    buyukluk.value2 = "Ağırlık";
                    buyukluk.value3 = f1["txtagirlik"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txttahrik"].ToString() != "" && f1["txttahrik"] != null)
                {
                    var xs = f1["slctahrik"].ToString();
                    var listCLientResult = (from r in ent.buyukluk_tipleri
                                            where (r.name == xs)
                                            select r);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Nm";
                    buyukluk.value2 = "Tahrik şekli";
                    buyukluk.value3 = f1["txttahrik"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }

                if (f1["txtnotasyon"].ToString() != "" && f1["txtnotasyon"] != null)
                {
                    var xs = f1["slcnotasyon"].ToString();
                    var listCLientResult = (from s in ent.buyukluk_tipleri
                                            where (s.name == xs)
                                            select s);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "n";
                    buyukluk.value2 = "Konum notasyonu";
                    buyukluk.value3 = f1["txtnotasyon"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }

                if (f1["txtsicaklik"].ToString() != "" && f1["txtsicaklik"] != null)
                {
                    var xs = f1["slcsicaklik"].ToString();
                    var listCLientResult = (from t in ent.buyukluk_tipleri
                                            where (t.name == xs)
                                            select t);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "n";
                    buyukluk.value2 = "İşletme sıcaklığı";
                    buyukluk.value3 = f1["txtsicaklik"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtkanatadet"].ToString() != "" && f1["txtkanatadet"] != null)
                {
                    var xs = f1["slckanatadet"].ToString();
                    var listCLientResult = (from u in ent.buyukluk_tipleri
                                            where (u.name == xs)
                                            select u);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Ls";
                    buyukluk.value2 = "Kanat adedi";
                    buyukluk.value3 = f1["txtkanatadet"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
                if (f1["txtkanatpozisyon"].ToString() != "" && f1["txtkanatpozisyon"] != null)
                {
                    var xs = f1["slckanatpozisyon"].ToString();
                    var listCLientResult = (from v in ent.buyukluk_tipleri
                                            where (v.name == xs)
                                            select v);
                    buyukluk.buyukluk_id = listCLientResult.FirstOrDefault().id;
                    buyukluk.teklif_id = tklf.id;
                    buyukluk.value1 = "Lu";
                    buyukluk.value2 = "Kanat pozisyonu";
                    buyukluk.value3 = f1["txtkanatpozisyon"].ToString();
                    ent.buyukluk_values.Add(buyukluk);
                    ent.SaveChanges();
                }
            }
            catch (Exception)
            {
            }

            return RedirectToAction("Teklifler");
        }

        public ActionResult MusteriEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult MusteriEkle( Musteri m)
        {
            vantsanEntities ent = new vantsanEntities();
            Musteri customer = new Musteri();
            customer.name = m.name;
            customer.phone = m.phone;
            customer.mail = m.mail;
            ent.Musteri.Add(customer);
            ent.SaveChanges();
            return RedirectToAction("Musteriler");
        }
        public ActionResult Musteriler()
        {
            vantsanEntities ent = new vantsanEntities();
            return View(ent.Musteri.ToList());
        }
        public ActionResult MusteriSil(int id)
        {
            vantsanEntities ent = new vantsanEntities();
            ent.Musteri.Remove(ent.Musteri.FirstOrDefault(x => x.id == id));
            return View();
        }
        public ActionResult MailAyarlari()
        {
            vantsanEntities ent=new vantsanEntities();
            return View(ent.Mail.FirstOrDefault());
        }
        [HttpPost]

        public ActionResult MailAyarlari(Mail m)
        {
            vantsanEntities ent = new vantsanEntities();
            ent.Mail.RemoveRange(ent.Mail.ToList());
            ent.Mail.Add(m);
            ent.SaveChanges();
            return RedirectToAction("Teklifler");
        }
    }
}