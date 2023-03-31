using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using BlockChain;
namespace Game
{
    public class SkillManager: MonoBehaviour
    {
        #region instance
        private static SkillManager instance = null;
        public static SkillManager Instance { get { return instance; } }

        private void Awake()
        {
            // Scene�� �̹� �ν��Ͻ��� ���� �ϴ��� Ȯ�� �� ó��
            if (instance)
            {
                Destroy(this.gameObject);
                return;
            }
            // instance�� ���� ������Ʈ�� �����
            instance = this;

            // Scene �̵� �� ���� ���� �ʵ��� ó��
            DontDestroyOnLoad(this.gameObject);
        }
        #endregion

        [SerializeField] private List<Skill> skillPool = new List<Skill>();
        private List<Skill> owenedSkills = new List<Skill>();
        private List<ActiveSkill> activeSkills = new List<ActiveSkill>();
        private List<PassiveSkill> passiveSkills = new List<PassiveSkill>();
        private bool isSkillLoaded = false;
        public List<Skill> GetOwnedSkills()
        {
            if (!isSkillLoaded)
            {
                Debug.LogWarning("Owned Skill not Loaded,");
                return null;
            }
            return owenedSkills;
        }
        public void LoadOwnedSkills(List<Skill> owenedNFTSkills)
        {
            foreach (Skill skill in owenedNFTSkills)
            {
                Skill targetSkill = skillPool.Find(s => s.skillInfo.skillId == skill.skillInfo.skillId);
                if (targetSkill == null)
                {
                    continue;
                }
                switch (targetSkill.skillType)
                {
                    case SkillType.Active:
                        targetSkill.SetSkillInfo(skill.skillInfo);
                        activeSkills.Add(targetSkill as ActiveSkill);
                        break;
                    case SkillType.Passive:
                        targetSkill.SetSkillInfo(skill.skillInfo);
                        passiveSkills.Add(targetSkill as PassiveSkill);
                        break;
                    default:
                        break;
                }
                owenedSkills.Add(targetSkill);
            }
            isSkillLoaded = true;
        }
    }
}