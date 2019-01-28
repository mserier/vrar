using System;
using Dissonance.Networking;
using UnityEngine.Networking;

namespace Dissonance.Integrations.UNet_LLAPI
{
    public class UNetServer
        : BaseServer<UNetServer, UNetClient, int>
    {
        private readonly UNetCommsNetwork _network;
        private readonly ServerConnectionDetails _connection;

        private readonly byte[] _receiveBuffer = new byte[1024];

        private bool _socketValid;
        private int _socket = -1;

        public UNetServer(UNetCommsNetwork network, ServerConnectionDetails connection)
        {
            _network = network;
            _connection = connection;
        }

        public override void Connect()
        {
            base.Connect();

            _socket = NetworkTransport.AddHost(_network.Topology, _connection.Port);
            if (_socket == -1)
                throw new DissonanceException(string.Format("Failed to create Dissonance server on port '{0}', (port may already be in use)", _connection.Port));
            
            _socketValid = true;
        }

        public override void Disconnect()
        {
            base.Disconnect();

            if (_socketValid)
            {
                NetworkTransport.RemoveHost(_socket);
                _socketValid = false;
            }
        }

        protected override void ReadMessages()
        {
            if (!_socketValid)
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

                eventType = NetworkTransport.ReceiveFromHost(_socket, out senderConnectionId, out channelId, _receiveBuffer, _receiveBuffer.Length, out dataSize, out error);

                var nErr = (NetworkError)error;
                if (nErr != NetworkError.Ok)
                {
                    if (nErr == NetworkError.Timeout)
                        ClientDisconnected(senderConnectionId);
                    else if (IsFatalError(nErr))
                    {
                        ClientDisconnected(senderConnectionId);
                        Log.Error("Fatal error encountered receiving packet: {0}", nErr);
                    }
                    else
                        Log.Debug("Non fatal error reading from client socket: {0}", nErr);
                }
                else
                {
                    switch (eventType)
                    {
                        case NetworkEventType.DataEvent:
                            NetworkReceivedPacket(senderConnectionId, new ArraySegment<byte>(_receiveBuffer, 0, dataSize));
                            break;

                        case NetworkEventType.DisconnectEvent:
                            ClientDisconnected(senderConnectionId);
                            break;

                        case NetworkEventType.ConnectEvent:
                        case NetworkEventType.Nothing:
                        case NetworkEventType.BroadcastEvent:
                            break;

                        default:
                            Log.Error("Dissonance UNet received unknown event: '{0}'", eventType);
                            break;
                    }
                }

            } while (eventType != NetworkEventType.Nothing);
        }

        private void Send(int connection, int channel, ArraySegment<byte> packet)
        {
            if (!_socketValid)
            {
                Log.Warn("Attempted to send message to an invalid socket");
                return;
            }

            var result = UNetCommsNetwork.Send(_socket, connection, channel, packet);
            if (result == NetworkError.Ok)
                return;

            if (IsFatalError(result))
                FatalError(string.Format("Fatal error encountered sending packet: {0}", result));
        }

        protected override void SendReliable(int connection, ArraySegment<byte> packet)
        {
            Send(connection, _network.SystemMessagesChannel, packet);
        }

        protected override void SendUnreliable(int connection, ArraySegment<byte> packet)
        {
            Send(connection, _network.VoiceDataChannel, packet);
        }

        private bool IsFatalError(NetworkError error)
        {
            switch (error)
            {
                case NetworkError.WrongHost:
                case NetworkError.WrongConnection:
                case NetworkError.WrongChannel:
                case NetworkError.NoResources:
                case NetworkError.VersionMismatch:
                case NetworkError.DNSFailure:
                case NetworkError.CRCMismatch:
                case NetworkError.BadMessage:
                case NetworkError.UsageError:
                    return true;

                case NetworkError.Timeout:          // Timeout is not considered an error for the server. A client can timeout without killing the server.
                case NetworkError.Ok:
                case NetworkError.MessageToLong:
                case NetworkError.WrongOperation:
                    return false;

                default:
                    Log.CreatePossibleBugException(string.Format("Dissonance UNet received unknown NetworkError: '{0}'", error), "7975E556-3821-4360-8C60-31E9C03C4C4F");
                    return true;
            }
        }
    }
}
