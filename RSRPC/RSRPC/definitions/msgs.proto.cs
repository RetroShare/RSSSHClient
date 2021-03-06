//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: msgs.proto
// Note: requires additional types generated from: core.proto
namespace rsctrl.msgs
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RequestPeers")]
  public partial class RequestPeers : global::ProtoBuf.IExtensible
  {
    public RequestPeers() {}
    
    private rsctrl.msgs.RequestPeers.SetOption _set;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"set", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public rsctrl.msgs.RequestPeers.SetOption set
    {
      get { return _set; }
      set { _set = value; }
    }
    private rsctrl.msgs.RequestPeers.InfoOption _info;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"info", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public rsctrl.msgs.RequestPeers.InfoOption info
    {
      get { return _info; }
      set { _info = value; }
    }
    private readonly global::System.Collections.Generic.List<string> _gpg_ids = new global::System.Collections.Generic.List<string>();
    [global::ProtoBuf.ProtoMember(3, Name=@"gpg_ids", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<string> gpg_ids
    {
      get { return _gpg_ids; }
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"SetOption")]
    public enum SetOption
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"OWNID", Value=1)]
      OWNID = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LISTED", Value=2)]
      LISTED = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ONLINE", Value=3)]
      ONLINE = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"FRIENDS", Value=4)]
      FRIENDS = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"VALID", Value=5)]
      VALID = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"SIGNED", Value=6)]
      SIGNED = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ALL", Value=7)]
      ALL = 7
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"InfoOption")]
    public enum InfoOption
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"NAMEONLY", Value=1)]
      NAMEONLY = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"BASIC", Value=2)]
      BASIC = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"LOCATION", Value=3)]
      LOCATION = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ALLINFO", Value=4)]
      ALLINFO = 4
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ResponsePeerList")]
  public partial class ResponsePeerList : global::ProtoBuf.IExtensible
  {
    public ResponsePeerList() {}
    
    private rsctrl.core.Status _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public rsctrl.core.Status status
    {
      get { return _status; }
      set { _status = value; }
    }
    private readonly global::System.Collections.Generic.List<rsctrl.core.Person> _peers = new global::System.Collections.Generic.List<rsctrl.core.Person>();
    [global::ProtoBuf.ProtoMember(2, Name=@"peers", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<rsctrl.core.Person> peers
    {
      get { return _peers; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RequestAddPeer")]
  public partial class RequestAddPeer : global::ProtoBuf.IExtensible
  {
    public RequestAddPeer() {}
    
    private string _gpg_id;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"gpg_id", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string gpg_id
    {
      get { return _gpg_id; }
      set { _gpg_id = value; }
    }
    private rsctrl.msgs.RequestAddPeer.AddCmd _cmd;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"cmd", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public rsctrl.msgs.RequestAddPeer.AddCmd cmd
    {
      get { return _cmd; }
      set { _cmd = value; }
    }

    private string _cert = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"cert", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string cert
    {
      get { return _cert; }
      set { _cert = value; }
    }
    [global::ProtoBuf.ProtoContract(Name=@"AddCmd")]
    public enum AddCmd
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"NOOP", Value=0)]
      NOOP = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ADD", Value=1)]
      ADD = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"REMOVE", Value=2)]
      REMOVE = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"IMPORT", Value=3)]
      IMPORT = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"EXAMINE", Value=4)]
      EXAMINE = 4
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ResponseAddPeer")]
  public partial class ResponseAddPeer : global::ProtoBuf.IExtensible
  {
    public ResponseAddPeer() {}
    
    private rsctrl.core.Status _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public rsctrl.core.Status status
    {
      get { return _status; }
      set { _status = value; }
    }
    private readonly global::System.Collections.Generic.List<rsctrl.core.Person> _peers = new global::System.Collections.Generic.List<rsctrl.core.Person>();
    [global::ProtoBuf.ProtoMember(2, Name=@"peers", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<rsctrl.core.Person> peers
    {
      get { return _peers; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RequestModifyPeer")]
  public partial class RequestModifyPeer : global::ProtoBuf.IExtensible
  {
    public RequestModifyPeer() {}
    
    private rsctrl.msgs.RequestModifyPeer.ModCmd _cmd;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"cmd", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public rsctrl.msgs.RequestModifyPeer.ModCmd cmd
    {
      get { return _cmd; }
      set { _cmd = value; }
    }
    private readonly global::System.Collections.Generic.List<rsctrl.core.Person> _peers = new global::System.Collections.Generic.List<rsctrl.core.Person>();
    [global::ProtoBuf.ProtoMember(2, Name=@"peers", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<rsctrl.core.Person> peers
    {
      get { return _peers; }
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"ModCmd")]
    public enum ModCmd
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"NOOP", Value=0)]
      NOOP = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"ADDRESS", Value=1)]
      ADDRESS = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"DYNDNS", Value=2)]
      DYNDNS = 2
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ResponseModifyPeer")]
  public partial class ResponseModifyPeer : global::ProtoBuf.IExtensible
  {
    public ResponseModifyPeer() {}
    
    private rsctrl.core.Status _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public rsctrl.core.Status status
    {
      get { return _status; }
      set { _status = value; }
    }
    private readonly global::System.Collections.Generic.List<rsctrl.core.Person> _peers = new global::System.Collections.Generic.List<rsctrl.core.Person>();
    [global::ProtoBuf.ProtoMember(2, Name=@"peers", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<rsctrl.core.Person> peers
    {
      get { return _peers; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"RequestMsgIds")]
    public enum RequestMsgIds
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"MsgId_RequestPeers", Value=1)]
      MsgId_RequestPeers = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MsgId_RequestAddPeer", Value=2)]
      MsgId_RequestAddPeer = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MsgId_RequestModifyPeer", Value=3)]
      MsgId_RequestModifyPeer = 3
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"ResponseMsgIds")]
    public enum ResponseMsgIds
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"MsgId_ResponsePeerList", Value=1)]
      MsgId_ResponsePeerList = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MsgId_ResponseAddPeer", Value=2)]
      MsgId_ResponseAddPeer = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"MsgId_ResponseModifyPeer", Value=3)]
      MsgId_ResponseModifyPeer = 3
    }
  
}