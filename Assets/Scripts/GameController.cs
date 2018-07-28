using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameController : MonoBehaviour {

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
                else if (msgTpye == Config.MSG_SC_OBJECT_DELETE)
                {
                    MsgSCObjectDelete msg = new MsgSCObjectDelete();
                    msg.unpack(rawmsg);
                    HandleObjectDelete(ref msg);
                }
                else if (msgTpye == Config.MSG_SC_TRAPPLACE)
                {
                    MsgSCTrapPlace msg = new MsgSCTrapPlace();
                    msg.unpack(rawmsg);
                    HandleTrapPlace(ref msg);
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
        playerInfo.playerstate = 0;
        spawn.InstantiatePlayer(playerInfo.GetPlayerPosition());
    }

    private void HandleNewPlayer(ref MsgSCNewPlayer msg)
    {
        Vector3 pos = new Vector3((float)(double)msg.params_dict["x"],0, (float)(double)msg.params_dict["z"]);
        UInt32 uid = (UInt32)msg.params_dict["uid"];
        //Vector3 rot = new Vector3((float)(double)msg.params_dict["rx"], (float)(double)msg.params_dict["ry"]);
        if (uid == playerInfo.GetPlayerId())
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
            UInt16 mtype = (UInt16)msg.params_dict["mtype"];
            Quaternion rot = new Quaternion();
            GameObject thismonster = spawn.InstantiateMonster(monstername,mtype, pos, rot);
            monsterDict.Add(monstername, thismonster);
        }
    }

    private void HandleMonsterState(ref MsgSCMonsterState msg)
    {
        UInt32 mid = (UInt32)msg.params_dict["mid"];
        Int16 hp = (Int16)msg.params_dict["hp"];
        UInt16 state = (UInt16)msg.params_dict["state"];
        UInt32 taruid = (UInt32)msg.params_dict["taruid"];
        string monstername = "m" + mid.ToString();
        if (monsterDict.ContainsKey(monstername))
        {
            AICharactorController monsterAI = monsterDict[monstername].GetComponent<AICharactorController>();
            monsterAI.UpdateState(hp, state);
            // attack cmd
            if (taruid != 0)
            {
                String targetname;
                if (taruid == playerInfo.GetPlayerId())
                    targetname = "localplayer";
                else
                    targetname = "player" + taruid.ToString();
                monsterAI.ThrowBox(GameObject.Find(targetname));
            }
        }
    } 

    private void HandleObjectDelete(ref MsgSCObjectDelete msg)
    {
        UInt16 type = (UInt16)msg.params_dict["type"];
        UInt32 id = (UInt32)msg.params_dict["id"];
        if (type == 1)
        {
            if (id == playerInfo.GetPlayerId())
            {
                //on local player death
                GameObject player = GameObject.Find("localplayer");
                Camera[] cams = Camera.allCameras;
                foreach(Camera cam in cams)
                {
                    if (cam.name == "ThirdPersonCamera")
                    {
                        cam.enabled= true;
                    }
                }
                playerInfo.gameState = Config.PLAYER_STATE_DEAD;
                Debug.Log("player dead");
            }
            else
            {
                string name = "player" + id.ToString();
                if (playerDict.ContainsKey(name))
                {
                    playerDict[name].GetComponent<RemotePlayerController>().OnLeave();
                    playerDict.Remove(name);
                }
            }
        }
        else if (type == 2)
        {
            string name = "m" + id.ToString();
            if (monsterDict.ContainsKey(name))
            {
                monsterDict[name].GetComponent<AICharactorController>().OnDeath();
                monsterDict.Remove(name);
            }
        }
        else if (type == 3)
        {
            string name = "t" + id.ToString();
            if (trapDict.ContainsKey(name))
            {
                Destroy(trapDict[name]);
                trapDict.Remove(name);
            }
        }
    }

    private void HandleTrapPlace(ref MsgSCTrapPlace msg)
    {
        UInt32 tid = (UInt32)msg.params_dict["tid"];
        string trapname = "m" + tid.ToString();
        if (trapDict.ContainsKey(trapname))
        {
            return;
        }
        else
        {
            GameObject trap = spawn.InstantiateTrap(trapname, (UInt16)msg.params_dict["type"], (float)(double)msg.params_dict["x"], (float)(double)msg.params_dict["z"]);
            trapDict.Add(trapname, trap);
        }
            }

    private void HandleUpdatePlayerInfo(ref MsgSCPlayerInfo msg)
    {
        UInt32 uid = (UInt32)msg.params_dict["uid"];
        UInt16 state = (UInt16)msg.params_dict["state"];
        if (uid == playerInfo.GetPlayerId())
        {
            if (state == Config.PLAYER_STATE_DEAD)
            {
                playerInfo.playerstate = Config.PLAYER_STATE_DEAD;
            }
            playerInfo.UpdatePlayerInfo((UInt16)msg.params_dict["level"], (Int16)msg.params_dict["hp"], (UInt32)msg.params_dict["coin"], (UInt32)msg.params_dict["exp"], (UInt16)msg.params_dict["spike"], (UInt16)msg.params_dict["freeze"]);
        }
    }

    private void HandleUpdateRoundState(ref MsgSCRoundState msg)
    {
        playerInfo.baseHP = (Int16)msg.params_dict["basehp"];
        playerInfo.round = (UInt16)msg.params_dict["rid"];
        playerInfo.gameState = (UInt16)msg.params_dict["gamestate"];
        playerInfo.remainMonster = (UInt16)msg.params_dict["remain"];
    }


    // Use this for initialization
    private void Awake () {
        //DontDestroyOnLoad(this.gameObject);
        mySocket = ConnectSocket.getSocketInstance();
        playerInfo = PlayerInfo.getinstance();
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
