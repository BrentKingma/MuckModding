using HarmonyLib;
using System;
using System.Reflection;

namespace FunTimes
{
    class SharePowerups
    {
        private static MethodInfo ServerSendToAllTCP;
        private static MethodInfo ClientSendTCP;

        [HarmonyPatch(typeof(LocalClient), "InitializeClientData")]
        [HarmonyPostfix]
        static void PostInitializeClientData()
        {
            BaseAlterations.instance.Log("Adding packethandler");
            LocalClient.packetHandlers.Add(70, GetSharedPowerup);
            BaseAlterations.instance.Log("Packethandler added");
        }

        [HarmonyPatch(typeof(ServerHandle), "ItemPickedUp")]
        [HarmonyPrefix]
        static bool PreItemPickup(int fromClient, Packet packet)
        {
            BaseAlterations.instance.Log(" Pre Item Pickup");
            Packet cpyPacket = new Packet(packet.CloneBytes());
            int num = cpyPacket.ReadInt(true);
            num = cpyPacket.ReadInt(true);
            num = cpyPacket.ReadInt(true);

            Item component = ItemManager.Instance.list[num].GetComponent<Item>();

            if (component.powerup)
            {

                BaseAlterations.instance.Log("Starting packet");
                using (Packet packet2 = new Packet(70))
                {
                    BaseAlterations.instance.Log("Writing packet");
                    packet2.Write(fromClient);
                    packet2.Write(component.powerup.id);
                    BaseAlterations.instance.Log("Powerup ID: " + component.powerup.id);
                    BaseAlterations.instance.Log("Sending to server");
                    try
                    {
                        ServerSendToAllTCP.Invoke(new ServerSend(), new Object[] { packet2 });
                    }
                    catch (Exception e)
                    {
                        BaseAlterations.instance.Log("Reflection call failed: " + e.Message + " # " + ServerSendToAllTCP.Name);
                    }
                }
            }
            return true;
        }
        public static void GetInfo()
        {
            BaseAlterations.instance.Log("Starting reflections");
            try
            {
                ServerSendToAllTCP = typeof(ServerSend).GetMethod("SendTCPDataToAll", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance, null, new Type[] { typeof(Packet) }, null);
                ClientSendTCP = typeof(ClientSend).GetMethod("SendTCPData", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(Packet) }, null);
            }
            catch (Exception e)
            {
                BaseAlterations.instance.Log(e.Message);
            }

            BaseAlterations.instance.Log("########################");
            BaseAlterations.instance.Log(ServerSendToAllTCP.Name);
            BaseAlterations.instance.Log(ClientSendTCP.Name);
            BaseAlterations.instance.Log("########################");
        }

        static void GetSharedPowerup(Packet packet)
        {
            BaseAlterations.instance.Log("Received at client");
            int fromClient = packet.ReadInt(true);
            if (fromClient != LocalClient.instance.myId)
            {
                int powerupID = packet.ReadInt(true);
                BaseAlterations.instance.Log("Powerup ID: " + powerupID);
                int[] currentPowerups = typeof(PowerupInventory).GetField("powerups", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).GetValue(PowerupInventory.Instance) as int[];
                currentPowerups[powerupID]++;
                UiEvents.Instance.AddPowerup(ItemManager.Instance.allPowerups[powerupID]);
                PlayerStatus.Instance.UpdateStats();
                PowerupUI.Instance.AddPowerup(powerupID);
                PlayerStatus.Instance.UpdateStats();
            }
        }

    }
}
