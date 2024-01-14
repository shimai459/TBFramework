using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace TBFramework.Net.Udp
{

    public class UDPAsyncSocket : BaseAsyncSocket
    {
        private IPEndPoint ServiceIP;
        
        public void Connect(string serviceIP,int servicePort,string localIP,int localPort,int bytesMaxLength,E_NetOperationMode netOperationMode){
            if(isWork){
                return;
            }
            SetMaxByteAndInitIndex(bytesMaxLength);
            this.netOperationMode=netOperationMode;
            try{
                if(socket==null){
                    socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
                }
                ServiceIP=new IPEndPoint(IPAddress.Parse(serviceIP),servicePort);
                isWork=true;
                socket.Bind(new IPEndPoint(IPAddress.Parse(localIP),localPort));
                switch(netOperationMode){
                    case E_NetOperationMode.AsyncWithArgs:
                        ConnectWithArgs();
                        break;
                    case E_NetOperationMode.AsyncWithBegin:
                        BeginConnect();
                        break;
                }
                SendHeartMessage();
            }catch(SocketException se){
                Debug.Log($"网络问题({se.SocketErrorCode}):{se.Message}!");
            }catch(Exception e){
                Debug.Log($"非网络问题:{e.Message}!");
            }
        }

        private void ConnectWithArgs(){
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.SetBuffer(cacheBytes, cacheNum, cacheBytes.Length-cacheNum);
            args.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            args.Completed += ReceiveFromWithArgs;
            socket.ReceiveFromAsync(args);
        }

        private void BeginConnect(){
            EndPoint receiveIP=new IPEndPoint(IPAddress.Any, 0);
            socket.BeginReceiveFrom(cacheBytes, cacheNum, cacheBytes.Length-cacheNum,SocketFlags.None,ref receiveIP,BeginReceiveFrom,(socket,receiveIP));
        }

        private void ReceiveFromWithArgs(object socket,SocketAsyncEventArgs args){
            if(args.RemoteEndPoint.Equals(ServiceIP)){
                Socket s=socket as Socket;
                ReceiveFromBytes(args.BytesTransferred);
                args.SetBuffer(cacheNum, args.Buffer.Length - cacheNum);
                if(socket!=null&&isWork){
                    s.ReceiveFromAsync(args);
                }else{
                    Close();
                }
            }
        }

        private void BeginReceiveFrom(IAsyncResult result){
            (Socket s,EndPoint receiveIP) info=((Socket,EndPoint))result.AsyncState;
            int length= info.s.EndReceiveFrom(result,ref info.receiveIP);
            if(info.receiveIP.Equals(ServiceIP)){
                ReceiveFromBytes(length);
                if(socket!=null&&isWork){
                    info.s.BeginReceiveFrom(cacheBytes, cacheNum, cacheBytes.Length-cacheNum,SocketFlags.None,ref info.receiveIP,BeginReceiveFrom,socket);
                }else{
                    Close();
                }
            }
        }

        public override void Send(object message)
        {
            byte[] bytes=new byte[byteMaxLength];
            int index=0;
            int length=MessageManager.Instance.MessageToBytes(bytes,message,ref index,true);
            switch(netOperationMode){
                case E_NetOperationMode.AsyncWithArgs:
                    SocketAsyncEventArgs sendArgs=new SocketAsyncEventArgs();
                    sendArgs.RemoteEndPoint=ServiceIP;
                    sendArgs.SetBuffer(bytes,index-length,length);
                    sendArgs.Completed+=SendToWithArgs;
                    socket.SendToAsync(sendArgs);
                    break;
                case E_NetOperationMode.AsyncWithBegin:
                    socket.BeginSendTo(bytes,index-length,length,SocketFlags.None,ServiceIP,BeginSendTo,socket);
                    break;
            }
        }

        private void SendToWithArgs(object socket,SocketAsyncEventArgs args){
            if(args.SocketError==SocketError.Success){
                Debug.Log("消息发送成功!");
            }else{
                Debug.Log("消息发送失败!");
            }
        }

        private void BeginSendTo(IAsyncResult result){
            try{
                Socket s= result.AsyncState as Socket;
                s.EndSendTo(result);
            }catch(SocketException se){
                Debug.Log($"发送失败(网络问题 {se.SocketErrorCode}):{se.Message}!");
            }catch(Exception e){    
                Debug.Log($"发送失败(非网络问题):{e.Message}!");
            }
        }
    }
}