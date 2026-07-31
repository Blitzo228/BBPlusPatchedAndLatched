using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    public class GrapplingHookExtraComponent : MonoBehaviour
    {
        public List<IClickable<int>> usedClickables = new List<IClickable<int>>();
        public List<Transform> interactedTransforms = new List<Transform>();
    }

    public class GrapplingHookBalderDetector : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            CheckCollisions(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckCollisions(other);
        }

        private void CheckCollisions(Collider collider)
        {
            if (collider == null) return;

            if (PatchedAndLatchedPlugin.GrapplingHookBreakBalder!.Value)
            {
                var balder = collider.GetComponentInParent<Balder_Entity>();
                if (balder != null && !balder.Crumbled)
                {
                    balder.Crumble(playSound: true);

                    var hook = GetComponent<ITM_GrapplingHook>();
                    if (hook != null)
                    {
                        EndHook(hook);
                    }
                    return;
                }
            }

            if (PatchedAndLatchedPlugin.GrapplingHookPushNPCs!.Value)
            {
                var npc = collider.GetComponentInParent<NPC>();
                if (npc != null)
                {
                    var comp = GetComponent<GrapplingHookExtraComponent>();
                    if (comp != null && !comp.interactedTransforms.Contains(npc.transform))
                    {
                        Vector3 toNpc = npc.transform.position - transform.position;
                        Vector3 sideDir = transform.right;
                        if (Vector3.Dot(toNpc, transform.right) < 0f)
                        {
                            sideDir = -transform.right;
                        }

                        npc.Navigator.Entity.AddForce(new Force(sideDir, 15f, -12f));
                        comp.interactedTransforms.Add(npc.transform);
                    }
                }
            }
        }

        private static void EndHook(ITM_GrapplingHook hook)
        {
            var endMethod = typeof(ITM_GrapplingHook).GetMethod("End", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (endMethod != null)
            {
                endMethod.Invoke(hook, null);
            }
            else
            {
                var pmField = typeof(ITM_GrapplingHook).GetField("pm", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                             ?? typeof(Item).GetField("pm", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                var moveModField = typeof(ITM_GrapplingHook).GetField("moveMod", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (pmField != null && moveModField != null)
                {
                    var pm = pmField.GetValue(hook) as PlayerManager;
                    var moveMod = moveModField.GetValue(hook) as MovementModifier;
                    if (pm != null && moveMod != null && pm.Am != null)
                    {
                        pm.Am.moveMods.Remove(moveMod);
                    }
                }
                Destroy(hook.gameObject);
            }
        }
    }

    [HarmonyPatch(typeof(ITM_GrapplingHook))]
    public static class GrapplingHookPatch
    {
        public static readonly HashSet<Type> NonAllowedClickables = new HashSet<Type>
        {
            typeof(WaterFountain),
            typeof(Pickup),
            typeof(MathMachine),
            typeof(HideableLocker)
        };

        public static void AddNonAllowedClickable(Type type)
        {
            if (type != null)
                NonAllowedClickables.Add(type);
        }

        [HarmonyPrefix]
        [HarmonyPatch("Use")]
        private static void AddComponents(ITM_GrapplingHook __instance)
        {
            if (__instance.GetComponent<GrapplingHookExtraComponent>() == null)
                __instance.gameObject.AddComponent<GrapplingHookExtraComponent>();

            if (__instance.GetComponent<GrapplingHookBalderDetector>() == null)
                __instance.gameObject.AddComponent<GrapplingHookBalderDetector>();
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
            if (comp == null || ___locked) return true;

            if (PatchedAndLatchedPlugin.GrapplingHookBreakWindows!.Value)
            {
                if (___layerMask.Contains(hit.collider.gameObject.layer))
                {
                    var window = hit.collider.GetComponentInParent<Window>();
                    if (window != null && !comp.interactedTransforms.Contains(window.transform))
                    {
                        window.Break(true);
                        comp.interactedTransforms.Add(window.transform);
                        __instance.transform.position += __instance.transform.forward * ___speed * ___ec.EnvironmentTimeScale;
                        return false;
                    }
                }
            }

            if (hit.collider != null)
            {
                if (PatchedAndLatchedPlugin.GrapplingHookPushNPCs!.Value)
                {
                    var npc = hit.collider.GetComponentInParent<NPC>();
                    if (npc != null && !comp.interactedTransforms.Contains(npc.transform))
                    {
                        Vector3 toNpc = npc.transform.position - __instance.transform.position;
                        Vector3 sideDir = __instance.transform.right;
                        if (Vector3.Dot(toNpc, __instance.transform.right) < 0f)
                        {
                            sideDir = -__instance.transform.right;
                        }

                        npc.Navigator.Entity.AddForce(new Force(sideDir, 15f, -12f));
                        comp.interactedTransforms.Add(npc.transform);

                        __instance.transform.position += __instance.transform.forward * ___speed * ___ec.EnvironmentTimeScale;
                        return false;
                    }
                }

                var door = hit.collider.GetComponentInParent<Door>();
                if (door != null && !comp.interactedTransforms.Contains(door.transform))
                {
                    if (door.locked && PatchedAndLatchedPlugin.GrapplingHookUnlockDoors!.Value)
                    {
                        door.Unlock();
                        float openTime = 3f;
                        if (door is StandardDoor stdDoor) openTime = stdDoor.DefaultTime;
                        door.OpenTimed(openTime, true);

                        comp.interactedTransforms.Add(door.transform);
                        __instance.transform.position += __instance.transform.forward * ___speed * ___ec.EnvironmentTimeScale;
                        return false;
                    }
                    else if (!door.locked && door is SwingDoor swingDoor && PatchedAndLatchedPlugin.GrapplingHookOpenDoors!.Value)
                    {
                        swingDoor.OpenTimed(2f, true);
                        comp.interactedTransforms.Add(door.transform);
                        __instance.transform.position += __instance.transform.forward * ___speed * ___ec.EnvironmentTimeScale;
                        return false;
                    }
                }
            }

            if (PatchedAndLatchedPlugin.GrapplingHookOpenDoors!.Value)
            {
                var clickable = hit.collider?.GetComponentInParent<IClickable<int>>();
                if (clickable != null && !NonAllowedClickables.Contains(clickable.GetType()))
                {
                    if (!comp.usedClickables.Contains(clickable))
                    {
                        clickable.Clicked(___pm.playerNumber);
                        comp.usedClickables.Add(clickable);
                    }
                    __instance.transform.position += __instance.transform.forward * ___speed * ___ec.EnvironmentTimeScale;
                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ITM_Scissors))]
    public static class GrapplingHookCutPatch
    {
        private static FieldInfo? _audSnipField;
        private static FieldInfo? _hookPmField;
        private static FieldInfo? _hookMoveModField;

        private static SoundObject? GetAudSnip(ITM_Scissors scissors)
        {
            if (_audSnipField == null)
                _audSnipField = typeof(ITM_Scissors).GetField("audSnip", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return _audSnipField?.GetValue(scissors) as SoundObject;
        }

        private static PlayerManager? GetHookOwner(ITM_GrapplingHook hook)
        {
            if (_hookPmField == null)
                _hookPmField = typeof(ITM_GrapplingHook).GetField("pm", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                               ?? typeof(Item).GetField("pm", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return _hookPmField?.GetValue(hook) as PlayerManager;
        }

        private static MovementModifier? GetMoveMod(ITM_GrapplingHook hook)
        {
            if (_hookMoveModField == null)
                _hookMoveModField = typeof(ITM_GrapplingHook).GetField("moveMod", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return _hookMoveModField?.GetValue(hook) as MovementModifier;
        }

        [HarmonyPostfix]
        [HarmonyPatch("Use")]
        public static void Use_Postfix(ITM_Scissors __instance, PlayerManager pm, ref bool __result)
        {
            if (__result) return;
            if (!PatchedAndLatchedPlugin.CutGrapplingHook!.Value) return;

            foreach (ITM_GrapplingHook hook in UnityEngine.Object.FindObjectsOfType<ITM_GrapplingHook>())
            {
                if (hook == null || GetHookOwner(hook) != pm) continue;

                MovementModifier? moveMod = GetMoveMod(hook);
                if (moveMod != null && pm != null && pm.Am != null)
                {
                    pm.Am.moveMods.Remove(moveMod);
                }

                UnityEngine.Object.Destroy(hook.gameObject);

                SoundObject? audSnip = GetAudSnip(__instance);
                if (audSnip != null && pm != null)
                {
                    CoreGameManager.Instance.audMan.PlaySingle(audSnip);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Gum), "EntityTriggerEnter")]
    internal static class GumGotHitByGrapplingHook
    {
        private static bool Prefix(Gum __instance, Entity otherEntity, Collider other, bool validCollision, bool ___flying, AudioManager ___audMan, SoundObject ___audSplat)
        {
            if (!PatchedAndLatchedPlugin.GrapplingHookHitGum!.Value) return true;
            if (!validCollision || !___flying) return true;

            if (other.CompareTag("GrapplingHook") || otherEntity?.GetComponent<ITM_GrapplingHook>() != null || other.GetComponentInParent<ITM_GrapplingHook>() != null)
            {
                __instance.Hide();
                __instance.beans.GumHit(__instance, false);
                ___audMan.FlushQueue(true);
                ___audMan.PlaySingle(___audSplat);
                return false;
            }
            return true;
        }
    }
}
