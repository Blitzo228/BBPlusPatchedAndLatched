using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(Baldi))]
    public static class OnlyBaldiEveryFloorPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("UpdateSlapDistance")]
        public static void UpdateSlapDistance_Prefix(Baldi __instance)
        {
            if (!PatchedAndLatchedPlugin.OnlyBaldiEveryFloor.Value) return;

            __instance.speedMultiplier = 5f;
        }
    }

    [HarmonyPatch(typeof(LevelBuilder), "AddNpcsFromPreviousLevels")]
    public static class AddNpcsFromPreviousLevelsPatch
    {
        [HarmonyPrefix]
        public static bool AddNpcsFromPreviousLevels_Prefix(LevelBuilder __instance)
        {
            if (!PatchedAndLatchedPlugin.OnlyBaldiEveryFloor.Value) return true;

            return false;
        }
    }

    [HarmonyPatch(typeof(EnvironmentController), "SpawnNPCs")]
    public static class EnvironmentControllerPatch
    {
        [HarmonyPostfix]
        public static void SpawnNPCs_Postfix(EnvironmentController __instance)
        {
            if (!PatchedAndLatchedPlugin.OnlyBaldiEveryFloor.Value) return;

            int baldiCount = 0;
            for (int i = __instance.Npcs.Count - 1; i >= 0; i--)
            {
                NPC npc = __instance.Npcs[i];
                if (npc == null) continue;

                if (npc.Character == Character.Baldi)
                {
                    baldiCount++;
                    if (baldiCount > 1)
                    {
                        __instance.Npcs.RemoveAt(i);
                        Object.Destroy(npc.gameObject);
                    }
                }
                else
                {
                    __instance.Npcs.RemoveAt(i);
                    Object.Destroy(npc.gameObject);
                }
            }
        }
    }
}