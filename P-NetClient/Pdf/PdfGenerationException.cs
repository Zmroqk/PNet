using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetClient.Pdf
{
    public class PdfGenerationException : Exception
    {
        public PdfGenerationException() : base("Cannot generate pdf") { }
        public PdfGenerationException(string message) : base(message) { }
    }
}
