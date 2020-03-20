using UnityEngine;
using System.Collections.Generic;

namespace RTS
{
    public class SkillComponent : MomentComponentBase
    {
        public int SkillId1;
        public int SkillId2;
        public int SkillId3;
        public int SkillId4;
        // 保存目标entity的ID
        public List<int> SkillTargets = new List<int>();

        public override MomentComponentBase DeepCopy()
        {
            SkillComponent c = new SkillComponent();
            c.SkillId1 = SkillId1;
            c.SkillId2 = SkillId2;
            c.SkillId3 = SkillId3;
            c.SkillId4 = SkillId4;

            c.SkillTargets = new List<int>();
            foreach (var t in SkillTargets)
            {
                c.SkillTargets.Add(t);
            }

            return c;
        }
    }
}
