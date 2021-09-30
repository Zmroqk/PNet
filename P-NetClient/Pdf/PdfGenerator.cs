using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.IO;
using PNetDll;
using PNetDll.Sqlite;
using PNetDll.Sqlite.Models;

namespace PNetClient.Pdf
{
    public class PdfGenerator
    {
        public double margin
        {
            set
            {
                marginLeft = value;
                marginTop = value;
                marginRight = value;
                marginBottom = value;
            }
        }
        public double marginLeft { get; set; } = 5;
        public double marginTop { get; set; } = 5;
        public double marginRight { get; set; } = 5;
        public double marginBottom { get; set; } = 5;

        public double rowHeight { get; set; } = 15;
        public double fontSize { get; set; } = 10;

        public void GeneratePdf(DateTime startTime, DateTime endTime, Ip ip)
        {
            if(ip == null)
            {
                throw new PdfGenerationException();
            }

            PdfDocument document = new PdfDocument();
            PingContext db = Database.Db;
            PingDataModel[] pingDatas = db.PingsData.Where((pingData) => ip.IpId == pingData.Ip.IpId &&
                                                        pingData.Date >= startTime &&
                                                        pingData.Date <= endTime).OrderBy((pingData) => pingData.PingDataModelId)
                                                        .ToArray();
            double records = pingDatas.Length;
            PdfPage page = document.AddPage();
            double pageWidth = page.Width.Value;
            double pageHeight = page.Height.Value;
            double linesForPage = (pageHeight - marginBottom - marginTop) / rowHeight;
            double rectangleWidth = (pageWidth - marginLeft - marginRight) / 2;
            double currentHeight = marginTop;
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdena", fontSize, XFontStyle.Regular);
            XFont boldFont = new XFont("Verdena", fontSize, XFontStyle.Bold);
            gfx.DrawString(
                "Test data for: ",
                boldFont,
                XBrushes.Black,
                new XRect(0, currentHeight, pageWidth, fontSize),
                XStringFormats.Center);
            currentHeight += fontSize;
            gfx.DrawString(
                $"{ip.IPAddress} {ip.Hostname}",
                boldFont, 
                XBrushes.Black, 
                new XRect(0, currentHeight, pageWidth, fontSize), 
                XStringFormats.Center);
            currentHeight += fontSize;
            gfx.DrawString(
                $"Between {startTime.ToShortDateString()}-{endTime.ToShortDateString()}",
                boldFont,
                XBrushes.Black,
                new XRect(0, currentHeight, pageWidth, fontSize),
                XStringFormats.Center);
            currentHeight += fontSize;
            for (int i = 1; i <= records; i++)
            {
                gfx.DrawRectangle(new XPen(XBrushes.Black), 
                    XBrushes.White, 
                    new XRect(marginLeft, currentHeight, rectangleWidth, rowHeight));
                gfx.DrawRectangle(new XPen(XBrushes.Black), 
                    XBrushes.White, 
                    new XRect(marginLeft + rectangleWidth, currentHeight, rectangleWidth, rowHeight));
                gfx.DrawString(
                    pingDatas[i-1].Date.ToShortTimeString(),
                    font,
                    XBrushes.Black,
                    new XRect(marginLeft, currentHeight, rectangleWidth, rowHeight),
                    XStringFormats.Center);
                gfx.DrawString(
                    pingDatas[i-1].Ping.ToString(),
                    font,
                    XBrushes.Black,
                    new XRect(marginLeft + rectangleWidth, currentHeight, rectangleWidth, rowHeight),
                    XStringFormats.Center);
                currentHeight += rowHeight;
                if (i % linesForPage == 0)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    pageWidth = page.Width.Value;
                    pageHeight = page.Height.Value;
                    linesForPage = pageHeight - marginBottom - marginTop;
                    rectangleWidth = (pageWidth - marginLeft - marginRight) / 2;
                    currentHeight = marginTop;
                }
            }
            document.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PNet", "pdf_test.pdf"));
        }
    }
}
