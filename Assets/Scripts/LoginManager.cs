using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class LoginManager : MonoBehaviour
{
    public Button[] login;

    private ConnectSocket mSocket;
    //private GameController gameController;
    private PlayerInfo playerInfo;
    private bool isgaming;
    private UInt32 localPlayerId;

    void Awake()
    {
        login[0].onClick.AddListener(Player1LoginCallback);
        login[1].onClick.AddListener(Player2LoginCallback);
        mSocket = ConnectSocket.getSocketInstance();
        //gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerInfo = PlayerInfo.getinstance();
    }

    void Update()
    {
        if (isgaming)
        {
            return;
        }
        byte[] rawmsg = mSocket.Recv();
        while (rawmsg != null)
        {
            UInt16 msgTpye = BitConverter.ToUInt16(rawmsg, 0);
            try
            {
                if (msgTpye == Config.MSG_SC_GAMESTART)
                {
                    MsgSCGameStart msg = new MsgSCGameStart();
                    msg.unpack(rawmsg);
                    HandleGameStart(ref msg);
                    break; 
                }
            }
            catch (Exception e)
            {
                Debug.Log("Msg unpack error" + e.ToString());
                mSocket.Destroy();
            }
            rawmsg = mSocket.Recv();
        }
    }
    // 鼠标光标管理 切换场景后显示光标
    //Cursor.visible = true;
    //Cursor.lockState = 0;


    public void Player1LoginCallback()
    {
        if (playerInfo.GetPlayerId() != 0)
            return;
        Debug.Log("Player 1 trying to login.");
        //向服务器发送登录信息
        Message msg = new MsgCSLogin(1);
        localPlayerId = 1;
        //gameController.SendMessage(ref msg);
        mSocket.Send(ref msg);
    }

    public void Player2LoginCallback()
    {
        if (playerInfo.GetPlayerId() != 0)
            return;
        Debug.Log("Player 2 trying to login.");
        //向服务器发送登录信息
        Message msg = new MsgCSLogin(2);
        localPlayerId = 2;
        //gameController.SendMessage(ref msg);
        mSocket.Send(ref msg);
    }

    public void HandleGameStart(ref MsgSCGameStart msg)
    {
        Debug.Log("Trying to load game start scene.");
        playerInfo.SetPlayerId(localPlayerId);
        Vector3 pos = new Vector3((float)(double)msg.params_dict["x"], 0, (float)(double)msg.params_dict["z"]);
        playerInfo.SetPlayerPosition(pos);
        playerInfo.UpdatePlayerInfo((Int16)msg.params_dict["hp"], 0, 0);
        SceneManager.LoadScene("main", LoadSceneMode.Single);

        isgaming = true;
    }


}

