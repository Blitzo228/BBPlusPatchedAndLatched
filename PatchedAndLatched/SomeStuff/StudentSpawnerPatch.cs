using HarmonyLib;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(Structure_StudentSpawner))]
    internal static class StudentSpawnerPatch
    {
        [HarmonyPatch("SpawnStudents")]
        [HarmonyPrefix]
        private static bool Prefix(Structure_StudentSpawner __instance, int totalStudents, bool startInLevel)
        {
            int compensation = totalStudents * 25;
            if (compensation > 0)
            {
                Singleton<CoreGameManager>.Instance.AddPoints(compensation, 0, true, true, false);
            }

            return false;
        }
    }
}
