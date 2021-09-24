using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField]
    private int _sceneLoadBuildingIndex;
    
    public void LoadScene()
    {
        if (Mkey.SceneLoader.Instance == null)
        {
            SceneManager.LoadSceneAsync(_sceneLoadBuildingIndex);
            return;
        }
        Mkey.SceneLoader.Instance.LoadScene(_sceneLoadBuildingIndex);
    }
}
