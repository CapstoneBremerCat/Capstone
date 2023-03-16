using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockChain;
namespace BlockChain
{
    public class BalanceOfKlay : MonoBehaviour
    {
        public static float _balanceOfKlay;
        // Start is called before the first frame update
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _balanceOfKlay = 0;

        }

        public static void SetBalanceOfKlay(float balanceOfKlay)
        {
            _balanceOfKlay = balanceOfKlay;
        }

        public static float GetBalanceOfKlay()
        {
            return _balanceOfKlay;
        }
    }
}