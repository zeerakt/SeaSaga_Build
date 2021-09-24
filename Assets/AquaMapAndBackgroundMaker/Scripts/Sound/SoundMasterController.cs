using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SoundMasterController : MonoBehaviour {

    [Space(8, order = 0)]
    [Header("Default Sound Settings", order = 1)]
    private bool soundOn = true;
    private bool musicOn = true;
    private float volume = 1.0f;
    private string saveNameSound = "mk_soundon";
    private string saveNameMusic = "mk_musicon";
    private string saveNameVolume = "mk_volume";
    public int sourcesCount;

    public bool SoundOn
    {
        get
        {
            return soundOn;
        }
        set
        {
            soundOn = value;
            PlayerPrefs.SetInt(saveNameSound, (soundOn) ? 1 : 0);
        }
    }

    public bool MusicOn
    {
        get
        {
            return musicOn;
        }
        set
        {
            musicOn = value;
            PlayerPrefs.SetInt(saveNameMusic,(musicOn)? 1: 0);
        }
    }

    public float Volume
    {
        get
        {
            return volume;
        }
        set
        {
            volume =Mathf.Clamp(value, 0, 1);
 
            PlayerPrefs.SetFloat(saveNameVolume, volume);
            ApplyVolume();
        }
    }

    public static SoundMasterController Instance;
    [Space(8, order = 0)]
    [Header("Audio Sources", order = 1)]
    public AudioSource aSclick;
    public AudioSource aSbkg;
    public AudioSource aSloop;

    [Space(8, order = 0)]
    [Header("AudioClips", order = 1)]
    public AudioClip menuClick;
    public AudioClip menuPopup;
    public AudioClip menuCheck;
    public AudioClip screenChange;
    public AudioClip bkgMusicMap;
    public AudioClip bkgMusicGame;
    public AudioClip winCoins;
    public AudioClip winLevel;
    public AudioClip looseLevel;
    public AudioClip makeBomb;
    public AudioClip getStar;
    public AudioClip buy;
    public AudioClip leftMoves5;
    public AudioClip leftSeconds10;
    public AudioClip good;
    public AudioClip dropItem;

    public List<AudioSource> tempAudioSources;

    private int audioSoucesMaxCount = 10; 

    WaitForEndOfFrame wff;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

      wff = new WaitForEndOfFrame();
    }

    void Start()
    {
        tempAudioSources = new List<AudioSource>();
        if (!PlayerPrefs.HasKey(saveNameSound))
        {
            PlayerPrefs.SetInt(saveNameSound, (soundOn) ? 1 : 0);
        }
        soundOn = (PlayerPrefs.GetInt(saveNameSound) > 0) ? true : false;

        if (!PlayerPrefs.HasKey(saveNameMusic))
        {
            PlayerPrefs.SetInt(saveNameMusic, (musicOn) ? 1 : 0);
        }
        musicOn = (PlayerPrefs.GetInt(saveNameMusic) > 0) ? true : false;

        if (!PlayerPrefs.HasKey(saveNameVolume))
        {
            PlayerPrefs.SetFloat(saveNameVolume, 1.0f);
        }

        volume = PlayerPrefs.GetFloat(saveNameVolume);
        ApplyVolume();

      //  PlayLevelBkgMusik();
    }

    #region play sounds

    public void SoundPlayDropItem(float playDelay, Action callBack)
    {
        SoundPlayClipAtPos(playDelay, dropItem, callBack, transform.position, 1);
    }

    public void SoundPlayClipAtPos(float playDelay, AudioClip aC, Action callBack, Vector3 pos, float volumeK)
    {
        StartCoroutine(PlayClipAtPoint(playDelay, aC, callBack, pos, volumeK));
    }

    public void SoundPlayClip(float playDelay, AudioClip aC, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, aC, callBack));
    }

    /// <summary>
    /// Button click sound
    /// </summary>
    /// <param name="playDelay"></param>
    /// <param name="callBack"></param>
    public void SoundPlayClick(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, menuClick, callBack));
    }

    /// <summary>
    /// Warning sound
    /// </summary>
    /// <param name="playDelay"></param>
    /// <param name="callBack"></param>
    public void SoundPlayMovesLeft(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, leftMoves5, callBack));
    }

    /// <summary>
    /// Warning sound
    /// </summary>
    /// <param name="playDelay"></param>
    /// <param name="callBack"></param>
    public void SoundPlayTimeLeft(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, leftSeconds10, callBack));
    }

    /// <summary>
    /// Greating sound
    /// </summary>
    /// <param name="playDelay"></param>
    /// <param name="callBack"></param>
    public void SoundPlayGood(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, good, callBack));
    }

    /// <summary>
    /// Greating sound
    /// </summary>
    /// <param name="playDelay"></param>
    /// <param name="callBack"></param>
    public void SoundPlayGetStar(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, getStar, callBack));
    }

    /// <summary>
    /// Buy button click sound
    /// </summary>
    /// <param name="playDelay"></param>
    /// <param name="callBack"></param>
    public void SoundPlayBuy(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, buy, callBack));
    }

    public void SoundPlayPopUp(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, menuPopup, callBack));
    }

    public void SoundPlayCheck(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, menuCheck, callBack));
    }

    public void SoundPlayScreenChange(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, screenChange, callBack));
    }

    public void SoundPlayWinCoins(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, winCoins, callBack));
    }

    public void SoundPlayWinLevel(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, winLevel, callBack));
    }

    public void SoundPlayLooseLevel(float playDelay, Action callBack)
    {
        StartCoroutine(PlayClip(playDelay, looseLevel, callBack));
    }

    IEnumerator PlayClip(float playDelay, AudioClip aC, Action callBack)
    {
        if (soundOn)
        {
            if (!aSclick) aSclick = GetComponent<AudioSource>();
            float delay = 0f;
            while (delay < playDelay)
            {
                delay += Time.deltaTime;
                yield return wff;
            }

            if (aSclick && aC)
            {
                aSclick.clip = aC;
                aSclick.Play();
            }

            while (aSclick.isPlaying)
                yield return wff;
            if (callBack != null)
            {
                callBack();
            }
        }
    }

    IEnumerator PlayClipAtPoint(float playDelay, AudioClip aC, Action callBack, Vector3 pos, float volumeK)
    {
        if (soundOn && tempAudioSources.Count < audioSoucesMaxCount)
        {
            AudioSource aSt = CreateASAtPos(pos);
            tempAudioSources.Add(aSt);
            aSt.volume = Volume * volumeK;

            float delay = 0f;
            while (delay < playDelay)
            {
                delay += Time.deltaTime;
                yield return wff;
            }

            if (aC)
            {
                aSt.clip = aC;
                aSt.Play();
            }

            while (aSt && aSt.isPlaying)
                yield return wff;

            tempAudioSources.Remove(aSt);
            if(aSt)  Destroy(aSt.gameObject);
            if (callBack != null)
            {
                callBack();
            }
        }
    }

    IEnumerator PlayLoopClip(float playDelay, bool loop, AudioClip aC, Action callBack)
    {
        if (soundOn)
        {
            if (!aSloop) aSloop = GetComponent<AudioSource>();
            float delay = 0f;
            while (delay < playDelay)
            {
                delay += Time.deltaTime;
                yield return wff;
            }

            if (aSloop && aC)
            {
                aSloop.clip = aC;
                aSloop.loop = loop;
                aSloop.Play();
            }
            while (aSloop.isPlaying)
                yield return wff;
            if (callBack != null)
            {
                callBack();
            }
        }
    }

    public void StopLoopClip()
    {
        if (aSloop)
        {
            aSloop.Stop();
        }
    }

    private void ApplyVolume()
    {
        if (aSclick)
        {
            aSclick.volume = Volume;
        }

        if (aSbkg)
        {
            aSbkg.volume = Volume;
        }

        if (aSloop)
        {
            aSloop.volume = Volume;
        }

        if (tempAudioSources.Count > 0)
        {
            tempAudioSources.ForEach((ast)=> { ast.volume = Volume; });
        }

}

    private AudioSource CreateASAtPos(Vector3 pos)
    {
        GameObject aS = new GameObject();
        aS.transform.position = pos;
        return aS.AddComponent<AudioSource>();
    }

    #endregion play sounds

}
