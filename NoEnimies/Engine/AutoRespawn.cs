using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NoEnimies.respawn
{
    internal class God
    {
        private static IEnumerator yayyyyyyyyyyy(PlayerDeathHead head)
        {
            yield return new WaitForSeconds(0.5f);
            head.playerAvatar.Revive(false);
        }
        [HarmonyPatch(typeof(PlayerDeathHead))]
        [HarmonyPatch("SetupDone")]
        [HarmonyPostfix]
        static public void godmod(PlayerDeathHead __instance)
        {
            yayyyyyyyyyyy(__instance);
        }
    }
}