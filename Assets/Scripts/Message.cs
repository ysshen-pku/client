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

public class MsgSCLogin: Message
{
    public MsgSCLogin(double x = 0, double z = 0):base(Config.MSG_SC_LOGIN)
    {
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
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
    public MsgCSMoveto(double x=0, double z=0):base(Config.MSG_CS_MOVETO)
    {
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
    }
}

public class MsgSCMoveto: Message
{
    public MsgSCMoveto(UInt32 uid=0, double x =0, double z = 0):base(Config.MSG_SC_MOVETO)
    {
        appendParam("uid", uid, 'I');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
    }
}

public class MsgSCNewPlayer : Message
{
    public MsgSCNewPlayer(UInt32 uid = 0, double x =0, double z =0):base(Config.MSG_SC_NEWPLAYER)
    {
        appendParam("uid", uid, 'I');
        appendParam("x", x, 'd');
        appendParam("z", z, 'd');
    }
}

