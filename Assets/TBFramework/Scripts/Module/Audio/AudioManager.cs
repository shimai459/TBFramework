using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TBFramework.Pool;
using TBFramework.Mono;
using TBFramework.Resource;

namespace TBFramework.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        //背景音乐播放器
        AudioSource Music;
        //背景音乐音量
        float musicVolume=1f;
        //音效音量
        float soundVolume=1f;
        //激活的音效播放器
        List<AudioSource> soundList=new List<AudioSource>();
        //要放回缓存池的音效播放器
        List<AudioSource> disSoundList =new List<AudioSource>();
        
        //创建AudioManager之后,就自动检测不播放的音效播放器,将其放入播放器缓存池中
        public AudioManager(){
            MonoManager.Instance.AddUpdateListener(()=>{
                foreach(AudioSource audioSource in soundList){
                    if(!audioSource.isPlaying){
                        CPoolManager.Instance.Push<AudioSource>(audioSource);
                        disSoundList.Add(audioSource);
                    }
                }
                foreach(AudioSource audioSource in disSoundList){
                    soundList.Remove(audioSource);
                }
                disSoundList.Clear();
            });
        }
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="musicName">音乐名</param>
        public void PlayMusic(string musicName){
            if(Music==null){
                Music=CPoolManager.Instance.Pop<AudioSource>();
            }
            ResourceManager.Instance.LoadAsync<AudioClip>(Path.Combine(AudioSet.MUSIC_PATH,musicName),(clip)=>{
                Music.clip=clip as AudioClip;
                Music.loop=true;
                Music.volume=musicVolume;
                Music.Play();
            });
            
        }
        /// <summary>
        /// 暂停音乐
        /// </summary>
        public void PauseMusic(){
            if(Music!=null){
                Music.Pause();
            }
        }
        /// <summary>
        /// 停止播放音乐
        /// </summary>
        public void StopMusic(){
            if(Music!=null){
                Music.Stop();
                CPoolManager.Instance.Push<AudioSource>(Music);
                Music=null;
            }
        }
        /// <summary>
        /// 改变音乐音量
        /// </summary>
        /// <param name="value">音量值</param>
        public void ChangeMusicVolume(float value){
            musicVolume=value;
            if(Music!=null){
                Music.volume=musicVolume;
            }
        }
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="soundName">音效名</param>
        /// <param name="isLoop">是否重复播放</param>
        /// <param name="callBack">播放音效后的行为</param>
        public void PlaySound(string soundName,bool isLoop=false,Action<AudioSource> callBack=null){
            
            ResourceManager.Instance.LoadAsync<AudioClip>(Path.Combine(AudioSet.SOUND_PATH,soundName),(clip)=>{
                AudioSource soundAS=CPoolManager.Instance.Pop<AudioSource>();
                soundAS.clip=clip as AudioClip;
                soundAS.loop=isLoop;
                soundAS.volume=soundVolume;
                soundAS.Play();
                soundList.Add(soundAS);
                if(callBack!=null){
                    callBack(soundAS);
                }
            });
        }
        /// <summary>
        /// 改变音效音量
        /// </summary>
        /// <param name="value">音量值</param>
        public void ChangeSoundVolume(float value){
            soundVolume=value;
            foreach(AudioSource soundAS in soundList){
                soundAS.volume=value;
            }
        }
        /// <summary>
        /// 停止播放音效
        /// </summary>
        /// <param name="soundAS">要关闭音效播放器</param>
        public void StopSound(AudioSource soundAS){
            if(soundList.Contains(soundAS)){
                soundList.Remove(soundAS);
                soundAS.Stop();
                CPoolManager.Instance.Push<AudioSource>(soundAS);
            }
        }
        /// <summary>
        /// 停止播放音效
        /// </summary>
        /// <param name="index">要关闭音效播放器的索引</param>
        public void StopSound(int index){
            if(index>=0&&index<soundList.Count){
                soundList[index].Stop();
                CPoolManager.Instance.Push<AudioSource>(soundList[index]);
                soundList.Remove(soundList[index]);
            }
        }
    }
}