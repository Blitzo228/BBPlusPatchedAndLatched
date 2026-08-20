using HarmonyLib;
using PatchedAndLatched;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Chalkboard), "OnPlayerExit")]
    internal static class ChalkboardExitFix
    {
        private static FieldInfo? _chalkFaceField = AccessTools.Field(typeof(Chalkboard), "chalkFace");

        [HarmonyPrefix]
        private static bool Prefix(Chalkboard __instance)
        {
            if (_chalkFaceField == null) return true;
            var chalkFace = _chalkFaceField.GetValue(__instance) as ChalkFace;
            if (chalkFace == null) return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(Chalkboard), "OnPlayerEnter")]
    internal static class ChalkboardEnterFix
    {
        private static FieldInfo? _chalkFaceField = AccessTools.Field(typeof(Chalkboard), "chalkFace");

        [HarmonyPrefix]
        private static bool Prefix(Chalkboard __instance)
        {
            if (_chalkFaceField == null) return true;
            var chalkFace = _chalkFaceField.GetValue(__instance) as ChalkFace;
            if (chalkFace == null) return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(Elevator), "SetState")]
    internal static class FinalLevelPreEndingPatch
    {
        private static bool _triggered = false;
        private static EnvironmentController? _ec = null;
        private static Coroutine? _rageCoroutine;
        private static FieldInfo? _eventsField;

        static FinalLevelPreEndingPatch()
        {
            _eventsField = AccessTools.Field(typeof(EnvironmentController), "events");
        }

        [HarmonyPatch(typeof(EnvironmentController), "BeginPlay")]
        [HarmonyPostfix]
        private static void ResetTrigger(EnvironmentController __instance)
        {
            _triggered = false;
            if (_rageCoroutine != null && _ec != null)
            {
                _ec.StopCoroutine(_rageCoroutine);
                _rageCoroutine = null;
            }
            _ec = null;
        }

        [HarmonyPostfix]
        private static void CheckIfPenultimateElevator(Elevator __instance, ElevatorState state, EnvironmentController ___ec)
        {
            if (!PatchedAndLatchedPlugin.FinalLevelPreEndingEnabled!.Value) return;
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
            _ec = ___ec;

            var chalkboards = Object.FindObjectsOfType<Chalkboard>();
            foreach (var cb in chalkboards)
            {
                if (cb != null && cb.gameObject != null)
                {
                    cb.enabled = false;
                    var field = typeof(Chalkboard).GetField("chalkFace", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field != null) field.SetValue(cb, null);
                    Object.Destroy(cb.gameObject);
                }
            }

            var npcsToRemove = new List<NPC>();
            foreach (var npc in ___ec.Npcs)
            {
                if (npc != null && !(npc is Baldi))
                    npcsToRemove.Add(npc);
            }

            foreach (var npc in npcsToRemove)
            {
                try
                {
                    npc.enabled = false;
                    foreach (Transform child in npc.transform)
                        child.gameObject.SetActive(false);
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

            if (_eventsField != null)
            {
                var events = _eventsField.GetValue(___ec) as List<RandomEvent>;
                if (events != null)
                {
                    foreach (var ev in events)
                    {
                        if (ev != null && ev.Type != RandomEventType.TimeOut && ev.Active)
                            ev.End();
                    }
                }
            }

            var baldi = ___ec.GetBaldi();
            if (baldi != null)
            {
                _rageCoroutine = ___ec.StartCoroutine(Rage(baldi));
            }
        }

        private static IEnumerator Rage(Baldi baldi)
        {
            float rageDelay = 1f;
            float angerAmount = 0.5f;
            WaitForSeconds delay = new WaitForSeconds(rageDelay);
            while (baldi != null && baldi.gameObject != null)
            {
                baldi.GetAngry(angerAmount);
                yield return delay;
            }
        }
    }
}