using UnityEngine;

namespace Mkey
{
    public class RayCaster : MonoBehaviour
    {
        public int ID { get; set; } // for calcs

        public SlotSymbol GetSymbol()
        {
            Collider2D hit = Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y));
            if (hit) { return hit.GetComponent<SlotSymbol>(); }
            else return null;
        }

    }
}