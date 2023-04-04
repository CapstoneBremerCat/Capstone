using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;	// UnityWebRequest사용을 위해서 적어준다.
using Newtonsoft.Json;
using TMPro;
using BlockChain;
using Game;
namespace BlockChain
{
    public class rewardNFT : MonoBehaviour
    {
        public string name;
        public string description;

        /*    //좀비 1000마리 죽이기
            if (GameManager.Instance.zombieKillCount >= 1000) 
            {
                name = "Zombie Apocalypse Survivor"; //좀비 아포칼립스 생존자
                description = "Kill 1000 zombies";
            }

            //좀비 5000마리 죽이기
            if (GameManager.Instance.zombieKillCount >= 5000) 
            {
                name = "Zombie Slayer"; //좀비 학살자
                description = "Kill 5000 zombies";
            }

        //좀비 10000마리 죽이기
        if (GameManager.Instance.zombieKillCount >= 10000)
        {
            name = "Undead Exterminator"; //좀비 근절자
            description = "Kill 10000 zombies";
        }

        //좀비 50000마리 죽이기
        if (GameManager.Instance.zombieKillCount >= 50000)
        {
            name = "Zombie Annihilator"; //좀비 소멸자
            description = "Kill 50000 zombies";
        }

        //보스 3분 만에 죽이기
        if (GameManager.Instance.bossKilledTime <= 180)
        {
            name = "Boss slayer"; //보스 슬레이어
            description = " Clear the boss in 3 minutes";
        }

        //브레멘 동료들 다 모으기
        if (GameManager.Instance.allMusiciansRecruited)
        {
            name = "Squad Leader"; //분대장
            description = "Recruiting Bremen Musicians";
        }

        //절대반지 소유한 경험이 생기기
        if (GameManager.Instance.ringOwner)
        {
            name = "Ring Bearer"; //반지의 제왕에서 절대반지 소유자를 지칭한다함.
            description = "Being the absolute ring owner";
        }

        //NFT 아이템을 첫 구매하기
        if (GameManager.Instance.firstNFTPurchase)
        {
            name = "NFT Pioneer"; //NFT 개척자
            description = "Buying NFT items for the first time";
        }

        //모든 스테이지를 클러어하기
        if (GameManager.Instance.allStagesCleared)
        {
            name = "Ultimate Survivor"; //궁극의 생존자
            description = "Clear All Stages";
        }

        NFTManager.Instance.mintRewardNFT(name, description);
        }
        */
    }
}