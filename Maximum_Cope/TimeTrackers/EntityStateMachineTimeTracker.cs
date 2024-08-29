using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maximum_Cope.TimeTrackers
{
    public static class EntityStateMachineTimeTracker
    {
        public static Dictionary<EntityStateMachine, float> lastUpdateDict = new Dictionary<EntityStateMachine, float>();

        internal static void Init()
        {
            On.RoR2.EntityStateMachine.ManagedFixedUpdate += EntityStateMachine_ManagedFixedUpdate;
            Stage.onStageStartGlobal += ClearNullEntries;
            On.RoR2.EntityStateMachine.Awake += EntityStateMachine_Awake;
            On.RoR2.EntityStateMachine.OnDestroy += EntityStateMachine_OnDestroy;
        }

        private static void EntityStateMachine_ManagedFixedUpdate(On.RoR2.EntityStateMachine.orig_ManagedFixedUpdate orig, EntityStateMachine self, float deltaTime)
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

        private static void EntityStateMachine_Awake(On.RoR2.EntityStateMachine.orig_Awake orig, EntityStateMachine self)
        {
            lastUpdateDict.Add(self, Time.time);
            orig(self);
        }

        private static void EntityStateMachine_OnDestroy(On.RoR2.EntityStateMachine.orig_OnDestroy orig, EntityStateMachine self)
        {
            orig(self);
            lastUpdateDict.Remove(self);
        }
    }
}
