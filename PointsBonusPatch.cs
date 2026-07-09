using HarmonyLib;
using UnityEngine;

namespace SmallChanges.Patches
{
    [HarmonyPatch(typeof(CoreGameManager))]
    public static class PointsBonusPatch
    {
        private static int _lastPoints = 0;
        private static int _bonusPoints = 0;

        [HarmonyPostfix]
        [HarmonyPatch("AddPoints", new[] { typeof(int), typeof(int), typeof(bool), typeof(bool), typeof(bool) })]
        public static void AddPoints_Postfix(int points, int player, bool playAnimation, bool includeInLevelTotal, bool multiply)
        {
            if (!SmallChangesPlugin.PointsBonus.Value) return;
            if (points <= 0) return;

            int totalPoints = Singleton<CoreGameManager>.Instance.GetPoints(player);

            if (totalPoints < _lastPoints)
            {
                _lastPoints = 0;
                _bonusPoints = 0;
            }

            int expectedBonus = (totalPoints / 30) * 5;

            if (expectedBonus > _bonusPoints)
            {
                int bonusToAdd = expectedBonus - _bonusPoints;
                _bonusPoints = expectedBonus;
                _lastPoints = totalPoints;

                Singleton<CoreGameManager>.Instance.AddPoints(bonusToAdd, player, false, false, false);
            }
        }
    }
}