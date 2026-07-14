using HarmonyLib;
using PatchedAndLatched;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    public class GrapplingHookExtraComponent : MonoBehaviour
    {
        public List<IClickable<int>> usedClickables = new List<IClickable<int>>();
        public List<Transform> interactedTransforms = new List<Transform>();
    }

    [HarmonyPatch(typeof(ITM_GrapplingHook))]
    public static class GrapplingHookPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Use")]
        private static void AddComponentThere(ITM_GrapplingHook __instance)
        {
            if (__instance.GetComponent<GrapplingHookExtraComponent>() == null)
                __instance.gameObject.AddComponent<GrapplingHookExtraComponent>();
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnEntityMoveCollision")]
        private static bool CheckForPassableObjects(
            ITM_GrapplingHook __instance,
            PlayerManager ___pm,
            ref RaycastHit hit,
            LayerMaskObject ___layerMask,
            bool ___locked,
            float ___speed,
            EnvironmentController ___ec)
        {
            var comp = __instance.GetComponent<GrapplingHookExtraComponent>();
            if (comp == null) return true;

            if (PatchedAndLatchedPlugin.GrapplingHookBreakWindows.Value)
            {
                if (!___locked && ___layerMask.Contains(hit.collider.gameObject.layer) &&
                    hit.transform.parent.CompareTag("Window") &&
                    !comp.interactedTransforms.Contains(hit.transform.parent))
                {
                    var w = hit.transform.parent.GetComponent<Window>();
                    if (w != null)
                    {
                        w.Break(true);
                        comp.interactedTransforms.Add(hit.transform.parent);
                        __instance.transform.position += __instance.transform.forward * ___speed * ___ec.EnvironmentTimeScale;
                        return false;
                    }
                }
            }

            if (PatchedAndLatchedPlugin.GrapplingHookOpenDoors.Value)
            {
                var clickable = hit.transform.parent.GetComponent<IClickable<int>>();
                clickable ??= hit.transform.GetComponent<IClickable<int>>();

                HashSet<Type> nonAllowed = new HashSet<Type>
                {
                    typeof(WaterFountain),
                    typeof(Pickup),
                    typeof(MathMachine),
                    typeof(HideableLocker)
                };

                if (clickable != null &&
                    !nonAllowed.Contains(clickable.GetType()) &&
                    !comp.usedClickables.Contains(clickable) &&
                    !clickable.ClickableHidden() &&
                    (!clickable.ClickableRequiresNormalHeight() ||
                     (clickable.ClickableRequiresNormalHeight() && !___pm.plm.Entity.Squished)))
                {
                    clickable.Clicked(___pm.playerNumber);
                    comp.usedClickables.Add(clickable);
                    if (!hit.collider.isTrigger)
                        __instance.transform.position += __instance.transform.forward * ___speed * ___ec.EnvironmentTimeScale;
                    return false;
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(NPC), "EntityTriggerEnter")]
        private static class GotHitByGrapplingHook
        {
            private static void Prefix(NPC __instance, Collider other)
            {
                if (PatchedAndLatchedPlugin.GrapplingHookPushNPCs.Value && other.CompareTag("GrapplingHook"))
                {
                    __instance.Navigator.Entity.AddForce(new Force(other.transform.right, 15f, -12f));
                }
            }
        }

        [HarmonyPatch(typeof(Gum), "EntityTriggerEnter")]
        private static class GumGotHitByGrapplingHook
        {
            private static bool Prefix(Gum __instance, Collider other, bool ___flying, AudioManager ___audMan, SoundObject ___audSplat, Beans ___beans)
            {
                if (!PatchedAndLatchedPlugin.GrapplingHookHitGum.Value) return true;
                if (___flying && other.CompareTag("GrapplingHook"))
                {
                    __instance.Hide();
                    ___beans.GumHit(__instance, false);
                    ___audMan.FlushQueue(true);
                    ___audMan.PlaySingle(___audSplat);
                    return false;
                }
                return true;
            }
        }
    }
}