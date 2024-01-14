namespace TBFramework
{
    /// <summary>
    /// 不继承单例模式的基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T:Singleton<T>,new()
    {
        private static T instance;
        public static T Instance{
            get{
                if(instance==null){
                    instance=new T();
                }
                    return instance;
            }
        }
        protected Singleton(){}
    }
}
