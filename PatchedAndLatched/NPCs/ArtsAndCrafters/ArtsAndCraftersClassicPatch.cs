using HarmonyLib;
using UnityEngine;
using System.Reflection;
using PatchedAndLatched;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(ArtsAndCrafters_Chasing))]
    public static class ArtsAndCraftersChasingPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnStateTriggerEnter")]
        public static bool OnStateTriggerEnter_Prefix(ArtsAndCrafters_Chasing __instance, Entity otherEntity, Collider other, bool validCollision)
        {
            if (!PatchedAndLatchedPlugin.ClassicArtsAndCrafters.Value) return true;

            if (other.CompareTag("Player"))
            {
                var craftersField = typeof(ArtsAndCrafters_Chasing).GetField("crafters",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                var playerField = typeof(ArtsAndCrafters_Chasing).GetField("player",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (craftersField != null && playerField != null)
                {
                    ArtsAndCrafters? crafters = craftersField.GetValue(__instance) as ArtsAndCrafters;
                    PlayerManager? player = playerField.GetValue(__instance) as PlayerManager;

                    if (crafters != null && player != null)
                    {
                        if (validCollision)
                        {
                            crafters.TeleportPlayer(player);
                            return false;
                        }
                        else
                        {
                            crafters.DisappearForever();
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ArtsAndCrafters_Teleporting))]
    public static class ArtsAndCraftersTeleportingPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static void Update_Prefix(ArtsAndCrafters_Teleporting __instance)
        {
            if (!PatchedAndLatchedPlugin.ClassicArtsAndCrafters.Value) return;

            var timeField = typeof(ArtsAndCrafters_Teleporting).GetField("time",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (timeField != null)
            {
                float time = (float)timeField.GetValue(__instance);
                if (time > 0.5f)
                {
                    timeField.SetValue(__instance, 0.5f);
                }
            }
        }
    }

    [HarmonyPatch(typeof(ArtsAndCrafters))]
    public static class ArtsAndCraftersPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Initialize")]
        public static void Initialize_Postfix(ArtsAndCrafters __instance)
        {
            if (!PatchedAndLatchedPlugin.ClassicArtsAndCrafters.Value) return;

            var runTimeField = typeof(ArtsAndCrafters).GetField("runTime",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (runTimeField != null)
            {
                runTimeField.SetValue(__instance, 0.5f);
            }
        }
    }

    [HarmonyPatch(typeof(ArtsAndCrafters_Ready))]
    public static class ArtsAndCraftersReadyPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static void Update_Prefix(ArtsAndCrafters_Ready __instance)
        {
            if (!PatchedAndLatchedPlugin.ClassicArtsAndCrafters.Value) return;

            var timeField = typeof(ArtsAndCrafters_Ready).GetField("time",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (timeField != null)
            {
                float time = (float)timeField.GetValue(__instance);
                if (time > 0.3f)
                {
                    timeField.SetValue(__instance, 0.3f);
                }
            }
        }
    }
}
