using UnityEngine;

/*
    11112019 - first
*/

namespace Mkey
{
    public class AnimCallBack : MonoBehaviour
    {
        private System.Action cBack;

        public void EndCallBack()
        {
           cBack?.Invoke();
        }

        public void SetEndCallBack(System.Action cBack)
        {
            this.cBack = cBack;
        }
    }
}