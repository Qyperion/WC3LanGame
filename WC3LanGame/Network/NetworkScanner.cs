using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WC3LanGame.Network
{
    internal static class NetworkScanner
    {
        private const int MaxParallelPings = 50;

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
            catch (SocketException) // No such host is known
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

        public static async Task<List<string>> ScanNetwork(
            UnicastIPAddressInformation ipAddress, CancellationToken cancellationToken = default)
        {
            string network = ipAddress.Address + "/" + ipAddress.PrefixLength;
            var ipNetwork = IPNetwork2.Parse(network);
            string[] networkClientAddresses = ipNetwork.ListIPAddress(Filter.Usable)
                .Select(ip => ip.ToString()).ToArray();

            ConcurrentBag<string> activeClients = [];
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = MaxParallelPings,
                CancellationToken = cancellationToken
            };

            await Parallel.ForEachAsync(networkClientAddresses, parallelOptions, async (ip, ct) =>
            {
                using Ping ping = new();
                PingReply reply = await ping.SendPingAsync(ip, 100);
                if (reply.Status == IPStatus.Success)
                    activeClients.Add(ip);
            });

            return activeClients.ToList();
        }

        public static async Task<List<string>> FindActiveIpInAllLocalNetworks(
            CancellationToken cancellationToken = default)
        {
            var localIpList = GetLocalIPv4Addresses();
            List<string> allActiveClients = [];

            foreach (var ip in localIpList)
            {
                cancellationToken.ThrowIfCancellationRequested();
                List<string> activeClients = await ScanNetwork(ip, cancellationToken);
                allActiveClients.AddRange(activeClients);
            }

            return allActiveClients;
        }

        public static async Task<string[]> FindAllActiveIpInAllLocalNetworks(
            ProgressBar progressBar, CancellationToken cancellationToken = default)
        {
            List<string> allNetworksClientAddresses = [];
            var localIpList = GetLocalIPv4Addresses();

            foreach (var localIp in localIpList)
            {
                string network = localIp.Address + "/" + localIp.PrefixLength;
                var ipNetwork = IPNetwork2.Parse(network);
                var networkClientAddresses = ipNetwork.ListIPAddress(Filter.Usable)
                    .Select(ip => ip.ToString());
                allNetworksClientAddresses.AddRange(networkClientAddresses);
            }

            progressBar.Minimum = 0;
            progressBar.Maximum = allNetworksClientAddresses.Count;

            ConcurrentBag<string> activeClients = [];
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = MaxParallelPings,
                CancellationToken = cancellationToken
            };

            try
            {
                await Parallel.ForEachAsync(allNetworksClientAddresses, parallelOptions, async (ip, ct) =>
                {
                    using Ping ping = new();
                    PingReply reply = await ping.SendPingAsync(ip, 250);
                    if (reply.Status == IPStatus.Success)
                        activeClients.Add(ip);
                    progressBar.BeginInvoke(() => progressBar.Value++);
                });
            }
            catch (OperationCanceledException)
            {
                // Scanning cancelled — return whatever was found so far
            }

            progressBar.BeginInvoke(() => progressBar.Visible = false);

            return activeClients.ToArray();
        }
    }
}
