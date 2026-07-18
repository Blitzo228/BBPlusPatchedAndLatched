using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(ITM_YTPs))]
    internal static class YTPSPointsPatch
    {
        private static FieldInfo _valueField;

        static YTPSPointsPatch()
        {
            _valueField = AccessTools.Field(typeof(ITM_YTPs), "value");
        }

        [HarmonyPrefix]
        [HarmonyPatch("Use")]
        private static void Prefix(ITM_YTPs __instance, ref int __state)
        {
            if (!PatchedAndLatchedPlugin.EnableYTPSMultiplier.Value) return;
            if (_valueField == null) return;

            int original = (int)_valueField.GetValue(__instance);
            __state = original;
            int multiplier = PatchedAndLatchedPlugin.YTPSMultiplier.Value;
            int newValue = original * multiplier;
            _valueField.SetValue(__instance, newValue);
        }

        [HarmonyPostfix]
        [HarmonyPatch("Use")]
        private static void Postfix(ITM_YTPs __instance, int __state)
        {
            if (!PatchedAndLatchedPlugin.EnableYTPSMultiplier.Value) return;
            if (_valueField == null) return;
            _valueField.SetValue(__instance, __state);
        }
    }
}