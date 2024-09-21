using System;
using System.Reflection;
using UnityEngine;

namespace TBFramework
{
    /// <summary>
    /// 不继承单例模式的基类,创建类使用反射创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SingletonReflect<T> where T : SingletonReflect<T>
    {
        private static T instance;

        protected static readonly object lockObj = new object();
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                                   null,
                                                                   Type.EmptyTypes,
                                                                   null);
                        if (info != null)
                        {
                            instance = info.Invoke(null) as T;
                        }
                        else
                        {
                            Debug.LogError($"类{type.Name}获取不到对应的无参构造函数");
                        }
                    }
                }
                return instance;
            }
        }
    }
}