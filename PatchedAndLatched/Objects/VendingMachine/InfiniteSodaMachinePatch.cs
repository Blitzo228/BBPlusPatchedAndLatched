using HarmonyLib;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(SodaMachine))]
    [HarmonyPatch("InsertItem")]
    internal static class InfiniteSodaMachinePatch
    {
        private static bool Prefix(ref int ___usesLeft)
        {
            ___usesLeft++;
            return true;
        }
    }
}
