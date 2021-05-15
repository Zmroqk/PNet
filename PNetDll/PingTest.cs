using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PNetDll
{
    public class PingTest : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PingCompletedEventHandler PingCompleted;

        public long ActualPing
        {
            get
            {
                return _ActualPing;
            }
            set
            {
                _ActualPing = value;
                NotifyPropertyChanged();
            }
        }

        public long AveragePing
        {
            get
            {
                return _AveragePing;
            }
            set
            {
                _AveragePing = value;
                NotifyPropertyChanged();
            }
        }

        public long MaxPing
        {
            get
            {
                return _MaxPing;
            }
            set
            {
                _MaxPing = value;
                NotifyPropertyChanged();
            }
        }

        public int PacketsSend
        {
            get
            {
                return _PacketsSend;
            }
            set
            {
                _PacketsSend = value;
                NotifyPropertyChanged();
            }
        }

        public int PacketsReceived
        {
            get
            {
                return _PacketsReceived;
            }
            set
            {
                _PacketsReceived = value;
                NotifyPropertyChanged();
            }
        }

        public float PacketLoss
        {
            get
            {
                return _PacketLoss;
            }
            set
            {
                _PacketLoss = value;
                NotifyPropertyChanged();
            }
        }

        public string Hostname
        {
            get
            {
                return _Hostname;
            }
            set
            {
                _Hostname = value;
                NotifyPropertyChanged();
            }
        }

        long _ActualPing;
        long _AveragePing;
        long _MaxPing;
        int _PacketsSend;
        int _PacketsReceived;
        float _PacketLoss;
        string _Hostname;

        IPAddress iPAddress;

        int timeout;

        List<long> lastPings;

        ushort connectionErrors;

        bool hostnameResolved;

        int index;

        int lastIndex;

        public PingTest(IPAddress ip, int timeout = 5000)
        {
            this.iPAddress = ip;
            this.timeout = timeout;
            lastPings = new List<long>();
            connectionErrors = 0;
            hostnameResolved = false;
            index = 0;
            lastIndex = 0;
        }

        public Task PingAsync()
        {
            if (hostnameResolved)
            {
                Hostname = Dns.GetHostEntry(iPAddress)?.HostName;
                hostnameResolved = true;
            }
            Ping ping = new Ping();
            PacketsSend++;
            return ping.SendPingAsync(iPAddress, timeout, BitConverter.GetBytes(index++)).ContinueWith(OnPingCompleted);
        }

        void OnPingCompleted(Task<PingReply> pr)
        {
            PingReply pingReply = pr.Result;
            int currentIndex = BitConverter.ToInt32(pingReply.Buffer);
            PacketsReceived++;
            if (pingReply.Status == IPStatus.Success)
            {
                connectionErrors = 0;
                if(lastIndex < currentIndex)
                {
                    ActualPing = pingReply.RoundtripTime;
                    lastIndex = currentIndex;
                }           
                if (pingReply.RoundtripTime < MaxPing)
                    MaxPing = pingReply.RoundtripTime;
            }
            else
            {
                connectionErrors++;
            }
            PacketLoss = PacketsReceived / PacketLoss;
            lock (lastPings)
            {
                lastPings.Add(pingReply.RoundtripTime);
                if(lastPings.Count > 100)
                {
                    lastPings.RemoveAt(0);
                }
            }
            if(Monitor.TryEnter(lastPings)){
                long sum = 0;
                foreach(long ping in lastPings)
                {
                    sum += ping;
                }
                AveragePing = sum / lastPings.Count;
                Monitor.Exit(lastPings);
            }
            PingData pingData = new PingData()
            {
                Index = index,
                Ping = pingReply.RoundtripTime,
                Success = pingReply.Status == IPStatus.Success,
                ErrorCount = connectionErrors
            };
            PingCompleted?.Invoke(this, pingData);
        }

        void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
