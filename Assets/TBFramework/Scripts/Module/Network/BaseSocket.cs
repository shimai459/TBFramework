using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TBFramework.Net
{
    public abstract class BaseScoket
    {
        //套接字
        protected Socket socket;

        //是否在工作
        protected bool isWork;

        //接受到的消息队列
        protected Queue receiveQueue=new Queue();

        //字节的最长长度
        protected int byteMaxLength;

        //存接受到消息字节
        protected byte[] cacheBytes;
        
        //数据已经存储到数组的什么位置
        protected int cacheNum=0;

        //发送心跳消息的时间间隔(单位毫秒)
        protected int heartMesageSendTimeInterval=2000;

        protected void SetMaxByteAndInitIndex(int byteMaxLength){
            this.byteMaxLength=byteMaxLength;
            if(cacheBytes==null||cacheBytes.Length!=byteMaxLength){
                cacheBytes=new byte[byteMaxLength];
            }
            cacheNum=0;
        }

        /// <summary>
        /// 添加要发送的消息
        /// </summary>
        /// <param name="data"></param>
        public abstract void Send(object data);

        /// <summary>
        /// 发送心跳消息的函数
        /// </summary>
        /// <returns></returns>
        protected async void SendHeartMessage(){
            while(isWork){
                Send(HeartMessage.Instance);
                await Task.Delay(heartMesageSendTimeInterval);
            }
        }

        /// <summary>
        /// 读取网络传输的字节数据成类对象的函数
        /// </summary>
        /// <param name="length"></param>
        protected void ReceiveFromBytes(int length){
            cacheNum+=length;
            int index=0;
            while(true){
                object message=MessageManager.Instance.MessageFromBytes(cacheBytes,ref index,ref cacheNum);
                if(message!=null){
                    Debug.Log(message);
                    receiveQueue.Enqueue(message);
                    cacheNum-=index;
                    Array.Copy(cacheBytes,index,cacheBytes,0,cacheNum);
                }else{
                    break;
                }
            }
        }

        /// <summary>
        /// 关闭套接字的函数
        /// </summary>
        public virtual void Close(){
            if(socket!=null){
                isWork=false;
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket=null;
            }
        }

    }
}