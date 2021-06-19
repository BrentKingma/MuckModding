

namespace FunTimes
{
    class SharePowerups
    {
        public static void GetSharedPowerup(Packet packet)
        {
            int powerupID = packet.ReadInt(true);
            int objectID = packet.ReadInt(true);
            PowerupInventory.Instance.AddPowerup("", powerupID, objectID);
        }
    }
}
