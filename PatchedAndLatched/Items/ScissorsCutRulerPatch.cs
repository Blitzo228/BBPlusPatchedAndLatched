using HarmonyLib;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(ITM_Scissors))]
    internal static class ScissorsCutRulerPatch
    {
        private static FieldInfo _audSnipField;

        static ScissorsCutRulerPatch()
        {
            _audSnipField = AccessTools.Field(typeof(ITM_Scissors), "audSnip");
        }

        [HarmonyPatch("Use")]
        [HarmonyPostfix]
        private static void Postfix(ITM_Scissors __instance, PlayerManager pm, ref bool __result)
        {
            if (!PatchedAndLatchedPlugin.EnableScissorsCutRuler.Value) return;

            bool foundBaldi = false;
            Collider[] colliders = new Collider[16];
            int count = Physics.OverlapSphereNonAlloc(pm.transform.position, 4f, colliders, 131072, QueryTriggerInteraction.Collide);
            for (int i = 0; i < count; i++)
            {
                if (colliders[i].isTrigger && colliders[i].CompareTag("NPC"))
                {
                    var baldi = colliders[i].GetComponent<Baldi>();
                    if (baldi != null)
                    {
                        baldi.BreakRuler();
                        baldi.behaviorStateMachine.ChangeState(new Baldi_Chase_Broken(baldi, baldi));
                        baldi.StartCoroutine(RestoreRulerAfterDelay(baldi, 15f));
                        foundBaldi = true;
                        break;
                    }
                }
            }

            if (foundBaldi)
            {
                SoundObject snipSound = _audSnipField?.GetValue(__instance) as SoundObject;
                if (snipSound != null)
                    Singleton<CoreGameManager>.Instance.audMan.PlaySingle(snipSound);
                __result = true;
            }
        }

        private static IEnumerator RestoreRulerAfterDelay(Baldi baldi, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            if (baldi == null) yield break;
            baldi.RestoreRuler();
            baldi.behaviorStateMachine.ChangeState(new Baldi_Chase(baldi, baldi));
        }
    }
}
