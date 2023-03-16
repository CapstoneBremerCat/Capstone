using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlockChain;
namespace BlockChain
{
    public class IsLogin : MonoBehaviour
    {
        public static string loginAddr;
        // Start is called before the first frame update
        private void Awake()
        {
            DontDestroyOnLoad(this);
            loginAddr = "---";

        }

        public static void SetAddr(string addr)
        {
            loginAddr = addr;
        }

        public static string GetAddr()
        {
            return loginAddr;
        }
    }
}