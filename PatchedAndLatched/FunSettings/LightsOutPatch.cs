using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    internal static class LightsOutPatch
    {
        private static LanternMode? _lanternMode;

        [HarmonyPatch("BeginPlay")]
        [HarmonyPostfix]
        private static void AddLanternMode(EnvironmentController __instance)
        {
            if (!PatchedAndLatchedPlugin.LightsOutEnabled.Value) return;

            _lanternMode = __instance.gameObject.GetComponent<LanternMode>();
            if (_lanternMode == null)
            {
                _lanternMode = __instance.gameObject.AddComponent<LanternMode>();
                _lanternMode.Initialize(__instance);
            }

            _lanternMode.AddSource(CoreGameManager.Instance.GetPlayer(0).transform, 6f, new Color(0.887f, 0.765f, 0.498f, 1f));
            Shader.SetGlobalColor("_SkyboxColor", Color.black);
        }

        [HarmonyPatch(typeof(NPC), "Initialize")]
        [HarmonyPostfix]
        private static void AddNPCLight(NPC __instance)
        {
            if (!PatchedAndLatchedPlugin.LightsOutEnabled.Value) return;
            if (_lanternMode == null) return;
            if (__instance.Character != Character.Principal) return;

            _lanternMode.AddSource(__instance.transform, 4f, Color.white);
        }

        [HarmonyPatch(typeof(NPC), "Despawn")]
        [HarmonyPrefix]
        private static void RemoveNPCLight(NPC __instance)
        {
            if (!PatchedAndLatchedPlugin.LightsOutEnabled.Value) return;
            if (_lanternMode == null) return;
            if (__instance.Character != Character.Principal) return;

            var sources = Traverse.Create(_lanternMode).Field("sources").GetValue<System.Collections.Generic.List<LanternSource>>();
            if (sources != null)
                sources.RemoveAll(x => x.transform == __instance.transform);
        }
    }
}