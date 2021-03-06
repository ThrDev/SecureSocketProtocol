﻿using SecureSocketProtocol2;
using SecureSocketProtocol2.Attributes;
using SecureSocketProtocol2.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Server.LiteCode
{
    public class SharedTest : ISharedTest
    {
        public SharedTest()
        {

        }

        [RemoteExecutionAttribute(0, null)]
        public void CallTest()
        {
            //Console.WriteLine("CallTest is called!");
        }

        [RemoteExecutionAttribute(0, null)]
        public string StringTest()
        {
            return "Some random message";
        }

        [RemoteExecutionAttribute(0, null)]
        public int IntegerTest()
        {
            return 1337;
        }

        [RemoteExecution(0, null)]
        public void SecretShit()
        {

        }

        [RemoteExecution(0, null)]
        public byte[] ByteArrayTest()
        {
            return new byte[60000];
        }

        [RemoteExecution(0, null)]
        public void DelegateTest(Callback<string> Delly)
        {
            //string RetStr = Delly("Message from server");
            //Console.WriteLine("Server received a string from client's private method: " + RetStr);
        }

        Stopwatch sw = Stopwatch.StartNew();
        int speed = 0;
        int CallsASec = 0;

        [UncheckedRemoteExecution]
        public void SendByteArray(byte[] data)
        {
            speed += data.Length;
            CallsASec++;

            if (sw.ElapsedMilliseconds>=1000)
            {
                Console.WriteLine("Call Speed: " + CallsASec + ", Speed: " + Math.Round(((float)speed / 1000F) / 1000F, 2) + "MBps ");
                speed = 0;
                CallsASec = 0;
                sw = Stopwatch.StartNew();
            }
        }
    }
}
