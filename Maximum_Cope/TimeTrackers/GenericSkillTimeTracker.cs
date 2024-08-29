using System.Collections.Generic;
using RoR2.Skills;
using RoR2;
using UnityEngine;
using System.Linq;

namespace Maximum_Cope.TimeTrackers
{
    public static class GenericSkillTimeTracker
    {
        public static Dictionary<GenericSkill, float> lastUpdateDict = new Dictionary<GenericSkill, float>();

        internal static void Init()
        {
            On.RoR2.GenericSkill.ManagedFixedUpdate += GenericSkill_ManagedFixedUpdate;
            On.RoR2.GenericSkill.Awake += GenericSkill_Awake;
            On.RoR2.GenericSkill.OnDestroy += GenericSkill_OnDestroy;
            Stage.onStageStartGlobal += ClearNullEntries;
        }

        private static void GenericSkill_ManagedFixedUpdate(On.RoR2.GenericSkill.orig_ManagedFixedUpdate orig, GenericSkill self, float deltaTime)
        {
            if (lastUpdateDict.TryGetValue(self, out float lastUpdateTime))
            {
                deltaTime = Time.time - lastUpdateTime;
                lastUpdateDict[self] = Time.time;
            }
            orig(self, deltaTime);
        }

        private static void ClearNullEntries(Stage obj)
        {
            lastUpdateDict = (from kv in lastUpdateDict
                              where kv.Key != null
                              select kv).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private static void GenericSkill_Awake(On.RoR2.GenericSkill.orig_Awake orig, GenericSkill self)
        {
            lastUpdateDict.Add(self, Time.time);
            orig(self);
        }

        private static void GenericSkill_OnDestroy(On.RoR2.GenericSkill.orig_OnDestroy orig, GenericSkill self)
        {
            orig(self);
            lastUpdateDict.Remove(self);
        }
    }
}
