﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

using rsctrl.chat;
using rsctrl.core;

namespace RetroShareSSHClient
{
    struct GuiChatLobby
    {
        ChatLobbyInfo _lobby;
        bool _joined;
        bool _unread;
        ushort _listIndex;
        string _chatText;
        List<string> _chatUser;

        public ChatLobbyInfo Lobby { get { return _lobby; } set { _lobby = value; } }
        public string ID { get { return _lobby.lobby_id; } }
        public bool Joined { get { return _joined; } set { _joined = value; } }
        public bool Unread { get { return _unread; } set { _unread = value; } }
        public ushort Index { get { return _listIndex; } set { _listIndex = value; } }
        public string ChatText { get { return _chatText; } set { _chatText = value; } }
        public List<string> ChatUser { get { return _chatUser; } set { _chatUser = value; } }
    }

    class ChatProcessor
    {
        const bool DEBUG = false;
        public const string GROUPCHAT = "%groupChat%";

        Dictionary<string, GuiChatLobby> _chatLobbies;
        bool _isRegistered;

        /// <summary>
        /// says if we have to redraw the chat (e.g. a new msg has arrived)
        /// </summary>
        bool _reDrawChat;
        string _nick;

        Bridge _b;

        public string Nick { get { return _nick; } }

        public ChatProcessor()
        {
            _b = Bridge.GetBridge();

            _chatLobbies = new Dictionary<string, GuiChatLobby>();
            _isRegistered = false;
            _reDrawChat = false;
        }

        public void Reset(bool justResetChatRegistration = false)
        {
            if (!justResetChatRegistration)
            {
                _chatLobbies.Clear();
                _b.GUI.clb_chatLobbies.Items.Clear();
                _b.GUI.clb_chatUser.Items.Clear();
                _b.GUI.rtb_chat.Clear();
            }
            _isRegistered = false;
        }
        
        public void UpdateChatLobbies(ResponseChatLobbies msg)
        {
            System.Diagnostics.Debug.WriteLineIf(DEBUG, "lobbyList "+ msg.lobbies.Count);
            foreach (ChatLobbyInfo lobby in msg.lobbies)
                ProcessLobby(lobby);

            CheckChatRegistration();
        }

        public void LobbyInvite(EventLobbyInvite msg)
        {
            System.Diagnostics.Debug.WriteLineIf(DEBUG, "-> invited");
            ProcessLobby(msg.lobby);
            CheckChatRegistration();
        }

        private void ProcessLobby(ChatLobbyInfo lobby)
        {
            string ID = lobby.lobby_id, nameToShow = "";
            GuiChatLobby cl = new GuiChatLobby();

            nameToShow = 
                    "[" + (lobby.no_peers <= 9 ? "0" : "") + lobby.no_peers + "] " + 
                    lobby.lobby_name +
                    // when there is no topic don't add "-"
                    (lobby.lobby_topic != "" ? " - " + lobby.lobby_topic : "");

            if (_chatLobbies.ContainsKey(ID)) //update
            {
                System.Diagnostics.Debug.WriteLineIf(DEBUG, "updating lobby " + lobby.lobby_name + " - state " + lobby.lobby_state);
                System.Diagnostics.Debug.WriteLineIf(DEBUG, "user: " + lobby.no_peers + " - names: " + lobby.nicknames.Count + " - friends: " + lobby.participating_friends.Count);
                cl = _chatLobbies[ID];
                cl.Lobby = lobby;
                cl.ChatUser = lobby.participating_friends;
                _chatLobbies[ID] = cl;
                _b.GUI.clb_chatLobbies.Items[cl.Index] = nameToShow;

                if (lobby.lobby_state == ChatLobbyInfo.LobbyState.LOBBYSTATE_INVITED && !cl.Joined)
                    _b.GUI.clb_chatLobbies.SetItemCheckState(cl.Index, CheckState.Indeterminate);
            }
            else //new 
            {
                System.Diagnostics.Debug.WriteLineIf(DEBUG, "adding lobby " + lobby.lobby_name + " - state " + lobby.lobby_state);
                System.Diagnostics.Debug.WriteLineIf(DEBUG, "user: " + lobby.no_peers + " - names: " + lobby.nicknames.Count + " - friends: " + lobby.participating_friends.Count);
                cl.Lobby = lobby;
                cl.Index = (ushort)_b.GUI.clb_chatLobbies.Items.Count;
                cl.Joined = (lobby.lobby_state == ChatLobbyInfo.LobbyState.LOBBYSTATE_JOINED);
                cl.Unread = false;
                cl.ChatText = "======= " + DateTime.Now.ToLongDateString() + " - " + lobby.lobby_name + " =======\n";
                cl.ChatUser = lobby.participating_friends;
                
                _b.GUI.clb_chatLobbies.Items.Add(nameToShow, cl.Joined);
                if (lobby.lobby_state == ChatLobbyInfo.LobbyState.LOBBYSTATE_INVITED)
                    _b.GUI.clb_chatLobbies.SetItemCheckState(cl.Index, CheckState.Indeterminate);                

                _chatLobbies.Add(cl.ID, cl);
            }
        }

