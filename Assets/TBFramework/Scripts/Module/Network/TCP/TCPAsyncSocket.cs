using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;

namespace TBFramework.Net.Tcp
{
    public class TCPAsyncSocket : BaseAsyncSocket
    {
        public void Connect(string ip,int port,int byteMaxLength,E_NetOperationMode netOperationMode){
            if(isWork){
                return;
            }
            SetMaxByteAndInitIndex(byteMaxLength);
            this.netOperationMode=netOperationMode;
            if(socket==null){
                socket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            }
            isWork=true;
            switch(netOperationMode){
                case E_NetOperationMode.AsyncWithArgs:
                    ConnectWithArgs(ip,port);
                    break;
                case E_NetOperationMode.AsyncWithBegin:
                    BeginConnect(ip,port);
                    break;
            }
        }
        
        private void ConnectWithArgs(string ip, int port)
        {
            SocketAsyncEventArgs args=new SocketAsyncEventArgs();
            args.RemoteEndPoint=new IPEndPoint(IPAddress.Parse(ip),port);
            args.Completed += ConnectCallBack;
            socket.ConnectAsync(args);
        }

        private void ConnectCallBack(object socket,SocketAsyncEventArgs args){
            if(args.SocketError == SocketError.Success)
            {
                Debug.Log("连接成功!");
                //发送心跳消息;
                SendHeartMessage();
                //收消息
                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.SetBuffer(cacheBytes, cacheNum, cacheBytes.Length-cacheNum);
                receiveArgs.Completed += ReceiveWithArgs;
                (socket as Socket).ReceiveAsync(receiveArgs);
            }
            else
            {
                Debug.Log("连接失败:" + args.SocketError);
            }
        }

        private void BeginConnect(string ip, int port){
            try{
                socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip),port),ConnectCallBack,socket);
            }catch(SocketException se){
                Debug.Log($"网络连接问题:({se.SocketErrorCode}) {se.Message}!");
            }catch(Exception e){
                Debug.Log($"非网络问题:{e.Message}");
            }
            
        }

        private void ConnectCallBack(IAsyncResult result){
            try{
                Socket s=result.AsyncState as Socket;
                s.EndConnect(result);
                //发送心动消息
                SendHeartMessage();
                //开启接受消息异步函数
                s.BeginReceive(cacheBytes,cacheNum,cacheBytes.Length-cacheNum,SocketFlags.None,BeginReceive,s);
            }catch(SocketException se){
                Debug.Log($"网络连接问题({se.SocketErrorCode}):{se.Message}!");
            }catch(Exception e){
                Debug.Log($"非网络问题:{e.Message}!");
            }
        }

        private void ReceiveWithArgs(object obj, SocketAsyncEventArgs args){
            if(args.SocketError == SocketError.Success)
            {
                Socket s=obj as Socket;
                ReceiveFromBytes(args.BytesTransferred);
                //继续去收消息
                args.SetBuffer(cacheNum, args.Buffer.Length - cacheNum);
                //继续异步收消息
                if (socket != null && socket.Connected && isWork)
                    s.ReceiveAsync(args);
                else
                    Close();
            }
            else
            {
                Debug.Log("接受消息出错" + args.SocketError);
            }
        }

        private void BeginReceive(IAsyncResult result){
            try{
                Socket s=result.AsyncState as Socket;
                ReceiveFromBytes(s.EndReceive(result));
                if(socket!=null&&socket.Connected&&isWork){
                    s.BeginReceive(cacheBytes,cacheNum,cacheBytes.Length-cacheNum,SocketFlags.None,BeginReceive,s);
                }
            }catch(SocketException se){
                Debug.Log($"网络接受消息问题({se.SocketErrorCode}):{se.Message}!");
            }catch(Exception e){
                Debug.Log($"非网络问题:{e.Message}!");
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
                    sendArgs.SetBuffer(bytes,index-length,length);
                    sendArgs.Completed+=SendWithArgs;
                    socket.SendAsync(sendArgs);
                    break;
                case E_NetOperationMode.AsyncWithBegin:
                    socket.BeginSend(bytes,index-length,length,SocketFlags.None,BeginSend,socket);
                    break;
            }
        }

        private void SendWithArgs(object obj,SocketAsyncEventArgs args){
            if(args.SocketError==SocketError.Success){
                Debug.Log("消息发送成功!");
            }else{
                Debug.Log("消息发送失败!");
            }
        }

        private void BeginSend(IAsyncResult result){
            try{
                Socket s= result.AsyncState as Socket;
                s.EndSend(result);
            }catch(SocketException se){
                Debug.Log($"发送失败(网络问题 {se.SocketErrorCode}):{se.Message}!");
            }catch(Exception e){    
                Debug.Log($"发送失败(非网络问题):{e.Message}!");
            }
            
        }

        
    }
}