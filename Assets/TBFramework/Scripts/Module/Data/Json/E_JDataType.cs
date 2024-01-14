namespace TBFramework.Data.Json
{
    //使用Json存储数据时,使用的序列化和反序列的方式
    public enum E_JDataType{
        //使用unity自带的Json序列化类
        JsonUtility,
        //使用第三方LitJson的Json序列化类
        LitJson,
    }
}