        private void CheckChatRegistration()
        {
            /*
             * If we register to early the server may send us messages from lobbies we don't know yet.
             * AddGroupChat() will call this function before we requested the lobby list.
             * If we don't get any lobby after 2 minutes ( +5 secs for answer) then we don't reach any other lobby -> we can register
             * otherwise the server will send the lobbies after the first request ( if participating ) or a bit later
             */

            if (_chatLobbies.Count <= 1 && _b.GUI.TickCounter < 125)
                return;

            bool joined = false;
            Dictionary<string, GuiChatLobby>.ValueCollection Lobbies = _chatLobbies.Values;
            foreach (GuiChatLobby lobby in Lobbies)
            {
                if (lobby.Joined)
                {
                    joined = true;
                    break;
                }
            }

            if (joined && !_isRegistered)
            {
                _b.RPC.ChatRegisterEvent(RequestRegisterEvents.RegisterAction.REGISTER);
                _b.GUI.tb_out.AppendText("Registered" + "\n");
                _isRegistered = true;
            }

            if (!joined && _isRegistered)
            {
                _b.RPC.ChatRegisterEvent(RequestRegisterEvents.RegisterAction.DEREGISTER);
                _b.GUI.tb_out.AppendText("Unregistered" + "\n");
                _isRegistered = false;
            }
        }

        public bool GetLobbyByListIndex(int index, out GuiChatLobby lobby)
        {
            GuiChatLobby[] values = new GuiChatLobby[_chatLobbies.Values.Count];
            _chatLobbies.Values.CopyTo(values, 0);
            foreach (GuiChatLobby l in values)
            {
                if (l.Index == index)
                {
                    lobby = l;
                    return true;
                }
            }
            lobby = default(GuiChatLobby);
            return false;
        }

        //public void SendChatMsg(string inMsg = "")
        //{
        //    int index = _b.GUI.clb_chatLobbies.SelectedIndex;
        //    if (index >= 0 && _b.GUI.clb_chatLobbies.GetItemChecked(index))
        //    {
        //        GuiChatLobby cl = new GuiChatLobby();
        //        if (GetLobbyByListIndex(index, out cl))
        //        {
        //            string text = (inMsg == "") ? _b.GUI.tb_chatMsg.Text : inMsg;

        //            ChatId id = new ChatId();
        //            if (cl.ID == GROUPCHAT)
        //            {
        //                id.chat_id = "";
        //                id.chat_type = ChatType.TYPE_GROUP;
        //            }
        //            else
        //            {
        //                id.chat_id = cl.ID;
        //                id.chat_type = ChatType.TYPE_LOBBY;
        //            }
        //            ChatMessage msg = new ChatMessage();
        //            msg.id = id;
        //            msg.msg = text;
        //            msg.peer_nickname = _nick;
        //            msg.send_time = (uint)DateTime.Now.Second;

        //            _b.RPC.ChatSendMsg(msg);
        //            if(cl.ID != GROUPCHAT) // needed ?!
        //                AddMsgToLobby(cl.ID, DateTime.Now.ToLongTimeString() + " - " + _nick + " > " + text + "\n");
        //            if (inMsg == "")
        //                _b.GUI.tb_chatMsg.Clear();
        //        }
        //    }
        //}

        public void SendChatMsg(GuiChatLobby cl, string inMsg = "")
        {
            string text = (inMsg == "") ? _b.GUI.tb_chatMsg.Text : inMsg;

            ChatId id = new ChatId();
            if (cl.ID == GROUPCHAT)
            {
                id.chat_id = "";
                id.chat_type = ChatType.TYPE_GROUP;
            }
            else
            {
                id.chat_id = cl.ID;
                id.chat_type = ChatType.TYPE_LOBBY;
            }
            ChatMessage msg = new ChatMessage();
            msg.id = id;
            msg.msg = text;
            msg.peer_nickname = _nick;
            msg.send_time = (uint)DateTime.Now.Second;

            _b.RPC.ChatSendMsg(msg);
            if (cl.ID != GROUPCHAT) // needed ?!
                AddMsgToLobby(cl.ID, DateTime.Now.ToLongTimeString() + " - " + _nick + " > " + text + "\n");
            if (inMsg == "")
                _b.GUI.tb_chatMsg.Clear();
        }

