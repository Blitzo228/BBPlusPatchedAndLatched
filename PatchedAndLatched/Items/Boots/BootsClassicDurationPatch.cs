using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(ITM_Boots))]
    public static class BootsClassicDurationPatch
    {
        private static FieldInfo? _setTimeField;

        [HarmonyPrefix]
        [HarmonyPatch("Use")]
        public static void Use_Prefix(ITM_Boots __instance)
        {
            if (!PatchedAndLatchedPlugin.BootsClassicDuration.Value) return;

            if (_setTimeField == null)
                _setTimeField = typeof(ITM_Boots).GetField("setTime", BindingFlags.NonPublic | BindingFlags.Instance);

            if (_setTimeField != null)
                _setTimeField.SetValue(__instance, 15f);
        }
    }
}