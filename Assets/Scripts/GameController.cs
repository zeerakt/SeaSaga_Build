using Mkey;
using UnityEngine;
using UnityEngine.SceneManagement;
using WinLoseSystem;

public class GameController : MonoBehaviour
{
    public int[] Garray;
    //[SerializeField]
    //private int _playerStartPoints;
    [SerializeField]
    private StarsController _starsController;

    private bool _levelPassed;
    
    private void Start()
    {
        _starsController.OnPlayerEarnsStar += EarnsStar;
        //SlotPlayer.Instance.SetCoinsCount(_playerStartPoints);
       // SlotController.CurrentSlot.PlayerHasNoMoney += ShowLoseScreen;
    }
    
    private void ShowLoseScreen()
    {
        // TODO: lose screen with to lobby button and watch add button
        Debug.Log("You have lost");
        
        SceneManager.LoadSceneAsync(0);

       // SlotController.CurrentSlot.PlayerHasNoMoney -= ShowLoseScreen;
    }
    
    private void EarnsStar(int starsCount)
    {
        var sceneNumber = MapController.currentLevel;
        PlayerPrefs.SetInt(sceneNumber + "_stars_", starsCount);
        Win();
    }

    private void Win()
    {
        if(!_levelPassed)
        {
            _levelPassed = true;
            if (MapController.currentLevel == MapController.topPassedLevel + 1)
            {
                PlayerPrefs.SetInt("topPassedLevel", ++MapController.topPassedLevel);
            }
        }
    }

    private void OnDestroy()
    {
        _starsController.OnPlayerEarnsStar -= EarnsStar;
    }
}
