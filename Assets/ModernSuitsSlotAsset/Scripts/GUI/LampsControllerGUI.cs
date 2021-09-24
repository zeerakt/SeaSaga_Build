using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Mkey;

namespace MkeyFW
{
  //  public enum LampsFlash {Random, All, Sequence, NoneEnabled, NoneDisabled }
    public class LampsControllerGUI : MonoBehaviour
    {
        [SerializeField]
        private Sprite lampOn;
        [SerializeField]
        private List <Image> lampsOn;

        private int enabledCount = 0;
        private bool cancel = false;
        public LampsFlash lampFlash = LampsFlash.Random;
        private LampsFlash lampFlashOld = LampsFlash.Random;

        #region regular
        void Start()
        {
            enabledCount = 0;

            if (lampsOn == null || lampsOn.Count == 0) // create lampson array
            {
                Image[] lamps = GetComponentsInChildren<Image>();
                lampsOn = new List<Image>();
                if (lamps != null && lampOn)
                {
                    for (int i = 0; i < lamps.Length; i++)
                    {
                        if (lamps[i])
                        {
                            GameObject lG = new GameObject();
                            lG.name = name + "On";
                            lG.transform.localScale = lamps[i].transform.lossyScale;
                            lG.transform.parent = lamps[i].transform;
                            lG.transform.localPosition = Vector3.zero;
                            Image sR = lG.AddComponent<Image>();
                            sR.sprite = lampOn;
                            lampsOn.Add(sR);
                        }
                    }
                }
            }
            DisableAll();
            StartCoroutine(Flashing());
        }

        void OnDestroy()
        {
            CancelTween();
        }
        #endregion regular

        private IEnumerator Flashing()
        {
            while (!cancel)
            {
                if (lampFlashOld != lampFlash)
                {
                    DisableAll();
                    yield return new WaitForSeconds(0.02f);
                }
                if (lampFlash == LampsFlash.Random)
                {
                    lampFlashOld = lampFlash;
                    int lampI = UnityEngine.Random.Range(0, lampsOn.Count - 1);
                    float lightDuration = UnityEngine.Random.Range(1, 4);

                    if (enabledCount < 5)
                    {
                        EnableLamp(lampI, lightDuration, null);
                    }
                    yield return new WaitForSeconds(0.05f);
                }
                else if (lampFlash == LampsFlash.All)
                {
                    lampFlashOld = lampFlash;
                    for (int i = 0; i < lampsOn.Count; i++)
                    {
                        EnableLamp(i, 1f, null);
                    }
                    yield return new WaitForSeconds(1.5f);
                }
                else if (lampFlash == LampsFlash.Sequence)
                {
                    lampFlashOld = lampFlash;
                    for (int i = 0; i < lampsOn.Count; i++)
                    {
                        EnableLamp(i, 0.3f, null);
                        yield return new WaitForSeconds(0.05f);
                    }
                    yield return new WaitForSeconds(0.05f);
                }
                else if (lampFlash == LampsFlash.NoneDisabled)
                {
                    lampFlashOld = lampFlash;
                    yield return new WaitForSeconds(0.05f);
                }

                else if (lampFlash == LampsFlash.NoneEnabled)
                {
                    if (lampFlashOld != lampFlash)
                    {
                        EnableAll();
                    }
                    lampFlashOld = lampFlash;
                    yield return new WaitForSeconds(0.05f);
                }

                else
                {
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }

        private void EnableLamp(int i, float lightDuration, Action completeCallBack)
        {
            if (!IndexOk(i) || lampsOn[i].gameObject.activeSelf)
            {
                completeCallBack?.Invoke();
                return;
            }

            lampsOn[i].gameObject.SetActive(true);
            enabledCount++;
            float fadeK = 0.1f;
            float fadeK1 = 1.0f / fadeK;
            float fadeTime = fadeK * lightDuration;
            float sumLightDuration = lightDuration + fadeTime + fadeTime;

            SimpleTween.Value(gameObject, 0, 1, sumLightDuration).SetOnUpdate(
                (float val) =>
                {
                    if (val <= fadeK)
                    {
                        if (lampsOn[i]) lampsOn[i].color = new Color(1, 1, 1, val * fadeK1);
                    }
                    else if (val >= fadeK)
                    {
                        if (lampsOn[i]) lampsOn[i].color = new Color(1, 1, 1, (1.0f - val) * fadeK1);
                    }
                }).AddCompleteCallBack(()=> 
                {
                    if (lampsOn[i])
                    {
                        lampsOn[i].color = new Color(1, 1, 1, 0);
                        lampsOn[i].gameObject.SetActive(false);
                    }
                    enabledCount--;
                    completeCallBack?.Invoke();
                }); 
        }

        private bool IndexOk(int i)
        {
            return ((i >= 0) && (i < lampsOn.Count));
        }

        private void CancelTween()
        {
            cancel = true;
            SimpleTween.Cancel(gameObject, true);
            StopCoroutine(Flashing());
        }

        private void DisableAll()
        {
            SimpleTween.Cancel(gameObject, true);
            for (int i = 0; i < lampsOn.Count; i++)
            {
                lampsOn[i].color = new Color(1, 1, 1, 0);
                lampsOn[i].gameObject.SetActive(false);
            }
        }

        private void EnableAll()
        {
            SimpleTween.Cancel(gameObject, true);
            for (int i = 0; i < lampsOn.Count; i++)
            {
                lampsOn[i].color = new Color(1, 1, 1, 1);
                lampsOn[i].gameObject.SetActive(true);
            }
        }
    }
}