        public void AddMsgToLobby(string ID, ChatMessage response)
        {
            System.Diagnostics.Debug.WriteLineIf(DEBUG, "Chat: PrintMsgToLobby ID: " + response.id);
            if (!_chatLobbies.ContainsKey(ID))
            {
                // we don't know this lobby :S
                System.Diagnostics.Debug.WriteLineIf(DEBUG, "Chat: ID (" + response.id + ") is unknown");
                return;
            }

            GuiChatLobby cl = _chatLobbies[ID];
            string msg = Processor.RemoteTags(response.msg);

            System.Diagnostics.Debug.WriteLineIf(DEBUG, "Chat: lobby: " + cl.Lobby.lobby_name);
            System.Diagnostics.Debug.WriteLineIf(DEBUG, "Chat: msg: " + msg + " from " + response.peer_nickname);
            System.Diagnostics.Debug.WriteLineIf(DEBUG, "Chat: rec: " + response.recv_time + " send: " + response.send_time);

            // add "*time* - *nick* > *msg*" 
            cl.ChatText += Processor.conv_Timestamp2Date(response.send_time).ToLocalTime().ToLongTimeString() + " - " + response.peer_nickname + " > " + msg + "\n";

            _chatLobbies[ID] = cl;
            if (_b.GUI.clb_chatLobbies.SelectedIndex == cl.Index)
                //SetChatText(ID);
                _reDrawChat = true;
            else
            {
                if (!cl.Unread)
                {
                    cl.Unread = true;
                    _b.GUI.clb_chatLobbies.SetItemCheckState(cl.Index, CheckState.Indeterminate);
                    _chatLobbies[ID] = cl;
                }
            }

            //AutoAnswer(Processor.RemoteTags(response.msg.msg));
            AutoAnswer(response, cl);            
        }

        public void AddMsgToLobby(string ID, string msg)
        {
            GuiChatLobby cl = _chatLobbies[ID];
            cl.ChatText += msg;
            _chatLobbies[ID] = cl;
            if (_b.GUI.clb_chatLobbies.SelectedIndex == cl.Index)
                //SetChatText(ID);
                _reDrawChat = true;
        }

        //public void AddMsgToGroupChat(EventChatMessage response)
        //{
        //    GuiChatLobby cl = _chatLobbies[GROUPCHAT];
        //    cl.ChatText += DateTime.Now.ToShortTimeString() + " - " + response.msg.peer_nickname + " > " + Processor.RemoteTags(response.msg.msg) + "\n";
        //    _chatLobbies[GROUPCHAT] = cl;

        //    if (_b.GUI.clb_chatLobbies.SelectedIndex == cl.Index)
        //        //SetChatText(GROUPCHAT);
        //        _reDrawChat = true;
        //    else            
        //        if (!cl.Unread)
        //        {
        //            cl.Unread = true;
        //            _b.GUI.clb_chatLobbies.SetItemCheckState(cl.Index, CheckState.Indeterminate);
        //        }            
        //    _chatLobbies[GROUPCHAT] = cl;
        //    AutoAnswer(Processor.RemoteTags(response.msg.msg), cl);
        //}

        /// <summary>
        /// prints the text from a lobby to screen (textbox)
        /// </summary>
        /// <param name="LobbyID">lobby ID</param>
        private void SetChatText(string LobbyID)
        {
            //rtb_chat.Clear();
            //rtb_chat.Rtf =@"{\rtf1\ansi " + _chatText[index] + "}";
            _b.GUI.rtb_chat.Text = _chatLobbies[LobbyID].ChatText;
            _b.GUI.rtb_chat.SelectionStart = _b.GUI.rtb_chat.Text.Length;
            _b.GUI.rtb_chat.ScrollToCaret();

            GuiChatLobby cl = _chatLobbies[LobbyID];
            cl.Unread = false;
            _chatLobbies[LobbyID] = cl;
            if (cl.Joined && _b.GUI.clb_chatLobbies.GetItemCheckState(cl.Index) == CheckState.Indeterminate)
                _b.GUI.clb_chatLobbies.SetItemCheckState(cl.Index, CheckState.Checked);
        }

