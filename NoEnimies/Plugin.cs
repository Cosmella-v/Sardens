using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RunManager;

namespace KillthemAll
{

    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public static List<PlayerAvatar> Hiders = new List<PlayerAvatar>();
        public const string modGUID = "viper.NoMonsters";
        public const string modName = "No Monsters";
        public const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);


        public static ManualLogSource mls;

        public static void updateListMaps()
        {
            NoEnimies.Engine.Display.HudPatch.NameHolder.GetComponent<KillthemAll.Clients>().SendList();
            foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
            {
                StatsManager.instance.playerUpgradeMapPlayerCount[SemiFunc.PlayerGetSteamID(playerAvatar)] = Hiders.Count;
            }
            SemiFunc.StatSyncAll();
        }

        void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo($"{modGUID} is now awake!");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(PlayerAvatarCollision))]
        [HarmonyPatch("Update", MethodType.Normal)]
        public static class PlayerAvatarCollision_Update_Patch
        {
            public static bool Prefix(PlayerAvatarCollision __instance)
            {
                if (!NoEnimies.Lib.Dependant.IsHost()) return true;
                if (__instance.Collider != null && KillthemAll.Plugin.Hiders.Contains(__instance.PlayerAvatar))
                {
                    Collider[] hitColliders = Physics.OverlapSphere(__instance.transform.position, 1.3f);

                    foreach (Collider hitCollider in hitColliders)
                    {

                        var Col = hitCollider.gameObject.transform.parent.gameObject.GetComponent<PlayerAvatarCollision>();
                        if (Col) {
                            if (!KillthemAll.Plugin.Hiders.Contains(Col.PlayerAvatar))
                            {
                                KillthemAll.Plugin.Hiders.Add(Col.PlayerAvatar);
                                KillthemAll.Plugin.updateListMaps();
                                break;
                            }
                        }
                    }
                }

                return true;
            }
        }
        [HarmonyPatch(typeof(StatsManager), "PlayerAdd")]
        public class StatsManagerHook
        {
            static void Postfix(StatsManager __instance, string _steamID, string _playerName)
            {
                if (!NoEnimies.Lib.Dependant.IsHost()) return;

                if (__instance == null || string.IsNullOrEmpty(_steamID) || PunManager.instance == null)
                {
                    mls.LogWarning("StatsManager hook failed due to null references.");
                    return;
                }
                mls.LogInfo($"{modGUID} Setting Stats for {_playerName}({_steamID})!");
                __instance.playerUpgradeExtraJump[_steamID] = 4;
                __instance.playerUpgradeLaunch[_steamID] = 10;
                __instance.playerUpgradeSpeed[_steamID] = 10;
                __instance.playerUpgradeStrength[_steamID] = -1;
                __instance.playerUpgradeThrow[_steamID] = -1;
                __instance.playerUpgradeHealth[_steamID] = 9995;
                __instance.playerUpgradeStamina[_steamID] = 9996;
                __instance.playerUpgradeMapPlayerCount[_steamID] = Hiders.Count;
                __instance.playerHealth[_steamID] = 10000;
                SemiFunc.StatSyncAll();
            }
        } //                 __instance.playerUpgradeMapPlayerCount[_steamID] = 1;  SemiFunc.StatSyncAll();
        [HarmonyPatch(typeof(RunManager), "Update")]
        public static class RunManagerUpdate
        {
            static void Postfix(RunManager __instance)
            {
                if (!NoEnimies.Lib.Dependant.IsHost()) return;
                if (Input.GetKeyDown(KeyCode.F4))
                {
                    __instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Normal);
                    mls.LogInfo("F4 pressed - Level change triggered ^w^!");
                }
                if (Input.GetKeyDown(KeyCode.F5))
                {
                    Traverse.Create(__instance).Field("previousRunLevel").SetValue(__instance.levelCurrent);
                    __instance.ChangeLevel(true, false, RunManager.ChangeLevelType.RunLevel);
                    mls.LogInfo("F5 pressed - Level change triggered ^w^!");
                }
                if (Input.GetKeyDown(KeyCode.F6))
                {
                    __instance.ChangeLevel(true, false, RunManager.ChangeLevelType.Recording);
                    mls.LogInfo("F6 pressed - Level change triggered ^w^!");
                }
                if (Input.GetKeyDown(KeyCode.F7))
                {
                    __instance.levelsCompleted++;
                    SemiFunc.StatSetRunLevel(__instance.levelsCompleted);
                }
                if (Input.GetKeyDown(KeyCode.F8))
                {
                    __instance.levelsCompleted--;
                    SemiFunc.StatSetRunLevel(__instance.levelsCompleted);
                }
                if (Input.GetKeyDown(KeyCode.F9))
                {
                    int currentIndex = __instance.levels.FindIndex(level => level == __instance.levelCurrent);


                    if (currentIndex >= 0 && currentIndex < __instance.levels.Count - 1)
                    {
                        __instance.levelCurrent = __instance.levels[currentIndex + 1];
                    }
                    else
                    {
                        __instance.levelCurrent = __instance.levels[0];
                    }
                    // force a update
                    if (GameManager.Multiplayer())
                    {
                        var photonView = Traverse.Create(__instance).Field("runManagerPUN").Field("photonView").GetValue<Photon.Pun.PhotonView>();
                        if (photonView == null) {
                            mls.LogInfo("Failed stage f9 tp 3:");
                            return;
                        }

                        var levelCurrent = Traverse.Create(__instance).Field("levelCurrent").GetValue<Level>();
                        var levelsCompleted = Traverse.Create(__instance).Field("levelsCompleted").GetValue<int>();
                        var gameOver = Traverse.Create(__instance).Field("gameOver").GetValue<bool>();

                        photonView.RPC("UpdateLevelRPC", Photon.Pun.RpcTarget.OthersBuffered, new object[] { levelCurrent?.name, levelsCompleted, gameOver });
                    }
                        SemiFunc.StatSetSaveLevel(Traverse.Create(__instance).Field("saveLevel").GetValue<int>());
                    __instance.RestartScene();
                    SemiFunc.OnSceneSwitch(Traverse.Create(__instance).Field("gameOver").GetValue<bool>(), false);
                }

            }
        }



        [HarmonyPatch(typeof(EnemyStateDespawn), "Start")]
        public class EnemyClasshook
        {
            static void Postfix(EnemyStateDespawn __instance)
            {
                    __instance.Despawn();
               // Destroy(__instance.gameObject);
                mls.LogInfo($"{modGUID} removed a entity!");
            }
        }

        [HarmonyPatch(typeof(EnemyStateDespawn), "Update")]
        public class NoStart
        {
            static void Postfix(EnemyStateDespawn __instance)
            {
                //Destroy(__instance.gameObject);
                __instance.Despawn();
                mls.LogInfo($"{modGUID} removed a entity!");
            }
        }

    }
}