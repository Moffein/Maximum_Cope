using BepInEx;
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
            EntityStateTimeTracker.Init();
        }
    }
}
