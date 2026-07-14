using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Playtime))]
    internal static class RandomJumpsPatch
    {
        [HarmonyPatch("StartJumprope")]
        [HarmonyPostfix]
        private static void SetRandomJumps(Playtime __instance, Jumprope ___currentJumprope)
        {
            if (!PatchedAndLatchedPlugin.RandomJumpsEnabled.Value) return;
            if (___currentJumprope == null) return;

            int min = PatchedAndLatchedPlugin.MinJumps.Value;
            int max = PatchedAndLatchedPlugin.MaxJumps.Value;
            if (min > max)
            {
                int temp = min;
                min = max;
                max = temp;
            }
            int randomJumps = Random.Range(min, max + 1);

            Traverse.Create(___currentJumprope).Field("maxJumps").SetValue(randomJumps);
        }
    }
}