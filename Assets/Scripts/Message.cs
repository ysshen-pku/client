using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Message 
{
    string bfmt;
    List<string> params_name;
    //public Dictionary<string,Type> params_type;
    public Dictionary<string, object> params_dict;

    public Message()
    {
        bfmt = "";
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
        if (raw.GetLength(1) == 0)
        {
            return;
        }
        object[] tmp = StructConverter.Unpack(bfmt, raw);
        int i = 0;
        foreach (object item in tmp)
        {
            params_dict[params_name[i]] = item;
        }
    }

    public byte [] enpack()
    {
        List<object> valueList = new List<object>();
        foreach (string item in params_name)
        {
            valueList.Add(params_dict[item]);
        }
        return StructConverter.Pack(valueList.ToArray());
    }

}

public class MsgCSLogin: Message
{
    public MsgCSLogin(UInt32 id=0, int icon = -1)
    {
        appendParam("id", id, 'I');
        appendParam("icon", icon, 'i');
    }
}

public class MsgSCLogin: Message
{
    public MsgSCLogin(float x = 0, float y = 0, UInt32 hp = 0)
    {
        appendParam("x", x, 'f');
        appendParam("y", y, 'f');
        appendParam("hp", hp, 'I');
    }
}

public class MsgSCConfirm: Message
{
    public MsgSCConfirm(UInt32 uid =0, UInt32 result = 0)
    {
        appendParam("uid", uid, 'I');
        appendParam("result", result, 'I');
    }
}

public class MsgCSMoveto : Message
{
    public MsgCSMoveto(int x=0, int y=0)
    {
        appendParam("x", x, 'i');
        appendParam("y", y, 'i');
    }
}

public class MsgSCMoveto: Message
{
    public MsgSCMoveto(UInt32 uid=0, int x=0, int y = 0)
    {
        appendParam("uid", uid, 'I');
        appendParam("x", x, 'i');
        appendParam("y", y, 'i');
    }
}

public class MsgSCNewPlayer : Message
{
    public MsgSCNewPlayer(UInt32 uid = 0, float x =0, float y =0, float rx =0,float ry =0)
    {
        appendParam("uid", uid, 'I');
        appendParam("x", x, 'f');
        appendParam("y", y, 'f');
        appendParam("rx", rx, 'f');
        appendParam("ry", ry, 'f');
    }
}

