using DoAnDiemDanh.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DoAnDiemDanh.Controllers
{
    [Authorize(Roles = "SinhVien")]
    public class SinhVienController : Controller
    {
        private FACE_RECOGNITIONEntities db = new FACE_RECOGNITIONEntities();

        public int getMaSV()
        {
            HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                if (!string.IsNullOrEmpty(authCookie.Value))
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                    string str = authTicket.UserData;
                    string[] subs = str.Split(',');
                    return Int32.Parse(subs[0]);
                }
            }
            return -1;
        }

        public ActionResult XemDiemDanh()
        {
            int MSSV = getMaSV();
            ViewBag.MonHoc = db.MONHOCs.Where(s => s.SINHVIENs.Any(sv => sv.MaSV == MSSV));
            return View();   
        }

        public ActionResult XemThoiKhoaBieu()
        {
            int MSSV = getMaSV();
            ViewBag.TenSV = db.SINHVIENs.SingleOrDefault(s => s.MaSV == MSSV).TenSV.ToUpper();
            return View();
        }

        public ActionResult KetQua(int MaMH)
        {
            int MSSV = getMaSV();
            ViewBag.MaMH = MaMH;
            ViewBag.MONHOC = db.MONHOCs.Where(s => s.MaMH == MaMH);
            ViewBag.TenSV = db.SINHVIENs.SingleOrDefault(s => s.MaSV == MSSV).TenSV.ToUpper();
            return View();
        }

        [HttpGet]
        public ActionResult GuiKhieuNai(int MaDD, int MaSV, int MaMH)
        {
            ViewBag.MaDD = MaDD;
            ViewBag.MaSV = MaSV;
            ViewBag.MaMH = MaMH;
            return View();
        }

        [HttpPost]
        public ActionResult GuiKhieuNai([Bind(Include = "MaKN,NoiDung")] KHIEUNAI kHIEUNAI, int MaDD, int MaSV, int MaMH)
        {
            var img = Request.Files["Avatar"];
            string postedFileName = System.IO.Path.GetFileName(img.FileName);
            var path = Server.MapPath("~/Content/img/" + postedFileName);
            img.SaveAs(path);

            var khieunai = new KHIEUNAI();
            khieunai.NoiDung = kHIEUNAI.NoiDung;
            khieunai.TenHA = postedFileName;
            khieunai.NgayGui = DateTime.Now;
            khieunai.ThoiGianGui = DateTime.Now;
            db.KHIEUNAIs.Add(khieunai);
            db.SaveChanges();

            var ctdd = db.CTDDs.SingleOrDefault(s => s.MaDD == MaDD && s.MaSV == MaSV);
            ctdd.MaKN = khieunai.MaKN;
            db.Entry(ctdd).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("KetQua",new {MaMH = MaMH});
        }
        [HttpGet]
        public JsonResult getDiemDanh(int MaMH)
        {
          
            int MSSV = getMaSV();
            var diemdanhct = (from dd in db.DIEMDANHs
                              join ctdd in db.CTDDs on dd.MaDD equals ctdd.MaDD
                              join mh in db.MONHOCs on dd.MaMH equals mh.MaMH
                              where dd.MaMH == MaMH && dd.NgayDiemDanh < DateTime.Now && ctdd.MaSV == MSSV
                              select new { ngaydd = dd.NgayDiemDanh, daugio = mh.ThoiGianBDGD, cuoigio = mh.ThoiGianKTGD, tgdddg = ctdd.ThoiGianVao, tgddcg = ctdd.ThoiGianRa, tinhtrang = ctdd.TTDD, vangcophep = ctdd.VangCoPhep, khieunai = ctdd.MaKN, madd = ctdd.MaDD, masv = ctdd.MaSV}).ToList();
            List<diemdanh> list = new List<diemdanh>();

            CultureInfo vn = new CultureInfo("vi-VN");

            foreach (var item in diemdanhct)
            {
                diemdanh dd = new diemdanh();
                dd.MaDD = item.madd;
                dd.MaSV = item.masv;
                if (item.khieunai != null) dd.MaKN = (int)item.khieunai;
                DateTime dt = (DateTime)item.ngaydd;
                dd.NgayDiemDanh = Convert.ToDateTime(dt).ToString("dd/MM/yyyy");
                dd.Thu = vn.DateTimeFormat.GetDayName(dt.DayOfWeek);
                
                var diemdanhdg = "_";
                var diemdanhcg = "_";

                if (item.tgdddg != null)
                {
                    TimeSpan t5 = (TimeSpan)item.tgdddg;
                    diemdanhdg = t5.ToString(@"hh\:mm");
                }

                if (item.tgddcg != null)
                {
                    TimeSpan t6 = (TimeSpan)item.tgddcg;
                    diemdanhcg = t6.ToString(@"hh\:mm");
                }
                dd.TGDDDG = diemdanhdg;
                dd.TGDDCG = diemdanhcg;

                if(!(bool)item.tinhtrang && item.vangcophep == null)
                {
                    dd.TrangThai = "Vắng Không Phép";
                }

                else if (!(bool)item.tinhtrang && (bool)item.vangcophep)
                {
                    dd.TrangThai = "Vắng Có Phép";
                }
                else
                {
                    dd.TrangThai = "Có Mặt";
                }

                list.Add(dd);
            }
            

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ThoiKhoaBieu()
        {

            var list = new List<thoikhoabieu>();
            int MSSV = getMaSV();

            var MonHoc = db.MONHOCs.Where(s => s.SINHVIENs.Any(sv => sv.MaSV == MSSV)).ToList();
            
            foreach(var item in MonHoc)
            {
                DateTime dtbd = (DateTime)item.NgayBD;
                DateTime dtkt = (DateTime)item.NgayKT;
                TimeSpan tgbd = (TimeSpan)item.ThoiGianBDGD;
                TimeSpan tgkt = (TimeSpan)item.ThoiGianKTGD;

                if ((bool)item.THOIKHOABIEU.ThuHai){
                    var tkb = new thoikhoabieu();
                    tkb.MaMH = item.MaMH;
                    tkb.TenMH = item.TenMH;
                    tkb.SoTC = (int)item.SoTC;
                    tkb.Thu = "Thứ Hai";
                    tkb.NgayBD = Convert.ToDateTime(dtbd).ToString("dd/MM/yyyy");
                    tkb.ThoiGianBD = tgbd.ToString(@"hh\:mm");
                    tkb.ThoiGianKT = tgkt.ToString(@"hh\:mm");
                    tkb.vt = 2;
                    list.Add(tkb);
                }

                if ((bool)item.THOIKHOABIEU.ThuBa)
                {
                    var tkb = new thoikhoabieu();
                    tkb.MaMH = item.MaMH;
                    tkb.TenMH = item.TenMH;
                    tkb.SoTC = (int)item.SoTC;
                    tkb.Thu = "Thứ Ba";
                    tkb.NgayBD = Convert.ToDateTime(dtbd).ToString("dd/MM/yyyy");
                    tkb.ThoiGianBD = tgbd.ToString(@"hh\:mm");
                    tkb.ThoiGianKT = tgkt.ToString(@"hh\:mm");
                    tkb.vt = 3;
                    list.Add(tkb);
                }

                if ((bool)item.THOIKHOABIEU.ThuTu)
                {
                    var tkb = new thoikhoabieu();
                    tkb.MaMH = item.MaMH;
                    tkb.TenMH = item.TenMH;
                    tkb.SoTC = (int)item.SoTC;
                    tkb.Thu = "Thứ Tư";
                    tkb.NgayBD = Convert.ToDateTime(dtbd).ToString("dd/MM/yyyy");
                    tkb.ThoiGianBD = tgbd.ToString(@"hh\:mm");
                    tkb.ThoiGianKT = tgkt.ToString(@"hh\:mm");
                    tkb.vt = 4;
                    list.Add(tkb);
                }

                if ((bool)item.THOIKHOABIEU.ThuNam)
                {
                    var tkb = new thoikhoabieu();
                    tkb.MaMH = item.MaMH;
                    tkb.TenMH = item.TenMH;
                    tkb.SoTC = (int)item.SoTC;
                    tkb.Thu = "Thứ Năm";
                    tkb.NgayBD = Convert.ToDateTime(dtbd).ToString("dd/MM/yyyy");
                    tkb.ThoiGianBD = tgbd.ToString(@"hh\:mm");
                    tkb.ThoiGianKT = tgkt.ToString(@"hh\:mm");
                    tkb.vt = 5;
                    list.Add(tkb);
                }

                if ((bool)item.THOIKHOABIEU.ThuSau)
                {
                    var tkb = new thoikhoabieu();
                    tkb.MaMH = item.MaMH;
                    tkb.TenMH = item.TenMH;
                    tkb.SoTC = (int)item.SoTC;
                    tkb.Thu = "Thứ Sáu";
                    tkb.NgayBD = Convert.ToDateTime(dtbd).ToString("dd/MM/yyyy");
                    tkb.ThoiGianBD = tgbd.ToString(@"hh\:mm");
                    tkb.ThoiGianKT = tgkt.ToString(@"hh\:mm");
                    tkb.vt = 6;
                    list.Add(tkb);
                }

                if ((bool)item.THOIKHOABIEU.ThuBay)
                {
                    var tkb = new thoikhoabieu();
                    tkb.MaMH = item.MaMH;
                    tkb.TenMH = item.TenMH;
                    tkb.SoTC = (int)item.SoTC;
                    tkb.Thu = "Thứ Bảy";
                    tkb.NgayBD = Convert.ToDateTime(dtbd).ToString("dd/MM/yyyy");
                    tkb.ThoiGianBD = tgbd.ToString(@"hh\:mm");
                    tkb.ThoiGianKT = tgkt.ToString(@"hh\:mm");
                    tkb.vt = 7;
                    list.Add(tkb);
                }
            }
            List<thoikhoabieu> SortedList = list.OrderBy(o => o.vt).ToList();
            return Json(SortedList, JsonRequestBehavior.AllowGet);
        }

        public class diemdanh
        {
            public int MaKN { get; set; }
            public int MaDD { get; set; }
            public int MaSV { get; set; }
            public string NgayDiemDanh { get; set; }
            public string Thu { get; set; }

            public string TGDDDG { get; set; }

            public string TGDDCG { get; set; }

            public string TrangThai { get; set; }

            public string KhieuNai { get; set; }
        }

        public class thoikhoabieu
        {
            public int MaMH { get; set; }
            public string TenMH { get; set; }
            public int SoTC { get; set; }

            public string Thu { get; set; }

            public string ThoiGianBD { get; set; }

            public string ThoiGianKT { get; set; }

            public string NgayBD { get; set; }
            public int vt { get; set; }

        }

    }
}