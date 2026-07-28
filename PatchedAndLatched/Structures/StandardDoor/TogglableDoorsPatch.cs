using HarmonyLib;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(StandardDoor), nameof(StandardDoor.Clicked))]
    public class ToggleableDoorsPatch
    {
        public static bool Prefix(StandardDoor __instance, int player)
        {
            if (!PatchedAndLatchedPlugin.ToggleableDoors!.Value)
                return true;

            if (__instance.locked)
            {
                __instance.audMan.PlaySingle(__instance.audDoorLocked);
                return false;
            }

            if (__instance.open)
            {
                __instance.Shut();
            }
            else
            {
                __instance.OpenTimed(__instance.DefaultTime, __instance.makesNoise);

                var eventField = typeof(StandardDoor).GetField(
                    "OnPlayerOpen",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
                );

                if (eventField?.GetValue(__instance) is StandardDoor.OnPlayerOpenHandler handler)
                {
                    handler.Invoke();
                }
            }

            return false;
        }
    }
}
