using System.Collections.Generic;
using UnityEngine.Networking;

public class NetworkMessages
{
    public static short Ready = MsgType.Highest + 1;
    public static short Name = MsgType.Highest + 2;
    public static short Class = MsgType.Highest + 3;
    public static short Id = MsgType.Highest + 4;
    public static short PlayerDisconnected = MsgType.Highest + 5;
    public static short Player = MsgType.Highest + 6;
    public static short File = MsgType.Highest + 7;
}

public class ReadyMessage : MessageBase
{
    public int id; // which player is ready?
    public bool ready;
}

public class NameMessage : MessageBase
{
    public int id;
    public string name;
}

public class ClassMessage : MessageBase
{
    public int id;
    public int classType;
}

public class IdMessage : MessageBase
{
    public int id;
}

public class PlayerDisconnectedMessage : MessageBase
{
    public int id;
}

public class PlayerMessage : MessageBase
{
    public int id;
    public string name;
    public bool ready;
    public int classType;
}

public class FileMessage : MessageBase
{
    public string file;
}