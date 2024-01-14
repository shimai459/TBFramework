using System.Threading.Tasks;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;

namespace TBFramework.Net.Udp
{
    public class UDPSyncSocket : BaseSyncSocket
    {
        private IPEndPoint ServiceIP;
        public void Connect(string serviceIP, int servicePort,string localIP,int localPort,int byteMaxLength)
        {
            if(isWork){
                return;
            }
            SetMaxByteAndInitIndex(byteMaxLength);
            try{
                if(socket==null){
                    socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
                }
                ServiceIP=new IPEndPoint(IPAddress.Parse(serviceIP),servicePort);
                socket.Bind(new IPEndPoint(IPAddress.Parse(localIP),localPort));
                isWork=true;
                sendTask=Task.Run(DealWithSendMessage);
                receiveTask=Task.Run(DealWithReceiveMessage);
                SendHeartMessage();
            }catch(SocketException se){
                Debug.Log($"网络问题({se.SocketErrorCode}):{se.Message}!");
            }catch(Exception e){
                Debug.Log($"非网络问题:{e.Message}!");
            }
            
        }

        protected override void DealWithReceiveMessage()
        {
            while(isWork){
                if(socket!=null&&socket.Available>0){
                    try{
                        EndPoint ip=new IPEndPoint(IPAddress.Any,0);
                        int length=socket.ReceiveFrom(cacheBytes,cacheNum,cacheBytes.Length-cacheNum,SocketFlags.None,ref ip);
                        if(ip.Equals(ServiceIP)){
                            ReceiveFromBytes(length);
                        }
                    
                    }catch(SocketException se){
                        Debug.Log($"网络问题({se.SocketErrorCode}):{se.Message}!");
                    }catch(Exception e){
                        Debug.Log($"非网络问题:{e.Message}!");
                    }
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
                    try{
                        lock(sendQueue){
                            length=MessageManager.Instance.MessageToBytes(bytes,sendQueue.Dequeue(),ref index,true);
                        }
                        socket.SendTo(bytes,index-length,length,SocketFlags.None,ServiceIP);
                    }catch(SocketException se){
                        Debug.Log($"网络问题({se.SocketErrorCode}):{se.Message}!");
                    }catch(Exception e){
                        Debug.Log($"非网络问题:{e.Message}!");
                    }
                }
            }
        }
    }
}