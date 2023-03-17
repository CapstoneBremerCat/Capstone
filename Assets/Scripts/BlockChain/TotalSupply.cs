using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockChain;
namespace BlockChain
{
    public class TotalSupply : MonoBehaviour
    {
        public static int _totalSupply;
        // Start is called before the first frame update
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _totalSupply = 0;

        }

        public static void SetTotalSupply(int totalSupply)
        {
            _totalSupply = totalSupply;
        }

        public static int GetTotalSupply()
        {
            return _totalSupply;
        }
    }
}