using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Config {

    // msgType
    public const UInt16 MSG_CS_REGISTER = 0x1006;
    public const UInt16 MSG_CS_MOVETO = 0x1002;
    public const UInt16 MSG_SC_MOVETO = 0x2002;
    public const UInt16 MSG_SC_NEWPLAYER = 0x2003;
    public const UInt16 MSG_CS_LOGIN = 0x1001;
    public const UInt16 MSG_SC_GAMESTART = 0x2001;
    public const UInt16 MSG_SC_CONFIRM = 0x2011;
    public const UInt16 MSG_SC_NEWMONSTER = 0x2004;
    public const UInt16 MSG_SC_MONSTER_MOVE = 0x2005;
    public const UInt16 MSG_CS_TRAPPLACE = 0x1003;
    public const UInt16 MSG_SC_TRAPPLACE = 0x2006;
    public const UInt16 MSG_CS_MONSTER_DAMAGE = 0x1004;
    public const UInt16 MSG_SC_MONSTER_STATE = 0x2007;
    public const UInt16 MSG_SC_PLAYER_INFO = 0x2008;
    public const UInt16 MSG_SC_OBJECT_DELETE = 0x2009;
    public const UInt16 MSG_SC_ROUND_STATE = 0x2010;
    public const UInt16 MSG_SC_GAME_RESET = 0x2012;
    public const UInt16 MSG_CS_BUY = 0x1005;

    //player state
    public const UInt16 PLAYER_STATE_MOVE = 2;
    public const UInt16 PLAYER_STATE_COMMON = 1;
    public const UInt16 PLAYER_STATE_TRAPING = 5;
    public const UInt16 PLAYER_STATE_BUYING = 4;
    public const UInt16 PLAYER_STATE_DEAD = 3;

    public const UInt32 TRAP_COST = 100;
    public const UInt32 LEVELUP_COST = 1000;
    public const UInt16 FIREBALL_RANGE = 5;

    //monster state
    public const UInt16 MONSTER_STATE_IDLE = 1;
    public const UInt16 MONSTER_STATE_MOVE = 2;
    public const UInt16 MONSTER_STATE_CHASE = 3;
    public const UInt16 MONSTER_STATE_ATTACK = 4;
    public const UInt16 MONSTER_STATE_DEAD = 5;

    //object type
    public const UInt16 OBJECT_REMOTE_PLAYER = 1;
    public const UInt16 OBJECT_MONSTER = 2;
    public const UInt16 OBJECT_TRAP = 3;

    //server response

    public const UInt16 RESPONSE_WRONG_PASSWORD = 1;
    public const UInt16 RESPONSE_EXIST_PLAYER = 2;
    public const UInt16 RESPONSE_EXIST_NAME = 3;
    public const UInt16 RESPONSE_NON_EXIST_NAME = 4;
    public const UInt16 RESPONSE_REGISTER_SUCCESS = 5;

    //game state
    public const UInt16 GAME_STATE_CLOSE = 0;
    public const UInt16 GAME_STATE_WAIT = 1;
    public const UInt16 GAME_STATE_PLAY = 2;
    public const UInt16 GAME_STATE_WIN = 3;
    public const UInt16 GAME_STATE_LOSE = 4;

    // but cost type 1: exp 2:coin
    public const UInt16 BUY_COST_EXP = 1;
    public const UInt16 BUY_COST_COIN = 2;

    Config()
    {

    }

}
