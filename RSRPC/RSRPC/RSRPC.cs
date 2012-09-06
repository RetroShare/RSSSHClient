﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

using ProtoBuf;

using rsctrl.chat;
using rsctrl.core;
//using rsctrl.files;
//using rsctrl.gxs;
//using rsctrl.msgs;
using rsctrl.peers;
using rsctrl.system;

namespace Sehraf.RSRPC
{
    //public enum CallbackType
    //{
    //    Disconnect = 0
    //}

    public class RSRPC
    {
        public delegate void ReceivedMsgEvent(RSProtoBuffSSHMsg msg);
        public delegate void ErrorOccurredEvent(Exception e);

        RSSSHConnector _rsConnector;
        RSProtoBuf _rsProtoBuf;
        bool _connected;
        
        event ReceivedMsgEvent _receivedMsg;
        event ErrorOccurredEvent _error;
        Queue<RSProtoBuffSSHMsg> _sendQueue;

        public RSSSHConnector RSConnector { get { return _rsConnector; } }
        public RSProtoBuf RSProtoBuf { get { return _rsProtoBuf; } }
        public bool IsConnected { get { return _connected; } }
        public ReceivedMsgEvent ReceivedMsg { get { return _receivedMsg; } set { _receivedMsg = value; } }
        public ErrorOccurredEvent ErrorOccurred { get { return _error; } set { _error = value; } }

        public RSRPC()
        {
            _connected = false;
            _sendQueue = new Queue<RSProtoBuffSSHMsg>();
        }

        public bool Connect(string host, ushort port, string user, string pw)
        {
            if (!_connected)
            {
                _sendQueue.Clear();
                _rsConnector = new RSSSHConnector(host, port, user, pw);
                if (_rsConnector.Connect())
                {
                    _rsProtoBuf = new RSProtoBuf(_rsConnector.Stream, _sendQueue, this);
                    _connected = true;
                    return true;
                }
            }
            return false;
        }

        public void Disconnect()
        {
            if (_connected)
            {
                _connected = false;
                _rsProtoBuf.Stop();
                System.Threading.Thread.Sleep(250);
                _rsProtoBuf.BreakConnection();
                _rsProtoBuf = null;
                _rsConnector.Disconnect();
                _rsConnector = null;
            }
        }

        internal void Error(Exception e)
        {
            _error(e);
        }

        internal void ProcessMsg(RSProtoBuffSSHMsg msg)
        {
            if(_connected)
                _receivedMsg(msg);
        }

        // ---------- send ----------
        // ---------- generic send<T> ----------
        public uint Send<T>(T pbMsg, uint inMsgID)
        {
            RSProtoBuffSSHMsg msg = new RSProtoBuffSSHMsg();
            msg.MsgID = inMsgID;
            msg.ReqID = _rsProtoBuf.GetReqID();
            msg.ProtoBuffMsg = new MemoryStream();
            Serializer.Serialize<T>(msg.ProtoBuffMsg, pbMsg);
            msg.ProtoBuffMsg.Position = 0;
            msg.BodySize = (uint)msg.ProtoBuffMsg.Length;

            lock (_sendQueue)
                _sendQueue.Enqueue(msg);

            return msg.ReqID;
        }

        #region chat
        public uint GetChatLobbies(RequestChatLobbies.LobbySet type)
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.CHAT,
                    (byte)rsctrl.chat.RequestMsgIds.MsgId_RequestChatLobbies,
                    false
                );

            RequestChatLobbies request = new RequestChatLobbies();
            request.lobby_set = type;
            return Send<RequestChatLobbies>(request, msgID);
        }

        public uint CreateLobby(string name, string topic, LobbyPrivacyLevel privacy)
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.CHAT,
                    (byte)rsctrl.chat.RequestMsgIds.MsgId_RequestCreateLobby,
                    false
                );

            RequestCreateLobby request = new RequestCreateLobby();
            //request.invited_friends = ... // what strings?
            request.lobby_name = name;
            request.lobby_topic = topic;
            request.privacy_level = privacy;
            return Send<RequestCreateLobby>(request, msgID);
        }

        public uint JoinLeaveLobby(RequestJoinOrLeaveLobby.LobbyAction action, string lobbyID)
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.CHAT,
                    (byte)rsctrl.chat.RequestMsgIds.MsgId_RequestJoinOrLeaveLobby,
                    false
                );

            RequestJoinOrLeaveLobby request = new RequestJoinOrLeaveLobby();
            request.action = action;
            request.lobby_id = lobbyID;
            return Send<RequestJoinOrLeaveLobby>(request, msgID);
        }

        public uint RegisterEvent(RequestRegisterEvents.RegisterAction action)
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.CHAT,
                    (byte)rsctrl.chat.RequestMsgIds.MsgId_RequestRegisterEvents,
                    false
                );

            RequestRegisterEvents request = new RequestRegisterEvents();
            request.action = action;
            return Send<RequestRegisterEvents>(request, msgID);
        }

        public uint SendMsg(ChatMessage msg)
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.CHAT,
                    (byte)rsctrl.chat.RequestMsgIds.MsgId_RequestSendMessage,
                    false
                );

            RequestSendMessage request = new RequestSendMessage();
            request.msg = msg;
            return Send<RequestSendMessage>(request, msgID);
        }

        public uint SetLobbyNickname(string name, List<string> lobbyIDs = null)
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.CHAT,
                    (byte)rsctrl.chat.RequestMsgIds.MsgId_RequestSetLobbyNickname,
                    false
                );

            RequestSetLobbyNickname request = new RequestSetLobbyNickname();
            if(lobbyIDs != null)
                request.lobby_ids.AddRange(lobbyIDs);
            request.nickname = name;
            return Send<RequestSetLobbyNickname>(request, msgID);
        }
        #endregion

        #region files
        #endregion

        #region gxs
        #endregion

        #region msgs
        #endregion

        #region peers
        public uint AddPeer(string cert, string gpgID)
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.PEERS,
                    (byte)rsctrl.peers.RequestMsgIds.MsgId_RequestAddPeer,
                    false
                );

            RequestAddPeer request = new RequestAddPeer();
            request.cert = cert;
            request.gpg_id = gpgID;
            request.cmd = RequestAddPeer.AddCmd.ADD;
            return Send<RequestAddPeer>(request, msgID);
        }

        public uint ModifyPeer(Person peer, RequestModifyPeer.ModCmd cmd)
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.PEERS,
                    (byte)rsctrl.peers.RequestMsgIds.MsgId_RequestModifyPeer,
                    false
                );

            RequestModifyPeer request = new RequestModifyPeer();
            request.peers.Add(peer);
            request.cmd = cmd;
            return Send<RequestModifyPeer>(request, msgID);
        }

        public uint GetFriendList(RequestPeers.SetOption option)
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.PEERS,
                    (byte)rsctrl.peers.RequestMsgIds.MsgId_RequestPeers,
                    false
                );

            RequestPeers request = new RequestPeers();
            request.info = RequestPeers.InfoOption.ALLINFO;
            request.set = option;
            return Send<RequestPeers>(request, msgID);
        }
        #endregion

        #region system
        public uint GetSystemStatus()
        {
            uint msgID = RSProtoBuf.ConstructMsgId(
                    (byte)ExtensionId.CORE,
                    (ushort)PackageId.SYSTEM,
                    (byte)rsctrl.system.RequestMsgIds.MsgId_RequestSystemStatus,
                    false
                );

            RequestSystemStatus request = new RequestSystemStatus();
            return Send<RequestSystemStatus>(request, msgID);
        }
        #endregion
        
    }
}