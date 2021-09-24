using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/*
    30.06.2020 - first
    04.01.2020 - wait soun master
 */
namespace Mkey
{
    enum SoundMusic {Sound, Music}
	public class GUIMusicSoundButtonBehavior : MonoBehaviour
	{
        [SerializeField]
        private Image iconOnOff;
        [SerializeField]
        private Text textOnOff;
        [SerializeField]
        private Sprite buttonSpriteOn;
        [SerializeField]
        private Sprite buttonSpriteOff;
        [SerializeField]
        private Sprite iconSpriteOn;
        [SerializeField]
        private Sprite iconSpriteOff;
        [SerializeField]
        private string textOn;
        [SerializeField]
        private string textOff;

        [SerializeField]
        private SoundMusic soundOrMusic;

        #region temp vars
        private SoundMaster MSound => SoundMaster.Instance; 
        #endregion temp vars

        #region regular
        private IEnumerator Start()
		{
            Button b = GetComponent<Button>();
            if (b)
            {
                b.onClick.RemoveListener(Button_Click);
                b.onClick.AddListener(Button_Click);
            }
            // wait sound master
            while (!MSound) yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            MSound.ChangeMusicOnEvent += Refresh;
            MSound.ChangeSoundOnEvent += Refresh;
            Refresh();
		}

        private void OnDestroy()
        {
            if (MSound)
            {
                MSound.ChangeMusicOnEvent -= Refresh;
                MSound.ChangeSoundOnEvent -= Refresh;
            }
        }
        #endregion regular

        private void Button_Click()
        {
            if (!MSound) return;
            if(soundOrMusic == SoundMusic.Music) MSound.SetMusic(!MSound.MusicOn);
            else if(soundOrMusic == SoundMusic.Sound) MSound.SetSound(!MSound.SoundOn);
        }

        private void Refresh(bool on)
        {
            if (!MSound) return;
            Image image = GetComponent<Image>();
            
            if (image) image.sprite = (on) ? buttonSpriteOn : buttonSpriteOff;
            if (iconOnOff) iconOnOff.sprite = (on) ? iconSpriteOn : iconSpriteOff;
            if (textOnOff) textOnOff.text = (on) ? textOn : textOff;
        }

        private void Refresh()
        {
            if (!MSound) return;
            Image image = GetComponent<Image>();
            bool on = false;
            if (soundOrMusic == SoundMusic.Music)
                on = MSound.MusicOn;
            else if (soundOrMusic == SoundMusic.Sound)
                on = MSound.SoundOn;

            if (image) image.sprite = (on) ? buttonSpriteOn : buttonSpriteOff;
            if (iconOnOff) iconOnOff.sprite = (on) ? iconSpriteOn : iconSpriteOff;
            if (textOnOff) textOnOff.text = (on) ? textOn : textOff;
        }
    }
}
