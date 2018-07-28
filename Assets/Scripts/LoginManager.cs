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
    public Button[] fastLogin;
    public InputField username;
    public InputField pass;
    public Button regitster;
    public Button login;
    public Text tipText;

    private ConnectSocket mSocket;
    //private GameController gameController;
    private PlayerInfo playerInfo;
    private bool isgaming;
    private UInt32 localPlayerId;

    void Awake()
    {
        fastLogin[0].onClick.AddListener(Player1LoginCallback);
        fastLogin[1].onClick.AddListener(Player2LoginCallback);
        mSocket = ConnectSocket.getSocketInstance();
        //gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        playerInfo = PlayerInfo.getinstance();
        regitster.onClick.AddListener(RegisterCallback);
        login.onClick.AddListener(LoginCallback);
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
                else if (msgTpye == Config.MSG_SC_CONFIRM)
                {
                    MsgSCConfirm msg = new MsgSCConfirm();
                    msg.unpack(rawmsg);
                    HandleConfirm(ref msg);
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

    public void RegisterCallback()
    {
        Debug.Log("Register click and reqst sended.");
        Message msg = new MsgCSRegister(username.text, pass.text);
        mSocket.Send(ref msg);
    }

    public void LoginCallback()
    {
        if (playerInfo.GetPlayerId() != 0)
            return;
        Debug.Log("LoginCmdSend.");
        //向服务器发送登录信息
        Message msg = new MsgCSLogin(username.text, pass.text);
        mSocket.Send(ref msg);
    }

    public void Player1LoginCallback()
    {
        if (playerInfo.GetPlayerId() != 0)
            return;
        Debug.Log("Player 1 trying to login.");
        //向服务器发送登录信息
        Message msg = new MsgCSLogin("test1","163");
        //gameController.SendMessage(ref msg);
        mSocket.Send(ref msg);
    }

    public void Player2LoginCallback()
    {
        if (playerInfo.GetPlayerId() != 0)
            return;
        Debug.Log("Player 2 trying to login.");
        //向服务器发送登录信息
        Message msg = new MsgCSLogin("test2", "163");
        //gameController.SendMessage(ref msg);
        mSocket.Send(ref msg);
    }

    public void HandleGameStart(ref MsgSCGameStart msg)
    {
        Debug.Log("Trying to load game start scene.");
        playerInfo.SetPlayerId((UInt32)msg.params_dict["uid"]);
        Vector3 pos = new Vector3((float)(double)msg.params_dict["x"], 0, (float)(double)msg.params_dict["z"]);
        playerInfo.SetPlayerPosition(pos);
        playerInfo.UpdatePlayerInfo(0,(Int16)msg.params_dict["hp"], 0, 0, 0,0);
        SceneManager.LoadScene("main", LoadSceneMode.Single);

        isgaming = true;
    }

    public void HandleConfirm(ref MsgSCConfirm msg)
    {
        UInt32 result = (UInt32)msg.params_dict["result"];
        if (result == Config.RESPONSE_NON_EXIST_NAME)
            tipText.text = "Username is not register!";
        else if (result == Config.RESPONSE_WRONG_PASSWORD)
            tipText.text = "Password is wrong!";
        else if (result == Config.RESPONSE_EXIST_NAME)
            tipText.text = "Username is existed!";
        else if (result == Config.RESPONSE_EXIST_PLAYER)
            tipText.text = "User is playing online!";
        else if (result == Config.RESPONSE_REGISTER_SUCCESS)
            tipText.text = "Register success!";
    }

}

