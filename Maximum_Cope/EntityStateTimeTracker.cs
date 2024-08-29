using EntityStates;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maximum_Cope
{
    public static class EntityStateTimeTracker
    {
        public static Dictionary<EntityState, float> lastUpdateDict = new Dictionary<EntityState, float>();

        internal static void Init()
        {
            RoR2.Stage.onStageStartGlobal += ClearNullEntries;
            On.EntityStates.EntityState.OnEnter += EntityState_OnEnter;
            On.EntityStates.EntityState.GetDeltaTime += EntityState_GetDeltaTime;
            On.EntityStates.EntityState.OnExit += EntityState_OnExit;
        }

        private static void ClearNullEntries(RoR2.Stage obj)
        {
            lastUpdateDict = (from kv in lastUpdateDict
                              where kv.Key != null
                              select kv).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private static void EntityState_OnExit(On.EntityStates.EntityState.orig_OnExit orig, EntityState self)
        {
            orig(self);
            lastUpdateDict.Remove(self);
        }

        private static void EntityState_OnEnter(On.EntityStates.EntityState.orig_OnEnter orig, EntityState self)
        {
            lastUpdateDict.Add(self, Time.time);
            orig(self);
        }

        private static float EntityState_GetDeltaTime(On.EntityStates.EntityState.orig_GetDeltaTime orig, EntityState self)
        {
            if (lastUpdateDict.TryGetValue(self, out float lastUpdateTime))
            {
                float deltaTime = Time.time - lastUpdateTime;
                lastUpdateDict[self] = Time.time;
                return deltaTime;
            }
            return orig(self);
        }
    }
}
