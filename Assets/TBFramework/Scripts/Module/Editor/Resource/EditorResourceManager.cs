using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TBFramework.Editor.Resource
{
    public class EditorResourceManager : Singleton<EditorResourceManager>
    {
        /// <summary>
        /// 加载单个资源的泛型方法
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            string fullPath = Path.Combine(EditorResourceSet.EDITOR_RES_PATH, path) + GetSuffix(typeof(T));
            T res = AssetDatabase.LoadAssetAtPath<T>(fullPath);
            return res;
#else 
            return null;
#endif
        }

        /// <summary>
        /// 加载单个资源的类型方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public UnityEngine.Object Load(string path, Type type)
        {
#if UNITY_EDITOR
            string fullPath = Path.Combine(EditorResourceSet.EDITOR_RES_PATH, path) + GetSuffix(type);
            UnityEngine.Object res = AssetDatabase.LoadAssetAtPath(fullPath, type);
            return res;
#else
            return null;
#endif
        }

        /// <summary>
        /// 加载一个目录下的所有资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<string, UnityEngine.Object> LoadAll(string path)
        {
#if UNITY_EDITOR
            string fullPath = Path.Combine(EditorResourceSet.EDITOR_RES_PATH, path);
            Dictionary<string, UnityEngine.Object> res = new Dictionary<string, UnityEngine.Object>();
            UnityEngine.Object[] resObjs = AssetDatabase.LoadAllAssetsAtPath(fullPath);
            foreach (var resObj in resObjs)
            {
                res.Add(resObj.name, resObj);
            }
            return res;
#else
            return null;
#endif
        }

        /// <summary>
        /// 加载一个图集中的一张图
        /// </summary>
        /// <param name="path"></param>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite LoadSprite(string path, string spriteName)
        {
#if UNITY_EDITOR
            UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(Path.Combine(EditorResourceSet.EDITOR_RES_PATH, path));
            foreach (var sprite in sprites)
            {
                if (sprite.name == spriteName)
                {
                    return sprite as Sprite;
                }
            }
            return null;
#else
            return null;
#endif
        }

        /// <summary>
        /// 加载一个图集中的所有图
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<string, Sprite> LoadSprites(string path)
        {
#if UNITY_EDITOR
            Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
            UnityEngine.Object[] spriteObjs = AssetDatabase.LoadAllAssetRepresentationsAtPath(Path.Combine(EditorResourceSet.EDITOR_RES_PATH, path));
            foreach (var spriteObj in spriteObjs)
            {
                sprites.Add(spriteObj.name, spriteObj as Sprite);
            }
            return sprites;
#else
            return null;
#endif
        }

        /// <summary>
        /// 通过资源类型获取后缀名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetSuffix(Type type)
        {
#if UNITY_EDITOR
            string suffix = ".";
            if (type == typeof(GameObject))
            {
                suffix += "prefab";
            }
            else if (type == typeof(Texture2D))
            {
                suffix += "png";
            }
            else if (type == typeof(Sprite))
            {
                suffix += "png";
            }
            else if (type == typeof(Texture))
            {
                suffix = "png";
            }
            else if (type == typeof(Material))
            {
                suffix = "mat";
            }
            else if (type == typeof(AudioClip))
            {
                suffix = "mp3";
            }
            return suffix;
#else
            return null;
#endif
        }

    }
}

