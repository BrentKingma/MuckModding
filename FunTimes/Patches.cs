using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FunTimes
{
    class Patches
    {
        static bool godmodeON = false;
        static bool infiniteStamina = false;
        static bool infiniteHunger = false;
        static bool nightEnable = false;

        [HarmonyPatch(typeof(GameLoop), "FindMobCap")]
        [HarmonyPostfix]
        static void PostFindMobCap()
        {
            GameLoop.currentMobCap *= Configs.configMobMulti.Value;
        }
        [HarmonyPatch(typeof(ChatBox), "ChatCommand")]
        [HarmonyPrefix]
        static bool PreChatCommand(string message)
        {
            string a = message.Substring(1);
            if (a == "godmode")
            {
                godmodeON = !godmodeON;
                BaseAlterations.instance.Log("Godmode");
            }
            else if (a == "flash")
            {
                infiniteStamina = !infiniteStamina;
                BaseAlterations.instance.Log("Flash");
            }
            else if (a == "gluttony")
            {
                infiniteHunger = !infiniteHunger;
                BaseAlterations.instance.Log("Gluttony");
            }
            else if (a == "night")
            {
                nightEnable = true;
            }
            else if (a == "credit")
            {

            }
            return true;
        }
        [HarmonyPatch(typeof(PlayerStatus), "HandleDamage")]
        [HarmonyPrefix]
        static bool PreHandleDamage()
        {
            if (godmodeON)
            {
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(PlayerStatus), "Stamina")]
        [HarmonyPrefix]
        static bool PreStamina()
        {
            if (godmodeON || infiniteStamina)
            {
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(PlayerStatus), "Hunger")]
        [HarmonyPrefix]
        static bool PreHunger()
        {
            if (godmodeON || infiniteHunger)
            {
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(PlayerStatus), "Jump")]
        [HarmonyPrefix]
        static bool PreJump()
        {
            if (godmodeON || infiniteStamina)
            {
                return false;
            }
            return true;
        }
        [HarmonyPatch(typeof(DayCycle), "Update")]
        [HarmonyPostfix]
        static void PostDayCycleUpdate()
        {
            if (nightEnable)
            {
                DayCycle.time = 0.5f;
                nightEnable = false;
            }
        }
    }
}

