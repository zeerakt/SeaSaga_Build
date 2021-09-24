using UnityEngine;

namespace Mkey
{
    public class FbEvents : MonoBehaviour
    {

        private FBholder FB { get { return FBholder.Instance; } }

        void Start()
        {
           FBholder.LoadTextEvent += SetPlayerName;
           FBholder.LogoutEvent += SetDefName;
        }

        void OnDestroy()
        {
            FBholder.LoadTextEvent -= SetPlayerName;
            FBholder.LogoutEvent -= SetDefName;
        }

        public void SetPlayerName(bool logined, string firstName, string lastName)
        {
           
        }

        public void SetDefName()
        {
           
        }
    }
}