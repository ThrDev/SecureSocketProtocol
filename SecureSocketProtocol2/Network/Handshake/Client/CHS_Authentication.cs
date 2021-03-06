﻿using SecureSocketProtocol2.Network.Messages;
using SecureSocketProtocol2.Network.Messages.TCP;
using SecureSocketProtocol2.Network.Messages.TCP.Handshake;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureSocketProtocol2.Network.Handshake.Client
{
    class CHS_Authentication : Handshake
    {
        private ClientProperties Properties;
        public CHS_Authentication(SSPClient client, ClientProperties Properties)
            : base(client)
        {
            this.Properties = Properties;
        }

        public override HandshakeType[] ServerTypes
        {
            get
            {
                return new HandshakeType[]
                {
                    HandshakeType.ReceiveMessage,
                    HandshakeType.SendMessage
                };
            }
        }

        public override HandshakeType[] ClientTypes
        {
            get
            {
                return new HandshakeType[]
                {
                    HandshakeType.SendMessage,
                    HandshakeType.ReceiveMessage
                };
            }
        }

        public override bool onHandshake()
        {
            SyncObject syncObject = null;
            if ((Properties.Username != null && Properties.Username.Length > 0) &&
                (Properties.Password != null && Properties.Password.Length > 0))
            {
                base.SendMessage(new MsgAuthentication(Properties.Username, Properties.Password));
                if (!(syncObject = base.ReceiveMessage((IMessage message) =>
                {
                    MsgAuthenticationSuccess authResponse = message as MsgAuthenticationSuccess;

                    if (authResponse != null)
                    {
                        return authResponse.Success;
                    }
                    return false;
                })).Wait<bool>(false, 30000))
                {
                    Client.Disconnect(DisconnectReason.TimeOut);
                    Client.onException(new Exception("Handshake went wrong, CHS_Authentication"), ErrorType.Core);
                    if (syncObject.TimedOut)
                        throw new TimeoutException(TimeOutMessage);
                    throw new Exception("Username or Password is incorrect");
                }
            }
            else
            {
                //no authentication
                base.SendMessage(new MsgDummy());
                if (!(syncObject = base.ReceiveMessage((IMessage message) =>
                {
                    return message as MsgDummy != null;
                })).Wait<bool>(false, 30000))
                {
                    Client.Disconnect(DisconnectReason.TimeOut);
                    Client.onException(new Exception("Handshake went wrong, CHS_Authentication"), ErrorType.Core);
                    if (syncObject.TimedOut)
                        throw new TimeoutException(TimeOutMessage);
                    throw new Exception(OutOfSyncMessage);
                }
            }

            try
            {
                //at this point you could use extra keyfiles or other security measures
                Client.onAuthenticated();
            }
            catch (Exception ex)
            {
                Client.onException(ex, ErrorType.Core);
            }

            return true;
        }
    }
}