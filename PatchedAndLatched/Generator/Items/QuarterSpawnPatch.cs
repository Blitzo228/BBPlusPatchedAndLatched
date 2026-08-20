using HarmonyLib;
using UnityEngine;
using System.Linq;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    internal static class QuarterSpawnPatch
    {
        private static ItemObject? _quarterItem;
        private static bool _spawned;

        static QuarterSpawnPatch()
        {
            _quarterItem = Resources
                .FindObjectsOfTypeAll<ItemObject>()
                .FirstOrDefault(item => item.itemType == Items.Quarter);
        }

        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        private static void BeginPlayPostfix(EnvironmentController __instance)
        {
            if (_quarterItem == null) return;
            if (_spawned) return;

            SpawnQuarters(__instance);
            _spawned = true;
        }

        private static void SpawnQuarters(EnvironmentController ec)
        {
            float chance = PatchedAndLatchedPlugin.QuarterSpawnChance!.Value;
            int maxCoins = PatchedAndLatchedPlugin.QuarterMaxPerFloor!.Value;
            if (chance <= 0f || maxCoins <= 0) return;

            int spawned = 0;
            foreach (var cell in ec.AllCells())
            {
                if (cell == null) continue;
                if (cell.room == null) continue;
                if (cell.room.type != RoomType.Hall) continue;
                if (cell.HasAnyHardCoverage) continue;
                if (spawned >= maxCoins) break;
                if (Random.value > chance) continue;

                var pos = cell.FloorWorldPosition;
                var pickup = ec.CreateItem(cell.room, _quarterItem, new Vector2(pos.x, pos.z));
                if (pickup != null)
                {
                    pickup.Hide(false);
                    spawned++;
                }
            }
        }
    }
}
