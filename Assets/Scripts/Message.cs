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

public class MsgCSLogin: Message
{
    public MsgCSLogin(UInt32 id=0, int icon = -1):base(Config.MSG_CS_LOGIN)
    {
        appendParam("id", id, 'I');
        appendParam("icon", icon, 'i');
    }
}

public class MsgSCGameStart: Message
{
    public MsgSCGameStart(double x = 0, double z = 0, Int16 hp =0):base(Config.MSG_SC_GAMESTART)
    {
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
        appendParam("hp", hp, 'h');
    }
}

public class MsgSCConfirm: Message
{
    public MsgSCConfirm(UInt32 uid =0, UInt32 result = 0):base(Config.MSG_SC_CONFIRM)
    {
        appendParam("uid", uid, 'I');
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
    public MsgSCMonsterMove(UInt32 mid = 0, double x = 0, double z = 0) : base(Config.MSG_SC_MONSTER_MOVE)
    {
        appendParam("mid", mid, 'I');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
    }
}

public class MsgCSTrapPlace : Message
{
    public MsgCSTrapPlace(UInt32 uid= 0, UInt16 type =0, double x = 0, double z =0) : base(Config.MSG_CS_TRAPPLACE)
    {
        appendParam("uid", uid, 'I');
        appendParam("type", type, 'H');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
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
    public MsgCSMonsterDamage(UInt32 uid = 0, UInt32 mid = 0, UInt16 damage = 0, Int16 stun = 0, UInt16 range = 0, double cx =0, double cz=0) : base(Config.MSG_CS_MONSTER_DAMAGE)
    {
        appendParam("uid", uid, 'I');
        appendParam("mid", mid, 'I');
        appendParam("damage", damage, 'H');
        appendParam("stun", stun, 'h');
        appendParam("range", range, 'H');
        appendParam("cx", cx, 'd');
        appendParam("cz", cz, 'd');
    }
}

public class MsgSCMonsterState: Message
{
    public MsgSCMonsterState(UInt32 mid =0 ,Int16 hp =0 , UInt16 state = 0):base(Config.MSG_SC_MONSTER_STATE)
    {
        appendParam("mid", mid, 'I');
        appendParam("hp", hp, 'h');
        appendParam("state", state, 'H');
    }
}

public class MsgSCPlayerInfo : Message
{
    public MsgSCPlayerInfo(UInt32 uid =0, Int16 hp =0, UInt32 coin = 0, UInt32 exp = 0) : base(Config.MSG_SC_PLAYER_INFO)
    {
        appendParam("uid", uid, 'I');
        appendParam("hp", hp, 'h');
        appendParam("coin", coin, 'I');
        appendParam("exp", exp, 'I');
    }
}

public class MsgSCMonsterDeath : Message
{
    public MsgSCMonsterDeath(UInt32 uid =0 , UInt32 mid = 0) : base(Config.MSG_SC_MONSTER_DEATH)
    {
        appendParam("uid", uid, 'I');
        appendParam("mid", mid, 'I');
    }
}

public class MsgSCRoundState: Message
{
    public MsgSCRoundState(UInt16 rid = 0, Int16 basehp = 0) : base(Config.MSG_SC_ROUND_STATE)
    {
        appendParam("rid", rid, 'H');
        appendParam("basehp", basehp, 'h');
    }
}
