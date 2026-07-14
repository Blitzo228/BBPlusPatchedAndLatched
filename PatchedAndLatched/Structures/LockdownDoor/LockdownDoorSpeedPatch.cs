using HarmonyLib;
using PatchedAndLatched;
using System.Reflection;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(LockdownDoor))]
    internal static class LockdownDoorSpeedPatch
    {
        private static FieldInfo _speedField;

        static LockdownDoorSpeedPatch()
        {
            _speedField = typeof(LockdownDoor).GetField("speed", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void IncreaseSpeed(LockdownDoor __instance)
        {
            if (PatchedAndLatchedPlugin.LockdownDoorSpeedMultiplier == null) return;
            if (_speedField == null) return;

            float currentSpeed = (float)_speedField.GetValue(__instance);
            float newSpeed = currentSpeed * PatchedAndLatchedPlugin.LockdownDoorSpeedMultiplier.Value;
            _speedField.SetValue(__instance, newSpeed);
        }
    }
}