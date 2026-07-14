using HarmonyLib;
using PatchedAndLatched;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Notebook))]
    [HarmonyPatch("Clicked")]
    internal class NotebookStaminaPatch
    {
        private static void Prefix(Notebook __instance, int player)
        {
            if (!PatchedAndLatchedPlugin.NotebookRestoreStamina.Value) return;
            Singleton<CoreGameManager>.Instance.GetPlayer(player).plm.AddStamina(
                Singleton<CoreGameManager>.Instance.GetPlayer(player).plm.StaminaMax, true);
        }
    }
}