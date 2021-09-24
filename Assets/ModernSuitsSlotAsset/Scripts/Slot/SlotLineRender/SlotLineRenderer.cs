using System;
using UnityEngine;

namespace Mkey
{
	public class SlotLineRenderer : MonoBehaviour
	{
        protected LinesController linesController;
        protected LineBehavior lineBehavior;
        protected RayCaster[] rayCasters;
        protected LineCreator lineCreator;
        protected static int AddSortingOrder { get; set; }

        #region regular
        private void OnDestroy()
        {
            SimpleTween.Cancel(gameObject, false);
        }
        #endregion regular

        #region virtual 
        public virtual void Create(LinesController linesController, LineBehavior lineBehavior)
        {
            this.linesController = linesController;
            this.lineBehavior = lineBehavior;
            rayCasters = lineBehavior.rayCasters;
            lineCreator = lineBehavior.GetComponent<LineCreator>();
        }

        internal virtual void LineFlashing(bool flashing)
        {

        }

        internal virtual void LineBurn(bool burn, float burnDelay, Action completeCallBack)
        {
            
        }

        /// <summary>
        /// Enable or disable line elemnts.
        /// </summary>
        internal virtual void SetLineVisible(bool visible)
        {
           
        }
        #endregion virtual

        protected int GetNextAddSortingOrder()
        {
            AddSortingOrder =  (AddSortingOrder < 10) ? ++AddSortingOrder : 0;
            return AddSortingOrder ;
        }
    }
}
