using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Collections;
using Unity.VisualScripting;
using System.Reflection;

namespace Sardens.Engine
{
    internal class Display
    {
        [HarmonyPatch(typeof(HUDCanvas))]
        public class HudPatch
        {
            static public GameObject NameHolder;
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Awake(HUDCanvas __instance)
            {
                NameHolder = new GameObject("Names",typeof(Names.ListUpdater));
                NameHolder.transform.SetParent(__instance.transform.Find("HUD").Find("Game Hud"), false);
                NameHolder.transform.position = new UnityEngine.Vector3(45f, 270, 0);
                NameHolder.transform.localScale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
            }
            [HarmonyPatch(typeof(PlayerAvatar))]
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void PlayerAvatarAwake(PlayerAvatar __instance)
            {
                var killtheallplease = __instance.AddComponent<FishSardens.Clients>();
                // this syncs when a player joins to help stop desync when joining midmatch
                if (__instance.photonView.IsMine)
                {
                    //FishSardens.Plugin.mls.LogMessage("I should sync now lol");
                    killtheallplease.PleaseSyncMeWithTheHost();
                }

                }
            [HarmonyPatch(typeof(ChatManager))]
            [HarmonyPatch("MessageSend")]
            [HarmonyPrefix]
            public static bool Hostcommands(ChatManager __instance, bool _possessed)
            {
                if (_possessed) { return true; };
               
                var typeofFast = typeof(ChatManager);
                var chatMessageField = typeofFast.GetField("chatMessage", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                string message = chatMessageField.GetValue(__instance) as string;
                if (string.IsNullOrEmpty(message))
                {
                    return true;
                }
                if (CommandBuilder.process_Commands(message, true)) { return true; }
                chatMessageField.SetValue(__instance, "");
                typeofFast.GetField("spamTimer", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(__instance, 1f);
                ChatUI.instance.SemiUITextFlashColor(Color.magenta, 0.4f);
                ChatUI.instance.SemiUISpringShakeX(10f, 10f, 0.3f);
                ChatUI.instance.SemiUISpringScale(0.05f, 5f, 0.2f);
                MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Confirm, null, 1f, 1f, true);
                typeofFast.GetField("chatActive", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(__instance, false);
                return false;

            }


        }
    }
}
