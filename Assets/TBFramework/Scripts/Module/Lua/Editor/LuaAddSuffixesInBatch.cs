using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TBFramework.Lua
{
    public class LuaAddSuffixesInBatch : Editor
    {
        [MenuItem("Automation/Lua/LuaAddSuffixesInBatch")]
        public static void CopyLuaToTxt(){
            Debug.Log(System.IO.Path.DirectorySeparatorChar);
            List<string> luaPathList = new List<string>(){
                //所以Lua路径都写在这
                System.IO.Path.Combine( Application.dataPath,"TBFramework","Lua"),
            };
            //新生成的以.txt为后缀的lua文件存储的文件夹
            string newLuaPath= System.IO.Path.Combine(Application.dataPath,"TBFramework","LuaAddSuffixes");
            //LuaAB包名
            string luaABName="lua";
            //判断新Lua文件夹是否存在,不存在就创建
            if(!Directory.Exists(newLuaPath)){
                Directory.CreateDirectory(newLuaPath);
            }else{
                string[] oldLua=Directory.GetFiles(newLuaPath,".txt");
                foreach(string lua in oldLua){
                    File.Delete(lua);
                }
            }
            //找到所有Lua文件
            //拷贝Lua文件到指定路径并添加后缀
            string newFileName;
            List<string> newFiles=new List<string>();
            foreach(string path in luaPathList){
                if(!Directory.Exists(path)){
                    continue;
                }
                string[] luas=Directory.GetFiles(path,"*.lua");
                foreach(string lua in luas){
                    newFileName= System.IO.Path.Combine(newLuaPath, lua.Substring(lua.LastIndexOf(System.IO.Path.DirectorySeparatorChar)+1)+".txt");
                    newFiles.Add(newFileName);
                    File.Copy(lua,newFileName);
                }
            }
            AssetDatabase.Refresh();
            foreach(string lua in newFiles){
                AssetImporter importer=AssetImporter.GetAtPath(lua.Substring(lua.IndexOf("Assets")));
                if(importer!=null){
                    importer.assetBundleName=luaABName;
                }
            }
        }
    }
}