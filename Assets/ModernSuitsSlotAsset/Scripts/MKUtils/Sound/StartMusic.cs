using UnityEngine;

/*
  20.10.2020 - first
 */
namespace Mkey
{
    public class StartMusic : MonoBehaviour
    {
        [SerializeField]
        private AudioClip startClip;

        #region temp vars
        private SoundMaster MSound { get { return SoundMaster.Instance; } }
        #endregion temp vars

        void Start()
        {
          if(MSound) MSound.SetMusicAndPlay(startClip);
        }
    }
}
