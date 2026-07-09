using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(Structure_ConveyorBelt))]
    public static class ConveyorBeltSpeedPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Generate")]
        public static void Generate_Prefix(Structure_ConveyorBelt __instance)
        {
            var field = typeof(Structure_ConveyorBelt).GetField("beltSpeed",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null) field.SetValue(__instance, 13f);
        }

        [HarmonyPostfix]
        [HarmonyPatch("BuildBelt")]
        public static void BuildBelt_Postfix(BeltManager beltManager)
        {
            if (beltManager != null)
            {
                beltManager.SetSpeed(12.5f);
            }
        }
    }
}