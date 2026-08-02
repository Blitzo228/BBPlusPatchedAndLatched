using HarmonyLib;
using UnityEngine.UI;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(HudGauge))]
    internal static class GaugeHidePatch
    {
        [HarmonyPatch("Activate")]
        [HarmonyPostfix]
        private static void ActivatePostfix(HudGauge __instance)
        {
            if (!PatchedAndLatchedPlugin.DisableGaugeVisuals.Value) return;
            var graphics = __instance.GetComponentsInChildren<Graphic>(true);
            foreach (var graphic in graphics)
            {
                graphic.enabled = false;
            }
            var dialImageField = AccessTools.Field(typeof(HudGauge), "dialImage");
            var iconImageField = AccessTools.Field(typeof(HudGauge), "iconImage");
            var toDarkenImageField = AccessTools.Field(typeof(HudGauge), "toDarkenImage");

            var dial = dialImageField?.GetValue(__instance) as Image;
            if (dial != null) dial.enabled = false;

            var icon = iconImageField?.GetValue(__instance) as Image;
            if (icon != null) icon.enabled = false;

            var darken = toDarkenImageField?.GetValue(__instance) as Image[];
            if (darken != null)
            {
                foreach (var img in darken)
                    if (img != null) img.enabled = false;
            }
        }
    }
}