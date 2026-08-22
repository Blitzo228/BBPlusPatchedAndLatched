using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(EnvironmentController), "SpawnNPCs")]
    internal static class PrincipalSpawnPatch
    {
        [HarmonyPrefix]
        private static void Prefix(EnvironmentController __instance)
        {
            if (!PatchedAndLatchedPlugin.EnablePrincipalSpawnChance.Value) return;
            float chance = PatchedAndLatchedPlugin.PrincipalSpawnChance.Value;
            if (chance <= 0f) return;
            if (Random.value < chance)
            {
                for (int i = __instance.npcsToSpawn.Count - 1; i >= 0; i--)
                {
                    if (__instance.npcsToSpawn[i] is Principal)
                    {
                        __instance.npcsToSpawn.RemoveAt(i);
                    }
                }
            }
        }
    }
}