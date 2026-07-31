using HarmonyLib;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Principal))]
    public static class PrincipalNoFacultyKnockPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("FacultyDoorHit")]
        public static bool FacultyDoorHit_Prefix(Principal __instance, StandardDoor door, Cell otherSide)
        {
            door.OpenTimedWithKey(door.DefaultTime, makeNoise: false);
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch("KnockOnDoor")]
        public static bool KnockOnDoor_Prefix(Principal __instance, StandardDoor door, Cell otherSide)
        {
            door.OpenTimedWithKey(door.DefaultTime, makeNoise: false);
            return false;
        }
    }
}
