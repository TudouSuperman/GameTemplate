using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameApp
{
    public static class NetworkUtility
    {
        /// <summary>
        /// 检查基本网络连接状态。
        /// </summary>
        /// <returns>网络是否可用。</returns>
        public static bool IsNetworkAvailable()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }
}