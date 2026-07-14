using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(Principal))]
    public static class PrincipalNoFacultyKnockPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("FacultyDoorHit")]
        public static bool FacultyDoorHit_Prefix(Principal __instance, StandardDoor door, Cell otherSide)
        {
            if (!PatchedAndLatchedPlugin.NoPrincipalFacultyKnock.Value) return true;

            door.OpenTimedWithKey(door.DefaultTime, makeNoise: false);
            return false; 
        }
        [HarmonyPrefix]
        [HarmonyPatch("KnockOnDoor")]
        public static bool KnockOnDoor_Prefix(Principal __instance, StandardDoor door, Cell otherSide)
        {
            if (!PatchedAndLatchedPlugin.NoPrincipalFacultyKnock.Value) return true;

            door.OpenTimedWithKey(door.DefaultTime, makeNoise: false);
            return false; 
        }
    }
}