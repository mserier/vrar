using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class CustomNetMsg
{
    public static short Ready               = MsgType.Highest + 1;
    public static short Name                = MsgType.Highest + 2;
    public static short Role                = MsgType.Highest + 3;
    public static short Id                  = MsgType.Highest + 4;
    public static short PlayerDisconnected  = MsgType.Highest + 5;
    public static short Player              = MsgType.Highest + 6;
    public static short StartGame           = MsgType.Highest + 7;
    public static short Spawn               = MsgType.Highest + 8;
    public static short MoveRequest         = MsgType.Highest + 9;
    public static short Move                = MsgType.Highest + 10;
    public static short DissonanceId        = MsgType.Highest + 11;
    public static short Tile                = MsgType.Highest + 12;
    public static short AttackRequest       = MsgType.Highest + 13;
    public static short Attack              = MsgType.Highest + 14;
}

public class ReadyMessage : MessageBase
{
    public int id;
    public bool ready;
}

public class NameMessage : MessageBase
{
    public int id;
    public string name;
}

public class RoleMessage : MessageBase
{
    public int id;
    public int role;
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
    public int role;
    public bool host;
}

public class StartGameMessage : MessageBase
{
    public string level;
}

public class SpawnMessage : MessageBase
{
    public int id;
    public Vector2 spawnLocation;
}

public class MoveRequestMessage : MessageBase
{
    public int direction;
}

public class MoveMessage : MessageBase
{
    public int id;
    public int direction;
}

public class AttackRequestMessage : MessageBase
{
    public int attackId;
    public int targetTileX;
    public int targetTileY;
}

public class AttackMessage : MessageBase
{
    public int id;
    public int attackId;
    public int targetTileX;
    public int targetTileY;
    public int result;
}

public class DissonanceIdMessage : MessageBase
{
    public int id;
    public string dissonanceId;
}

public class TileMessage : MessageBase
{
    public int tileIndex_X;
    public int tileIndex_Y;
    public float height_;
    public bool removing;

    public int interactableObjectId;
    public string terrain;
    public bool walkable;

    public int[] dumbObjectIds;

    public int[] locationKeys;
    public Vector3[] locationValues;
    public int[] scaleKeys;
    public Vector3[] scaleValues;
    public int[] rotationKeys;
    public Quaternion[] rotationValues;
}