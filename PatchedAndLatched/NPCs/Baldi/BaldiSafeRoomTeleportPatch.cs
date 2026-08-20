using HarmonyLib;
using PatchedAndLatched;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    [HarmonyPriority(Priority.First)]
    internal static class BaldiSafeRoomTeleportPatch
    {
        private static int _teleportCount = 0;
        private static RoomController? _safeRoom = null;
        private static SoundObject? _teleportSound = null;

        [HarmonyPatch(typeof(Baldi), "CaughtPlayer")]
        [HarmonyPrefix]
        private static bool Prefix(Baldi __instance, PlayerManager player)
        {
            if (!PatchedAndLatchedPlugin.EnableBaldiSafeRoomTeleport!.Value)
                return true;

            int maxTeleports = PatchedAndLatchedPlugin.BaldiSafeRoomTeleportCount!.Value;
            if (maxTeleports <= 0 || _teleportCount >= maxTeleports)
                return true;

            if (_safeRoom == null)
                FindSafeRoom(__instance.ec);

            if (_safeRoom == null)
                return true;

            Cell cell = _safeRoom.RandomEntitySafeCellNoGarbage();
            if (cell == null)
                cell = _safeRoom.cells[0];
            if (cell == null)
                return true;

            Vector3 pos = cell.FloorWorldPosition + Vector3.up * 5f;
            player.Teleport(pos);
            _teleportCount++;

            PlayTeleportSound();

            return false;
        }

        private static void PlayTeleportSound()
        {
            if (_teleportSound == null)
            {
                _teleportSound = Resources.Load<SoundObject>("Sounds/Teleport");
                if (_teleportSound == null)
                    _teleportSound = Resources.Load<SoundObject>("Teleport");
                if (_teleportSound == null)
                {
                    foreach (var s in Resources.FindObjectsOfTypeAll<SoundObject>())
                    {
                        if (s.name == "Teleport" || s.name == "Teleporter")
                        {
                            _teleportSound = s;
                            break;
                        }
                    }
                }
                if (_teleportSound == null)
                    return;
            }

            var audMan = Singleton<CoreGameManager>.Instance?.audMan;
            if (audMan != null)
                audMan.PlaySingle(_teleportSound);
        }

        private static void FindSafeRoom(EnvironmentController ec)
        {
            if (ec == null)
                return;

            foreach (var room in ec.rooms)
            {
                if (room.GetComponentInChildren<BlockNavigationRoomFunction>() != null)
                {
                    _safeRoom = room;
                    return;
                }
            }

            foreach (var room in ec.rooms)
            {
                if (room.name.Contains("Safe") || room.name.Contains("SafeRoom"))
                {
                    _safeRoom = room;
                    return;
                }
            }
        }

        [HarmonyPatch(typeof(EnvironmentController), "BeginPlay")]
        [HarmonyPostfix]
        private static void ResetCounter(EnvironmentController __instance)
        {
            _teleportCount = 0;
            _safeRoom = null;
            _teleportSound = null;
        }
    }
}