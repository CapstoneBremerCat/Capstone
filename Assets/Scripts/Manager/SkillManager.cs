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
        private List<Skill> activeSkills = new List<Skill>();
        private List<Skill> passiveSkills = new List<Skill>();

        public void LoadSavedEquipments()
        {
            foreach (Skill skill in skillPool)
            {
                Skill targetSkill = NFTManager.Instance.GetNFTSkillByID(skill.skillInfo.skillId);
                switch (targetSkill.skillType)
                {
                    case SkillType.Active:
                        activeSkills.Add(targetSkill as ActiveSkill);
                        break;
                    case SkillType.Passive:
                        passiveSkills.Add(targetSkill as PassiveSkill);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}