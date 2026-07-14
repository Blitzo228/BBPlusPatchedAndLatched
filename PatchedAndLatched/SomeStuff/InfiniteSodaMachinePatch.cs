using HarmonyLib;
using PatchedAndLatched;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(SodaMachine))]
    [HarmonyPatch("InsertItem")]
    internal static class InfiniteSodaMachinePatch
    {
        private static bool Prefix(ref int ___usesLeft)
        {
            if (!PatchedAndLatchedPlugin.InfiniteSodaMachine.Value) return true;

            ___usesLeft++;
            return true;
        }
    }
}