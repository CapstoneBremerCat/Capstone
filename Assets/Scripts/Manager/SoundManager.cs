using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
namespace Game
{
    public class SoundManager : MonoBehaviour
    {
        #region instance
        private static SoundManager instance = null;
        public static SoundManager Instance { get { return instance; } }

        private void Awake()
        {
            // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }
            // instance�� ���� ������Ʈ�� �����
            instance = this;

            // Scene �̵� �� ���� ���� �ʵ��� ó��
            DontDestroyOnLoad(this.gameObject);

            SetAudioDictionary(BGMDictionary, "Audios/BGM");
            SetAudioDictionary(SFXDictionary, "Audios/SFX");

            for (int i = 0; i < objectPoolLength; i++)
            {
                GameObject soundObject = new GameObject();
                soundObject.transform.SetParent(instance.transform);
                soundObject.name = "Sound Effect";
                AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f;
                audioSource.minDistance = instance.soundDistance;
                audioSource.gameObject.SetActive(false);
                pool.Add(audioSource);
            }
            curPlayingSounds = 0;
        }
        #endregion

        // bgm key ���ڿ� ������
        public string keyMain = "Main";
        public string keyStageSun = "Stage_sun";
        public string keyStageMoon = "Stage_moon";

        private Dictionary<string, AudioClip> BGMDictionary = new Dictionary<string, AudioClip>(); // ���ҽ��� �ҷ��� ����� Ŭ���� ������ ��ųʸ�
        private Dictionary<string, AudioClip> SFXDictionary = new Dictionary<string, AudioClip>();  // ���ҽ��� �ҷ��� ����� Ŭ���� ������ ��ųʸ�
        [SerializeField] private AudioSource BGM;    // BGM
        [SerializeField] private AudioSource SubBgm;    // SubBgm
        [SerializeField] private AudioSource SFX;    // SFX

        private bool masterMute = false;
        private float masterVolume = 1;

        private bool SFXMute = false;
        private float SFXVolume = 1;

        private bool BGMMute = false;
        private float BGMVolume = 1;

        public bool SFXSoundMute { get { return (masterMute || SFXMute); } }
        public float SFXSoundVolume { get { return (SFXVolume * masterVolume); } }

        public bool BGMSoundMute { get { return (masterMute || BGMMute); } }
        public float BGMSoundVolume { get { return (BGMVolume * masterVolume); } }

        public int curPlayingSounds { get; private set; } // ���� ����� ����� �������� �ִ� ����
        public void AddPlayingSounds()
        {
            curPlayingSounds++;
        }
        public void InitPlayingSounds()
        {
            curPlayingSounds = 0;
        }

        private void SetAudioDictionary(Dictionary<string, AudioClip> dictionary, string path)
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>(path);
            foreach (AudioClip clip in clips)
            {
/*                if(!dictionary.ContainsKey(clip.name))
                {
                    dictionary.
                }*/
                dictionary.Add(clip.name, clip);
            }
        }

        public void ChangeMasterVolume(Slider slider)
        {
            masterVolume = slider.value;
            ApplyBGMVolume();
            ApplySFXVolume();
        }
        public void MuteMasterVolume(Toggle toggle)
        {
            masterMute = toggle.isOn;
            ApplyBGMVolume();
            ApplySFXVolume();
        }
        public void ChangeSFXVolume(Slider slider)
        {
            SFXVolume = slider.value;
            ApplySFXVolume();
        }
        public void MuteSFXVolume(Toggle toggle)
        {
            SFXMute = toggle.isOn;
            ApplySFXVolume();
        }
        public void ChangeBGMVolume(Slider slider)
        {
            BGMVolume = slider.value;
            ApplyBGMVolume();
        }
        public void MuteBGMVolume(Toggle toggle)
        {
            BGMMute = toggle.isOn;
            ApplyBGMVolume();
        }

        public void LoadAudio()
        {
/*            BGMDictionary = DataMgr.Instance.SetDictionary<AudioClip>("Sounds/BGM");
            SFXDictionary = DataMgr.Instance.SetDictionary<AudioClip>("Sounds/SFX");*/
            BGM.loop = true;
            SFX.loop = false;
        }


        [SerializeField]
        private bool muteSound;

        [SerializeField]
        private int objectPoolLength = 20;

        [SerializeField]
        private float soundDistance = 7f;

        [SerializeField]
        private bool logSounds = false;

        private List<AudioSource> pool = new List<AudioSource>();

        public static void PlaySound(AudioClip clip, Vector3 pos)
        {
            if (!instance)
            {
                Debug.LogError("No Audio Manager found in the scene.");
                return;
            }

            if (instance.muteSound)
            {
                return;
            }

            if (!clip)
            {
                Debug.LogError("Clip is null");
                return;
            }

            if (instance.logSounds)
            {
                Debug.Log("Playing Audio: " + clip.name);
            }

            for (int i = 0; i < instance.pool.Count; i++)
            {
                if (!instance.pool[i].gameObject.activeInHierarchy)
                {
                    instance.pool[i].clip = clip;
                    instance.pool[i].transform.position = pos;
                    instance.pool[i].gameObject.SetActive(true);
                    instance.pool[i].Play();
                    instance.StartCoroutine(instance.ReturnToPool(instance.pool[i].gameObject, clip.length));
                    return;
                }
            }

            GameObject soundObject = new GameObject();
            soundObject.transform.SetParent(instance.transform);
            soundObject.name = "Sound Effect";
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.minDistance = instance.soundDistance;
            instance.pool.Add(audioSource);
            audioSource.clip = clip;
            soundObject.transform.position = pos;
            audioSource.Play();
            instance.StartCoroutine(instance.ReturnToPool(soundObject, clip.length));
        }

        private IEnumerator ReturnToPool(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            obj.SetActive(false);
        }

        public void ApplyBGMVolume()
        {
            SubBgm.volume = BGM.volume = BGMSoundVolume;
            SubBgm.mute = BGM.mute = BGMSoundMute;
        }
        public void ApplySFXVolume()
        {
            SFX.volume = SFXSoundVolume;
            SFX.mute = SFXSoundMute;
        }

        public void StopBGM()
        {
            //���� ���� ����
            BGM.Stop();
        }

        public void OnPlayBGM(string key)
        {
            //���� ���� ����
            BGM.Stop();
            // �÷��� ���̶�� ����
            if (BGM.isPlaying) return;
            BGM.clip = BGMDictionary[key];
            // ������� ���� ��쿡�� ���
            if (!BGM.isPlaying) BGM.Play();
        }

        public void OnPlaySFX(string clipName)
        {
            if (SFX) SFX.Stop();
            if (SFX.isPlaying) return;
            SFX.clip = SFXDictionary[clipName];
            if (!SFX.isPlaying) SFX.Play();
        }

        public void ToMain()
        {
            // ���� ��� ���� ����
            BGM.Stop();
            SFX.Stop();

            // ����ȭ�� BGM ���
            OnPlayBGM(keyMain);
        }
    }
}