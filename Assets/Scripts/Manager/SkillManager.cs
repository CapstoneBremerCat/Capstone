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
        [SerializeField] private List<StatusObject> statusObjectPool = new List<StatusObject>();
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
        public void LoadOwnedSkills(List<SkillInfo> owenedNFTSkills)
        {
            activeSkills.Clear();
            passiveSkills.Clear();
            if (owenedNFTSkills == null) return;
            foreach (SkillInfo skillInfo in owenedNFTSkills)
            {
                Skill targetSkill = skillPool.Find(s => s.skillInfo.skillCode == skillInfo.skillCode);
                if (targetSkill == null || skillInfo.skillType == SkillType.Reward)
                {
                    continue;
                }
                switch (skillInfo.skillType)
                {
                    case SkillType.Active:
                        targetSkill.SetSkillInfo(skillInfo);
                        activeSkills.Add(targetSkill as ActiveSkill);
                        break;
                    case SkillType.Passive:
                        targetSkill.SetSkillInfo(skillInfo);
                        var passive = targetSkill as PassiveSkill;
                        passive.SetStatusData(GetStatusData(skillInfo.skillCode, skillInfo.skillValue));
                        passiveSkills.Add(targetSkill as PassiveSkill);
                        break;
                    default:
                        break;
                }
                owenedSkills.Add(targetSkill);
            }
            isSkillLoaded = true;
        }

        // Returns the StatusData for the given skillCode and skillValue.
        // Searches the statusObjectPool for the StatusObject with a matching skillCode.
        // Returns null if no matching StatusObject is found.
        private StatusData GetStatusData(int skillCode, int skillValue)
        {
            // Find the StatusObject in the statusObjectPool that matches the skillCode.
            StatusObject target = statusObjectPool.Find(x => x.skillCode == skillCode);

            if (target == null)
            {
                Debug.LogError("StatusObject with skillCode " + skillCode + " not found.");
                return null;
            }
            // for basic value
            if (skillValue == 0) skillValue = 10;
            // Calculate the StatusData based on the target StatusObject and the skillValue.
            return target.status.GetMultipliedStat(skillValue);
        }
    }
}