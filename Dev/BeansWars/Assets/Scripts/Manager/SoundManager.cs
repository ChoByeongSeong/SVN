using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YellowBean
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        private Dictionary<string, AudioClip> dicBgm = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> dicSfx = new Dictionary<string, AudioClip>();
       
        public GameObject selectModeOption;
        public GameObject playPause;

        public AudioSource bgm;
        Stack<AudioSource> stackSfx;

        public bool bgmMute = false;
        public bool sfxMute = false;

        GameObject goSfxPool;

        private void Awake()
        {
            Instance = this;
        }

        public void Init()
        {            
            string bgmFildPath = "Sound/Bgm";
            string sfxFilePath = "Sound/Sfx";       

            var arrBgm = Resources.LoadAll<AudioClip>(bgmFildPath);
            for (int i = 0; i < arrBgm.Length; i++)
            {
                dicBgm.Add(arrBgm[i].name, arrBgm[i]);

                //Debug.Log(arrBgm[i].name);
            }

            var arrSfx = Resources.LoadAll<AudioClip>(sfxFilePath);
            for (int i = 0; i < arrSfx.Length; i++)
            {
                dicSfx.Add(arrSfx[i].name, arrSfx[i]);             
            }         

            // BGM 오디오 소스
            {
                var goBgm = new GameObject();
                bgm = goBgm.AddComponent<AudioSource>();
                goBgm.name = "BGM Audio";
                goBgm.SetActive(false);

                goBgm.transform.SetParent(transform);
            }

            // SFX 오디오 풀
            {
                goSfxPool = new GameObject();
                goSfxPool.name = "SFX Pool";
                goSfxPool.transform.SetParent(transform);

                stackSfx = new Stack<AudioSource>();

                for (int i = 0; i < 30; i++)
                {
                    var goSfx = new GameObject();
                    var audioSource = goSfx.AddComponent<AudioSource>();
                    goSfx.name = "SFX Audio";
                    goSfx.SetActive(false);

                    goSfx.transform.SetParent(goSfxPool.transform);

                    stackSfx.Push(audioSource);
                }             
            }
        }

        public void PlayBgm(string name, bool loop = true, float volume = 0.3f)
        {
            if (dicBgm.ContainsKey(name))
            {
                bgm.clip = dicBgm[name];
            }
            else
            {
                Debug.Log("DIC 에 없습니다.");
                return;
            }

            bgm.gameObject.SetActive(true);

            bgm.volume = volume;
            bgm.loop = loop;
            bgm.Play();
        }

        public void PlaySFX(string name, bool loop = false, float volume = 0.5f)
        {
            AudioClip clip = null;

            if (dicSfx.ContainsKey(name))
            {
                clip = dicSfx[name];
            }
            else
            {
                Debug.Log("DIC 에 없습니다.");
                return;
            }

            if (stackSfx.Count <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    var goSfx = new GameObject();
                    var audioSource = goSfx.AddComponent<AudioSource>();
                    goSfx.name = "SFX Audio";
                    goSfx.SetActive(false);

                    goSfx.transform.SetParent(goSfxPool.transform);

                    stackSfx.Push(audioSource);
                }
            }

            var newSfxAudioSource = stackSfx.Pop();
            newSfxAudioSource.gameObject.SetActive(true);

            newSfxAudioSource.clip = clip;
            newSfxAudioSource.volume = volume;
            newSfxAudioSource.loop = loop;
            newSfxAudioSource.Play();

            StartCoroutine(PushAudioSource(newSfxAudioSource, clip.length));
        }    

        IEnumerator PushAudioSource(AudioSource source, float time)
        {
            yield return new WaitForSeconds(time);

            source.gameObject.SetActive(false);
            stackSfx.Push(source);
        }

        public void SfxMute(bool isMute)
        {
            sfxMute = isMute;

            for (int i = 0; i < goSfxPool.transform.childCount; i++)
            {
                goSfxPool.transform.GetChild(i).GetComponent<AudioSource>().mute = isMute;
            }
           
        }

        public void BgmMute(bool isMute)
        {
            bgmMute = isMute;
            this.bgm.mute = isMute;
        }      
    }
}


