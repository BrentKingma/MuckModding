using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace FunTimes
{
    [BepInPlugin("com.github.brentkingma.basealterations", "Base Alterations", "0.0.1")]
    public class BaseAlterations : BaseUnityPlugin
    {
        public ConfigEntry<int> configMobMulti;
        public ConfigEntry<bool> configShareItem;
        public GameObject Canvas = null;
        public GameObject uiObject;

        public static BaseAlterations instance;
        public Harmony harmony;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }

            configMobMulti = Config.Bind("Multies",
                                         "MobMulti",
                                         1,
                                         "The number multiplied by calculated mob count");
            configShareItem = Config.Bind("General",
                                         "ShareItem",
                                         false,
                                         "If one person picks up an item, everyone gets it");

            //CreateUI();
            Logger.LogMessage("Hello");
            
            harmony = new Harmony("com.github.brentkingma.basealterations");
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

        private void CreateUI()
        {
            if(Canvas == null)
            {
                Canvas = new GameObject("ModUICanvas");
                Canvas.AddComponent<Canvas>();
                Canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                Canvas.GetComponent<Canvas>().sortingOrder = -10;
                Canvas.AddComponent<CanvasScaler>();
                Canvas.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                Canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
                Canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            }

            uiObject = new GameObject();
            uiObject.transform.SetParent(Canvas.transform);
            uiObject.AddComponent(typeof(RectTransform));
            RectTransform myRect = (RectTransform)uiObject.GetComponent(typeof(RectTransform));
            var value = myRect.rect;
            value.xMin = 0.0f;
            value.xMax = 0.0f;
            value.yMin = 0.0f;
            value.yMax = 0.0f;

            uiObject.AddComponent(typeof(CanvasRenderer));
            uiObject.AddComponent(typeof(Image));
            Image myImage = (Image)uiObject.GetComponent(typeof(Image));
            Color newColor = new Color(0.0f, 0.0f, 0.0f, 255.0f);
            myImage.color = newColor;
            Logger.LogMessage("Create UI element");
            uiObject.SetActive(true);
        }
    }
}
