using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace SmallChanges.Patches
{
    public static class BSODAReplacePatch
    {
        [HarmonyPatch(typeof(ItemManager))]
        public static class ItemManagerPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("AddItem", new[] { typeof(ItemObject) })]
            public static void AddItem_Prefix(ref ItemObject item)
            {
                if (!PatchedAndLatchedPlugin.ReplaceDietBSODA.Value) return;
                if (item == null) return;

                if (item.name.ToLower().Contains("diet") && item.name.ToLower().Contains("bsoda"))
                {
                    foreach (var obj in Singleton<PlayerFileManager>.Instance.itemObjects)
                    {
                        if (obj != null && obj.name.ToLower().Contains("bsoda") && !obj.name.ToLower().Contains("diet"))
                        {
                            item = obj;
                            break;
                        }
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch("AddItem", new[] { typeof(ItemObject), typeof(Pickup) })]
            public static void AddItemWithPickup_Prefix(ref ItemObject item)
            {
                if (!PatchedAndLatchedPlugin.ReplaceDietBSODA.Value) return;
                if (item == null) return;

                if (item.name.ToLower().Contains("diet") && item.name.ToLower().Contains("bsoda"))
                {
                    foreach (var obj in Singleton<PlayerFileManager>.Instance.itemObjects)
                    {
                        if (obj != null && obj.name.ToLower().Contains("bsoda") && !obj.name.ToLower().Contains("diet"))
                        {
                            item = obj;
                            break;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerFileManager))]
        public static class PlayerFileManagerPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("Load")]
            public static void Load_Postfix(PlayerFileManager __instance)
            {
                if (!PatchedAndLatchedPlugin.ReplaceDietBSODA.Value) return;

                for (int i = __instance.itemObjects.Count - 1; i >= 0; i--)
                {
                    if (__instance.itemObjects[i] != null &&
                        __instance.itemObjects[i].name.ToLower().Contains("diet") &&
                        __instance.itemObjects[i].name.ToLower().Contains("bsoda"))
                    {
                        __instance.itemObjects.RemoveAt(i);
                    }
                }
            }
        }
    }
}
