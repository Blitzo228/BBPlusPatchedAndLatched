using HarmonyLib;
using PatchedAndLatched;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Gum), "EntityTriggerEnter")]
    internal static class GumBootsPatch
    {
        private static FieldInfo _playerModField = AccessTools.Field(typeof(Gum), "playerMod");
        internal static Dictionary<Entity, MovementModifier> _gumMods = new Dictionary<Entity, MovementModifier>();

        private static bool HasBoots(Entity entity)
        {
            var field = AccessTools.Field(typeof(Entity), "resistAddend");
            if (field != null) return (bool)field.GetValue(entity);
            var prop = AccessTools.Property(typeof(Entity), "ResistAddend");
            if (prop != null) return (bool)prop.GetValue(entity);
            return false;
        }

        [HarmonyPostfix]
        private static void Postfix(Gum __instance, Entity otherEntity, Collider other, bool validCollision)
        {
            if (!validCollision) return;
            if (!other.isTrigger) return;
            if (!other.CompareTag("Player")) return;

            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player == null) return;

            var actMod = otherEntity.ExternalActivity;
            if (actMod == null) return;

            var playerMod = _playerModField?.GetValue(__instance) as MovementModifier;
            if (playerMod == null) return;

            Entity entity = player.plm.Entity;
            if (_gumMods.ContainsKey(entity))
                _gumMods[entity] = playerMod;
            else
                _gumMods.Add(entity, playerMod);

            if (HasBoots(entity))
                playerMod.movementMultiplier = 1f;
        }
    }

    [HarmonyPatch(typeof(Entity), "SetResistAddend")]
    internal static class EntitySetResistAddendPatch
    {
        [HarmonyPrefix]
        private static void Prefix(Entity __instance, bool value)
        {
            if (!PatchedAndLatchedPlugin.EnableBootsIgnoreGum.Value) return;

            if (GumBootsPatch._gumMods.TryGetValue(__instance, out MovementModifier mod))
            {
                if (value)
                {
                    mod.movementMultiplier = 1f;
                }
                else
                {
                    mod.movementMultiplier = 0.25f;
                    GumBootsPatch._gumMods.Remove(__instance);
                }
            }
        }
    }
}