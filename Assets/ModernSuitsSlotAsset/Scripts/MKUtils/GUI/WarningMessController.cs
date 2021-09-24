using UnityEngine.UI;
/*
 *changes
 * 02102019 Fix
 *      -if (yesButton) yesButton.gameObject.SetActive(yesButtonActive);
        -if (cancelButton) cancelButton.gameObject.SetActive(cancelButtonActive);
        -if (noButton) noButton.gameObject.SetActive(noButtonActive);
 */
namespace Mkey
{
    public enum MessageAnswer { None , Yes, Cancel, No }
    public class WarningMessController : PopUpsController
    {
        public Text caption;
        public Text message;
        public Button yesButton;
        public Button noButton;
        public Button cancelButton;

        public MessageAnswer Answer
        {
            get; private set;
        }

        public void Cancel_Click()
        {
            Answer = MessageAnswer.Cancel;
            CloseWindow();
        }

        public void Yes_Click()
        {
            Answer = MessageAnswer.Yes;
            CloseWindow();
        }

        public void No_Click()
        {
            Answer = MessageAnswer.No;
            CloseWindow();
        }

        public string Caption
        {
            get { if (caption) return caption.text; else return string.Empty; }
            set { if (caption) caption.text = value; }
        }

        public string Message
        {
            get { if (message) return message.text; else return string.Empty; }
            set { if (message) message.text = value; }
        }

        internal void SetMessage(string caption, string message, bool yesButtonActive, bool cancelButtonActive, bool noButtonActive)
        {
            Caption = caption;
            Message = message;
            if (yesButton) yesButton.gameObject.SetActive(yesButtonActive);
            if (cancelButton) cancelButton.gameObject.SetActive(cancelButtonActive);
            if (noButton) noButton.gameObject.SetActive(noButtonActive);
        }
    }
}