﻿using SecureSocketProtocol2;
using SecureSocketProtocol2.Misc;
using SecureSocketProtocol2.Network;
using SecureSocketProtocol2.Network.Messages;
using SecureSocketProtocol2.Network.Protections;
using SecureSocketProtocol2.Network.Protections.Compression;
using SecureSocketProtocol2.Plugin;
using Server.LiteCode;
using Server.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Client : SSPClient
    {
        ulong Received = 0;
        int PacketsPerSec = 0;
        Stopwatch sw = Stopwatch.StartNew();
        PayloadWriter pw = new PayloadWriter();
        Stopwatch speedSW = Stopwatch.StartNew();

        private List<SecureStream> streams = new List<SecureStream>();

        public Client()
            : base(typeof(TestChannel), new object[0], true)
        {

        }

        public override void onClientConnect()
        {
            Console.WriteLine("Client accepted");
            base.MessageHandler.AddMessage(typeof(TestMessage), "TEST_MESSAGE");

            while(false)
            {
                Console.Clear();
                Console.WriteLine("Streams:" + streams.Count);

                for (int i = 0; i < streams.Count; i++)
                {
                    Console.WriteLine("Streams[" + i + "] size buffer " + streams[i].Length);
                }

                Thread.Sleep(1000);
            }

            /*while (true)
            {
                Console.WriteLine("Server Time: " + DateTime.Now.Hour.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2") + ":" + DateTime.Now.Second.ToString("D2") + ", " + DateTime.Now.Millisecond);
                Thread.Sleep(1000);
            }*/
        }

        public override void onValidatingComplete()
        {
            Console.WriteLine("Validating connection...");
        }

        public override void onDisconnect(DisconnectReason Reason)
        {
            Console.WriteLine("Client disconnected");
        }

        public override void onKeepAlive()
        {
            Console.WriteLine("Received keep-alive");
        }

        public override void onException(Exception ex, ErrorType errorType)
        {

        }

        public override void onReconnect()
        {

        }

        public override void onNewChannelOpen(Channel channel)
        {

        }

        public override bool onVerifyCertificate(CertInfo certificate)
        {
            return true;
        }

        public override IPlugin[] onGetPlugins()
        {
            return new IPlugin[]
            {

            };
        }

        public override void onAddProtection(Protection protection)
        {
            protection.AddProtection(new QuickLzProtection());
        }

        public override uint HeaderJunkCount
        {
            get { return 5; }
        }
        public override uint PrivateKeyOffset
        {
            get { return 45634232; }
        }

        public override void onAuthenticated()
        {

        }

        public override void onNewStreamOpen(SecureStream stream)
        {
            streams.Add(stream);

            stream.AutoFlush = true;
            byte[] data = File.ReadAllBytes(@"C:\Users\DragonHunter\Desktop\ss (2013-05-22 at 09.30.58).jpg");
            stream.Write(data, 0, data.Length);
        }

        public override void onShareClasses()
        {
            base.ShareClass("SharedTest", typeof(SharedTest));
        }

        public override bool onPeerConnectionRequest(SecureSocketProtocol2.Network.RootSocket.RootPeer peer)
        {
            //should never happen at server side
            Console.WriteLine("onPeerConnectionRequest got executed which should never happen... strange");
            return false;
        }

        public override SecureSocketProtocol2.Network.RootSocket.RootPeer onGetNewPeerObject()
        {
            return new Peer();
        }
    }
}