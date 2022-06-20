using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WC3LanGame.Network
{
    internal static class NetworkScanner
    {
        public static IPAddress ResolveHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                return null;

            if (IPAddress.TryParse(host, out var ipAddress))
                return ipAddress;

            try
            {
                return Dns.GetHostEntry(host).AddressList.FirstOrDefault();
            }
            catch(SocketException) // SocketException : No such host is known.
            {
                return null;
            }
        }

        public static List<UnicastIPAddressInformation> GetLocalIPv4Addresses()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(inter => inter.OperationalStatus == OperationalStatus.Up)
                .SelectMany(inter => inter.GetIPProperties().UnicastAddresses)
                .Where(address => address.Address.AddressFamily == AddressFamily.InterNetwork && address.PrefixLength >= 24)
                .ToList();
        }

        public static async Task<List<string>> ScanNetwork(UnicastIPAddressInformation ipAddress)
        {
            string network = ipAddress.Address + "/" + ipAddress.PrefixLength;
            IPNetwork ipNetwork = IPNetwork.Parse(network);
            string[] networkClientAddresses = ipNetwork.ListIPAddress(FilterEnum.Usable)
                .Select(ip => ip.ToString()).ToArray();

            List<string> activeClients = new List<string>();
            ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = networkClientAddresses.Length };

            await Parallel.ForEachAsync(networkClientAddresses, parallelOptions, async (ip, _) =>
            {
                Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(ip, 100);
                if (reply.Status == IPStatus.Success)
                    activeClients.Add(ip);
            });

            return activeClients;
        }

        public static async Task<List<string>> FindActiveIpInAllLocalNetworks()
        {
            var localIpList = GetLocalIPv4Addresses();
            List<string> allActiveClients = new List<string>();

            foreach (var ip in localIpList)
            {
                List<string> activeClients = await ScanNetwork(ip);
                allActiveClients.AddRange(activeClients);
            }

            return allActiveClients;
        }

        public static async Task<string[]> FindAllActiveIpInAllLocalNetworks(ProgressBar progressBar)
        {
            List<string> allNetworksClientAddresses = new List<string>();
            var localIpList = GetLocalIPv4Addresses();

            foreach (var localIp in localIpList)
            {
                string network = localIp.Address + "/" + localIp.PrefixLength;
                IPNetwork ipNetwork = IPNetwork.Parse(network);
                var networkClientAddresses = ipNetwork.ListIPAddress(FilterEnum.Usable)
                    .Select(ip => ip.ToString());
                allNetworksClientAddresses.AddRange(networkClientAddresses);
            }

            progressBar.Minimum = 0;
            progressBar.Maximum = allNetworksClientAddresses.Count;

            List<string> activeClients = new List<string>();
            ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = allNetworksClientAddresses.Count };

            await Parallel.ForEachAsync(allNetworksClientAddresses, parallelOptions, async (ip, _) =>
            {
                Ping ping = new Ping();
                PingReply reply = await ping.SendPingAsync(ip, 250);
                if (reply.Status == IPStatus.Success)
                    activeClients.Add(ip);
                progressBar.BeginInvoke(() => progressBar.Value++);
            });

            progressBar.BeginInvoke(() => progressBar.Visible = false);

            return activeClients.ToArray();
        }
    }
}
