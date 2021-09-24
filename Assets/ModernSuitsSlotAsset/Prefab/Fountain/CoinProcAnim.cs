using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	public class CoinProcAnim : MonoBehaviour
	{
        [SerializeField]
        private Transform coin;
        [SerializeField]
        private float startSpeed = 0;
        [SerializeField]
        private float endSpeed = 5;

        [Space(8)]
        [Header("Fountain")]
        [SerializeField]
        private float gravity = 9.8f;
        [SerializeField]
        private float lifeTime = 4f;
        [SerializeField]
        private Vector3 V01 = new Vector3(1, 2, 0);
        [SerializeField]
        private Vector3 V02 = new Vector3(1, 2, 0);
        [SerializeField]
        private GameObject [] coinPrefab;
        [SerializeField]
        private float maxDelay = 1;
        [SerializeField]
        private int coinsCount = 5;
        [SerializeField]
        private float radius = 1;
        [SerializeField]
        private float coinScale=1;

        [SerializeField]
        private bool autoJump = false;
        [SerializeField]
        private float autoJumpdelay = 0;

        #region temp vars

        #endregion temp vars


        #region regular
        private void Start()
		{
            if (autoJump) StartCoroutine(JumpC());
		}

		private void Update()
		{
			
		}
       
        #endregion regular

        public void Move()
        {
            SceneCurve sc = GetComponent<SceneCurve>();
            sc.MoveAlongPath(coin.gameObject, transform, startSpeed, endSpeed, 0f, ()=> { coin.position = transform.position; } );
        }

        public void Jump()
        {
            Debug.Log("jump");
            if (coinPrefab== null || coinPrefab.Length == 0) return;
            List<GameObject> coinsL = new List<GameObject>();

            for (int i = 0; i < coinsCount; i++)
            {
                GameObject cP = coinPrefab[(int)Mathf.Repeat(i, coinPrefab.Length)];
                coinsL.Add(Instantiate(cP, transform));
            }

            foreach (var item in coinsL)
            {
                item.transform.localPosition = RandomRange(new Vector3(-radius, - radius, 0), new Vector3(radius, radius, 0));
                item.transform.localEulerAngles = RandomRange(new Vector3(0, 0, -30), new Vector3(0,0, 30));
                item.transform.localScale *= coinScale;
                StartCoroutine(JumpC(item.transform, UnityEngine.Random.Range(0, maxDelay), lifeTime, () => { Destroy(item); }));
            }
        }

        private IEnumerator JumpC()
        {
            yield return new WaitForSeconds(autoJumpdelay);
            Jump();
        }

        private IEnumerator JumpC(Transform t, float delay, float time, Action completeCallBack)
        {
            yield return delay;
            WaitForEndOfFrame wfef = new WaitForEndOfFrame();
            Vector3 a = new Vector3(0, - gravity, 0); 
            float dt = 0;
            Vector3 jumpV0 = RandomRange(V01, V02);
            Vector3 lPos = t.localPosition;

            while (time > dt)
            {
              //  Debug.Log("move: " + dt);
                dt += Time.deltaTime;
                if (t) t.localPosition = lPos + jumpV0 * dt + a * dt * dt / 2f;
                yield return wfef;
            }

            completeCallBack?.Invoke();
        }

        private Vector3 RandomRange(Vector3 a, Vector3 b)
        {
            return new Vector3(UnityEngine.Random.Range(a.x, b.x), UnityEngine.Random.Range(a.y, b.y), UnityEngine.Random.Range(a.z, b.z));
        }
    }
}
