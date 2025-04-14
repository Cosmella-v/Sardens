using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
                NameHolder.transform.position = new UnityEngine.Vector3(85.3f, 254, 0);

            }
        }
    }
}
