using HarmonyLib;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch(typeof(NanaPeelRoomFunction), "OnGenerationFinished")]
    internal static class BananaCafeteriaPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(NanaPeelRoomFunction __instance)
        {
            if (!PatchedAndLatchedPlugin.DisableBananasInCafeteria!.Value)
                return true;

            var room = __instance.Room;
            if (room == null)
                return true;

            string roomName = room.name;
            if (roomName != null && (roomName.IndexOf("Cafeteria", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            roomName.IndexOf("Caf", System.StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return false;
            }

            return true;
        }
    }
}
