using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	public class JackPotController : MonoBehaviour
	{
        [SerializeField]
        private TextMesh jackPotTitle;
        [SerializeField]
        private TextMesh jackPotAmount;
        [SerializeField]
        private LampsController[] lamps;
        [SerializeField]
        private CoinProcAnim[] coinsFountains;
        [SerializeField]
        private SpriteRenderer[] winRenderers;

        #region properties
        public TextMesh JackPotTitle { get { return jackPotTitle; } }
        public TextMesh JackPotAmount { get { return jackPotAmount; } }
        public LampsController[] Lamps { get { return lamps; } }
        public CoinProcAnim[] CoinsFoutains { get { return coinsFountains; } }
        public SpriteRenderer[] WinRenderers { get { return winRenderers; } }
        #endregion properties

        #region temp vars

        #endregion temp vars


        #region regular
        private void Awake()
        {
          
        }
		
		private void Start()
		{	
			
		}
		#endregion regular

	}
}
