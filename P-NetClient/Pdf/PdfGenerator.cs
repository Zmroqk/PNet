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
using Microsoft.EntityFrameworkCore;

namespace PNetClient.Pdf
{
    public class PdfGenerator
    {
        public double Margin
        {
            set
            {
                MarginLeft = value;
                MarginTop = value;
                MarginRight = value;
                MarginBottom = value;
            }
        }
        public double MarginLeft { get; set; } = 5;
        public double MarginTop { get; set; } = 5;
        public double MarginRight { get; set; } = 5;
        public double MarginBottom { get; set; } = 5;

        public double RowHeight { get; set; } = 15;
        public double FontSize { get; set; } = 10;
        public double SmallerFontSize { get; set; } = 9;
        public double RectangleDifference { get; set; } = 40;

        TestCase TestCase { get; set; }
        PdfDocument Document { get; set; }
        PdfPage CurrentPage { get; set; }
        double PageWidth { get; set; }
        double PageHeight { get; set; }
        XGraphics gfx { get; set; }
        double CurrentHeight { get; set; }
        double RectangleWidth { get; set; }
        XFont Font { get; set; }
        XFont SmallerFont { get; set; }
        XFont BoldFont { get; set; }

        public void GeneratePdf(TestCase testCase)
        {
            if(testCase == null)
            {
                throw new PdfGenerationException();
            }
            TestCase = testCase;

            Document = new PdfDocument();
            PingContext db = Database.Db;
            PingDataModel[] pingDatas = db.PingsData.Include(pingData => pingData.Ip)
                                                    .Where((pingData) => testCase == pingData.TestCase)
                                                    .OrderBy((pingData) => pingData.Date)
                                                    .ToArray();
            Disconnect[] disconnects = db.Disconnects.Include(dc => dc.ConnectedIp)
                                                    .Where(dc => dc.Test == testCase)
                                                    .OrderBy(dc => dc.DisconnectDate)
                                                    .ToArray();
            double records = pingDatas.Length;
            CurrentPage = Document.AddPage();
            PageWidth = CurrentPage.Width.Value;
            PageHeight = CurrentPage.Height.Value;
            RectangleWidth = (PageWidth - MarginLeft - MarginRight) / 3;
            CurrentHeight = MarginTop;
            gfx = XGraphics.FromPdfPage(CurrentPage);
            Font = new XFont("Arial", FontSize, XFontStyle.Regular);
            SmallerFont = new XFont("Arial", SmallerFontSize, XFontStyle.Regular);
            BoldFont = new XFont("Arial", FontSize, XFontStyle.Bold);
            GenerateTopOfPage();
            if(disconnects.Length > 0)
            {
                GenerateDisconnectTableHeader();
                foreach (Disconnect dc in disconnects)
                {
                    GenerateDisconnectTableRow(dc);
                }
                CurrentHeight += 20;
            }
            if(pingDatas.Length > 0)
            {
                GenerateTableHeader();
                foreach (PingDataModel pdm in pingDatas)
                {
                    GenerateTableRow(pdm);
                }
            }
            string filename = $"{testCase.DestinationHost.Hostname} " +
                $"{testCase.testStarted.ToString("dd-MM-yyyy HH-mm-ss")}-" +
                $"{testCase.testEnded.ToString("dd-MM-yyyy HH-mm-ss")}.pdf";
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PNet", "pdfs");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            Document.Save(Path.Combine(folderPath, filename));
        }

        private void GenerateNewPage()
        {
            CurrentPage = Document.AddPage();
            gfx = XGraphics.FromPdfPage(CurrentPage);
            PageWidth = CurrentPage.Width.Value;
            PageHeight = CurrentPage.Height.Value;
            RectangleWidth = (PageWidth - MarginLeft - MarginRight) / 3;
            CurrentHeight = MarginTop;
            GenerateTopOfPage();
        }

