using UnityEngine;
using System.Collections;

public class DontDestroyObj : MonoBehaviour {

    public bool dontDestroy = true;

    void Awake()
    {
            DontDestroyOnLoad(transform.gameObject);
    }
}
