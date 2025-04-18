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

namespace NoEnimies.Engine
{
    internal class Display
    {
        [HarmonyPatch(typeof(HUDCanvas))]
        public class HudPatch
        {
            static public GameObject NameHolder;
            static public int registerdid;
            [HarmonyPatch("Awake")]
            [HarmonyPostfix]
            public static void Awake(HUDCanvas __instance)
            {
                NameHolder = new GameObject("Names",typeof(Names.ListUpdater));
                NameHolder.transform.SetParent(__instance.transform.Find("HUD").Find("Game Hud"), false);
                NameHolder.transform.position = new UnityEngine.Vector3(45f, 270, 0);
                NameHolder.transform.localScale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
                PhotonView view = NameHolder.AddComponent<PhotonView>();
               
                if (!Lib.Dependant.IsHost())
                {
                    __instance.StartCoroutine(WaitForViewID(view));
                }
                else
                {
                    registerdid = PhotonNetwork.AllocateViewID(true);
                    view.ViewID = registerdid;
                    PhotonNetwork.RegisterPhotonView(view);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "NameHolderViewID", registerdid } });
                }

                NameHolder.AddComponent<KillthemAll.Clients>();
            }
            private static IEnumerator WaitForViewID(PhotonView view)
            {
                object id;
                while (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("NameHolderViewID", out id))
                {
                    yield return null; 
                }
                view.ViewID = (int)id;
            }

        }
    }
}
