namespace TBFramework.Net
{
    public enum E_NetOperationMode{
        Sync,//同步使用线程
        AsyncWithArgs,//异步第一种方法SocketAsyncEventArgs
        AsyncWithBegin,//异步第二种方法使用Socket自身的方法
    }
}