        private void GenerateDisconnectTableRow(Disconnect dc)
        {
            if (CurrentHeight + RowHeight + MarginBottom > PageHeight)
            {
                GenerateNewPage();
                GenerateDisconnectTableHeader();
            }
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                    XBrushes.White,
                    new XRect(MarginLeft, CurrentHeight, RectangleWidth + 2 * RectangleDifference, RowHeight));
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                XBrushes.White,
                new XRect(MarginLeft + RectangleWidth + 2 * RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight));
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                XBrushes.White,
                new XRect(MarginLeft + 2 * RectangleWidth + RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight));
            gfx.DrawString(
                dc.ConnectedIp.Hostname,
                SmallerFont,
                XBrushes.Black,
                new XRect(MarginLeft, CurrentHeight, RectangleWidth + 2 * RectangleDifference, RowHeight),
                XStringFormats.Center);
            gfx.DrawString(
                $"{dc.DisconnectDate.Date.ToShortDateString()} {dc.DisconnectDate.ToString("HH:mm:ss:fff")}",
                Font,
                XBrushes.Black,
                new XRect(MarginLeft + RectangleWidth + 2 * RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight),
                XStringFormats.Center);
            gfx.DrawString(
                $"{dc.ReconnectDate.Date.ToShortDateString()} {dc.ReconnectDate.ToString("HH:mm:ss:fff")}",
                Font,
                XBrushes.Black,
                new XRect(MarginLeft + 2 * RectangleWidth + RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight),
                XStringFormats.Center);
            CurrentHeight += RowHeight;
        }

        private void GenerateTableRow(PingDataModel pdm)
        {
            if (CurrentHeight + RowHeight + MarginBottom > PageHeight)
            {
                GenerateNewPage();
                GenerateTableHeader();
            }
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                    XBrushes.White,
                    new XRect(MarginLeft, CurrentHeight, RectangleWidth + RectangleDifference, RowHeight));
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                XBrushes.White,
                new XRect(MarginLeft + RectangleWidth + RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight));
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                XBrushes.White,
                new XRect(MarginLeft + 2 * RectangleWidth, CurrentHeight, RectangleWidth, RowHeight));
            gfx.DrawString(
                pdm.Ip.Hostname,
                SmallerFont,
                XBrushes.Black,
                new XRect(MarginLeft, CurrentHeight, RectangleWidth + RectangleDifference, RowHeight),
                XStringFormats.Center);
            gfx.DrawString(
                $"{pdm.Date.ToShortDateString()} {pdm.Date.ToString("HH:mm:ss:fff")}",
                Font,
                XBrushes.Black,
                new XRect(MarginLeft + RectangleWidth + RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight),
                XStringFormats.Center);
            gfx.DrawString(
                pdm.Ping.ToString(),
                Font,
                XBrushes.Black,
                new XRect(MarginLeft + 2 * RectangleWidth, CurrentHeight, RectangleWidth, RowHeight),
                XStringFormats.Center);
            CurrentHeight += RowHeight;
        }

        private void GenerateTopOfPage()
        {
            gfx.DrawString(
                "Test data for: ",
                Font,
                XBrushes.Black,
                new XRect(0, CurrentHeight, PageWidth, FontSize),
                XStringFormats.Center);
            CurrentHeight += FontSize;
            gfx.DrawString(
                $"{TestCase.DestinationHost.IPAddress} {TestCase.DestinationHost.Hostname}",
                Font,
                XBrushes.Black,
                new XRect(0, CurrentHeight, PageWidth, FontSize),
                XStringFormats.Center);
            CurrentHeight += FontSize;
            gfx.DrawString(
                $"Between {TestCase.testStarted.ToString("dd.MM.yyyy HH:mm:ss")} - {TestCase.testEnded.ToString("dd.MM.yyyy HH:mm:ss")}",
                Font,
                XBrushes.Black,
                new XRect(0, CurrentHeight, PageWidth, FontSize),
                XStringFormats.Center);
            CurrentHeight += FontSize + MarginTop;
        }

        private void GenerateTableHeader()
        {          
            if(CurrentHeight + RowHeight > PageHeight)
            {
                GenerateNewPage();
            }
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                    XBrushes.White,
                    new XRect(MarginLeft, CurrentHeight, RectangleWidth + RectangleDifference, RowHeight));
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                XBrushes.White,
                new XRect(MarginLeft + RectangleWidth + RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight));
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                XBrushes.White,
                new XRect(MarginLeft + 2 * RectangleWidth, CurrentHeight, RectangleWidth, RowHeight));
            gfx.DrawString(
                "Hostname",
                Font,
                XBrushes.Black,
                new XRect(MarginLeft, CurrentHeight, RectangleWidth + RectangleDifference, RowHeight),
                XStringFormats.Center);
            gfx.DrawString(
                "Time",
                Font,
                XBrushes.Black,
                new XRect(MarginLeft + RectangleWidth + RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight),
                XStringFormats.Center);
            gfx.DrawString(
                "Ping",
                Font,
                XBrushes.Black,
                new XRect(MarginLeft + 2 * RectangleWidth, CurrentHeight, RectangleWidth, RowHeight),
                XStringFormats.Center);
            CurrentHeight += RowHeight;
        }

        private void GenerateDisconnectTableHeader()
        {
            if (CurrentHeight + RowHeight > PageHeight)
            {
                GenerateNewPage();
            }
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                XBrushes.White,
                new XRect(MarginLeft, CurrentHeight, RectangleWidth + 2 * RectangleDifference, RowHeight));
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                XBrushes.White,
                new XRect(MarginLeft + RectangleWidth + 2 * RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight));
            gfx.DrawRectangle(new XPen(XBrushes.Black),
                XBrushes.White,
                new XRect(MarginLeft + 2 * RectangleWidth + RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight));
            gfx.DrawString(
                "Hostname",
                SmallerFont,
                XBrushes.Black,
                new XRect(MarginLeft, CurrentHeight, RectangleWidth + 2 * RectangleDifference, RowHeight),
                XStringFormats.Center);
            gfx.DrawString(
                "Disconnect Date",
                Font,
                XBrushes.Black,
                new XRect(MarginLeft + RectangleWidth + 2 * RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight),
                XStringFormats.Center);
            gfx.DrawString(
                "Reconnect Date",
                Font,
                XBrushes.Black,
                new XRect(MarginLeft + 2 * RectangleWidth + RectangleDifference, CurrentHeight, RectangleWidth - RectangleDifference, RowHeight),
                XStringFormats.Center);
            CurrentHeight += RowHeight;
        }
    }
}
