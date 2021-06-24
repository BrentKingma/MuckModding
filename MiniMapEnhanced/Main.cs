using BepInEx;
using HarmonyLib;

namespace MiniMapEnhanced
{
    [BepInPlugin("com.brentkingma.minimapenhanced", "Base Alterations", "0.0.1")]
    public class Main : BaseUnityPlugin
    {
        public static Main instance;
        public Harmony harmony;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }

            harmony = new Harmony("com.brentkingma.minimapenhanced");
            Patches.Setup();
            harmony.PatchAll(typeof(Patches));
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        public void Log(string message)
        {
            Logger.LogMessage(message);
        }
    }
}
