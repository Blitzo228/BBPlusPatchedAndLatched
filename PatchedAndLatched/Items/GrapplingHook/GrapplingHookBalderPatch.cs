using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    public class GrapplingHookBalderDetector : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            CheckForBalder(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForBalder(other);
        }

        private void CheckForBalder(Collider collider)
        {
            if (!PatchedAndLatchedPlugin.GrapplingHookBreakBalder.Value) return;

            var balder = collider.GetComponentInParent<Balder_Entity>();
            if (balder != null && !balder.Crumbled)
            {
                balder.Crumble(playSound: true);

                var hook = GetComponent<ITM_GrapplingHook>();
                if (hook != null)
                {
                    var endMethod = typeof(ITM_GrapplingHook).GetMethod("End", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (endMethod != null)
                    {
                        endMethod.Invoke(hook, null);
                    }
                    else
                    {
                        var pmField = typeof(ITM_GrapplingHook).GetField("pm", BindingFlags.NonPublic | BindingFlags.Instance);
                        var moveModField = typeof(ITM_GrapplingHook).GetField("moveMod", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (pmField != null && moveModField != null)
                        {
                            var pm = pmField.GetValue(hook) as PlayerManager;
                            var moveMod = moveModField.GetValue(hook) as MovementModifier;
                            if (pm != null && moveMod != null)
                            {
                                pm.Am.moveMods.Remove(moveMod);
                            }
                        }
                        Destroy(gameObject);
                    }
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    [HarmonyPatch(typeof(ITM_GrapplingHook))]
    internal static class GrapplingHookBalderPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Use")]
        private static void AddBalderDetector(ITM_GrapplingHook __instance)
        {
            if (__instance.GetComponent<GrapplingHookBalderDetector>() == null)
            {
                __instance.gameObject.AddComponent<GrapplingHookBalderDetector>();
            }
        }
    }
}
