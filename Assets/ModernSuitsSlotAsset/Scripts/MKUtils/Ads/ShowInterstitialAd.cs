using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
    12.01.2020
*/

namespace Mkey
{
    public class ShowInterstitialAd : MonoBehaviour
    {
        [Tooltip("The number of object starts per ad impression")]
        [SerializeField]
        private int startsPerShow = 1;

        [Tooltip("Show ads on first start")]
        [SerializeField]
        private bool showOnFirstStart = true;

        [Tooltip("Ad show delay in seconds")]
        [SerializeField]
        private float delay = 0;

       // [Tooltip("Show ads on first start")]
        [SerializeField]
        private bool singleTon = true;

        #region temp vars
        private AdsControl Ads => AdsControl.Instance;
        private SoundMaster MSound { get { return SoundMaster.Instance; } }

        private  int loadsCounter = 0;

        private static Dictionary<string, int> loadsDict;

        private static ShowInterstitialAd Instance;
        #endregion temp vars

        #region regular
        private void OnValidate()
        {
            Validate();
        }

        private IEnumerator Start()
        {
            if (singleTon)
            {
                if (Instance) 
                {
                    Destroy(gameObject);
                    while (true) yield return new WaitForEndOfFrame(); // wait destroy
                }
                else Instance = this;
            }

            int scene = SceneLoader.GetCurrentSceneBuildIndex();
            Validate();
            string id = scene.ToString() + GetAllParentsString();
            if (loadsDict == null) loadsDict = new Dictionary<string, int>();

            if (!loadsDict.ContainsKey(id))
            {
                loadsCounter = 0;
                loadsDict.Add(id, loadsCounter);
            }
            else
            {
                loadsCounter = loadsDict[id];
            }
            Debug.Log("loadsCounter: " + loadsCounter + " id: " + id);
            bool show = (loadsCounter > 0 || (loadsCounter == 0 && showOnFirstStart));
            if (show && (loadsCounter % startsPerShow == 0))
            {
                yield return new WaitForSeconds(delay);
                while (!Ads) yield return new WaitForEndOfFrame();

                Ads.ShowInterstitial(
                   () =>
                   {
                       MSound.ForceStopMusic();
                   },
                   () =>
                   {
                       MSound.PlayCurrentMusic();
                   });
            }
            loadsDict[id]++;
        }
        #endregion regular

        private void Validate()
        {
            delay = Mathf.Max(0, delay);
            startsPerShow = Mathf.Max(1, startsPerShow);
        }

        private string GetAllParentsString()
        {
            Transform[] parents = GetComponentsInParent<Transform>();
            string result = "";
            foreach (var item in parents)
            {
                result += item.name;
            }
            return result;
        }
    }
}
