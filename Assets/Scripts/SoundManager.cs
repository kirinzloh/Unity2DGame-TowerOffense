using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;

    public AudioSource musicAudio;
    public AudioSource sfxAudio;
    public AudioClip[] sceneMusic;
    public AudioClip winMusic;
    public AudioClip loseMusic;
    public int currentlyPlaying;

    public AudioClip sendMonster;
    public AudioClip spawnMonster;

    public float masterVol;
    public float musicVol;
    public float sfxVol;
    
    void Awake() {
        // For singleton
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    void AddSliderHooks() {
        foreach (Slider slider in Resources.FindObjectsOfTypeAll<Slider>()) {
            if (slider.name == "MasterVol Slider") {
                slider.value = masterVol;
                slider.onValueChanged.AddListener(setMasterVol);
            } else if (slider.name == "MusicVol Slider") {
                slider.value = musicVol;
                slider.onValueChanged.AddListener(setMusicVol);
            } else if (slider.name == "SoundEffectsVol Slider") {
                slider.value = sfxVol;
                slider.onValueChanged.AddListener(setSfxVol);
            }
        }
    }

    void AddSliderHooks(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 0) {
            AddSliderHooks();
        }
    }

    void PlayMusic(Scene x, Scene active) {
        currentlyPlaying = active.buildIndex;
        musicAudio.clip = sceneMusic[currentlyPlaying];
        musicAudio.Play();
    }

    // Use this for initialization
    void Start () {
        SceneManager.sceneLoaded += AddSliderHooks;
        SceneManager.activeSceneChanged += PlayMusic;
        AddSliderHooks();
        musicAudio.volume = masterVol * musicVol;
        PlayMusic(SceneManager.GetActiveScene(), SceneManager.GetActiveScene());
    }
    
    // Update is called once per frame
    void Update () {
    }

    #region public api
    public void PlayWin() {
        currentlyPlaying = -1;
        musicAudio.clip = winMusic;
        musicAudio.Play();
    }

    public void PlayLose() {
        currentlyPlaying = -2;
        musicAudio.clip = loseMusic;
        musicAudio.Play();
    }

    public void PlayShoot(AudioClip clip) {
        if (clip == null) { return; }
        sfxAudio.PlayOneShot(clip, 0.5f);
    }

    public void PlaySendMonster() {
        sfxAudio.PlayOneShot(sendMonster,1);
    }

    public void PlaySpawnMonster() {
        sfxAudio.PlayOneShot(spawnMonster, 1);
    }

    public void setMasterVol(float vol) {
        masterVol = vol;
        musicAudio.volume = masterVol * musicVol;
        sfxAudio.volume = masterVol * sfxVol;
    }

    public void setMusicVol(float vol) {
        musicVol = vol;
        musicAudio.volume = masterVol * musicVol;
    }

    public void setSfxVol(float vol) {
        sfxVol = vol;
        sfxAudio.volume = masterVol * sfxVol;
    }
    #endregion
}
