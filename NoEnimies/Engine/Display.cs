using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Photon.Pun;

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
                PhotonView view = NameHolder.AddComponent<PhotonView>();
                view.ViewID = PhotonNetwork.AllocateViewID(false);
                NameHolder.AddComponent<KillthemAll.Clients>();
            }
        }
    }
}
