using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockChain;

namespace BlockChain
{
    public class Achivements : MonoBehaviour
    {
        public static int _achivements;
        // Start is called before the first frame update
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _achivements = 0;

        }

        public static void SetAchivements(int achivements)
        {
            _achivements = achivements;
        }

        public static int GetAchivements()
        {
            return _achivements;
        }
    }
}