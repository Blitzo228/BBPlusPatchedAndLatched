using HarmonyLib;
using UnityEngine;

namespace PatchedAndLatched.Patches
{
    [HarmonyPatch]
    internal static class MysteryRoomMapPatch
    {
        private static Material? _mysteryMat;

        static MysteryRoomMapPatch()
        {
            _mysteryMat = Resources.Load<Material>("Materials/MapBG_Mystery");
        }

        private static RoomController? GetRoom(MysteryRoom instance)
        {
            var field = AccessTools.Field(typeof(RandomEvent), "room");
            return field?.GetValue(instance) as RoomController;
        }

        private static void SetRoomColor(RoomController room, Color color)
        {
            if (room == null) return;

            room.color = color;
            if (_mysteryMat != null)
                room.mapMaterial = _mysteryMat;

            var map = room.ec.map;
            if (map == null) return;

            foreach (var cell in room.cells)
                map.UpdateTile(cell.position.x, cell.position.z, cell.ConstBin, room);
        }

        [HarmonyPatch(typeof(MysteryRoom), "AfterUpdateSetup")]
        [HarmonyPostfix]
        private static void AfterUpdateSetup(MysteryRoom __instance)
        {
            var room = GetRoom(__instance);
            if (room == null) return;

            foreach (var cell in room.cells)
                cell.hideFromMap = false;

            SetRoomColor(room, Color.gray);
        }

        [HarmonyPatch(typeof(MysteryRoom), "Begin")]
        [HarmonyPostfix]
        private static void Begin(MysteryRoom __instance)
        {
            var room = GetRoom(__instance);
            if (room == null) return;

            SetRoomColor(room, new Color(0f, 77f / 255f, 0f));
        }

        [HarmonyPatch(typeof(MysteryRoom), "End")]
        [HarmonyPostfix]
        private static void End(MysteryRoom __instance)
        {
            var room = GetRoom(__instance);
            if (room == null) return;

            SetRoomColor(room, Color.gray);
        }
    }
}
