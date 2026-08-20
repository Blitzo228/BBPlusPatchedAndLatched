using HarmonyLib;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(WaterFountain), "Clicked")]
    internal static class DrinkingPenaltyPatch
    {
        private static void Prefix(int playerNumber)
        {
            Singleton<CoreGameManager>.Instance.GetPlayer(playerNumber).RuleBreak("Drinking", 0.8f);
        }
    }
}