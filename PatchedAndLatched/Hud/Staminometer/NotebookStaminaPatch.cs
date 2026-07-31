using HarmonyLib;
namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Notebook))]
    [HarmonyPatch("Clicked")]
    internal class NotebookStaminaPatch
    {
        private static void Prefix(Notebook __instance, int player)
        {
            Singleton<CoreGameManager>.Instance.GetPlayer(player).plm.AddStamina(
                Singleton<CoreGameManager>.Instance.GetPlayer(player).plm.StaminaMax, true);
        }
    }
}
