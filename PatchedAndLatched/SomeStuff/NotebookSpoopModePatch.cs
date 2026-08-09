using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    public static class NotebookSpoopModePatch
    {
        public static bool spoopModeStarted = false;

        [HarmonyPatch(typeof(HappyBaldi), nameof(HappyBaldi.Activate))]
        [HarmonyPrefix]
        private static bool HappyBaldiActivatePrefix()
        {
            return false;
        }

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.CollectNotebooks))]
        [HarmonyPostfix]
        private static void CollectNotebooksPostfix(BaseGameManager __instance)
        {
            if (spoopModeStarted) return;

            int targetNotebooks = Mathf.Min(2, __instance.NotebookTotal);

            if (targetNotebooks > 0 && __instance.FoundNotebooks >= targetNotebooks)
            {
                spoopModeStarted = true;

                Singleton<MusicManager>.Instance.StopMidi();
                __instance.BeginSpoopMode();
                __instance.Ec.SpawnNPCs();
                __instance.Ec.StartEventTimers();

                HappyBaldi happyBaldi = Object.FindObjectOfType<HappyBaldi>();
                if (happyBaldi != null)
                {
                    if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
                    {
                        Baldi baldi = __instance.Ec.GetBaldi();
                        if (baldi != null)
                            baldi.transform.position = happyBaldi.transform.position;
                    }
                    else if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Free)
                    {
                        Baldi baldi = __instance.Ec.GetBaldi();
                        if (baldi != null)
                        {
                            baldi.Despawn();
                        }
                    }

                    Object.Destroy(happyBaldi.gameObject);
                }
            }
        }

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.Initialize))]
        [HarmonyPostfix]
        private static void InitializePostfix()
        {
            spoopModeStarted = false;
        }
    }
}
