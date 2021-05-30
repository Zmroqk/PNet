using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Net.Sockets;

namespace PNetDll
{
    /// <summary>
    /// Class that sends pings to destination and creates history about this connection
    /// </summary>
    public class PingTest : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when ping is completed
        /// </summary>
        public event PingCompletedEventHandler PingCompleted;

        /// <summary>
        /// Last ping
        /// </summary>
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

        /// <summary>
        /// Average ping
        /// </summary>
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

        /// <summary>
        /// Max ping
        /// </summary>
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

        /// <summary>
        /// Packets send
        /// </summary>
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

        /// <summary>
        /// Packets received
        /// </summary>
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

        /// <summary>
        /// Percent of packets lost
        /// </summary>
        public double PacketLoss
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

        /// <summary>
        /// Destination domain name
        /// </summary>
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

        /// <summary>
        /// IP address for this test
        /// </summary>
        public IPAddress IpAddress { get; set; }

        long _ActualPing;
        long _AveragePing;
        long _MaxPing;
        int _PacketsSend;
        int _PacketsReceived;
        double _PacketLoss;
        string _Hostname;

        /// <summary>
        /// Timeout value
        /// </summary>
        int timeout;

        /// <summary>
        /// Last pings history
        /// </summary>
        List<long> lastPings;

        /// <summary>
        /// Connection errors count
        /// </summary>
        ushort connectionErrors;

        /// <summary>
        /// True if we already tried getting dns name for ip address
        /// </summary>
        bool hostnameResolved;

        /// <summary>
        /// Index of current packet
        /// </summary>
        int index;

        /// <summary>
        /// Last packet index received
        /// </summary>
        int lastIndex;

        /// <summary>
        /// Create instance of test
        /// </summary>
        /// <param name="ip">IP Address for this test</param>
        /// <param name="timeout">Timeout value for ping connection</param>
        public PingTest(IPAddress ip, int timeout = 5000)
        {
            this.IpAddress = ip;
            this.timeout = timeout;
            lastPings = new List<long>();
            connectionErrors = 0;
            hostnameResolved = false;
            index = 0;
            lastIndex = 0;
        }

        /// <summary>
        /// Send a ping to destination
        /// </summary>
        /// <returns>Running task for sending a ping to destination</returns>
        public Task PingAsync()
        {
            if (!hostnameResolved)
            {
                Task.Run(() =>
                {
                    try
                    {
                        Hostname = Dns.GetHostEntry(IpAddress)?.HostName;
                    }
                    catch (SocketException e) { }
                    if (string.IsNullOrEmpty(Hostname))
                    {
                        Hostname = IpAddress.ToString();
                    }
                    hostnameResolved = true;
                });              
            }
            Ping ping = new Ping();
            PacketsSend++;
            return ping.SendPingAsync(IpAddress, timeout, BitConverter.GetBytes(index++)).ContinueWith(OnPingCompleted);
        }

        /// <summary>
        /// Operation called when ping returns
        /// </summary>
        /// <param name="pr">Task with ping reply data</param>
        void OnPingCompleted(Task<PingReply> pr)
        {
            PingReply pingReply = pr.Result;
            int currentIndex = index;
            if (pingReply.Buffer.Length != 0)
                currentIndex = BitConverter.ToInt32(pingReply.Buffer);
            if (pingReply.Status == IPStatus.Success)
            {
                PacketsReceived++;
                connectionErrors = 0;
                if(lastIndex < currentIndex)
                {
                    ActualPing = pingReply.RoundtripTime;
                    lastIndex = currentIndex;
                }           
                if (pingReply.RoundtripTime > MaxPing)
                    MaxPing = pingReply.RoundtripTime;
            }
            else
            {
                if (lastIndex < currentIndex)
                {
                    ActualPing = timeout;
                    lastIndex = currentIndex;
                }
                connectionErrors++;
            }
            PacketLoss = Math.Round(100 - (PacketsReceived / (float)PacketsSend) * 100, 3);
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
                DateTime = DateTime.Now.AddMilliseconds(-pingReply.RoundtripTime),
                IPAddress = IpAddress,
                IPAddressString = IpAddress.ToString(),
                Hostname = Hostname,
                Index = currentIndex,
                Ping = pingReply.RoundtripTime,
                Success = pingReply.Status == IPStatus.Success,
                ErrorCount = connectionErrors
            };
            PingCompleted?.Invoke(this, pingData);
        }

        /// <summary>
        /// Notify property has changed
        /// </summary>
        /// <param name="propertyName">Name of property</param>
        void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"Test for: {IpAddress, -20}, {Hostname}\n" +
                $"Actual ping: {ActualPing, -5} Average ping: {AveragePing, -5} Max ping: {MaxPing, -5}\n" +
                $"Packets send: {PacketsSend, -8} Packets received: {PacketsReceived, -8} Packet loss: {PacketLoss, -3}%\n";
        }
    }
}
