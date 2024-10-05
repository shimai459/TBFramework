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
        //创建AudioManager之后,就自动检测不播放的音效播放器,将其放入播放器缓存池中
        public AudioManager()
        {
            MonoConManager.Instance.AddUpdateListener(CheckStopSound);
            MonoConManager.Instance.AddUpdateListener(CheckStopSound3D);
            PoolManager.Instance.SetPoolCreatNew("Sound3D", CreateSound3D);
        }

        #region 背景音乐

        //背景音乐播放器
        AudioSource music;
        //背景音乐音量
        float musicVolume = 1f;

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="musicName">音乐名</param>
        public void PlayMusic(string musicName)
        {
            this.PlayMusic((action) =>
            {
                ResourceManager.Instance.LoadAsync<AudioClip>(Path.Combine(AudioSet.MUSIC_PATH, musicName), action);
            });
        }

        public void PlayMusic(AudioClip clip)
        {
            this.PlayMusic((action) =>
            {
                action?.Invoke(clip);
            });
        }

        public void PlayMusic(string musicName, Action<string, Action<AudioClip>> action)
        {
            PlayMusic((a) =>
            {
                action?.Invoke(musicName, a);
            });
        }

        public void PlayMusic(Action<Action<AudioClip>> action)
        {
            if (music == null)
            {
                music = MonoCPoolManager.Instance.Pop<AudioSource>();
            }
            action?.Invoke((clip) =>
            {
                music.Stop();
                music.clip = clip as AudioClip;
                music.loop = true;
                music.volume = musicVolume;
                music.Play();
            });
        }

        /// <summary>
        /// 暂停音乐
        /// </summary>
        public void PauseOrPlayMusic(bool isPlay)
        {
            if (music != null)
            {
                if (isPlay)
                {
                    music.Play();
                }
                else
                {
                    music.Pause();
                }
            }
        }

        /// <summary>
        /// 停止播放音乐
        /// </summary>
        public void StopMusic()
        {
            if (music != null)
            {
                music.Stop();
                music.clip = null;
                MonoCPoolManager.Instance.Push<AudioSource>(music);
                music = null;
            }
        }

        /// <summary>
        /// 改变音乐音量
        /// </summary>
        /// <param name="value">音量值</param>
        public void ChangeMusicVolume(float value)
        {
            musicVolume = value;
            if (music != null)
            {
                music.volume = musicVolume;
            }
        }

        public E_AudioStatus GetMusicStatus()
        {
            if (music != null)
            {
                if (music.isPlaying)
                {
                    return E_AudioStatus.Play;
                }
                else
                {
                    return E_AudioStatus.Pause;
                }
            }
            else
            {
                return E_AudioStatus.Stop;
            }
        }

        #endregion

        #region 音效

        //音效音量
        float soundVolume = 1f;
        //激活的音效播放器
        List<AudioSource> soundList = new List<AudioSource>();
        //暂停的音效播放器
        List<AudioSource> pauseList = new List<AudioSource>();
        //要放回缓存池的音效播放器
        List<AudioSource> disSoundList = new List<AudioSource>();

        /// <summary>
        /// 播放音效
        /// /// </summary>
        /// <param name="soundName">音效名</param>
        /// <param name="isLoop">是否重复播放</param>
        /// <param name="callBack">播放音效后的行为</param>
        public void PlaySound(string soundName, bool isLoop = false, Action<AudioSource> callBack = null)
        {
            PlaySound((action) =>
            {
                ResourceManager.Instance.LoadAsync<AudioClip>(Path.Combine(AudioSet.SOUND_PATH, soundName), action);
            }
            , isLoop, callBack);
        }

        public void PlaySound(AudioClip clip, bool isLoop = false, Action<AudioSource> callBack = null)
        {
            PlaySound((action) => { action?.Invoke(clip); }, isLoop, callBack);
        }

        public void PlaySound(string soundName, Action<string, Action<AudioClip>> action, bool isLoop = false, Action<AudioSource> callBack = null)
        {
            PlaySound((a) =>
            {
                action?.Invoke(soundName, a);
            }, isLoop, callBack);
        }

        public void PlaySound(Action<Action<AudioClip>> action, bool isLoop = false, Action<AudioSource> callBack = null)
        {
            action?.Invoke((clip) =>
            {
                AudioSource soundAS = MonoCPoolManager.Instance.Pop<AudioSource>();
                soundAS.Stop();
                soundAS.clip = clip as AudioClip;
                soundAS.loop = isLoop;
                soundAS.volume = soundVolume;
                soundAS.Play();
                if (!soundList.Contains(soundAS))
                {
                    soundList.Add(soundAS);
                }
                if (callBack != null)
                {
                    callBack(soundAS);
                }
            });
        }

        public void PauseOrPlaySound(AudioSource soundAS, bool isPlay)
        {
            if (isPlay)
            {
                if (pauseList.Contains(soundAS))
                {
                    pauseList.Remove(soundAS);
                    soundList.Add(soundAS);
                    soundAS.Play();
                }
            }
            else
            {
                if (soundList.Contains(soundAS))
                {
                    soundList.Remove(soundAS);
                    pauseList.Add(soundAS);
                    soundAS.Pause();
                }
            }
        }

        public void PauseOrPlayAllSound(bool isPlay)
        {
            if (isPlay)
            {
                foreach (AudioSource sound in pauseList)
                {
                    soundList.Add(sound);
                    sound.Play();
                }
                pauseList.Clear();
            }
            else
            {
                foreach (AudioSource sound in soundList)
                {
                    pauseList.Add(sound);
                    sound.Pause();
                }
                soundList.Clear();
            }
        }

        /// <summary>
        /// 停止播放音效
        /// </summary>
        /// <param name="soundAS">要关闭音效播放器</param>
        public void StopSound(AudioSource soundAS)
        {
            bool haveSound = false;
            if (soundList.Contains(soundAS))
            {
                soundList.Remove(soundAS);
                haveSound = true;
            }
            else if (pauseList.Contains(soundAS))
            {
                pauseList.Remove(soundAS);
                haveSound = true;
            }
            if (haveSound)
            {
                soundAS.Stop();
                soundAS.clip = null;
                MonoCPoolManager.Instance.Push<AudioSource>(soundAS);
            }
        }

        /// <summary>
        /// 清理所有的音效
        /// </summary>
        public void StopAllSound()
        {
            foreach (AudioSource sound in soundList)
            {
                sound.Stop();
                sound.clip = null;
                MonoCPoolManager.Instance.Push<AudioSource>(sound);
            }
            soundList.Clear();
            foreach (AudioSource sound in pauseList)
            {
                sound.Stop();
                sound.clip = null;
                MonoCPoolManager.Instance.Push<AudioSource>(sound);
            }
            pauseList.Clear();
        }

        /// <summary>
        /// 改变音效音量
        /// </summary>
        /// <param name="value">音量值</param>
        public void ChangeSoundVolume(float value)
        {
            soundVolume = value;
            foreach (AudioSource soundAS in soundList)
            {
                soundAS.volume = value;
            }
            foreach (AudioSource soundAS in pauseList)
            {
                soundAS.volume = value;
            }
        }

        public E_AudioStatus GetSoundStatus(AudioSource audioSource)
        {
            if (soundList.Contains(audioSource))
            {
                return E_AudioStatus.Play;
            }
            else if (pauseList.Contains(audioSource))
            {
                return E_AudioStatus.Pause;
            }
            else
            {
                return E_AudioStatus.Stop;
            }
        }



        /// <summary>
        /// 检测停止的音效并回收
        /// </summary>
        private void CheckStopSound()
        {
            foreach (AudioSource audioSource in soundList)
            {
                if (!audioSource.isPlaying)
                {
                    MonoCPoolManager.Instance.Push<AudioSource>(audioSource);
                    disSoundList.Add(audioSource);
                }
            }
            foreach (AudioSource audioSource in disSoundList)
            {
                soundList.Remove(audioSource);
            }
            disSoundList.Clear();
        }

        #endregion

        #region 3D音效

        //音效音量
        float sound3DVolume = 1f;
        //激活的音效播放器
        List<AudioSource> sound3DList = new List<AudioSource>();
        //暂停的音效播放器
        List<AudioSource> pause3DList = new List<AudioSource>();
        //要放回缓存池的音效播放器
        List<AudioSource> disSound3DList = new List<AudioSource>();

        public void PlaySound3D(string soundName, bool isLoop = false, Action<AudioSource> callBack = null)
        {
            PlaySound3D((action) =>
            {
                ResourceManager.Instance.LoadAsync<AudioClip>(Path.Combine(AudioSet.SOUND_PATH, soundName), action);
            }
            , isLoop, callBack);
        }

        public void PlaySound3D(AudioClip clip, bool isLoop = false, Action<AudioSource> callBack = null)
        {
            PlaySound3D((action) => { action?.Invoke(clip); }, isLoop, callBack);
        }

        public void PlaySound3D(string soundName, Action<string, Action<AudioClip>> action, bool isLoop = false, Action<AudioSource> callBack = null)
        {
            PlaySound3D((a) =>
            {
                action?.Invoke(soundName, a);
            }, isLoop, callBack);
        }

        public void PlaySound3D(Action<Action<AudioClip>> action, bool isLoop = false, Action<AudioSource> callBack = null)
        {
            action?.Invoke((clip) =>
            {
                PoolManager.Instance.Pop("Sound3D", (obj) =>
                {
                    AudioSource soundAS = obj.GetComponent<AudioSource>();
                    soundAS.Stop();
                    soundAS.clip = clip as AudioClip;
                    soundAS.loop = isLoop;
                    soundAS.volume = sound3DVolume;
                    soundAS.Play();
                    if (!sound3DList.Contains(soundAS))
                    {
                        sound3DList.Add(soundAS);
                    }
                    if (callBack != null)
                    {
                        callBack(soundAS);
                    }
                });

            });
        }

        public void PauseOrPlaySound3D(AudioSource soundAS, bool isPlay)
        {
            if (isPlay)
            {
                if (pause3DList.Contains(soundAS))
                {
                    pause3DList.Remove(soundAS);
                    sound3DList.Add(soundAS);
                    soundAS.Play();
                }
            }
            else
            {
                if (sound3DList.Contains(soundAS))
                {
                    sound3DList.Remove(soundAS);
                    pause3DList.Add(soundAS);
                    soundAS.Pause();
                }
            }
        }

        public void PauseOrPlayAllSound3D(bool isPlay)
        {
            if (isPlay)
            {
                foreach (AudioSource sound in pause3DList)
                {
                    sound3DList.Add(sound);
                    sound.Play();
                }
                pause3DList.Clear();
            }
            else
            {
                foreach (AudioSource sound in sound3DList)
                {
                    pause3DList.Add(sound);
                    sound.Pause();
                }
                sound3DList.Clear();
            }
        }

        /// <summary>
        /// 停止播放音效
        /// </summary>
        /// <param name="soundAS">要关闭音效播放器</param>
        public void StopSound3D(AudioSource soundAS)
        {
            bool haveSound = false;
            if (sound3DList.Contains(soundAS))
            {
                sound3DList.Remove(soundAS);
                haveSound = true;
            }
            else if (pause3DList.Contains(soundAS))
            {
                pause3DList.Remove(soundAS);
                haveSound = true;
            }
            if (haveSound)
            {
                soundAS.Stop();
                soundAS.clip = null;
                PoolManager.Instance.Push("Sound3D", soundAS.gameObject);
            }
        }

        /// <summary>
        /// 清理所有的音效
        /// </summary>
        public void StopAllSound3D()
        {
            foreach (AudioSource sound in sound3DList)
            {
                sound.Stop();
                sound.clip = null;
                PoolManager.Instance.Push("Sound3D", sound.gameObject);
            }
            sound3DList.Clear();
            foreach (AudioSource sound in pause3DList)
            {
                sound.Stop();
                sound.clip = null;
                PoolManager.Instance.Push("Sound3D", sound.gameObject);
            }
            pause3DList.Clear();
        }

        /// <summary>
        /// 改变音效音量
        /// </summary>
        /// <param name="value">音量值</param>
        public void ChangeSound3DVolume(float value)
        {
            sound3DVolume = value;
            foreach (AudioSource soundAS in sound3DList)
            {
                soundAS.volume = value;
            }
            foreach (AudioSource soundAS in pause3DList)
            {
                soundAS.volume = value;
            }
        }

        public E_AudioStatus GetSound3DStatus(AudioSource audioSource)
        {
            if (sound3DList.Contains(audioSource))
            {
                return E_AudioStatus.Play;
            }
            else if (pause3DList.Contains(audioSource))
            {
                return E_AudioStatus.Pause;
            }
            else
            {
                return E_AudioStatus.Stop;
            }
        }



        /// <summary>
        /// 检测停止的音效并回收
        /// </summary>
        private void CheckStopSound3D()
        {
            foreach (AudioSource audioSource in sound3DList)
            {
                if (!audioSource.isPlaying)
                {
                    PoolManager.Instance.Push("Sound3D", audioSource.gameObject);
                    disSound3DList.Add(audioSource);
                }
            }
            foreach (AudioSource audioSource in disSound3DList)
            {
                sound3DList.Remove(audioSource);
            }
            disSound3DList.Clear();
        }

        private void CreateSound3D(string pool, Action<GameObject> action)
        {
            GameObject o = new GameObject();
            AudioSource s = o.AddComponent<AudioSource>();
            s.spatialBlend = 1f;
            action?.Invoke(o);
        }

        #endregion

        public void ClearAll()
        {
            StopMusic();
            StopAllSound();
            StopAllSound3D();
        }
    }
}