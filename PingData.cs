using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PNet
{
    public class PingData
    {
        public long ActualPing { get; set; }

        public long AveragePing { get; set; }

        public long MaxPing { get; set; }

        public int PacketsSend { get; set; }

        public int PacketsReceived { get; set; }

        public float PacketLoss { get; set; }

        public string Hostname { get; set; }


        IPAddress iPAddress;

        int timeout;

        List<long> lastIps = new List<long>();

        int connectionErrors = 0;

        bool hostnameResolved = false;

        public PingData(IPAddress ip, int timeout = 5000)
        {
            this.iPAddress = ip;
            this.timeout = timeout;
        }

        public Task PingAsync()
        {
            if (hostnameResolved)
            {
                Hostname = Dns.GetHostEntry(iPAddress).HostName;
                hostnameResolved = true;
            }
            Ping ping = new Ping();
            PacketsSend++;
            return ping.SendPingAsync(iPAddress, timeout).ContinueWith(PingCompleted);
        }

        void PingCompleted(Task<PingReply> pr)
        {
            PingReply pingReply = pr.Result;
            if(pingReply.Status == IPStatus.Success)
            {
                PacketsReceived++;
                ActualPing = pingReply.RoundtripTime;
                if (pingReply.RoundtripTime < MaxPing)
                    MaxPing = pingReply.RoundtripTime;
            }
        }
    }
}
