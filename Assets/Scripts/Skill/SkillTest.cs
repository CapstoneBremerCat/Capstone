using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class SkillTest : MonoBehaviour
{
    [SerializeField] private Skill[] skills;
    // Start is called before the first frame update
    public void DebugSkill()
    {
        foreach (Skill skill in skills)
        {
            SkillInventory.Instance.AddSkill(skill);
        }
    }

}
