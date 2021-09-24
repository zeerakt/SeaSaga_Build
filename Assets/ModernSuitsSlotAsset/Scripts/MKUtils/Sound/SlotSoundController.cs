using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
   021218 
   add stopallclip

    160119
    set clips as private
    add StopAllClip(bool stopBackgroundMusik)

    240119
    add   public void SetNewBackGroundClipAndPlay(AudioClip newBackGroundClip)
    fix  void PlayBkgMusik(bool play)
            // set clip if failed
            if (aSbkg && !aSbkg.clip && bkgMusic) aSbkg.clip = bkgMusic;
    25.06.2019
        add base sound class
    20.0.2020 
        start method changes
*/

namespace Mkey
{
    public class SlotSoundController : SoundMaster
    {
        public static new SlotSoundController Instance;

        [Space(8, order = 0)]
        [Header("AudioClips", order = 1)]
        [SerializeField]
        private AudioClip winCoins;
        [SerializeField]
        public AudioClip[] slotRotation;
        [SerializeField]
        private AudioClip slotLoose;
     
        [SerializeField]
        public AudioClip[] reelStop;

        [SerializeField]
        private AudioClip winFreeSpin;
        public AudioClip bonusGame;
        #region temp
        private AudioSource aSloop;
        #endregion temp

        #region regular
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("Awake slot sm");
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        protected override void Start()
        {
			aSloop = CreateLoopAudioSourceAtPos(transform.position);
            base.Start();
        }
        #endregion rgular

        #region play sounds
        public void SoundPlayWinCoins(float playDelay, Action callBack)
        {
            PlayClip(playDelay, winCoins, callBack);
        }

        public void SoundPlayWinFreeSpin(float playDelay, Action callBack)
        {
            PlayClip(playDelay, winFreeSpin, callBack);
        }

        public void SoundPlaySlotLoose(float playDelay, Action callBack)
        {
            PlayClip(playDelay, slotLoose, callBack);
        }

        public void SoundPlayBonusGame(float playDelay, Action callBack)
        {
            PlayClip(playDelay, bonusGame, callBack);
        }
        #endregion play sounds

        #region play loop sounds 
        public void SoundPlayReelStop(float playDelay,  Action callBack)
        {
            int index = UnityEngine.Random.Range(0, reelStop.Length);
            PlayLoopClip(playDelay, reelStop[index], callBack);
        }

        public void SoundPlayRotation(float playDelay,  Action callBack)
        {
            int index = UnityEngine.Random.Range(0, slotRotation.Length);
            PlayLoopClip(playDelay, slotRotation[index], callBack);
        }
        #endregion play loop sounds

        #region stop clips
        /// <summary>
        /// Stop loop clip inner audiosource 
        /// </summary>
        public void StopLoopClip()
        {
            if (aSloop)
            {
                aSloop.Stop();
            }
        }
        #endregion stop clips

        #region private 
        private void PlayLoopClip(float playDelay, AudioClip aC, Action callBack)
        {
            StartCoroutine(PlayLoopClipC(playDelay, true, aC, callBack));
        }

        /// <summary>
        /// play loop clip using inner audiosource - asLoop
        /// </summary>
        /// <param name="playDelay"></param>
        /// <param name="loop"></param>
        /// <param name="aC"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private IEnumerator PlayLoopClipC(float playDelay, bool loop, AudioClip aC, Action callBack)
        {
            if (SoundOn)
            {
                if (!aSloop) aSloop = GetComponent<AudioSource>();
                float delay = 0f;
                while (delay < playDelay)
                {
                    delay += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                if (aSloop && aC)
                {
                    aSloop.clip = aC;
                    aSloop.loop = loop;
                    aSloop.Play();
                }
                while (aSloop.isPlaying)
                    yield return new WaitForEndOfFrame();
                callBack?.Invoke();
            }
        }

        private AudioSource CreateLoopAudioSourceAtPos(Vector3 pos)
        {
            GameObject aS = new GameObject();
            aS.transform.position = pos;
            aS.transform.parent = transform;
            aS.name = "loop";
            AudioSource audio = aS.AddComponent<AudioSource>();
            audio.loop = true;
            audio.playOnAwake = false;
            audio.volume = Volume;
            return audio;
        }
        #endregion private
    }
}