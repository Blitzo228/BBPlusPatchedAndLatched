using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Notebook))]
    internal static class NotebookSpinPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void StartPostfix(Notebook __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableNotebookSpin.Value) return;

            Animator animator = __instance.GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = true;

                string[] stateNames = { "NotebookSpin", "Spin", "IdleSpin", "Rotate" };
                bool started = false;
                foreach (string name in stateNames)
                {
                    int hash = Animator.StringToHash(name);
                    for (int layer = 0; layer < animator.layerCount; layer++)
                    {
                        if (animator.HasState(layer, hash))
                        {
                            animator.Play(hash, layer, 0f);
                            animator.Update(0f);
                            started = true;
                            break;
                        }
                    }
                    if (started) break;
                }

                if (!started)
                {
                    foreach (var p in animator.parameters)
                    {
                        if (p.name.ToLower().Contains("spin"))
                        {
                            if (p.type == AnimatorControllerParameterType.Bool)
                                animator.SetBool(p.nameHash, true);
                            else if (p.type == AnimatorControllerParameterType.Trigger)
                                animator.SetTrigger(p.nameHash);
                            else if (p.type == AnimatorControllerParameterType.Float)
                                animator.SetFloat(p.nameHash, 1f);
                            started = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}