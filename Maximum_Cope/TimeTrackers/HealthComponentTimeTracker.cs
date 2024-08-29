using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maximum_Cope.TimeTrackers
{
    public static class HealthComponentTimeTracker
    {
        public static Dictionary<HealthComponent, float> lastUpdateDict = new Dictionary<HealthComponent, float>();

        internal static void Init()
        {
            On.RoR2.HealthComponent.ManagedFixedUpdate += HealthComponent_ManagedFixedUpdate;
            On.RoR2.HealthComponent.Awake += HealthComponent_Awake;
            On.RoR2.HealthComponent.OnDestroy += HealthComponent_OnDestroy;
            Stage.onStageStartGlobal += ClearNullEntries;
        }

        private static void HealthComponent_ManagedFixedUpdate(On.RoR2.HealthComponent.orig_ManagedFixedUpdate orig, HealthComponent self, float deltaTime)
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

        private static void HealthComponent_Awake(On.RoR2.HealthComponent.orig_Awake orig, HealthComponent self)
        {
            lastUpdateDict.Add(self, Time.time);
            orig(self);
        }

        private static void HealthComponent_OnDestroy(On.RoR2.HealthComponent.orig_OnDestroy orig, HealthComponent self)
        {
            orig(self);
            lastUpdateDict.Remove(self);
        }
    }
}
