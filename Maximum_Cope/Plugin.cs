using BepInEx;
using Maximum_Cope.TimeTrackers;
using System;

namespace R2API.Utils
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ManualNetworkRegistrationAttribute : Attribute
    {
    }
}

namespace Maximum_Cope
{
    [BepInPlugin("com.Moffein.MaximumCope", "Maximum Cope", "1.0.0")]
    public class MaximumCopePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            //Lots of duplicated code, should refactor later.
            EntityStateTimeTracker.Init();
            BaseAITimeTracker.Init();
            HealthComponentTimeTracker.Init();
            GenericSkillTimeTracker.Init();
            EntityStateMachineTimeTracker.Init();
        }
    }
}
