using UnityEngine;

/*
  26.11.2019 - first
 */
namespace Mkey
{
    public class StartSound : MonoBehaviour
    {
        [SerializeField]
        private AudioClip startClip;
        [SerializeField]
        private float delay;

        #region temp vars
        private SoundMaster MSound { get { return SoundMaster.Instance; } }
        #endregion temp vars

        void Start()
        {
          if(MSound) MSound.PlayClip(delay, startClip);
        }
    }
}
