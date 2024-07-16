using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PNetDll.Sqlite;
using PNetDll.Sqlite.Models;
using Microsoft.EntityFrameworkCore;
using Avalonia.Input;
using System.ComponentModel;

namespace PNetClient.Pages
{
    public class PdfGenerationPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<TestCase> LoggedTests { get; set; }
        public int LoadedCount { get; set; }
        public int TestsCount { get; set; }
        public bool PossibleToLoadMore { 
            get
            {
                return _possibleToLoadMore;
            }
            set
            {
                _possibleToLoadMore = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PossibleToLoadMore"));
            }
        }
        bool _possibleToLoadMore;

        public PdfGenerationPageViewModel()
        {
            LoggedTests = new ObservableCollection<TestCase>();
            LoadedCount = 0;
            PossibleToLoadMore = false;
        }     

        public void GeneratePdf(object tc)
        {
            Pdf.PdfGenerator generator = new Pdf.PdfGenerator();
            generator.GeneratePdf((TestCase)tc);
        }

        public void LoadMoreData()
        {
            if(LoadedCount == 0)
            {
                using(PingContext db = new PingContext())
                {
                    TestsCount = db.TestCases.Count();
                }             
            }
            try
            {
                LoadData();
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        void LoadData()
        {
            using(PingContext db = new PingContext())
            {
                List<TestCase> tests = db.TestCases.Include(tc => tc.DestinationHost).ToList().TakeLast(LoadedCount + 50).Reverse().ToList();
                LoggedTests.Clear();
                foreach(TestCase test in tests)
                {
                    LoggedTests.Add(test);
                }
                LoadedCount += tests.Count;
                if(LoadedCount < TestsCount)
                {
                    PossibleToLoadMore = true;
                }
                else
                {
                    PossibleToLoadMore = false;
                }
            }
        }
    }
}
