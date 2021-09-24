using UnityEngine;
using UnityEngine.UI;
public class LevelButton : MonoBehaviour {

    public Image LeftStar;
    public Image MiddleStar;
    public Image RightStar;
    public Sprite ActiveButtonSprite;
    public Sprite LockedButtonSprite;
    public Sprite EmptyStarSprite;
    public Sprite FullStarSprite;
    public Button button;
    public Text numberText;
    public bool Interactable { get; private set; }

    /// <summary>
    /// Set button interactable if button "active" or appropriate level is passed. Show stars or Lock image
    /// </summary>
    /// <param name="active"></param>
    /// <param name="activeStarsCount"></param>
    /// <param name="isPassed"></param>
    internal void SetActive(bool active, int activeStarsCount, bool isPassed)
    {
        LeftStar.gameObject.SetActive(isPassed);
        MiddleStar.gameObject.SetActive(isPassed);
        RightStar.gameObject.SetActive(isPassed);
        button.image.sprite = (isPassed || active) ? ActiveButtonSprite : LockedButtonSprite;

        if (isPassed)
        {
            LeftStar.sprite   = (activeStarsCount > 0) ? FullStarSprite : EmptyStarSprite;
            MiddleStar.sprite = (activeStarsCount > 1) ? FullStarSprite : EmptyStarSprite;
            RightStar.sprite  = (activeStarsCount > 2) ? FullStarSprite : EmptyStarSprite;
        }

        Interactable = active || isPassed;
        button.interactable = Interactable;
        if (active)
        {
            MapController.Instance.ActiveButton = this;
        }

    }
}
