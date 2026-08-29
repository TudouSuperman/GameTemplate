using System.Net;
using System.Threading;
using GameFramework.Network;
using UnityGameFramework.Extension;
using Cysharp.Threading.Tasks;

namespace GameApp
{
    public class NetworkServiceHelper : INetworkServiceHelper
    {
        public int State { get; private set; }

        // private IWebSocketChannel m_WebSocketChannel;
        private INetworkChannel m_NetworkChannel;

        public void OnInitialize()
        {
            // m_WebSocketChannel = GameEntry.WebSocket.CreateWebSocketChannel("WebSocket", new WebSocketChannelHelper());
            m_NetworkChannel = GameEntry.Network.CreateNetworkChannel("Socket", ServiceType.Tcp, new NetworkChannelHelper());
        }

        public void OnShutdown()
        {
            // GameEntry.WebSocket.DestroyWebSocketChannel(m_WebSocketChannel.Name);
            // m_WebSocketChannel = null;
            GameEntry.Network.DestroyNetworkChannel(m_NetworkChannel.Name);
            m_NetworkChannel = null;
        }

        public void Connect(object userData)
        {
            // m_WebSocketChannel.Connect("wss://echo.websocket.events");
            m_NetworkChannel.Connect(IPAddress.Parse("127.0.0.1"), 8098);
        }

        public void Disconnect(object userData)
        {
            // m_WebSocketChannel.Close();
            m_NetworkChannel.Close();
        }

        public void Send<T>(T packet, object userData) where T : Packet
        {
            // m_WebSocketChannel.Send(packet);
            m_NetworkChannel.Send(packet);
        }

        public UniTask<T2> SendAsync<T1, T2>(T1 packet, object userData, CancellationToken cancellationToken) where T1 : Packet where T2 : Packet
        {
            throw new System.NotImplementedException();
        }

        public void OnConnected(object channel)
        {
        }

        public void OnDisconnected(object channel)
        {
        }

        public void OnMissHeartBeat(object channel)
        {
        }

        public void OnError(object channel, string errorMessage)
        {
        }

        public void OnCustomError(object channel, string customErrorData)
        {
        }
    }
}