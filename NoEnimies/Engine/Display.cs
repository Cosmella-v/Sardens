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

namespace NoEnimies.Engine
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
                var killtheallplease = __instance.AddComponent<KillthemAll.Clients>();
                // this syncs when a player joins to help stop desync when joining midmatch
                if (__instance.photonView.IsMine)
                {
                    KillthemAll.Plugin.mls.LogMessage("I should sync now lol");
                    killtheallplease.PleaseSyncMeWithTheHost();
                }

                }
            [HarmonyPatch(typeof(PlayerAvatar))]
            [HarmonyPatch("ChatMessageSend")]
            [HarmonyPrefix]
            public static bool Hostcommands(PlayerAvatar __instance, string _message, bool _debugMessage)
            {
                if (_debugMessage) return true; // dunno why you would try sending a command as a test message????
                return CommandBuilder.process_Commands(_message, __instance.photonView.IsMine); // crazy one liner
            }


        }
    }
}
