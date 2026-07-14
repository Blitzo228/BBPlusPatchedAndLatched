using HarmonyLib;
using PatchedAndLatched;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Elevator), "SetState")]
    internal static class FinalLevelPreEndingPatch
    {
        private static bool _triggered = false;

        [HarmonyPatch(typeof(EnvironmentController), "BeginPlay")]
        [HarmonyPostfix]
        private static void ResetTrigger() => _triggered = false;

        [HarmonyPostfix]
        private static void CheckIfPenultimateElevator(Elevator __instance, ElevatorState state, EnvironmentController ___ec)
        {
            if (!PatchedAndLatchedPlugin.FinalLevelPreEndingEnabled.Value) return;
            if (_triggered) return;
            if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Free) return;
            if (___ec == null) return;

            var manager = Singleton<BaseGameManager>.Instance;
            if (manager == null || manager.levelObject == null || !manager.levelObject.finalLevel) return;

            if (state != ElevatorState.OutOfOrder) return;

            int totalElevators = ___ec.Elevators.Count;
            int brokenCount = ___ec.Elevators.Count(e => e.CurrentState == ElevatorState.OutOfOrder);
            if (brokenCount != totalElevators - 1) return;

            _triggered = true;

            var toRemove = new List<NPC>();
            foreach (var npc in ___ec.Npcs)
            {
                if (npc != null && !(npc is Baldi))
                    toRemove.Add(npc);
            }

            foreach (var npc in toRemove)
            {
                try
                {
                    var despawn = npc.GetType().GetMethod("Despawn");
                    if (despawn != null)
                        despawn.Invoke(npc, null);
                    else
                        Object.Destroy(npc.gameObject);
                }
                catch
                {
                    Object.Destroy(npc.gameObject);
                }
                ___ec.Npcs.Remove(npc);
            }

            ___ec.npcsToSpawn?.Clear();
            ___ec.npcsLeftToSpawn?.Clear();

            var baldi = ___ec.GetBaldi();
            if (baldi != null)
            {
                ___ec.StartCoroutine(AccelerateBaldiFast(baldi));
            }
        }

        private static IEnumerator AccelerateBaldiFast(Baldi baldi)
        {
            float elapsed = 0f;
            while (baldi != null && baldi.gameObject != null)
            {
                elapsed += Time.deltaTime;
                if (elapsed >= 0.5f)
                {
                    elapsed = 0f;
                    baldi.GetAngry(0.3f);
                }
                yield return null;
            }
        }
    }
}