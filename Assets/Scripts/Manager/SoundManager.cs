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