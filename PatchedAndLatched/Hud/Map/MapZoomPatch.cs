using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Map))]
    internal static class MapZoomPatch
    {
        private static FieldInfo? _zoomMinField;
        private static FieldInfo? _zoomMaxField;
        private static FieldInfo? _zoomSpeedField;

        static MapZoomPatch()
        {
            _zoomMinField = AccessTools.Field(typeof(Map), "zoomMin");
            _zoomMaxField = AccessTools.Field(typeof(Map), "zoomMax");
            _zoomSpeedField = AccessTools.Field(typeof(Map), "zoomSpeed");
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AwakePostfix(Map __instance)
        {
            if (!PatchedAndLatchedPlugin.EnableUnlimitedMapZoom!.Value) return;

            if (_zoomMinField != null)
                _zoomMinField.SetValue(__instance, 0.01f);
            if (_zoomMaxField != null)
                _zoomMaxField.SetValue(__instance, 9999f);
            if (_zoomSpeedField != null)
                _zoomSpeedField.SetValue(__instance, 4f);
        }
    }
}