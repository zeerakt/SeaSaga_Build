using UnityEngine;

/*
  20.09.2020 - first
 */
namespace Mkey
{
    public class PlaySound : MonoBehaviour
    {
        [SerializeField]
        private AudioClip clip;
        [SerializeField]
        private float delay;

        #region temp vars
        private SoundMaster MSound { get { return SoundMaster.Instance; } }
        #endregion temp vars

       public void PlayClip()
        {
          if(MSound) MSound.PlayClip(delay, clip);
        }
    }
}
