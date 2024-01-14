using System.Threading.Tasks;
using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace TBFramework.Net.Tcp
{
    public class TCPSyncSocket : BaseSyncSocket
    {
        public void Connect(string ip, int port,int byteMaxLength)
        {
            if(isWork){
                return;
            }
            SetMaxByteAndInitIndex(byteMaxLength);
            try{
                if(socket==null){
                    socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
                }
                socket.Connect(new IPEndPoint(IPAddress.Parse(ip),port));
                isWork=true;
                //开启发消息线程
                sendTask=Task.Run(DealWithSendMessage);
                //开启收消息线程
                receiveTask=Task.Run(DealWithReceiveMessage);
                //发送心跳消息
                SendHeartMessage();
            }catch(SocketException se){
                Debug.Log($"连接失败:({se.ErrorCode}) {se.Message}");
            }catch(Exception e){
                Debug.Log($"连接失败:{e.Message}");
            }
        }

        protected override void DealWithReceiveMessage()
        {
            while(isWork){
                if(socket.Available>0){
                    byte[] receiveBytes=new byte[byteMaxLength];
                    int receiveNum=socket.Receive(receiveBytes);
                    receiveBytes.CopyTo(cacheBytes,cacheNum);
                    ReceiveFromBytes(receiveNum);
                }
            }
        }

        protected override void DealWithSendMessage()
        {
            while(isWork){
                if(sendQueue.Count>0){
                    byte[] bytes=new byte[byteMaxLength];
                    int index=0;
                    int length=0;
                    lock(sendQueue){
                        length=MessageManager.Instance.MessageToBytes(bytes,sendQueue.Dequeue(),ref index,true);
                    }
                    socket.Send(bytes,index-length,length,SocketFlags.None);
                }
            }
        }

        
    }
}