using System.Collections.Generic;
using Mkey;
using UnityEngine;

public class SingleReelSpin : MonoBehaviour
{
    public bool UseSingleReelSpin
    {
        get => _useSingleReelSpin;
        set => _useSingleReelSpin = value;
    }
    
    [SerializeField]
    private bool _useSingleReelSpin = true;
    
    [SerializeField]
    private HoldFeature _holdFeature;
    [SerializeField]
    private List<GameObject> _reels;
    
    private void Update()
    {
        if(!UseSingleReelSpin) return;
        
        if (Input.GetMouseButtonUp(0))
        {
            var reelIndex = GetClickedReelIndex();

            if (reelIndex >= 0)
            {
                _holdFeature.HoldAllButOneReel(reelIndex);
                _holdFeature.UseMultiplier = false;
                GetComponent<SlotController>().SpinPress();
                _holdFeature.ReleaseAllButtons();
                _holdFeature.UseMultiplier = true;
            }
        }
    }

    private int GetClickedReelIndex()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            if (IsAReel(hit.collider.gameObject))
            {
                for(int i = 0; i < _reels.Count; i++)
                {
                    if (_reels[i] == hit.collider.gameObject)
                    {
                        return i;
                    }
                }
            }
        }

        return -1;
    }

    private bool IsAReel(GameObject obj)
    {
        return obj.CompareTag("Reel");
    }
}
