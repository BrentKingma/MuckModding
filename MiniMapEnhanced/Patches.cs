using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MiniMapEnhanced
{
    class Patches
    {
        private static Dictionary<int, PlayerInfo> players;
        private static List<GameObject> markers;

        public static void Setup()
        {
            try
            {
                markers = new List<GameObject>();
                players = new Dictionary<int, PlayerInfo>();
            }
            catch (Exception e)
            {
                Main.instance.Log(e.Message);
            }
        }

        //Client Side Patches
        [HarmonyPatch(typeof(Map), "ShowPlayers")]
        [HarmonyPostfix]
        static void PostShowPlayers(ref float ___mapRatio)
        {
            if(Input.GetKey(KeyCode.P))
            {
                GameObject newMarker = UnityEngine.Object.Instantiate(Map.Instance.playerIcon.transform.gameObject, Map.Instance.playerIcon.transform.parent);
                markers.Add(newMarker);
            }
            foreach(KeyValuePair<int, PlayerManager> a_player in GameManager.players)
            {

                if (a_player.Value.id == LocalClient.instance.myId)
                {
                    return;
                }

                if (!players.ContainsKey(a_player.Value.id))
                {
                    players.Add(a_player.Value.id, new PlayerInfo());
                    GameObject newPointer = UnityEngine.Object.Instantiate(Map.Instance.playerIcon.transform.gameObject, Map.Instance.playerIcon.transform.parent);
                    players[a_player.Value.id].myPointer = newPointer.transform;
                }
                Vector3 pointPos = new Vector3(a_player.Value.onlinePlayer.desiredPos.x, a_player.Value.onlinePlayer.desiredPos.z, 0.0f);
                players[a_player.Value.id].myPointer.localPosition = pointPos * ___mapRatio;
                players[a_player.Value.id].myPointer.localRotation = Quaternion.Euler(0.0f, 0.0f, -a_player.Value.onlinePlayer.orientationY);
            }
        }
    }
}
