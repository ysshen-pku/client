using UnityEngine;
using System;


public class PlayerInfo
{
    public UInt16 playerstate = 0;
    public float leftShootCD = 0.15f;
    public float rightShootCD = 0.5f;
    public float shootRange = 100f;
    public UInt16 leftShootDamage = 40;
    public UInt16 rightShootDamage = 50;

    public UInt16 round = 0;
    public Int16 baseHP = 10;

    public UInt16 spikeTrapRemain = 0;
    public UInt16 freezeTrapRemain = 0;

    private static PlayerInfo instance;

    private UInt32 uid = 0;
    private int health= 100;
    private UInt32 localcoin= 0;
    private UInt32 localexp = 0;
    private Vector3 position;

    public static PlayerInfo getinstance()
    {
        if (instance == null)
        {
            instance = new PlayerInfo();
        }
        return instance;
    }

    public void UpdatePlayerInfo(Int16 hp, UInt32 coin, UInt32 exp, UInt16 spike, UInt16 freeze)
    {
        health = hp;
        localcoin = coin;
        localexp = exp;
        spikeTrapRemain = spike;
        freezeTrapRemain = freeze;
    }

    public void GetInfo(ref Int16 hp, ref UInt32 coin, ref UInt32 exp)
    {
        hp = (Int16)health;
        coin = localcoin;
        exp = localexp;
    }

    public void SetPlayerPosition(Vector3 pos)
    {
        position = pos;
    }

    public Vector3 GetPlayerPosition()
    {
        return position;
    }

    public void SetPlayerId(UInt32 id)
    {
        uid = id;
    }

    public UInt32 GetPlayerId()
    {
        return uid;
    }

    public UInt32 GetMoney()
    {
        return localcoin;
    }

}
