using HarmonyLib;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(MainMenu), "Quit")]
    internal static class MainMenuQuitPatch
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            CursorController.Instance.Hide(true);
        }
    }
}
