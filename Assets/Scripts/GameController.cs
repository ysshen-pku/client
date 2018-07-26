using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameController : MonoBehaviour {

    public UInt32 localPlayerId;
    const int headSize = 4;
    ConnectSocket mySocket;
    SpawnManager spawn;
    PlayerInfo playerInfo;

    private Dictionary<string, GameObject> playerDict;
    private Dictionary<string, GameObject> monsterDict;
    private Dictionary<string, GameObject> trapDict;

    public void SendMessage(ref Message msg)
    {
        mySocket.Send(ref msg);
    }

    private void ProcessMessage()
    {
        // whether handle all recv msgs in one frame ?
        byte[] rawmsg= mySocket.Recv();
        while (rawmsg!=null)
        {
            UInt16 msgTpye = BitConverter.ToUInt16(rawmsg, 0);
            try
            {
                if (msgTpye == Config.MSG_SC_MOVETO)
                {
                    MsgSCMoveto msg = new MsgSCMoveto();
                    msg.unpack(rawmsg);
                    HandlePlayerMoveto(ref msg);
                }
                else if (msgTpye == Config.MSG_SC_NEWPLAYER)
                {
                    MsgSCNewPlayer msg = new MsgSCNewPlayer();
                    msg.unpack(rawmsg);
                    HandleNewPlayer(ref msg);
                }
                else if (msgTpye == Config.MSG_SC_MONSTER_MOVE)
                {
                    MsgSCMonsterMove msg = new MsgSCMonsterMove();
                    msg.unpack(rawmsg);
                    HandleMonsterMove(ref msg);
                }
                else if (msgTpye == Config.MSG_SC_TRAPPLACE)
                {
                    MsgSCTrapPlace msg = new MsgSCTrapPlace();
                    msg.unpack(rawmsg);
                    HandleTrapPlace(ref msg);
                }
                else if (msgTpye == Config.MSG_SC_MONSTER_DEATH)
                {
                    MsgSCMonsterDeath msg = new MsgSCMonsterDeath();
                    msg.unpack(rawmsg);
                    HandleMonsterDeath(ref msg);
                }
                else if (msgTpye == Config.MSG_SC_MONSTER_STATE)
                {
                    MsgSCMonsterState msg = new MsgSCMonsterState();
                    msg.unpack(rawmsg);
                    HandleMonsterState(ref msg);
                }
                else if (msgTpye == Config.MSG_SC_PLAYER_INFO)
                {
                    MsgSCPlayerInfo msg = new MsgSCPlayerInfo();
                    msg.unpack(rawmsg);
                    HandleUpdatePlayerInfo(ref msg);
                }
                else if (msgTpye == Config.MSG_SC_ROUND_STATE)
                {
                    MsgSCRoundState msg = new MsgSCRoundState();
                    msg.unpack(rawmsg);
                    HandleUpdateRoundState(ref msg);
                }
                else if (msgTpye == Config.MSG_SC_GAME_RESET)
                {
                    MsgSCGameReset msg = new MsgSCGameReset();
                    msg.unpack(rawmsg);
                    GameReset(ref msg);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Msg unpack error" + e.ToString());
                mySocket.Destroy();
            }
            rawmsg = mySocket.Recv();
        }
    }

    private void GameStart()
    {
        spawn.InstantiatePlayer(playerInfo.GetPlayerPosition());
    }

    private void GameReset(ref MsgSCGameReset msg)
    {
        UInt16 rid = (UInt16)msg.params_dict["rid"];
        double x = (double)msg.params_dict["x"], z = (double)msg.params_dict["z"];
        Vector3 pos = new Vector3((float)x, 0, (float)z);
        foreach(var player in playerDict)
            Destroy(player.Value);
        foreach (var monster in monsterDict)
            Destroy(monster.Value);
        foreach (var trap in trapDict)
            Destroy(trap.Value);
        playerDict.Clear();
        monsterDict.Clear();
        trapDict.Clear();
        GameObject localplayer = GameObject.Find("localplayer");
        Destroy(localplayer);
        playerInfo.SetPlayerPosition(pos);
        playerInfo.round = rid;
        playerInfo.UpdatePlayerInfo(100, 0, 0,0,0);
        playerInfo.playerstate = 0;
        spawn.InstantiatePlayer(playerInfo.GetPlayerPosition());
    }

    private void HandleNewPlayer(ref MsgSCNewPlayer msg)
    {
        Vector3 pos = new Vector3((float)(double)msg.params_dict["x"],0, (float)(double)msg.params_dict["z"]);
        UInt32 uid = (UInt32)msg.params_dict["uid"];
        //Vector3 rot = new Vector3((float)(double)msg.params_dict["rx"], (float)(double)msg.params_dict["ry"]);
        if (uid == localPlayerId)
            return;
        string newplayername = "player" + uid.ToString();
        if (playerDict.ContainsKey(newplayername))
        {
            // existed player
            return;
        }
        Quaternion rot = new Quaternion();
        rot.y = (float)(double)msg.params_dict["ry"];
        GameObject thisplayer = spawn.InstantiateRemotePlayer(newplayername, pos,rot);
        playerDict.Add(newplayername, thisplayer);
    }

    private void HandlePlayerMoveto(ref MsgSCMoveto msg)
    {
        Vector3 vec = new Vector3((float)(double)msg.params_dict["x"],0, (float)(double)msg.params_dict["z"]);
        UInt32 uid = (UInt32)msg.params_dict["uid"];
        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(0, (float)(double)msg.params_dict["ry"], 0);

        string playername = "player" + uid.ToString();
        if (playerDict.ContainsKey(playername))
        {
            playerDict[playername].transform.rotation = rot;
            playerDict[playername].GetComponent<AICharactorController>().SetDestination(vec);
        }
        else
        {
            //  if cannot find player with uid , debun in  Instantiate
            GameObject thisplayer = spawn.InstantiateRemotePlayer(playername, vec, rot);
            // Debug.Log("trying to set velocity on a unexisted player with uid:" + uid.ToString());
            playerDict.Add(playername, thisplayer);
            return;
        }
    }

    private void HandleMonsterMove(ref MsgSCMonsterMove msg)
    {
        Vector3 pos = new Vector3((float)(double)msg.params_dict["x"], 0, (float)(double)msg.params_dict["z"]);
        UInt32 mid = (UInt32)msg.params_dict["mid"];

        string monstername = "m" + mid.ToString();
        if (monsterDict.ContainsKey(monstername))
        {
            monsterDict[monstername].GetComponent<AICharactorController>().SetDestination(pos);
        }
        else
        {
            // cannot find monster with mid
            Quaternion rot = new Quaternion();
            GameObject thismonster = spawn.InstantiateMonster(monstername, pos, rot);
            monsterDict.Add(monstername, thismonster);
        }
    }

    private void HandleMonsterState(ref MsgSCMonsterState msg)
    {
        UInt32 mid = (UInt32)msg.params_dict["mid"];
        Int16 hp = (Int16)msg.params_dict["hp"];
        UInt16 state = (UInt16)msg.params_dict["state"];
        string monstername = "m" + mid.ToString();
        if (monsterDict.ContainsKey(monstername))
        {
            monsterDict[monstername].GetComponent<AICharactorController>().UpdateState(hp, state);
        }
    } 

    private void HandleMonsterDeath(ref MsgSCMonsterDeath msg)
    {
        UInt32 killuid = (UInt32)msg.params_dict["uid"];
        UInt32 mid = (UInt32)msg.params_dict["mid"];
        string monstername = "m" + mid.ToString();
        if (monsterDict.ContainsKey(monstername))
        {
            // delete 
            monsterDict[monstername].GetComponent<AICharactorController>().OnDeath();
            monsterDict.Remove(monstername);
        }
        else
        {
            // cannot find monster with mid
            // Debug
        }
    }

    private void HandleTrapPlace(ref MsgSCTrapPlace msg)
    {
        UInt32 tid = (UInt32)msg.params_dict["tid"];
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Finish");
        string trapname = "m" + tid.ToString();
        foreach (GameObject trap in traps)
        {
            if (trap.name == trapname)
            {
                return;
            }
        }
        spawn.InstantiateTrap(trapname,(UInt16)msg.params_dict["type"], (float)(double)msg.params_dict["x"], (float)(double)msg.params_dict["z"]);
    }

    private void HandleUpdatePlayerInfo(ref MsgSCPlayerInfo msg)
    {
        UInt32 uid = (UInt32)msg.params_dict["uid"];
        if (uid == localPlayerId)
        {
            playerInfo.UpdatePlayerInfo((Int16)msg.params_dict["hp"], (UInt32)msg.params_dict["coin"], (UInt32)msg.params_dict["exp"], (UInt16)msg.params_dict["spike"], (UInt16)msg.params_dict["freeze"]);
        }
        else
        {
            //
        }
    }

    private void HandleUpdateRoundState(ref MsgSCRoundState msg)
    {
        playerInfo.baseHP = (Int16)msg.params_dict["basehp"];
        playerInfo.round = (UInt16)msg.params_dict["rid"];
    }


    // Use this for initialization
    private void Awake () {
        //DontDestroyOnLoad(this.gameObject);
        mySocket = ConnectSocket.getSocketInstance();
        playerInfo = PlayerInfo.getinstance();
        localPlayerId = playerInfo.GetPlayerId();
        monsterDict = new Dictionary<string, GameObject>();
        trapDict = new Dictionary<string, GameObject>();
        playerDict = new Dictionary<string, GameObject>();
        spawn = GameObject.FindGameObjectWithTag("Respawn").GetComponent<SpawnManager>();
    }

    private void Start()
    {
        GameStart();
    }
    // Update is called once per frame
    void Update () {

        ProcessMessage();
	}

    private void OnDisable()
    {
        mySocket.Destroy();
    }

}