        //private void AutoAnswer(string msg, GuiChatLobby cl)
        //{
        //    if (_b.GUI.cb_chatAutoRespEnable.Checked && _b.GUI.tb_chatAutoRespSearch.Text != "" && _b.GUI.tb_chatAutoRespAnswer.Text != "")            
        //        if (msg.ToLower().Contains(_b.GUI.tb_chatAutoRespSearch.Text.ToLower()))
        //            // can't replace %nick% until we know who sent the msg
        //            SendChatMsg(cl, _b.GUI.tb_chatAutoRespAnswer.Text);
        //}

        private void AutoAnswer(ChatMessage response, GuiChatLobby cl)
        {
            string[] msgOUT = _b.AutoResponse.Process(response, cl);
            if (msgOUT != null)
                foreach (string s in msgOUT)
                    if (s != "")
                        SendChatMsg(cl, s);
        }

        public void SetNick(string nick)
        {
                _nick = nick;
            _b.GUI.tb_chatNickname.Text = _nick;

            if (_b.RPC.IsConnected)
            {
                List<string> ids = new List<string>();
                Dictionary<string, GuiChatLobby>.ValueCollection lobbies = _chatLobbies.Values;
                foreach (GuiChatLobby lobby in lobbies)
                    if (_b.GUI.clb_chatLobbies.GetItemChecked(lobby.Index))
                        if (lobby.ID != GROUPCHAT)
                            ids.Add(lobby.ID);

                if (ids.Count == 0)
                    _b.RPC.ChatSetLobbyNickname(_nick);
                else
                    _b.RPC.ChatSetLobbyNickname(_nick, ids);
            }
        }

        public void AddGroupChat()
        {
            if (!_chatLobbies.ContainsKey(GROUPCHAT))
            {
                ChatLobbyInfo lobby = new ChatLobbyInfo();
                lobby.lobby_name = "Group chat";
                lobby.lobby_id = GROUPCHAT;

                GuiChatLobby cl = new GuiChatLobby();
                cl.Lobby = lobby;
                cl.Joined = true;
                cl.Unread = false;
                {
                    _b.GUI.clb_chatLobbies.Items.Add(cl.Lobby.lobby_name, true);
                    cl.Index = (ushort)_b.GUI.clb_chatLobbies.Items.IndexOf(cl.Lobby.lobby_name);
                }
                cl.ChatText= DateTime.Now.ToShortDateString() + " - " + DateTime.Now.ToShortTimeString() + "\n";
                cl.ChatUser = new List<string> { };
                _chatLobbies.Add(cl.ID, cl);
                CheckChatRegistration();
            }
        }

        public void ChatLobbyIndexChange(int index)
        {
            if (index >= 0)
            {
                GuiChatLobby cl = new GuiChatLobby();
                if (GetLobbyByListIndex(index, out cl))
                {
                    SetChatText(cl.ID);
                    _b.GUI.clb_chatUser.Items.Clear();
                    foreach (string s in cl.ChatUser)
                        _b.GUI.clb_chatUser.Items.Add(s);
                }
            }
        }

        //public void ChatLobbyItemChecked(ItemCheckEventArgs e)
        //{
        //    GuiChatLobby cl = new GuiChatLobby();
        //    if (GetLobbyByListIndex(e.Index, out cl))
        //    {
        //        if (e.NewValue != CheckState.Indeterminate)
        //        {
        //            if (cl.Joined && e.NewValue == CheckState.Unchecked) // leave lobby
        //            {
        //                _b.RPC.ChatJoinLeaveLobby(RequestJoinOrLeaveLobby.LobbyAction.LEAVE_OR_DENY, cl.ID);
        //                cl.Joined = false;
        //            }
        //            else if (!cl.Joined && e.NewValue == CheckState.Checked) // join lobby
        //            {
        //                _b.RPC.ChatJoinLeaveLobby(RequestJoinOrLeaveLobby.LobbyAction.JOIN_OR_ACCEPT, cl.ID);
        //                cl.Joined = true;
        //                //if(cl.Lobby.lobby_state == ChatLobbyInfo.LobbyState.LOBBYSTATE_INVITED)
        //                cl.Lobby.lobby_state = ChatLobbyInfo.LobbyState.LOBBYSTATE_JOINED;
        //            }
        //            _chatLobbies[cl.ID] = cl;
        //            CheckChatRegistration();
        //        }
        //    }
        //}

