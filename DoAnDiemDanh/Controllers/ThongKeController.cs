using DoAnDiemDanh.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace DoAnDiemDanh.Controllers
{
    [Authorize(Roles = "Admin, GiangVien")]
    public class ThongKeController : Controller
    {
        private BaseModel db = new BaseModel();

        public ActionResult Index()
        {
            ViewBag.MonHoc = db.Entity.MONHOCs;
            ViewBag.Khoa = db.Entity.KHOAs;
            ViewBag.Lop = db.Entity.LOPs;
            return View();
        }
        public IEnumerable<DateTime> EachDay(DateTime d1, DateTime d2)
        {
            for (var day = d1.Date; day.Date <= d2.Date; day = day.AddDays(1))
                yield return day;
        }

        public JsonResult GetBieuDo(int MaNMH)
        {
            List<string> listNgay = new List<string>();
            List<int> listDiHoc = new List<int>();
            List<int> listVangHoc = new List<int>();
            List<string> colorDiHoc = new List<string>();
            List<string> borderDiHoc = new List<string>();
            List<string> colorVangHoc = new List<string>();
            List<string> borderVangHoc = new List<string>();
            IEnumerable<DateTime> listime;
            var NHOMMONHOC = db.Entity.NHOMMONHOCs.SingleOrDefault(_ => _.MaNMH == MaNMH);
          
            if((DateTime)NHOMMONHOC.NgayKT <= DateTime.Now)
            {
                listime = EachDay((DateTime)NHOMMONHOC.NgayBD, (DateTime)NHOMMONHOC.NgayKT);
            }
            else
            {
                listime = EachDay((DateTime)NHOMMONHOC.NgayBD, DateTime.Now);
            }

            foreach(var item in listime)
            {
                var ctdiemdanh = from diemdanh in db.Entity.DIEMDANHs
                            join ctdd in db.Entity.CTDDs on diemdanh.MaDD equals ctdd.MaDD
                            where diemdanh.MaNMH == MaNMH && diemdanh.NgayDiemDanh == item
                            select ctdd;
                

                int vanghoc = ctdiemdanh.Where(s => s.TTDD == false).Count();
                int dihoc = ctdiemdanh.Where(s => s.TTDD == true).Count();
                if(dihoc != 0 || vanghoc != 0)
                {
                    
                    listNgay.Add(Convert.ToDateTime(item).ToString("dd/MM"));
                    listDiHoc.Add(dihoc);
                    listVangHoc.Add(vanghoc);
                    colorDiHoc.Add("rgba(54, 162, 235, 0.2)");
                    borderDiHoc.Add("rgba(54, 162, 235, 1)");
                    colorVangHoc.Add("rgba(255, 99, 132, 0.2)");
                    borderVangHoc.Add("rgba(255, 99, 132, 1)");
                }
               
            }
            int SiSo = NHOMMONHOC.SINHVIENs.Count();
            var data = new {
                TenMH = NHOMMONHOC.MONHOC.TenMH,
                SiSo = SiSo,
                listNgay = listNgay,
                listDiHoc = listDiHoc,
                listVangHoc = listVangHoc,
                colorDiHoc = colorDiHoc,
                borderDiHoc = borderDiHoc,
                colorVangHoc = colorVangHoc,
                borderVangHoc = borderVangHoc
            };


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public string getTenKhoa(int MaKhoa)
        {
            var Khoa = db.Entity.KHOAs.Single(s => s.MaKhoa == MaKhoa);
            return Khoa.TenKhoa;
        }

        public void XuatBaoCaoDiemDanhFull(int MaNMH)
        {
            var nhommonhoc = db.Entity.NHOMMONHOCs.Where(s => s.MaNMH == MaNMH).Single();
            var monhoc = db.Entity.MONHOCs.Where(s => s.MaMH == nhommonhoc.MaMH).Single();
            var fileName = "ExcelData.xlsx";
            var file = new FileInfo(fileName);
            var package = new ExcelPackage(file);
            
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            worksheet.Cells["C1"].Value = "TÊN TRƯỜNG: ĐẠI HỌC NHA TRANG";
            worksheet.Cells["C1:X1"].Style.Font.Size = 9;
            worksheet.Cells["C1:X1"].Style.Font.Bold = true;
            worksheet.Cells["C1:X1"].Style.Font.Name = "Arial";

            worksheet.Cells["A2"].Value = "BẢNG ĐIỂM DANH SINH VIÊN";
            worksheet.Cells["A2:X2"].Style.Font.Size = 16;
            worksheet.Cells["A2:X2"].Style.Font.Bold = true;
            worksheet.Cells["A2:X2"].Style.Font.Name = "Arial";
            worksheet.Cells["A2:X2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            worksheet.Cells["B3"].Value = "Môn học: " + monhoc.TenMH + "    Lớp(theo môn học): " + nhommonhoc.MaNMH;
            worksheet.Cells["B3:X3"].Style.Font.Size = 10;
            worksheet.Cells["B3:X3"].Style.Font.Bold = true;
            worksheet.Cells["B3:X3"].Style.Font.Name = "Arial";
            worksheet.Cells["B3:X3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B3:X3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["A5:Z60"].Style.Font.Size = 10;
            worksheet.Cells["A5:Z60"].Style.Font.Name = "Arial";
            worksheet.Cells["A5:Z60"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells["A5"].Value = "TT";
            worksheet.Cells["A5:A6"].Merge = true;
            worksheet.Column(1).Width = 5;

            worksheet.Cells["B5"].Value = "Mã sinh viên";
            worksheet.Cells["B5:B6"].Merge = true;
            worksheet.Column(2).Width = 15;

            worksheet.Cells["C5"].Value = "Họ Và Tên";
            worksheet.Cells["C5:C6"].Merge = true;
            worksheet.Column(3).Width = 18;

            worksheet.Cells["D5"].Value = "Khoa";
            worksheet.Cells["D5:D6"].Merge = true;
            worksheet.Column(4).Width = 25;

            worksheet.Cells["E5"].Value = "Lớp theo khóa";
            worksheet.Cells["E5:E6"].Merge = true;
            worksheet.Column(5).Width = 18;

            worksheet.Cells["F5"].Value = "Thời khóa biểu";

            var ngayhoc =  from nmh in db.Entity.NHOMMONHOCs
                           join dd in db.Entity.DIEMDANHs on nmh.MaNMH equals dd.MaNMH
                           where nmh.MaNMH == MaNMH 
                           select dd;
            var row = 6;
            var col = 6;
            foreach(var item in ngayhoc)
            {
                worksheet.Cells[row, col].Value = Convert.ToDateTime(item.NgayDiemDanh).ToString("dd/MM/yyyy");
                worksheet.Cells[row, col].Style.TextRotation = 90;
                worksheet.Cells[row, col].Style.Font.Size = 11;
                worksheet.Cells[row, col].Style.Font.Name = "Calibri";
                worksheet.Column(col).Width = 3;
                col++;
            }
            worksheet.Cells[5, 6, 5, col - 1].Merge = true;

            worksheet.Cells[1, 3, 1, col + 3].Merge = true;

            worksheet.Cells[2, 1, 2, col + 3].Merge = true;
            worksheet.Cells[3, 2, 3, col + 3].Merge = true;

            worksheet.Cells[5,col].Value = "Tổng Cộng";
            worksheet.Cells[5,col,6,col].Merge = true;
            worksheet.Column(col).Width = 15;

            worksheet.Cells[5, col + 1].Value = "Vắng";
            worksheet.Cells[5, col + 1, 5, col + 2].Merge = true;

            worksheet.Cells[6,col+1].Value = "Có phép";
            worksheet.Column(col + 1).Width = 15;

            worksheet.Cells[6, col+2].Value = "Không phép";
            worksheet.Column(col + 2).Width = 15;

            worksheet.Cells[6, col + 3].Value = "Điểm chuyên cần";
            worksheet.Column(col + 3).Width = 15;



            var data = from dd in db.Entity.DIEMDANHs
                           join ctdd in db.Entity.CTDDs on dd.MaDD equals ctdd.MaDD
                           where dd.MaNMH == MaNMH && dd.NgayDiemDanh <= DateTime.Now
                           group ctdd by ctdd.MaSV into ctdd_sv
                           select ctdd_sv;
            var stt = 1;
            var col1 = 1;
            var row1 = 7;
            foreach(var item in data)
            {
                var data1 =    from sv in db.Entity.SINHVIENs
                               join k in db.Entity.KHOAs on sv.MaKhoa equals k.MaKhoa
                               join l in db.Entity.LOPs on sv.MaLop equals l.MaLop
                               where sv.MaSV == item.Key
                               select new { sinhvien = sv, tenkhoa = k.TenKhoa, tenlop = l.TenLop};

                worksheet.Cells[row1, col1].Value = stt;
                worksheet.Cells[row1, col1 + 1].Value = data1.Single().sinhvien.MaSV;
                worksheet.Cells[row1, col1 + 2].Value = data1.Single().sinhvien.TenSV;
                worksheet.Cells[row1, col1 + 3].Value = data1.Single().tenkhoa;
                worksheet.Cells[row1, col1 + 4].Value = data1.Single().tenlop;

                int t = 5;
                int dihoc = 0;
                int nghihoc = 0;
                int cophep = 0;
                foreach (var item1 in item)
                {
                    if ((bool)item1.TTDD)
                    {
                        worksheet.Cells[row1, col1 + t].Value = "X";
                        dihoc++;
                        if (item1.VangCoPhep != null)
                        {
                            if ((bool)item1.VangCoPhep)
                            {
                                cophep++;
                            }
                        }
                       
                    }
                    else
                    {
                        worksheet.Cells[row1, col1 + t].Value = "V";
                        nghihoc++;
                    }
                    t++;
                }
                worksheet.Cells[row1, col].Value = dihoc;
                worksheet.Cells[row1, col + 1].Value = cophep;
                worksheet.Cells[row1, col + 2].Value = nghihoc - cophep;

                double x = 1 - 0.25 * (nghihoc);
                if (x < 0) x = 0;
                worksheet.Cells[row1, col+3].Value = x;

                row1++;
                stt++;
            }

            worksheet.Cells[row1+2, 1].Value =$" Ngày xuất: {DateTime.Now}";
            worksheet.Cells[row1+2, 1, row1+2, 3].Merge = true;


            string tenfile = $"DiemDanh_{monhoc.TenMH}_{Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy")}";

            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader(
                        "content-disposition",
                        string.Format("attachment;  filename={0}", $"{tenfile}.xlsx"));
            this.Response.BinaryWrite(package.GetAsByteArray());    
        }

        public void XuatBaoCaoCamThi(int MaNMH)
        {
            var nhommonhoc = db.Entity.NHOMMONHOCs.Where(s => s.MaNMH == MaNMH).Single();
            var monhoc = db.Entity.MONHOCs.Where(s => s.MaMH == nhommonhoc.MaNMH).Single();
            var fileName = "ExcelData.xlsx";
            var file = new FileInfo(fileName);
            var package = new ExcelPackage(file);

            var worksheet = package.Workbook.Worksheets.Add("Sheet1");

            worksheet.Cells["C1"].Value = "TÊN TRƯỜNG: ĐẠI HỌC NHA TRANG";
            worksheet.Cells["C1:X1"].Style.Font.Size = 9;
            worksheet.Cells["C1:X1"].Style.Font.Bold = true;
            worksheet.Cells["C1:X1"].Style.Font.Name = "Arial";

            worksheet.Cells["A2"].Value = "DANH SÁCH SINH VIÊN CẤM THI";
            worksheet.Cells["A2:X2"].Style.Font.Size = 16;
            worksheet.Cells["A2:X2"].Style.Font.Bold = true;
            worksheet.Cells["A2:X2"].Style.Font.Name = "Arial";
            worksheet.Cells["A2:X2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


            worksheet.Cells["B3"].Value = "Môn học: " + monhoc.TenMH + "    Lớp(theo môn học): " + monhoc.MaMH;
            worksheet.Cells["B3:X3"].Style.Font.Size = 10;
            worksheet.Cells["B3:X3"].Style.Font.Bold = true;
            worksheet.Cells["B3:X3"].Style.Font.Name = "Arial";
            worksheet.Cells["B3:X3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["B3:X3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["A5:Z60"].Style.Font.Size = 10;
            worksheet.Cells["A5:Z60"].Style.Font.Name = "Arial";
            worksheet.Cells["A5:Z60"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells["A5"].Value = "TT";
            worksheet.Cells["A5:A6"].Merge = true;
            worksheet.Column(1).Width = 5;

            worksheet.Cells["B5"].Value = "Mã sinh viên";
            worksheet.Cells["B5:B6"].Merge = true;
            worksheet.Column(2).Width = 15;

            worksheet.Cells["C5"].Value = "Họ Và Tên";
            worksheet.Cells["C5:C6"].Merge = true;
            worksheet.Column(3).Width = 18;

            worksheet.Cells["D5"].Value = "Khoa";
            worksheet.Cells["D5:D6"].Merge = true;
            worksheet.Column(4).Width = 25;

            worksheet.Cells["E5"].Value = "Lớp theo khóa";
            worksheet.Cells["E5:E6"].Merge = true;
            worksheet.Column(5).Width = 18;

            worksheet.Cells["F5"].Value = "Thời khóa biểu";

            var ngayhoc = from nmh in db.Entity.NHOMMONHOCs
                          join dd in db.Entity.DIEMDANHs on nmh.MaNMH equals dd.MaNMH
                          where nmh.MaNMH == MaNMH
                          select dd;
            var row = 6;
            var col = 6;
            foreach (var item in ngayhoc)
            {
                worksheet.Cells[row, col].Value = Convert.ToDateTime(item.NgayDiemDanh).ToString("dd/MM/yyyy");
                worksheet.Cells[row, col].Style.TextRotation = 90;
                worksheet.Cells[row, col].Style.Font.Size = 11;
                worksheet.Cells[row, col].Style.Font.Name = "Calibri";
                worksheet.Column(col).Width = 3;
                col++;
            }
            worksheet.Cells[5, 6, 5, col - 1].Merge = true;

            worksheet.Cells[1, 3, 1, col + 3].Merge = true;

            worksheet.Cells[2, 1, 2, col + 3].Merge = true;
            worksheet.Cells[3, 2, 3, col + 3].Merge = true;

            worksheet.Cells[5, col].Value = "Tổng Cộng";
            worksheet.Cells[5, col, 6, col].Merge = true;
            worksheet.Column(col).Width = 15;

            worksheet.Cells[5, col + 1].Value = "Vắng";
            worksheet.Cells[5, col + 1, 5, col + 2].Merge = true;

            worksheet.Cells[6, col + 1].Value = "Có phép";
            worksheet.Column(col + 1).Width = 15;

            worksheet.Cells[6, col + 2].Value = "Không phép";
            worksheet.Column(col + 2).Width = 15;

            var data = from dd in db.Entity.DIEMDANHs
                       join ctdd in db.Entity.CTDDs on dd.MaDD equals ctdd.MaDD
                       where dd.MaNMH == MaNMH && dd.NgayDiemDanh < DateTime.Now
                       group ctdd by ctdd.MaSV into ctdd_sv
                       select ctdd_sv;

            var stt = 1;
            var col1 = 1;
            var row1 = 7;
            foreach (var item in data)
            {
                var data1 = from sv in db.Entity.SINHVIENs
                            join k in db.Entity.KHOAs on sv.MaKhoa equals k.MaKhoa
                            join l in db.Entity.LOPs on sv.MaLop equals l.MaLop
                            where sv.MaSV == item.Key
                            select new { sinhvien = sv, tenkhoa = k.TenKhoa, tenlop = l.TenLop };

                worksheet.Cells[row1, col1].Value = stt;
                worksheet.Cells[row1, col1 + 1].Value = data1.Single().sinhvien.MaSV;
                worksheet.Cells[row1, col1 + 2].Value = data1.Single().sinhvien.TenSV;
                worksheet.Cells[row1, col1 + 3].Value = data1.Single().tenkhoa;
                worksheet.Cells[row1, col1 + 4].Value = data1.Single().tenlop;

                int t = 5;
                int dihoc = 0;
                int nghihoc = 0;
                int cophep = 0;
                foreach (var item1 in item)
                {
                    if ((bool)item1.TTDD)
                    {
                        worksheet.Cells[row1, col1 + t].Value = "X";
                        dihoc++;
                        if (item1.VangCoPhep != null)
                        {
                            if ((bool)item1.VangCoPhep)
                            {
                                cophep++;
                            }
                        }

                    }
                    else
                    {
                        worksheet.Cells[row1, col1 + t].Value = "V";
                        nghihoc++;
                    }
                    t++;
                }
                worksheet.Cells[row1, col].Value = dihoc;
                worksheet.Cells[row1, col + 1].Value = cophep;
                worksheet.Cells[row1, col + 2].Value = nghihoc - cophep;

                
                if (nghihoc - cophep <= monhoc.SoNgayVang)
                {
                    worksheet.DeleteRow(row1);
                }
                else
                {
                    row1++;
                    stt++;
                }
            }

            worksheet.Cells[row1 + 2, 1].Value = $" Ngày xuất: {DateTime.Now}";
            worksheet.Cells[row1 + 2, 1, row1 + 2, 3].Merge = true;


            string tenfile = $"CamThi_{monhoc.TenMH}_{Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy")}";

            this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            this.Response.AddHeader(
                        "content-disposition",
                        string.Format("attachment;  filename={0}", $"{tenfile}.xlsx"));
            this.Response.BinaryWrite(package.GetAsByteArray());

        }

        public static double GetTrueColumnWidth(double width)
        {
            //DEDUCE WHAT THE COLUMN WIDTH WOULD REALLY GET SET TO
            double z = 1d;
            if (width >= (1 + 2 / 3))
            {
                z = Math.Round((Math.Round(7 * (width - 1 / 256), 0) - 5) / 7, 2);
            }
            else
            {
                z = Math.Round((Math.Round(12 * (width - 1 / 256), 0) - Math.Round(5 * width, 0)) / 12, 2);
            }

            //HOW FAR OFF? (WILL BE LESS THAN 1)
            double errorAmt = width - z;

            //CALCULATE WHAT AMOUNT TO TACK ONTO THE ORIGINAL AMOUNT TO RESULT IN THE CLOSEST POSSIBLE SETTING 
            double adj = 0d;
            if (width >= (1 + 2 / 3))
            {
                adj = (Math.Round(7 * errorAmt - 7 / 256, 0)) / 7;
            }
            else
            {
                adj = ((Math.Round(12 * errorAmt - 12 / 256, 0)) / 12) + (2 / 12);
            }

            //RETURN A SCALED-VALUE THAT SHOULD RESULT IN THE NEAREST POSSIBLE VALUE TO THE TRUE DESIRED SETTING
            if (z > 0)
            {
                return width + adj + 0.11;
            }

            return 0d;
        }

        //public void XuatChiTietDiemDanh(int MaMH, int MaSV)
        //{
        //    var monhoc = db.Entity.MONHOCs.Where(s => s.MaMH == MaMH).Single();
        //    var fileName = "ExcelData.xlsx";
        //    var file = new FileInfo(fileName);
        //    var package = new ExcelPackage(file);

        //    var worksheet = package.Workbook.Worksheets.Add("Sheet1");

        //    var thongtinsv = (from sv in db.Entity.SINHVIENs
        //                      where sv.MaSV == MaSV
        //                      select new { masv = sv.MaSV, tensv = sv.TenSV }).Single();

        //    var thongtinmh = (from mh in db.Entity.MONHOCs
        //                      where mh.MaMH == MaMH
        //                      select new { mamh = mh.MaMH, tenmh = mh.TenMH, sotc = mh.SoTC }).Single();

        //    var thongtindd1 = (from dd in db.Entity.DIEMDANHs
        //                      where dd.MaMH == MaMH
        //                      select dd).ToList();

        //    var thongtindd2 = (from dd in db.Entity.DIEMDANHs
        //                      where dd.MaMH == MaMH && dd.NgayDiemDanh < DateTime.Now
        //                      select dd).ToList();

        //    var diemdanh = (from dd in db.Entity.DIEMDANHs
        //                     join ctdd in db.Entity.CTDDs on dd.MaDD equals ctdd.MaDD
        //                     where dd.MaMH == MaMH && dd.NgayDiemDanh < DateTime.Now && ctdd.MaSV == MaSV && ctdd.TTDD == true
        //                     select ctdd).ToList();

        //    var diemdanhct = (from dd in db.Entity.DIEMDANHs
        //                      join ctdd in db.Entity.CTDDs on dd.MaDD equals ctdd.MaDD
        //                      join mh in db.Entity.MONHOCs on dd.MaMH equals mh.MaMH
        //                      where dd.MaMH == MaMH && dd.NgayDiemDanh < DateTime.Now && ctdd.MaSV == MaSV
        //                      select new {ngaydd = dd.NgayDiemDanh, daugio = mh.ThoiGianBDGD, cuoigio = mh.ThoiGianKTGD, tgdddg = ctdd.ThoiGianVao, tgddcg = ctdd.ThoiGianRa, tinhtrang = ctdd.TTDD, }).ToList();

        //    worksheet.Column(1).Width = GetTrueColumnWidth(27.11);
        //    worksheet.Column(2).Width = GetTrueColumnWidth(9.11);
        //    worksheet.Column(3).Width = GetTrueColumnWidth(21);
        //    worksheet.Column(4).Width = GetTrueColumnWidth(23.22);
        //    worksheet.Column(5).Width = GetTrueColumnWidth(28.44);
        //    worksheet.Column(6).Width = GetTrueColumnWidth(28.22);
        //    worksheet.Column(7).Width = GetTrueColumnWidth(14);
        //    worksheet.Column(8).Width = GetTrueColumnWidth(11);

        //    worksheet.Cells["A2"].Value = $"Mã sinh viên: {thongtinsv.masv}   Tên sinh viên: {thongtinsv.tensv}    Môn học: {thongtinmh.tenmh}    Lớp: {thongtinmh.mamh}";
        //    worksheet.Cells["A2:H2"].Merge = true;
        //    worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    worksheet.Cells["A2"].Style.Font.Name = "Calibri";
        //    worksheet.Cells["A2"].Style.Font.Bold = true;
        //    worksheet.Cells["A2"].Style.Font.Color.SetColor(0,32,55,100);
        //    worksheet.Cells["A2:G2"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["A3"].Value = "Điểm danh";
        //    worksheet.Cells["A3"].Style.Font.Bold = true;
        //    worksheet.Cells["A3:B3"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["A4"].Value = "Tổng số tín chỉ";
        //    worksheet.Cells["A4"].Style.Font.Bold = true;
        //    worksheet.Cells["A4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["B4"].Value = thongtinmh.sotc;
        //    worksheet.Cells["B4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["A5"].Value = "Tổng số buổi học phải tham gia";
        //    worksheet.Cells["A5"].Style.Font.Bold = true;
        //    worksheet.Cells["A5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["B5"].Value = thongtindd1.Count();
        //    worksheet.Cells["B5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["A6"].Value = "Số buổi học đã qua";
        //    worksheet.Cells["A6"].Style.Font.Bold = true;
        //    worksheet.Cells["A6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["B6"].Value = thongtindd2.Count();
        //    worksheet.Cells["B6"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["A7"].Value = "Số buổi học còn lại";
        //    worksheet.Cells["A7"].Style.Font.Bold = true;
        //    worksheet.Cells["A7"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["B7"].Value = thongtindd1.Count() - thongtindd2.Count();
        //    worksheet.Cells["B7"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["D4"].Value = "Số buổi học có điểm danh";
        //    worksheet.Cells["D4"].Style.Font.Bold = true;
        //    worksheet.Cells["D4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["E4"].Value = diemdanh.Count();
        //    worksheet.Cells["E4"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["D5"].Value = "Số buổi học vắng";
        //    worksheet.Cells["D5"].Style.Font.Bold = true;
        //    worksheet.Cells["D5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["E5"].Value = thongtindd2.Count() - diemdanh.Count();
        //    worksheet.Cells["E5"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["A8"].Value = "Chi tiết";
        //    worksheet.Cells["A8"].Style.Font.Italic = true;

        //    worksheet.Cells["A9"].Value = "Ngày";
        //    worksheet.Cells["A9"].Style.Font.Bold = true;
        //    worksheet.Cells["A9:A10"].Merge = true;
        //    worksheet.Cells["A9:A10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    worksheet.Cells["A9:A10"].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            

        //    worksheet.Cells["B9"].Value = "Thứ";
        //    worksheet.Cells["B9"].Style.Font.Bold = true;
        //    worksheet.Cells["B9:B10"].Merge = true;
        //    worksheet.Cells["B9:B10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    worksheet.Cells["B9:B10"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["C9"].Value = "Thời gian điểm danh qđ";
        //    worksheet.Cells["C9"].Style.Font.Bold = true;
        //    worksheet.Cells["C9:D9"].Merge = true;
        //    worksheet.Cells["C9:D9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    worksheet.Cells["C9:D9"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["C10"].Value = "Đầu giờ";
        //    worksheet.Cells["C10"].Style.Font.Bold = true;
        //    worksheet.Cells["C10"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["D10"].Value = "Cuối giờ";
        //    worksheet.Cells["D10"].Style.Font.Bold = true;
        //    worksheet.Cells["D10"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["E9"].Value = "Thời gian SV điểm danh đầu giờ";
        //    worksheet.Cells["E9"].Style.Font.Bold = true;
        //    worksheet.Cells["E9:E10"].Merge = true;
        //    worksheet.Cells["E9:E10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    worksheet.Cells["E9:E10"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["F9"].Value = "Thời gian SV điểm danh cuối giờ";
        //    worksheet.Cells["F9"].Style.Font.Bold = true;
        //    worksheet.Cells["F9:F10"].Merge = true;
        //    worksheet.Cells["F9:F10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    worksheet.Cells["F9:F10"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["G9"].Value = "Lượt điểm danh";
        //    worksheet.Cells["G9"].Style.Font.Bold = true;
        //    worksheet.Cells["G9:G10"].Merge = true;
        //    worksheet.Cells["G9:G10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    worksheet.Cells["G9:G10"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    worksheet.Cells["H9"].Value = "Ghi chú";
        //    worksheet.Cells["H9"].Style.Font.Bold = true;
        //    worksheet.Cells["H9:H10"].Merge = true;
        //    worksheet.Cells["H9:H10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    worksheet.Cells["H9:H10"].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //    var col = 1;
        //    var row = 11;

        //    CultureInfo vn = new CultureInfo("vi-VN");
           
        //    foreach (var item in diemdanhct)
        //    {
             
        //        DateTime dt = (DateTime)item.ngaydd;
        //        worksheet.Cells[row, col].Value = Convert.ToDateTime(dt).ToString("dd/MM/yyyy");
        //        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //        worksheet.Cells[row, col+1].Value = vn.DateTimeFormat.GetDayName(dt.DayOfWeek);
        //        worksheet.Cells[row, col+1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //        TimeSpan t1 = (TimeSpan)item.daugio;
        //        TimeSpan t2 = (TimeSpan)item.daugio + TimeSpan.FromMinutes(15);

        //        TimeSpan t3 = (TimeSpan)item.cuoigio;
        //        TimeSpan t4 = (TimeSpan)item.cuoigio - TimeSpan.FromMinutes(15);

        //        var diemdanhdg = "_";
        //        var diemdanhcg = "_";

        //        if(item.tgdddg != null)
        //        {
        //            TimeSpan t5 = (TimeSpan)item.tgdddg;
        //            diemdanhdg = t5.ToString(@"hh\:mm");
        //        }

        //        if (item.tgddcg != null)
        //        {
        //            TimeSpan t6 = (TimeSpan)item.tgddcg;
        //            diemdanhcg = t6.ToString(@"hh\:mm");
        //        }

        //        worksheet.Cells[row, col + 2].Value = $"{t1.ToString(@"hh\:mm")} - {t2.ToString(@"hh\:mm")}";
        //        worksheet.Cells[row, col + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //        worksheet.Cells[row, col + 3].Value = $"{t4.ToString(@"hh\:mm")} - {t3.ToString(@"hh\:mm")}";
        //        worksheet.Cells[row, col + 3].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //        worksheet.Cells[row, col + 4].Value = diemdanhdg;
        //        worksheet.Cells[row, col + 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //        worksheet.Cells[row, col + 5].Value = diemdanhcg;
        //        worksheet.Cells[row, col + 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //        worksheet.Cells[row, col + 6].Value = ((bool)item.tinhtrang) ? "1": "v";
        //        worksheet.Cells[row, col + 6].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //        worksheet.Cells[row, col + 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        //        row += 1;
        //    }

        //    worksheet.Cells[row, 6].Value = "Tổng lượt điểm danh: ";
        //    worksheet.Cells[row, 6].Style.Font.Bold = true;

        //    worksheet.Cells[row, 7].Value = diemdanh.Count();
        //    worksheet.Cells[row, 7].Style.Font.Bold = true;

        //    var date = DateTime.Now;

        //    worksheet.Cells[row + 1,2].Value = $"{vn.DateTimeFormat.GetDayName(date.DayOfWeek)} Ngày {date.Day} Tháng {date.Month} Năm {date.Year}";
        //    worksheet.Cells[row + 1, 2].Style.Font.Name = "Arial";
        //    worksheet.Cells[row + 1, 2].Style.Font.Size = 10;
        //    worksheet.Cells[row + 1, 2, row + 1, 7].Merge = true;
        //    worksheet.Cells[row + 1, 2, row + 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        //    worksheet.Cells[row + 2, 2].Value = "Sinh viên";
        //    worksheet.Cells[row + 2, 2].Style.Font.Name = "Arial";
        //    worksheet.Cells[row + 2, 2].Style.Font.Size = 10;
        //    worksheet.Cells[row + 2, 2].Style.Font.Bold = true;

        //    worksheet.Cells[row + 3, 2].Value = "(Ký, họ tên)";
        //    worksheet.Cells[row + 3, 2].Style.Font.Name = "Arial";
        //    worksheet.Cells[row + 3, 2].Style.Font.Size = 10;
        //    worksheet.Cells[row + 3, 2].Style.Font.Italic = true;


        //    worksheet.Cells[row + 2, 5].Value = "Giảng viên";
        //    worksheet.Cells[row + 2, 5].Style.Font.Name = "Arial";
        //    worksheet.Cells[row + 2, 5].Style.Font.Size = 10;
        //    worksheet.Cells[row + 2, 5].Style.Font.Bold = true;

        //    worksheet.Cells[row + 3, 5].Value = "(Ký, họ tên)";
        //    worksheet.Cells[row + 3, 5].Style.Font.Name = "Arial";
        //    worksheet.Cells[row + 3, 5].Style.Font.Size = 10;
        //    worksheet.Cells[row + 3, 5].Style.Font.Italic = true;


        //    worksheet.Cells[row + 2, 7].Value = "Phòng giáo vụ";
        //    worksheet.Cells[row + 2, 7].Style.Font.Name = "Arial";
        //    worksheet.Cells[row + 2, 7].Style.Font.Size = 10;
        //    worksheet.Cells[row + 2, 7].Style.Font.Bold = true;

        //    worksheet.Cells[row + 3, 7].Value = "(Ký, họ tên)";
        //    worksheet.Cells[row + 3, 7].Style.Font.Name = "Arial";
        //    worksheet.Cells[row + 3, 7].Style.Font.Size = 10;
        //    worksheet.Cells[row + 3, 7].Style.Font.Italic = true;

        //    worksheet.Cells[row + 5, 1].Value = "Thời gian điểm danh dựa theo thời khóa biểu, quy định cách thời gian bắt đầu và thời gian kết thúc học 15 phút";
        //    worksheet.Cells[row + 6, 1].Value = "Điểm danh ngoài khung giờ quy định được coi là vắng";
        //    worksheet.Cells[row + 7, 1].Value = "Điểm danh không đủ hai lượt đầu giờ và cuối giờ được coi là vắng";
        //    worksheet.Cells[row + 8, 1].Value = "Không điểm danh được coi là vắng";
        //    worksheet.Cells[row + 9, 1].Value = "Cách tính điểm chuyên cần: Điểm chuyên cần là 1 điểm, vắng 1 buổi - 0,25 điểm";
        //    worksheet.Cells[row + 10, 1].Value = "Sinh viên được vắng tối đa 3 buổi, nếu hơn sẽ cấm thi";

        //    worksheet.Cells[row + 5, 1, row + 10, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //    worksheet.Cells[row + 5, 1, row + 10, 8].Style.Fill.BackgroundColor.SetColor(0, 255, 242, 204);

        //    string tenfile = $"ChiTietDiemDanh_MaSV_{MaSV}_{monhoc.TenMH}_{Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy")}";

        //    this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    this.Response.AddHeader(
        //                "content-disposition",
        //                string.Format("attachment;  filename={0}", $"{tenfile}.xlsx"));
        //    this.Response.BinaryWrite(package.GetAsByteArray());
        //}

        [HttpGet]
        public JsonResult GetMonHoc(int MaNMH)
        {
            db.Entity.Configuration.ProxyCreationEnabled = false;

            var NHOMMONHOC = db.Entity.NHOMMONHOCs.Single(s => s.MaNMH == MaNMH);

            var data = (from nmh in db.Entity.NHOMMONHOCs
                        where nmh.MaNMH == MaNMH
                        join mh in db.Entity.MONHOCs on nmh.MaMH equals mh.MaMH 
                        join gv in db.Entity.GIANGVIENs on nmh.MaGV equals gv.MaGV
                        select new {MaNMH = nmh.MaNMH ,MaMH = mh.MaMH, TenMH = mh.TenMH, SoTC = mh.SoTC, NgayBD = nmh.NgayBD, NgayKT = nmh.NgayKT,ThoiGianBD = nmh.ThoiGianBD, ThoiGianKT = nmh.ThoiGianKT ,SiSo = nmh.SINHVIENs.Count(), TenGV = gv.TenGV, DSSV = nmh.SINHVIENs.ToList() }).Single();

            TimeSpan t1 = (TimeSpan)data.ThoiGianBD;
            TimeSpan t2 = (TimeSpan)data.ThoiGianKT;
            var thoigianbd = t1.Hours + ":" + t1.Minutes;
            var thoigiankt = t2.Hours + ":" + t2.Minutes;
            var data1 = new
            {
                MaNMH = data.MaNMH,
                MaMH = data.MaMH,
                TenMH = data.TenMH,
                SoTC = data.SoTC,
                NgayBD = Convert.ToDateTime((DateTime)data.NgayBD).ToString("dd/MM/yyyy"),
                NgayKT = Convert.ToDateTime((DateTime)data.NgayKT).ToString("dd/MM/yyyy"),
                ThoiGianBD = t1.ToString(),
                ThoiGianKT = t2.ToString(),
                SiSo = data.SiSo,
                TenGV = data.TenGV,
                DSSV = data.DSSV
            };

            return Json(data1, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetSinhVien(int MaSV, int MaNMH)
        {
            db.Entity.Configuration.ProxyCreationEnabled = false;

            var tongsobuoi = db.Entity.DIEMDANHs.Where(_ => _.MaNMH == MaNMH).Count();

            var data1 =  (from sv in db.Entity.SINHVIENs
                         join ctdd in db.Entity.CTDDs on sv.MaSV equals ctdd.MaSV
                         join dd in db.Entity.DIEMDANHs on ctdd.MaDD equals dd.MaDD
                         where sv.MaSV == MaSV && dd.MaNMH == MaNMH && ctdd.TTDD == true
                         select ctdd).ToList();
            

            var data2 = (from sv in db.Entity.SINHVIENs
                       join k in db.Entity.KHOAs on sv.MaKhoa equals k.MaKhoa
                       join l in db.Entity.LOPs on sv.MaLop equals l.MaLop
                       where sv.MaSV == MaSV
                       select new { tenlop = l.TenLop, tenkhoa = k.TenKhoa }).Single();

            var data3 = new
            {
                tenlop = data2.tenlop,
                tenkhoa = data2.tenkhoa,
                tongsobuoi = tongsobuoi,
                sobuoithamgia = data1.Count()
            };

            return Json(data3, JsonRequestBehavior.AllowGet);
        }

    }
}