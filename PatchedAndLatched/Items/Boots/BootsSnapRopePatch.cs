using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using UnityEngine;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(Jumprope))]
    public static class BootsSnapRopePatch
    {
        private static FieldInfo? _playtimeField;
        private static FieldInfo? _resistAddendField;

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static void Update_Prefix(Jumprope __instance)
        {
            if (!PatchedAndLatchedPlugin.BootsSnapRope.Value) return;
            if (__instance.player == null) return;

            bool hasBootsEffect = false;

            Entity entity = __instance.player.plm.Entity;
            if (entity != null)
            {
                if (_resistAddendField == null)
                {
                    _resistAddendField = typeof(Entity).GetField("resistAddend",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                }

                if (_resistAddendField != null)
                {
                    hasBootsEffect = (bool)_resistAddendField.GetValue(entity);
                }
            }

            if (hasBootsEffect)
            {
                if (_playtimeField == null)
                {
                    _playtimeField = typeof(Jumprope).GetField("playtime",
                        BindingFlags.Public | BindingFlags.Instance);
                }

                if (_playtimeField != null)
                {
                    Playtime? playtime = _playtimeField.GetValue(__instance) as Playtime;

                    if (playtime != null)
                    {
                        playtime.EndJumprope(false);
                        __instance.Destroy();
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("Jump")]
        public static bool Jump_Prefix(Jumprope __instance)
        {
            if (!PatchedAndLatchedPlugin.BootsSnapRope.Value) return true;
            if (__instance.player == null) return true;

            bool hasBootsEffect = false;
            Entity entity = __instance.player.plm.Entity;
            if (entity != null)
            {
                if (_resistAddendField == null)
                {
                    _resistAddendField = typeof(Entity).GetField("resistAddend",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                }

                if (_resistAddendField != null)
                {
                    hasBootsEffect = (bool)_resistAddendField.GetValue(entity);
                }
            }

            if (hasBootsEffect)
            {
                return false;
            }

            return true;
        }
    }
}

