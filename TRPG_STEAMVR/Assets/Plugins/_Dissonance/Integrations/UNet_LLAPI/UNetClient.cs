using System;
using Dissonance.Networking;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_LLAPI
{
    public class UNetClient
        : BaseClient<UNetServer, UNetClient, int>
    {
        private readonly UNetCommsNetwork _network;
        private readonly ClientConnectionDetails _server;

        private readonly byte[] _readBuffer = new byte[1024];

        private bool _connectionEstablished;
        private int _socket = -1;
        private int _connection = -1;

        public UNetClient([NotNull] UNetCommsNetwork network, ClientConnectionDetails server)
            : base(network)
        {
            _network = network;
            _server = server;
        }

        public override void Connect()
        {
            byte error;
            _socket = NetworkTransport.AddHost(_network.Topology);
            _connection = NetworkTransport.Connect(_socket, _server.Address, _server.Port, 0, out error);

            if (error == (int) NetworkError.Ok)
                _connectionEstablished = true;
            else
                FatalError(string.Format("Failed to connect to Dissonance server on port {0}, Error {1}", _server.Port, (NetworkError)error));
        }

        public override void Disconnect()
        {
            base.Disconnect();

            if (_connectionEstablished)
            {
                byte error;
                NetworkTransport.Disconnect(_socket, _connection, out error);
                if (error != (int)NetworkError.Ok)
                    Log.Error("Failed to cleanly disconnect from Dissonance server at {0}:{1}, Error {2}", _server.Address, _server.Port, (NetworkError)error);

                NetworkTransport.RemoveHost(_socket);

                _connectionEstablished = false;
            }
        }

        protected override void ReadMessages()
        {
            if (!_connectionEstablished)
            {
                Log.Warn("Attempted to read messages from an invalid socket");
                return;
            }

            NetworkEventType eventType;

            do
            {
                int senderConnectionId;
                int channelId;
                int dataSize;
                byte error;

                eventType = NetworkTransport.ReceiveFromHost(_socket, out senderConnectionId, out channelId, _readBuffer, _readBuffer.Length, out dataSize, out error);

                if (error != 0)
                {
                    if (IsFatalError((NetworkError)error))
                        FatalError(string.Format("Fatal error encountered receiving packet: {0}", (NetworkError)error));
                    else
                        Log.Debug("Non fatal error reading from client socket: {0}", (NetworkError)error);
                }
                else
                {
                    switch (eventType)
                    {
                        case NetworkEventType.DataEvent:
                            NetworkReceivedPacket(new ArraySegment<byte>(_readBuffer, 0, dataSize));
                            break;

                        case NetworkEventType.ConnectEvent:
                            Connected();
                            break;

                        case NetworkEventType.DisconnectEvent:
                            Disconnect();
                            break;

                        case NetworkEventType.Nothing:
                        case NetworkEventType.BroadcastEvent:
                            break;

                        default:
                            Log.Error("Received unknown network event '{0}'", eventType);
                            break;
                    }
                }
            } while (eventType != NetworkEventType.Nothing && IsConnected);
        }

        private void Send(int channel, ArraySegment<byte> packet)
        {
            if (!_connectionEstablished)
            {
                Log.Warn("Attempted to send message to an invalid socket");
                return;
            }

            var result = UNetCommsNetwork.Send(_socket, _connection, channel, packet);
            if (result == NetworkError.Ok)
                return;

            if (IsFatalError(result))
                FatalError(string.Format("Fatal error encountered sending packet: {0}", result));
        }

        protected override void SendReliable(ArraySegment<byte> packet)
        {
            Send(_network.SystemMessagesChannel, packet);
        }

        protected override void SendUnreliable(ArraySegment<byte> packet)
        {
            Send(_network.VoiceDataChannel, packet);
        }

        private bool IsFatalError(NetworkError error)
        {
            switch (error)
            {
                case NetworkError.WrongHost:
                case NetworkError.WrongConnection:
                case NetworkError.WrongChannel:
                case NetworkError.NoResources:
                case NetworkError.Timeout:
                case NetworkError.VersionMismatch:
                case NetworkError.DNSFailure:
                case NetworkError.CRCMismatch:
                case NetworkError.BadMessage:
                case NetworkError.UsageError:
                    return true;

                case NetworkError.Ok:
                case NetworkError.MessageToLong:
                case NetworkError.WrongOperation:
                    return false;

                default:
                    Log.CreatePossibleBugException(string.Format("Dissonance UNet received unknown NetworkError: '{0}'", error), "BF18ADA4-CDC0-43F6-B99D-DC52E9A991F4");
                    return true;
            }
        }
    }
}
