using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TBFramework.Net
{
    public abstract class BaseSyncSocket : BaseScoket
    {
        //要发送的消息队列
        protected Queue sendQueue=new Queue();
        //发送消息线程
        protected Task sendTask;
        //接受消息线程
        protected Task receiveTask;

        public override void Send(object data)
        {
            lock(sendQueue){
                sendQueue.Enqueue(data);
            }
        }

        /// <summary>
        /// 处理发送消息的函数
        /// </summary>
        protected abstract void DealWithSendMessage();

        /// <summary>
        /// 处理接受数据的函数
        /// </summary>
        protected abstract void DealWithReceiveMessage();
    }
}