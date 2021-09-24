using UnityEngine;
using UnityEngine.UI;

/*
    23.10.2019 - first
    15.11.2019 - add photo behavior
    28.02.2020 - add strings connect/ disconnect
 */
namespace Mkey
{
    public class FaceBookGUIController : MonoBehaviour
    {
        [SerializeField]
        private Text loginText;
        [SerializeField]
        private string connectString;
        [SerializeField]
        private string disConnectString;
        [SerializeField]
        private Image fbPhoto;
        [SerializeField]
        private GameObject avatarGroup;

        #region temp vars
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private FBholder FB {get{ return FBholder.Instance; } }
        private Sprite defaultPlayerImage;
        #endregion temp vars
		
		#region regular
		private void Start()
		{
            FBholder.LoginEvent += FacebooLoginHandler;
            FBholder.LogoutEvent += FacebooLogoutHandler;
            FBholder.LoadPhotoEvent += LoadPhotoEventHandler;
            Refresh();
        }

        private void OnDestroy()
        {
            FBholder.LoginEvent -= FacebooLoginHandler;
            FBholder.LogoutEvent -= FacebooLogoutHandler;
            FBholder.LoadPhotoEvent -= LoadPhotoEventHandler;
        }
        #endregion regular

        private void Refresh()
        {
            if (loginText) loginText.text = (!FBholder.IsLogined) ? connectString : disConnectString;
            if (avatarGroup && fbPhoto)
            {
                avatarGroup.SetActive(FBholder.IsLogined && FB.playerPhoto);
                fbPhoto.sprite = FB.playerPhoto;
            }
            else if (avatarGroup)
            {
                avatarGroup.SetActive(false);
            }
        }

        public void FaceBook_Click()
        {
            if (FBholder.IsLogined)
            {
                FB.FBlogOut();
            }
            else
            {
                FB.FBlogin();
            }
        }

        #region event handlers
        private void FacebooLoginHandler(bool logined, string message)
        {
            Refresh();
            if (logined) MPlayer.AddFbCoins();
        }

        private void FacebooLogoutHandler()
        {
            Refresh();
        }

        private void LoadPhotoEventHandler(bool isLogined, Sprite photo)
        {
            if (avatarGroup && fbPhoto)
            {
                avatarGroup.SetActive(FBholder.IsLogined && photo);
                fbPhoto.sprite = photo;
            }
            else if (avatarGroup)
            {
                avatarGroup.SetActive(false);
            }
        }
        #endregion event handlers
    }
}
