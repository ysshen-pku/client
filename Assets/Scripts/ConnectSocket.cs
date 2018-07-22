using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;

/*
*客户端与服务端通信设施Socket
*/
public class ConnectSocket
{

    private static ConnectSocket instance;
    private Socket mySocket;
    private Thread readThd = null;//接收服务器消息的线程
    private Thread sendThd = null;//send thread

    List<byte> recvBuffer;//TCP合包的缓存区
    const int receiveBufferSize = 1024; //一次接收缓存大小

    const int headSize = 4;
    static bool ongaming;

    Queue<Message> sendMessageQ;
    Queue<byte[]> recvMessageQ;

    public static ConnectSocket getSocketInstance()
    {
        if (instance == null)
        {
            instance = new ConnectSocket();
        }
        return instance;
    }

    public void Send(ref Message msg)
    {
        sendMessageQ.Enqueue(msg);
    }

    public byte[] Recv()
    {
        if (recvMessageQ.Count > 0)
        {
            return recvMessageQ.Dequeue();
        }
        return null;
    }

    ConnectSocket()
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
            sendMessageQ = new Queue<Message>();
            recvMessageQ = new Queue<byte[]>();
            ongaming = true;

            recvBuffer = new List<byte>();
            readThd = new Thread(new ThreadStart(TryRecv));
            readThd.IsBackground = true;
            readThd.Start();//启动线程
            sendThd = new Thread(new ThreadStart(TrySend));
            sendThd.IsBackground = true;
            sendThd.Start();//启动线程

            
            // SendMessage(data);
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

        byte[] msg = recvBuffer.GetRange(headSize, (int)msglen - headSize).ToArray();


        recvMessageQ.Enqueue(msg);

        recvBuffer.RemoveRange(0, (int)msglen);
        return;
    }

    //接收消息
    private void TryRecv()
    {

        byte[] buffer = new byte[receiveBufferSize];
        while (ongaming)
        {
            try
            {
                bool state = (mySocket.Poll(1, SelectMode.SelectRead) && mySocket.Available == 0);

                int receivedSize = mySocket.Receive(buffer);
                //string rawMsg = Encoding.Default.GetString(buffer, 0, receivedSize);
                //Debug.Log("rawMsg : " + rawMsg);
                if (receivedSize > 0)
                {
                    for(int i = 0; i < receivedSize; i++) 
                        recvBuffer.Add(buffer[i]);
                    while (recvBuffer.Count >= headSize)
                    {
                        GetMsgtoRecvQ();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Recv error"+e.ToString());
                ongaming = false;
                buffer = null;
                Destroy();
                break;
            }

        }
        Debug.Log("read thread aborted");
    }


    //发送消息
    private void TrySend()
    {
        while (ongaming)
        {
            if (sendMessageQ.Count == 0)
                continue;
            /*
            GameObject [] controller = GameObject.FindGameObjectsWithTag("GameController");
            if (controller.Length == 0)
            {
                ongaming = false;
                Destroy();
                continue;
            }
            */
            try
            {
                bool state = (mySocket.Poll(1, SelectMode.SelectRead) && mySocket.Available == 0);

                Message msg = sendMessageQ.Dequeue();
                byte[] rawdata = msg.enpack();
                object[] tmp = { rawdata.Length + headSize };
                byte[] wsize = StructConverter.Pack(tmp);
                byte[] rawmsg = new byte[rawdata.Length + headSize];
                for (int i = 0; i < headSize; i++)
                {
                    rawmsg[i] = wsize[i];
                }
                for (int i = 0; i < rawdata.Length; i++)
                {
                    rawmsg[i+headSize] = rawdata[i];
                }
                //Debug.Log("send message:" + msg.ToString());
                mySocket.Send(rawmsg);
            }
            catch (Exception e)
            {
                ongaming = false;
                Debug.Log("Send error"+e.ToString());
                Destroy();
                break;
            }

        }
        Debug.Log("send thread aborted");
    }



    //关闭Socket和线程
    public void Destroy()
    {
        ongaming = false;
        mySocket.Close();
    }
}