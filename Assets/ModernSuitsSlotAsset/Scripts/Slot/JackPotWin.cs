using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class JackPotWin : MonoBehaviour
    {
        [SerializeField]
        private AudioClip coinsClip;

        #region temp vars
        private TextMesh jackPotTitle;
        [SerializeField]
        private TextMesh jackPotAmount;
        [SerializeField]
        private TextMesh jackPotAmountTemp;
        private LampsController[] lamps;
        private CoinProcAnim[] coinsFountains;
        private JackPotController jpController;
        private List<TextMesh> rend;
        private List<Color> colors;
        private SlotSoundController MSound { get { return SlotSoundController.Instance; } }
        private MeshRenderer mR;
        #endregion temp vars

        #region regular
        private void Awake()
        {

        }

        private void Start()
        {
            jpController = GetComponentInParent<JackPotController>();
            if (!jpController) return;

            if (jpController)
            {
                foreach (var item in jpController.WinRenderers)
                {
                    if (item) item.enabled = true;
                } 
            }

            jackPotAmount = jpController.JackPotAmount;
            if (jackPotAmount)
            {
                jackPotAmountTemp = Instantiate(jackPotAmount.gameObject, jackPotAmount.transform.position, jackPotAmount.transform.rotation).GetComponent<TextMesh>();

                mR = jackPotAmount.GetComponent<MeshRenderer>();
                if (mR) mR.enabled = false;
            }

            jackPotTitle = jpController.JackPotTitle;
            lamps = jpController.Lamps;
            coinsFountains = jpController.CoinsFoutains;

            rend = new List<TextMesh>();
            if (jackPotAmountTemp) rend.Add(jackPotAmountTemp);
            if(jackPotTitle)  rend.Add(jackPotTitle);

            colors = new List<Color>();
            foreach (var item in rend)
            {
                colors.Add(item.color);
            }

            if (lamps != null)
            {
                foreach (var item in lamps)
                {
                    if (item) item.lampFlash = LampsFlash.Sequence;
                }
            }

            StartCoroutine(FountainC());

            Flashing(true);
        }

        private void OnDestroy()
        {
            if (jpController)
            {
                foreach (var item in jpController.WinRenderers)
                {
                    if (item) item.enabled = false;
                }
            }

            if (mR) mR.enabled = true;
            if (jackPotAmountTemp) Destroy(jackPotAmountTemp.gameObject);

            StopCoroutine(FountainC());

            if (lamps != null)
            {
                foreach (var item in lamps)
                {
                    if (item) item.lampFlash = LampsFlash.NoneDisabled;
                }
            }
            Flashing(false);
        }
        #endregion regular


        private IEnumerator FountainC()
        {
            if (coinsFountains != null)
            {

                while (true)
                {
                    foreach (var item in coinsFountains)
                    {
                        if (item) item.Jump();
                        if (coinsClip) MSound.PlayClip(0.2f, coinsClip);
                        yield return new WaitForSeconds(0.5f);
                    }
                    yield return new WaitForSeconds(2);
                }
            }
        }

        private void Flashing(bool flashing)
        {

            Color c;
            if (flashing)
            {
                Color nC;
                SimpleTween.Value(gameObject, 0, Mathf.PI * 2f, 1f).SetOnUpdate((float val) =>
                {
                    for (int i = 0; i < rend.Count; i++)
                    {
                        float k = 0.5f * (Mathf.Cos(val) + 1f);
                        c = colors[i];
                        nC = new Color(c.r, c.g, c.b, c.a * k);
                        if (rend[i]) rend[i].color = nC;

                    }
                }).SetCycled();
            }
            else
            {
                SimpleTween.Cancel(gameObject, false);
                for (int i = 0; i < rend.Count; i++)
                {
                    c = colors[i];
                    if (rend[i]) rend[i].color = c;
                }
            }
        }
    }
}
