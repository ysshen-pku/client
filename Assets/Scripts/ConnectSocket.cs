using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/*
*客户端与服务端通信设施Socket
*/
public class ConnectSocket:MonoBehaviour
{

    public GameObject remotePlayer;
    public GameObject monster1;

    //private static ConnectSocket instance;
    private Socket mySocket;
    private Thread readThd = null;//接收服务器消息的线程
    private Thread sendThd = null;//send thread

    List<byte> recvBuffer;//TCP合包的缓存区
    const int receiveBufferSize = 1024; //一次接收缓存大小
    const int headSize = 4;

    Queue<byte []> sendMessageQ;
    Queue<byte []> recvMessageQ;

    // msgType
    const UInt16 MSG_CS_MOVETO = 0x1002;
    const UInt16 MSG_SC_MOVETO = 0x2002;
    const UInt16 MSG_SC_NEWPLAYER = 0x2003;

    const UInt16 MSG_CS_LOGIN = 0x1001;
    const UInt16 MSG_SC_CONFIRM = 0x2001;


    public void Send(ref Message msg)
    {
        byte[] rawmsg = msg.enpack();
        sendMessageQ.Enqueue(rawmsg);
    }

    private void Awake()
    {
        //初始化Socket
        mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        mySocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        int port = 50010;
        try
        {
            mySocket.Connect(ip, port);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }

        //连接成功
        if (mySocket.Connected)
        {
            Debug.Log("Connect Success");
            readThd = new Thread(new ThreadStart(TryRecv));
            readThd.IsBackground = true;
            readThd.Start();//启动线程
            sendThd = new Thread(new ThreadStart(TrySend));
            sendThd.IsBackground = true;
            sendThd.Start();//启动线程
            // SendMessage(data);
        }
    }

    private void Update()
    {
        ProcessMessage();
    }

    private void ProcessMessage()
    {
        // whether handle all recv msgs in one frame ?
        while (recvMessageQ.Count > 0)
        {
            byte[] rawmsg = recvMessageQ.Dequeue();
            UInt16 msgTpye = BitConverter.ToUInt16(rawmsg, 0);

            if (msgTpye == MSG_SC_MOVETO)
            {
                MsgSCMoveto msg = new MsgSCMoveto();
                msg.unpack(rawmsg);
                HandlePlayerMoveto(msg);
            }
            else if (msgTpye == MSG_SC_NEWPLAYER)
            {
                MsgSCNewPlayer msg = new MsgSCNewPlayer();
                msg.unpack(rawmsg);
                HandleNewPlayer(msg);
            }
        }
    }

    private void GetMsgtoRecvQ()
    {
        if (recvBuffer.Count < headSize)
        {
            return ;
        }
        byte[] tmp = recvBuffer.GetRange(0, headSize).ToArray();

        UInt32 msglen = BitConverter.ToUInt32(tmp,0);

        if (recvBuffer.Count < (int)msglen)
        {
            Debug.Log("Buffer size error, not enough size for msg with length:" + msglen.ToString());
            return ;
        }

        byte[] msg = recvBuffer.GetRange(headSize, (int)msglen).ToArray();

        recvMessageQ.Enqueue(msg);

        recvBuffer.RemoveRange(0, (int)msglen);
        return;
    }

    private void HandleState(MsgSCState msg)
    {
        UInt32 state = (UInt32)msg.params_dict["state"];
        if (state < SceneManager.sceneCount+1)
        {
            SceneManager.LoadScene((int)state-1);
        }

    }

    private void HandleNewPlayer(MsgSCNewPlayer msg)
    {
        Vector3 pos = new Vector3((float)msg.params_dict["x"], (float)msg.params_dict["y"]);
        UInt32 uid = (UInt32)msg.params_dict["uid"];
        //Vector3 rot = new Vector3((float)msg.params_dict["rx"], (float)msg.params_dict["ry"]);
        Quaternion rot = new Quaternion();

        GameObject[] players = GameObject.FindGameObjectsWithTag("player");
        foreach(GameObject player in players)
        {
            if(player.name == uid.ToString())
            {
                Debug.Log("trying to instantiate a existed remote player with uid:" + uid.ToString());
                return;
            }
        }

        var newplayer = Instantiate(remotePlayer, pos, rot);
        newplayer.name = uid.ToString();

    }

    private void HandlePlayerMoveto(MsgSCMoveto msg)
    {
        Vector3 vec = new Vector3((float)msg.params_dict["x"], (float)msg.params_dict["y"]);
        UInt32 uid = (UInt32)msg.params_dict["uid"];

        GameObject[] players = GameObject.FindGameObjectsWithTag("player");
        foreach (GameObject player in players)
        {
            if (player.name == uid.ToString())
            {
                player.GetComponent<Rigidbody>().velocity = vec;
                return;
            }
        }
        Debug.Log("trying to set velocity on a unexisted player with uid:" + uid.ToString());
        return;
    }


    //接收消息
    private void TryRecv()
    {
        while (true)
        {
            if (!mySocket.Connected)
            {
                Destory();
                break;
            }
            try
            { 
                byte[] buffer = new byte[receiveBufferSize];
                int receivedSize = mySocket.Receive(buffer);

                //string rawMsg = Encoding.Default.GetString(buffer, 0, receivedSize);
                //Debug.Log("rawMsg : " + rawMsg);

                recvBuffer.AddRange(buffer);
                while(recvBuffer.Count>= headSize)
                {
                    GetMsgtoRecvQ();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                Destory();
                break;
            }

        }
    }


    //发送消息
    private void TrySend()
    {
        while (true)
        {
            if (!mySocket.Connected)
            {
                Destory();
                break;
            }
            try
            {
                byte[] rawdata = sendMessageQ.Dequeue();
                object[] tmp = { rawdata.Length + headSize };
                byte[] wsize = StructConverter.Pack(tmp);
                byte[] msg = new byte[rawdata.Length + headSize];
                for (int i = 0; i < headSize; i++)
                {
                    msg[i] = wsize[i];
                }
                for (int i = 0; i < rawdata.Length; i++)
                {
                    msg[i+headSize] = rawdata[i];
                }
                Debug.Log("send message:" + msg.ToString());
                mySocket.Send(msg);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                Destory();
                break;
            }

        }
    }



    //关闭Socket和线程
    public void Destory()
    {
        mySocket.Close();
        if (null != readThd)
        {
            readThd.Abort();
        }
    }
}