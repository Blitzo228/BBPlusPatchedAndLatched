using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    public static class FastModePatch
    {
        private static TimeScaleModifier _timeScaleMod = new TimeScaleModifier
        {
            npcTimeScale = 3f,
            environmentTimeScale = 3f
        };
        private static MovementModifier _moveMod = new MovementModifier(Vector3.zero, 2f);

        [HarmonyPatch(typeof(EnvironmentController), "BeginPlay")]
        [HarmonyPostfix]
        private static void AddTimeScale(EnvironmentController __instance)
        {
            if (!PatchedAndLatchedPlugin.FastModeEnabled.Value) return;
            __instance.AddTimeScale(_timeScaleMod);
        }

        [HarmonyPatch(typeof(PlayerManager), "Start")]
        [HarmonyPostfix]
        private static void AddMoveMod(PlayerManager __instance)
        {
            if (!PatchedAndLatchedPlugin.FastModeEnabled.Value) return;
            if (!__instance.Am.moveMods.Contains(_moveMod))
                __instance.Am.moveMods.Add(_moveMod);
        }
    }
}