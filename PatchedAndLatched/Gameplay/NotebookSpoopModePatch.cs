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
        private static bool HappyBaldiActivatePrefix(HappyBaldi __instance)
        {
            if (Object.FindObjectOfType<TutorialGameManager>() != null)
                return true;
            return false;
        }

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.CollectNotebooks))]
        [HarmonyPostfix]
        private static void CollectNotebooksPostfix(BaseGameManager __instance)
        {
            if (spoopModeStarted) return;
            if (__instance is TutorialGameManager) return;

            int targetNotebooks = Mathf.Min(2, __instance.NotebookTotal);

            if (targetNotebooks > 0 && __instance.FoundNotebooks >= targetNotebooks)
            {
                StartSpoopMode(__instance);
            }
        }

        [HarmonyPatch(typeof(Activity), "Completed", new System.Type[] { typeof(int), typeof(bool) })]
        [HarmonyPostfix]
        private static void ActivityCompletedPostfix(Activity __instance, int player, bool correct)
        {
            if (spoopModeStarted) return;
            if (correct) return; 
            if (Singleton<BaseGameManager>.Instance is TutorialGameManager) return;

            var baseGameManager = Singleton<BaseGameManager>.Instance;
            if (baseGameManager != null)
            {
                StartSpoopMode(baseGameManager);
            }
        }

        private static void StartSpoopMode(BaseGameManager manager)
        {
            if (spoopModeStarted) return;
            spoopModeStarted = true;

            Singleton<MusicManager>.Instance.StopMidi();
            manager.BeginSpoopMode();
            manager.Ec.SpawnNPCs();
            manager.Ec.StartEventTimers();

            HappyBaldi happyBaldi = Object.FindObjectOfType<HappyBaldi>();
            if (happyBaldi != null)
            {
                if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
                {
                    Baldi baldi = manager.Ec.GetBaldi();
                    if (baldi != null)
                        baldi.transform.position = happyBaldi.transform.position;
                }
                else if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Free)
                {
                    Baldi baldi = manager.Ec.GetBaldi();
                    if (baldi != null)
                        baldi.Despawn();
                }

                Object.Destroy(happyBaldi.gameObject);
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
