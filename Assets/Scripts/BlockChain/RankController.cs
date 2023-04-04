using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BlockChain;

public class RankController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SetRank(string name, int score)
    {
        nameText.text = name;
        scoreText.text = score.ToString();
    }
}
