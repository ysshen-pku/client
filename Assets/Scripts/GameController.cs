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
    bool isLoginScene;

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
            else if (msgTpye == Config.MSG_SC_LOGIN)
            {
                MsgSCLogin msg = new MsgSCLogin();
                msg.unpack(rawmsg);
                HandleGameStart(ref msg);
            }
            rawmsg = mySocket.Recv();
        }
    }

    private void HandleGameStart(ref MsgSCLogin msg)
    {
        //SceneManager.LoadScene(1, LoadSceneMode.Single);
        //isLoginScene = false;
        double x= (double)msg.params_dict["x"],z= (double)msg.params_dict["z"];
        Vector3 pos = new Vector3((float)x,0, (float)z);

        spawn.InstantiatePlayer(pos);

    }

    private void HandleNewPlayer(ref MsgSCNewPlayer msg)
    {
        Vector3 pos = new Vector3((float)(double)msg.params_dict["x"],0, (float)(double)msg.params_dict["z"]);
        UInt32 uid = (UInt32)msg.params_dict["uid"];
        //Vector3 rot = new Vector3((float)(double)msg.params_dict["rx"], (float)(double)msg.params_dict["ry"]);
        if (uid == localPlayerId)
            return;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.name == uid.ToString())
            {
                Debug.Log("trying to instantiate a existed remote player with uid:" + uid.ToString());
                return;
            }
        }

        spawn.InstantiateRemotePlayer(uid.ToString(), pos);

    }

    private void HandlePlayerMoveto(ref MsgSCMoveto msg)
    {
        Vector3 vec = new Vector3((float)(double)msg.params_dict["x"],0, (float)(double)msg.params_dict["z"]);
        UInt32 uid = (UInt32)msg.params_dict["uid"];

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        string objectName = "player" + uid.ToString();
        foreach (GameObject player in players)
        {
            if (player.name == objectName)
            {
                player.GetComponent<AICharactorController>().SetDestination(vec);
                return;
            }
        }
        Debug.Log("trying to set velocity on a unexisted player with uid:" + uid.ToString());
        return;
    }

    // Use this for initialization
    private void Start () {
        DontDestroyOnLoad(this.gameObject);
        mySocket = ConnectSocket.getSocketInstance();
        isLoginScene = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (spawn == null)
        {
            spawn = GameObject.FindGameObjectWithTag("Respawn").GetComponent<SpawnManager>();
        }
        ProcessMessage();
	}

    private void OnDisable()
    {
        mySocket.Destroy();
    }

}
