﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Classlibary;
using System.Text.Json;
namespace SocketControl
{
    public class SocketHelper
    {
        public Ball BallRef;

        bool Initialized = false;
        Socket socketClient = null;
        Socket socketServer = null;
        IPEndPoint iep = null;
        IPEndPoint iep_Receive = null;
        private EndPoint ep_sever = null;
        private byte[] byteSendingArray = new byte[100000];
        private byte[] byteReceiveArray = new byte[100000];
        public SocketHelper()
        {
            iep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1001); 
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); 
            socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        public void Init(Ball ball)
        {
            BallRef = new Ball();
            BallRef.self = new little_ball();
            BallRef.Other_ID = new Dictionary<string, little_ball>();
            BallRef.little_balls = new List<little_ball>();
            BallRef.self = ball.self;
            BallRef.little_balls = ball.little_balls;
            BallRef.Other_ID = ball.Other_ID;
            byteSendingArray = new byte[10000];

            EndPoint ep = (EndPoint)iep;
            string jsonstring = JsonSerializer.Serialize(BallRef);
            byteSendingArray = Encoding.UTF8.GetBytes(jsonstring);
            socketClient.SendTo(byteSendingArray, ep);

            iep_Receive = new IPEndPoint(IPAddress.Parse("127.0.0.1"), ((IPEndPoint)socketClient.LocalEndPoint).Port);
            socketServer.Bind(iep_Receive);
            Initialized = true;
        }
        public void Send(ref Ball b_tmp)
        {
            BallRef = b_tmp;
            if (!Initialized)
                return;
            EndPoint ep = (EndPoint)iep;
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonstring = JsonSerializer.Serialize(BallRef, options);
            byteSendingArray = Encoding.UTF8.GetBytes(jsonstring);
            socketClient.SendTo(byteSendingArray, ep);
        }
        public string Receive()
        {
            if (!Initialized)
                return "";
            //接受收據
            EndPoint ep = (EndPoint)iep_Receive;
            socketServer.ReceiveTimeout = 1000;
            try
            {
                int intReceiveLenght = socketServer.ReceiveFrom(byteReceiveArray, ref ep);
                return Encoding.UTF8.GetString(byteReceiveArray, 0, intReceiveLenght);
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}