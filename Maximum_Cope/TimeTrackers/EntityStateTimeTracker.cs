using EntityStates;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maximum_Cope.TimeTrackers
{
    public static class EntityStateTimeTracker
    {
        public static Dictionary<EntityState, float> lastDeltaUpdateDict = new Dictionary<EntityState, float>();
        public static Dictionary<EntityState, float> lastUpdateDict = new Dictionary<EntityState, float>();
        public static Dictionary<EntityState, float> lastFixedUpdateDict = new Dictionary<EntityState, float>();

        internal static void Init()
        {
            On.EntityStates.EntityState.GetDeltaTime += EntityState_GetDeltaTime;
            On.EntityStates.EntityState.FixedUpdate += EntityState_FixedUpdate;
            On.EntityStates.EntityState.Update += EntityState_Update;
            RoR2.Stage.onStageStartGlobal += ClearNullEntries;
            On.EntityStates.EntityState.OnEnter += EntityState_OnEnter;
            On.EntityStates.EntityState.OnExit += EntityState_OnExit;
        }

        //Don't update lastDeltaUpdate here, because this can get called multiple times in 1 frame.
        private static float EntityState_GetDeltaTime(On.EntityStates.EntityState.orig_GetDeltaTime orig, EntityState self)
        {
            if (lastDeltaUpdateDict.TryGetValue(self, out float lastUpdateTime))
            {
                float deltaTime = Time.time - lastUpdateTime;
                lastDeltaUpdateDict[self] = Time.time;
                return deltaTime;
            }
            return orig(self);
        }

        private static void EntityState_FixedUpdate(On.EntityStates.EntityState.orig_FixedUpdate orig, EntityState self)
        {
            float deltaTime = Time.fixedDeltaTime;
            if (lastFixedUpdateDict.TryGetValue(self, out float lastUpdateTime))
            {
                deltaTime = Time.time - lastUpdateTime;
                lastFixedUpdateDict[self] = Time.time;
            }
            self.fixedAge += deltaTime;
        }

        private static void EntityState_Update(On.EntityStates.EntityState.orig_Update orig, EntityState self)
        {
            float deltaTime = Time.deltaTime;
            if (lastUpdateDict.TryGetValue(self, out float lastUpdateTime))
            {
                deltaTime = Time.time - lastUpdateTime;
                lastUpdateDict[self] = Time.time;
            }
            self.age += deltaTime;
        }

        private static void ClearNullEntries(RoR2.Stage obj)
        {
            lastDeltaUpdateDict = (from kv in lastDeltaUpdateDict
                                   where kv.Key != null
                                   select kv).ToDictionary(kv => kv.Key, kv => kv.Value);
            lastUpdateDict = (from kv in lastUpdateDict
                                   where kv.Key != null
                                   select kv).ToDictionary(kv => kv.Key, kv => kv.Value);
            lastFixedUpdateDict = (from kv in lastFixedUpdateDict
                                   where kv.Key != null
                                   select kv).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private static void EntityState_OnExit(On.EntityStates.EntityState.orig_OnExit orig, EntityState self)
        {
            orig(self);
            lastDeltaUpdateDict.Remove(self);
            lastUpdateDict.Remove(self);
            lastFixedUpdateDict.Remove(self);
        }

        private static void EntityState_OnEnter(On.EntityStates.EntityState.orig_OnEnter orig, EntityState self)
        {
            lastDeltaUpdateDict.Add(self, Time.time);
            lastUpdateDict.Add(self, Time.time);
            lastFixedUpdateDict.Add(self, Time.time);
            orig(self);
        }
    }
}
