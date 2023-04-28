using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockChain;

namespace BlockChain
{
    public class OwnedRewardTickets : MonoBehaviour
    {
        public static int _ownedRewardTickets;
        // Start is called before the first frame update
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _ownedRewardTickets = 0;

        }

        public static void SetOwnedRewardTickets(int ownedRewardTickets)
        {
            _ownedRewardTickets = ownedRewardTickets;
        }

        public static int GetOwnedRewardTickets()
        {
            return _ownedRewardTickets;
        }
    }
}