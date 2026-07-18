using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(NoLateTeacher))]
    internal static class MrsPompTimePatch
    {
        private static FieldInfo _classTimeField;

        static MrsPompTimePatch()
        {
            _classTimeField = AccessTools.Field(typeof(NoLateTeacher), "classTime");
        }


        [HarmonyPatch("PlayerCaught")]
        [HarmonyPrefix]
        private static void OverrideClassTimeOnCatch(NoLateTeacher __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableMrsPompTimeControl.Value) return;
            if (_classTimeField == null) return;

            float newTime = PatchedAndLatchedPlugin.MrsPompClassTime.Value;

            if (PatchedAndLatchedPlugin.MrsPompRandomizeTime.Value)
            {
                float min = PatchedAndLatchedPlugin.MrsPompMinTime.Value;
                float max = PatchedAndLatchedPlugin.MrsPompMaxTime.Value;
                newTime = Random.Range(min, max);
            }


            if (newTime < 60f) newTime = 60f;      
            if (newTime > 9999f) newTime = 9999f;  

            _classTimeField.SetValue(__instance, newTime);
        }
    }
}