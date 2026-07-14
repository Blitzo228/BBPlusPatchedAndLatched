using HarmonyLib;
using PatchedAndLatched;
using System.Collections.Generic;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Structure_SteamValves))]
    internal static class AlwaysClosedValvesPatch
    {
        [HarmonyPatch("OnGenerationFinished")]
        [HarmonyPostfix]
        private static void ForceValvesClosed(Structure_SteamValves __instance)
        {
            if (!PatchedAndLatchedPlugin.AlwaysClosedValves.Value) return;

            var field = typeof(Structure_SteamValves).GetField("generatedValves",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null) return;

            var valves = field.GetValue(__instance) as List<GameButtonBase>;
            if (valves == null) return;

            foreach (var valve in valves)
            {
                if (valve != null)
                    valve.Set(false);
            }
        }
    }
}