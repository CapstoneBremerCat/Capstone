using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class SkillTest : MonoBehaviour
{
    [SerializeField] private Skill[] skills;
    public SkillWindow skillInventory;
    // Start is called before the first frame update
    public void DebugSkill()
    {
        foreach (Skill skill in skills)
        {
            skillInventory.AddSkill(skill);
        }
    }

}
