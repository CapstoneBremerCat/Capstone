using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;	// UnityWebRequest����� ���ؼ� �����ش�.
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

        /*    //���� 1000���� ���̱�
            if (GameManager.Instance.zombieKillCount >= 1000) 
            {
                name = "Zombie Apocalypse Survivor"; //���� ����Į���� ������
                description = "Kill 1000 zombies";
            }

            //���� 5000���� ���̱�
            if (GameManager.Instance.zombieKillCount >= 5000) 
            {
                name = "Zombie Slayer"; //���� �л���
                description = "Kill 5000 zombies";
            }

        //���� 10000���� ���̱�
        if (GameManager.Instance.zombieKillCount >= 10000)
        {
            name = "Undead Exterminator"; //���� ������
            description = "Kill 10000 zombies";
        }

        //���� 50000���� ���̱�
        if (GameManager.Instance.zombieKillCount >= 50000)
        {
            name = "Zombie Annihilator"; //���� �Ҹ���
            description = "Kill 50000 zombies";
        }

        //���� 3�� ���� ���̱�
        if (GameManager.Instance.bossKilledTime <= 180)
        {
            name = "Boss slayer"; //���� �����̾�
            description = " Clear the boss in 3 minutes";
        }

        //�극�� ����� �� ������
        if (GameManager.Instance.allMusiciansRecruited)
        {
            name = "Squad Leader"; //�д���
            description = "Recruiting Bremen Musicians";
        }

        //������� ������ ������ �����
        if (GameManager.Instance.ringOwner)
        {
            name = "Ring Bearer"; //������ ���տ��� ������� �����ڸ� ��Ī�Ѵ���.
            description = "Being the absolute ring owner";
        }

        //NFT �������� ù �����ϱ�
        if (GameManager.Instance.firstNFTPurchase)
        {
            name = "NFT Pioneer"; //NFT ��ô��
            description = "Buying NFT items for the first time";
        }

        //��� ���������� Ŭ�����ϱ�
        if (GameManager.Instance.allStagesCleared)
        {
            name = "Ultimate Survivor"; //�ñ��� ������
            description = "Clear All Stages";
        }

        NFTManager.Instance.mintRewardNFT(name, description);
        }
        */
    }
}