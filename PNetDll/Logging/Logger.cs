using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNetDll.Logging
{
    public abstract class Logger : IDisposable
    {
        public PingTestManager Manager { get; }

        public Logger(PingTestManager pingTestManager)
        {
            Manager = pingTestManager;
        }

        public abstract void StartAutomaticLogging();

        public abstract void Dispose();
    }
}
