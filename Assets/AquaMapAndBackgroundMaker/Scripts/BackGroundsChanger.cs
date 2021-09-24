using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundsChanger : MonoBehaviour {

    public List<Sprite> backGrounds;
    private int bgNumber = 0;
    public static BackGroundsChanger Instance;


	void Awake ()
    {
        if (Instance != null) { Destroy(gameObject); }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        SetSprite();
    }

    private void SetSprite()
    {
        if (backGrounds == null || backGrounds.Count == 0) return;
        bgNumber = (int) Mathf.Repeat(bgNumber, backGrounds.Count);
        SpriteRenderer sR = GetComponent<SpriteRenderer>();
        if (sR)
        {
            sR.sprite = backGrounds[bgNumber];
        }
    }

    public void SetNextSprite()
    {
        bgNumber++;
        
        SetSprite();
    }

}