        /// <summary>
        /// joins/leaves selected lobby
        /// </summary>
        /// <param name="action">true = join; false = leave</param>
        /// <param name="index">lobby index</param>
        public void JoinLeaveChatLobby(bool action, int index)
        {
            GuiChatLobby cl = new GuiChatLobby();
            if (GetLobbyByListIndex(index, out cl))
            {
                if (action && !cl.Joined) // join lobby
                {
                    _b.RPC.ChatJoinLeaveLobby(RequestJoinOrLeaveLobby.LobbyAction.JOIN_OR_ACCEPT, cl.ID);
                    cl.Joined = true;
                    cl.Lobby.lobby_state = ChatLobbyInfo.LobbyState.LOBBYSTATE_JOINED;
                }
                else if (!action && cl.Joined) // leave lobby 
                {
                    _b.RPC.ChatJoinLeaveLobby(RequestJoinOrLeaveLobby.LobbyAction.LEAVE_OR_DENY, cl.ID);
                    cl.Joined = false;
                    cl.Lobby.lobby_state = ChatLobbyInfo.LobbyState.LOBBYSTATE_VISIBLE;
                }

                // save changes
                _chatLobbies[cl.ID] = cl;
                CheckChatRegistration();
            }
        }

        /// <summary>
        /// returns if the server has joined a lobby ( by lobby list index )
        /// </summary>
        /// <param name="index">index in lobby list</param>
        /// <returns>true = joines; false = not joined</returns>
        //public bool Joined(int index)
        //{
        //    GuiChatLobby cl = new GuiChatLobby();
        //    if (GetLobbyByListIndex(index, out cl))
        //        return cl.Joined;

        //    return false;
        //}

        /// <summary>
        /// returns if the server has joined a lobby ( by lobby ID )
        /// </summary>
        /// <param name="ID">lobby ID</param>
        /// <returns>true = joines; false = not joined</returns>
        //public bool Joined(string ID)
        //{
        //    if (_chatLobbies.ContainsKey(ID))
        //        return _chatLobbies[ID].Joined;

        //    return false;
        //}

        #region tick

        public void Tick(uint counter)
        {
            NotifysAndSetCheckState();

            if (_reDrawChat)
            {
                System.Diagnostics.Debug.WriteLineIf(DEBUG, "redraw chat");
                GuiChatLobby gcl;
                if (GetLobbyByListIndex(_b.GUI.clb_chatLobbies.SelectedIndex, out gcl))
                    SetChatText(gcl.ID);
                _reDrawChat = false;
            }

            // update lobbies every 30 seconds OR every 10 seconds if the connection was just established ( this will speed up getting available lobbies) 
            if (counter % 30 == 0 || (counter < 30 && counter % 10 == 0))
            {
                //_bridge.RPC.ChatGetLobbies(rsctrl.chat.RequestChatLobbies.LobbySet.LOBBYSET_JOINED);
                //_bridge.RPC.ChatGetLobbies(rsctrl.chat.RequestChatLobbies.LobbySet.LOBBYSET_INVITED);
                //_bridge.RPC.ChatGetLobbies(rsctrl.chat.RequestChatLobbies.LobbySet.LOBBYSET_PUBLIC);
                _b.RPC.ChatGetLobbies(rsctrl.chat.RequestChatLobbies.LobbySet.LOBBYSET_ALL);
            }
        }

        private void NotifysAndSetCheckState()
        {
            GuiChatLobby[] values = new GuiChatLobby[_chatLobbies.Count];
            _chatLobbies.Values.CopyTo(values, 0);
            foreach (GuiChatLobby lobby in values)
            {
                if (lobby.Lobby == null)
                    continue;

                CheckState state = _b.GUI.clb_chatLobbies.GetItemCheckState(lobby.Index);
                if (lobby.Unread)
                {
                    // unread messages
                    if (state == CheckState.Indeterminate)
                        if (lobby.Joined)
                            state = CheckState.Checked;
                        else
                            state = CheckState.Unchecked;
                    else
                        state = CheckState.Indeterminate;
                    _b.GUI.clb_chatLobbies.SetItemCheckState(lobby.Index, state);
                }
                else if (lobby.Lobby.lobby_state == ChatLobbyInfo.LobbyState.LOBBYSTATE_INVITED)
                    // invited by someone
                    _b.GUI.clb_chatLobbies.SetItemCheckState(lobby.Index, (state == CheckState.Indeterminate) ? CheckState.Unchecked : CheckState.Indeterminate);
                else                
                    // nothing to notify - just set the correct CheckState
                    _b.GUI.clb_chatLobbies.SetItemCheckState(lobby.Index,
                        lobby.Joined ? CheckState.Checked : CheckState.Unchecked
                        );                
            }
        }

        #endregion
    }
}
