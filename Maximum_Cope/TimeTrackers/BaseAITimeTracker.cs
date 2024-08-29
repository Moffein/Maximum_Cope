using System.Collections.Generic;
using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace Maximum_Cope.TimeTrackers
{
    public static class BaseAITimeTracker
    {
        public static Dictionary<BaseAI, float> lastUpdateDict = new Dictionary<BaseAI, float>();

        internal static void Init()
        {
            Stage.onStageStartGlobal += ClearNullEntries;
            On.RoR2.CharacterAI.BaseAI.Awake += BaseAI_Awake;
            On.RoR2.CharacterAI.BaseAI.OnDestroy += BaseAI_OnDestroy;
            On.RoR2.CharacterAI.BaseAI.ManagedFixedUpdate += BaseAI_ManagedFixedUpdate;
        }

        private static void BaseAI_ManagedFixedUpdate(On.RoR2.CharacterAI.BaseAI.orig_ManagedFixedUpdate orig, BaseAI self, float deltaTime)
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

        private static void BaseAI_Awake(On.RoR2.CharacterAI.BaseAI.orig_Awake orig, BaseAI self)
        {
            lastUpdateDict.Add(self, Time.time);
            orig(self);
        }

        private static void BaseAI_OnDestroy(On.RoR2.CharacterAI.BaseAI.orig_OnDestroy orig, BaseAI self)
        {
            orig(self);
            lastUpdateDict.Remove(self);
        }
    }
}
