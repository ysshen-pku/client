using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class LoginManager : MonoBehaviour
{
    public Button[] login;

    private GameController gamelogic;

    void Start()
    {
        login[0].onClick.AddListener(Player1LoginCallback);
        login[1].onClick.AddListener(Player2LoginCallback);
        
        gamelogic = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>(); 
    }

    void Update()
    {

        // 鼠标光标管理 切换场景后显示光标
        //Cursor.visible = true;
        //Cursor.lockState = 0;
    }

    public void Player1LoginCallback()
    {
        if (gamelogic.localPlayerId != 0)
            return;
        Debug.Log("Player 1 trying to login.");
        //向服务器发送登录信息
        Message msg = new MsgCSLogin(1);
        gamelogic.localPlayerId = 1;
        gamelogic.SendMessage(ref msg);
    }

    public void Player2LoginCallback()
    {
        if (gamelogic.localPlayerId != 0)
            return;
        Debug.Log("Player 2 trying to login.");
        //向服务器发送登录信息
        Message msg = new MsgCSLogin(2);
        gamelogic.localPlayerId = 2;
        gamelogic.SendMessage(ref msg);
    }

    public void TryStartGame()
    {
        Debug.Log("Trying to load game start scene.");
        //SceneManager.LoadScene(1);
    }
}

