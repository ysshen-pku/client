using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Message 
{
    UInt16 mtype;
    string bfmt,hfmt;
    List<string> params_name;
    //public Dictionary<string,Type> params_type;
    public Dictionary<string, object> params_dict;

    public Message(UInt16 type)
    {
        mtype = type;
        bfmt = "";
        hfmt = "<H";
        params_name = new List<string>();
        //params_type = new Dictionary<string, Type>();
        params_dict = new Dictionary<string, object>();
    }

    public void appendParam(string pname, object pvalue , char ptype)
    {
        bfmt += ptype;
        params_name.Add(pname);
        params_dict[pname]=pvalue;
        //params_type[pname]=(pvalue.GetType());
    }

    public void unpack( byte [] raw)
    {
        if (raw.Length == 0)
        {
            return;
        }
        string ofmt = hfmt + bfmt;
        object[] tmp = StructConverter.Unpack(ofmt, raw);
        for (int i= 1;i<tmp.Length;i++)
        {
            params_dict[params_name[i-1]] = tmp[i];

        }
    }

    public byte [] enpack()
    {
        List<object> valueList = new List<object>();
        valueList.Add(mtype);
        foreach (string item in params_name)
        {
            valueList.Add(params_dict[item]);
        }
        return StructConverter.Pack(valueList.ToArray());
    }

}

public class MsgCSRegister: Message
{
    public MsgCSRegister(String name, String pass) : base(Config.MSG_CS_REGISTER)
    {
        appendParam("name", name, 'S');
        appendParam("pass", pass, 'S');
    }
}

public class MsgCSLogin: Message
{
    public MsgCSLogin(String name, String pass):base(Config.MSG_CS_LOGIN)
    {
        appendParam("name", name, 'S');
        appendParam("pass", pass, 'S');
    }
}

public class MsgSCGameStart: Message
{
    public MsgSCGameStart(UInt32 uid = 0, double x = 0, double z = 0, Int16 hp =0):base(Config.MSG_SC_GAMESTART)
    {
        appendParam("uid", uid, 'I');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
        appendParam("hp", hp, 'h');
    }
}

public class MsgSCConfirm: Message
{
    public MsgSCConfirm(UInt32 result = 0):base(Config.MSG_SC_CONFIRM)
    {
        appendParam("result", result, 'I');
    }
}

public class MsgCSMoveto : Message
{
    public MsgCSMoveto(double x=0, double z=0, double ry = 0):base(Config.MSG_CS_MOVETO)
    {
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
        appendParam("ry", ry, 'd');
    }
}

public class MsgSCMoveto: Message
{
    public MsgSCMoveto(UInt32 uid=0, double x =0, double z = 0,double ry =0):base(Config.MSG_SC_MOVETO)
    {
        appendParam("uid", uid, 'I');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
        appendParam("ry", ry, 'd');
    }
}

public class MsgSCNewPlayer : Message
{
    public MsgSCNewPlayer(UInt32 uid = 0, double x =0, double z =0,double ry =0):base(Config.MSG_SC_NEWPLAYER)
    {
        appendParam("uid", uid, 'I');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
        appendParam("ry", ry, 'd');
    }
}

public class MsgSCNewMonster : Message
{
    public MsgSCNewMonster(UInt32 mid = 0, double x = 0, double z = 0) : base(Config.MSG_SC_NEWMONSTER)
    {
        appendParam("mid", mid, 'I');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
    }
}

public class MsgSCMonsterMove : Message
{
    public MsgSCMonsterMove(UInt32 mid = 0, UInt16 mtype=0, double x = 0, double z = 0) : base(Config.MSG_SC_MONSTER_MOVE)
    {
        appendParam("mid", mid, 'I');
        appendParam("mtype", mtype, 'H');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
    }
}

public class MsgCSTrapPlace : Message
{
    public MsgCSTrapPlace(UInt32 uid= 0, UInt16 type =0, UInt16 x = 0, UInt16 z =0) : base(Config.MSG_CS_TRAPPLACE)
    {
        appendParam("uid", uid, 'I');
        appendParam("type", type, 'H');
        appendParam("ax", x, 'H');
        appendParam("az", z, 'H');
    }
}

public class MsgSCTrapPlace :Message
{
    public MsgSCTrapPlace(UInt32 tid = 0 , UInt16 type = 0, double x = 0, double z = 0) : base(Config.MSG_SC_TRAPPLACE)
    {
        appendParam("tid", tid, 'I');
        appendParam("type", type, 'H');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
    }
}


public class MsgCSMonsterDamage : Message
{
    public MsgCSMonsterDamage(UInt32 uid = 0, UInt32 mid = 0, UInt16 damage = 0, UInt16 range = 0, double cx =0, double cz=0) : base(Config.MSG_CS_MONSTER_DAMAGE)
    {
        appendParam("uid", uid, 'I');
        appendParam("mid", mid, 'I');
        appendParam("damage", damage, 'H');
        appendParam("range", range, 'H');
        appendParam("cx", cx, 'd');
        appendParam("cz", cz, 'd');
    }
}

public class MsgSCMonsterState: Message
{
    public MsgSCMonsterState(UInt32 mid =0 ,Int16 hp =0 , UInt16 state = 0, UInt32 taruid = 0):base(Config.MSG_SC_MONSTER_STATE)
    {
        appendParam("mid", mid, 'I');
        appendParam("hp", hp, 'h');
        appendParam("state", state, 'H');
        appendParam("taruid", taruid, 'I');
    }
}

public class MsgSCPlayerInfo : Message
{
    public MsgSCPlayerInfo(UInt32 uid = 0, UInt16 level = 0,UInt16 state = 0, Int16 hp = 0, UInt32 coin = 0, UInt32 exp = 0, UInt16 spike = 0, UInt16 freeze = 0) : base(Config.MSG_SC_PLAYER_INFO)
    {
        appendParam("uid", uid, 'I');
        appendParam("level", level, 'H');
        appendParam("state", state, 'H');
        appendParam("hp", hp, 'h');
        appendParam("coin", coin, 'I');
        appendParam("exp", exp, 'I');
        appendParam("spike", spike, 'H');
        appendParam("freeze", freeze, 'H');
    }
}

public class MsgSCObjectDelete : Message
{
    public MsgSCObjectDelete(UInt16 type = 0, UInt32 id = 0 ) : base(Config.MSG_SC_OBJECT_DELETE)
    {
        appendParam("type", type, 'H');
        appendParam("id", id, 'I');
    }
}

public class MsgSCRoundState: Message
{
    public MsgSCRoundState(UInt16 rid = 0,UInt16 gamestate = 0, UInt16 remain = 0, Int16 basehp = 0) : base(Config.MSG_SC_ROUND_STATE)
    {
        appendParam("rid", rid, 'H');
        appendParam("gamestate", gamestate, 'H');
        appendParam("remain", remain, 'H');
        appendParam("basehp", basehp, 'h');
    }
}

public class MsgSCGameReset: Message
{
    public MsgSCGameReset(UInt16 rid =0 ,double x =0, double z = 0) : base(Config.MSG_SC_GAME_RESET)
    {
        appendParam("rid", rid, 'H');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
    }
}

public class MsgCSBuy: Message
{
    public MsgCSBuy(UInt32 uid = 0, UInt16 costtype = 0, UInt16 ttype = 0): base(Config.MSG_CS_BUY)
    {
        appendParam("uid", uid, 'I');
        appendParam("ctype", costtype, 'H');
        appendParam("ttype", ttype, 'H');
    }
}



