using System;
using Dissonance.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_LLAPI
{
    public struct ClientConnectionDetails
    {
        public string Address { get; set; }
        public int Port { get; set; }
    }

    public struct ServerConnectionDetails
    {
        public int Port { get; set; }
    }

    [HelpURL("https://placeholder-software.co.uk/dissonance/docs/Basics/Quick-Start-UNet-LLAPI/")]
    public class UNetCommsNetwork
        : BaseCommsNetwork<UNetServer, UNetClient, int, ClientConnectionDetails, ServerConnectionDetails>
    {
        public string ServerAddress { get; private set; }

        [SerializeField]private int _maxConnections = 64;
        public int MaxConnections
        {
            get { return _maxConnections; }
        }

        private readonly int _voiceChannel;
        public int VoiceDataChannel
        {
            get { return _voiceChannel; }
        }

        private readonly int _sysChannel;
        public int SystemMessagesChannel
        {
            get { return _sysChannel; }
        }

        [SerializeField]private int _port = 5889;
        public ushort Port
        {
            get { return (ushort)_port; }
            set
            {
                if (Status != ConnectionStatus.Disconnected)
                    Log.Warn("Port changed while network is active. The network must be restarted for this change to be applied.");

                _port = value;
            }
        }

        private readonly HostTopology _topology;
        [NotNull] internal HostTopology Topology
        {
            get { return _topology; }
        }

        // ReSharper disable once FieldCanBeMadeReadOnly.Local (Justification: Used by editor)
        // ReSharper disable once ConvertToConstant.Local (Justification: Used by editor)
        [SerializeField, UsedImplicitly]private bool _disableNetworkLifetimeManagement;

        public UNetCommsNetwork()
        {
            var config = new ConnectionConfig();
            _voiceChannel = config.AddChannel(QosType.Unreliable);
            _sysChannel = config.AddChannel(QosType.ReliableSequenced);
            _topology = new HostTopology(config, MaxConnections);
        }

        protected override UNetServer CreateServer(ServerConnectionDetails details)
        {
            return new UNetServer(this, details);
        }

        protected override UNetClient CreateClient(ClientConnectionDetails details)
        {
            return new UNetClient(this, details);
        }

        protected override void Initialize()
        {
            if (!_disableNetworkLifetimeManagement)
                NetworkTransport.Init();
        }

        private void OnDestroy()
        {
            if (!_disableNetworkLifetimeManagement)
                NetworkTransport.Shutdown();
        }

        public void InitializeAsDedicatedServer()
        {
            RunAsDedicatedServer(new ServerConnectionDetails {
                Port = _port
            });
        }

        public void InitializeAsServer()
        {
            ServerAddress = "127.0.0.1";

            RunAsHost(
                new ServerConnectionDetails {
                    Port = _port
                },
                new ClientConnectionDetails {
                    Address = ServerAddress,
                    Port = _port
                }
            );
        }

        public void InitializeAsClient(string serverAddress)
        {
            // UNet doesn't like "localhost"
            if (serverAddress.Equals("localhost", StringComparison.InvariantCultureIgnoreCase))
                serverAddress = "127.0.0.1";
            ServerAddress = serverAddress;

            RunAsClient(new ClientConnectionDetails {
                Address = ServerAddress,
                Port = _port
            });
        }

        internal static NetworkError Send(int socket, int connection, int channel, ArraySegment<byte> packet)
        {
            if (packet.Offset != 0)
                throw new ArgumentException("non-zero packet offset");

            byte error;
            var success = NetworkTransport.Send(socket, connection, channel, packet.Array, packet.Count, out error);

            return success
                 ? NetworkError.Ok
                 : (NetworkError)error;
        }
    }
}
