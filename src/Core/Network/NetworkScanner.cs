using System.Collections.Concurrent;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WC3LanGame.Core.Network
{
    public static class NetworkScanner
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

        /// <summary>
        /// Scans all local networks in parallel and returns active IP addresses.
        /// Reports progress via optional IProgress (value 0.0 to 1.0).
        /// </summary>
        public static async Task<string[]> FindAllActiveIpInAllLocalNetworks(
            IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            var localIpList = GetLocalIPv4Addresses();

            List<string> allNetworkClientAddresses = [];
            foreach (var localIp in localIpList)
            {
                string network = localIp.Address + "/" + localIp.PrefixLength;
                var ipNetwork = IPNetwork2.Parse(network);
                var networkClientAddresses = ipNetwork.ListIPAddress(Filter.Usable)
                    .Select(ip => ip.ToString());
                allNetworkClientAddresses.AddRange(networkClientAddresses);
            }

            int totalCount = allNetworkClientAddresses.Count;
            if (totalCount == 0)
                return [];

            int scannedCount = 0;
            ConcurrentBag<string> activeClients = [];
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = MaxParallelPings,
                CancellationToken = cancellationToken
            };

            try
            {
                await Parallel.ForEachAsync(allNetworkClientAddresses, parallelOptions, async (ip, ct) =>
                {
                    using Ping ping = new();
                    PingReply reply = await ping.SendPingAsync(ip, 250);
                    if (reply.Status == IPStatus.Success)
                        activeClients.Add(ip);

                    int current = Interlocked.Increment(ref scannedCount);
                    progress?.Report((double)current / totalCount);
                });
            }
            catch (OperationCanceledException)
            {
                // Scanning cancelled — return whatever was found so far
            }

            return activeClients.ToArray();
        }
    }
}
