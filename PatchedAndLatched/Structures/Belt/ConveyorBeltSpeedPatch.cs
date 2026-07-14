using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using UnityEngine;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(Structure_ConveyorBelt))]
    public static class ConveyorBeltSpeedPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Generate")]
        public static void Generate_Prefix(Structure_ConveyorBelt __instance)
        {
            if (!PatchedAndLatchedPlugin.OldConveyorBelt.Value) return;

            var field = typeof(Structure_ConveyorBelt).GetField("beltSpeed",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) field.SetValue(__instance, 12.5f);
        }

        [HarmonyPostfix]
        [HarmonyPatch("BuildBelt")]
        public static void BuildBelt_Postfix(Structure_ConveyorBelt __instance)
        {
            if (!PatchedAndLatchedPlugin.OldConveyorBelt.Value) return;

            var builtBeltsField = typeof(Structure_ConveyorBelt).GetField("builtBelts",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (builtBeltsField != null)
            {
                var builtBelts = builtBeltsField.GetValue(__instance) as System.Collections.Generic.List<BeltManager>;
                if (builtBelts != null && builtBelts.Count > 0)
                {
                    foreach (var belt in builtBelts)
                    {
                        if (belt != null)
                        {
                            belt.SetSpeed(12.5f);
                        }
                    }
                }
            }
        }
    }
}