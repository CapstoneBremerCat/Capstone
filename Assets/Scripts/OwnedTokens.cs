using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockChain;
namespace BlockChain
{
    public class OwnedTokens : MonoBehaviour
    {
        public static int[] _ownedTokens;
        // Start is called before the first frame update
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _ownedTokens = new int[20];

        }

        public static void SetOwnedTokens(int[] ownedTokens)
        {
            _ownedTokens = ownedTokens;
        }

        public static int[] GetOwnedTokens()
        {
            return _ownedTokens;
        }
    }
}