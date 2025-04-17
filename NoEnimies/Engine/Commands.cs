using HarmonyLib;
using KillthemAll;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputRemoting;
using static UnityEngine.Rendering.PostProcessing.PostProcessResources;

namespace NoEnimies.Engine
{
   
    [HarmonyPatch(typeof(SemiFunc))]
    [HarmonyPatch("Command")]
    public class Commands : MonoBehaviour
    {
        private static readonly Dictionary<string, Func<string[], bool>> commandTable =
            new Dictionary<string, Func<string[], bool>>(StringComparer.OrdinalIgnoreCase)
            {
                { "/hider", Hider },
                { "/h", Hider },
            };
        public static int LevenshteinDistance(string a, string b)
        {
            if (a == null || b == null) return int.MaxValue;

            int n = a.Length, m = b.Length;
            if (n == 0) return m;  
            if (m == 0) return n;  

            int[] d = new int[m + 1];
            for (int j = 0; j <= m; d[j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                int prev = d[0];
                d[0] = i;

                for (int j = 1; j <= m; j++)
                {
                    int temp = d[j];
                    d[j] = Math.Min(Math.Min(d[j] + 1, d[j - 1] + 1), prev + (a[i - 1] == b[j - 1] ? 0 : 1));
                    prev = temp;
                }
            }

            return d[m];
        }


        [HarmonyPostfix]
        public static void Additional_Command(string _command)
        {
                if (!Lib.Dependant.IsHost()) return;

                string[] input = _command.Trim().Split(' ');
                string command = input[0].ToLower();

                string[] args;
                if (input.Length >= 2)
                {
                    args = new string[input.Length - 1];
                    Array.Copy(input, 1, args, 0, input.Length - 1);
                }
                else
                {
                    args = new string[0];
                }


                if (commandTable.ContainsKey(command))
                {
                     var result = commandTable[command](args);
                     if (result)
                        {

                            Chat.Say($"{command} with {args} success");
                            ChatUI.instance.SemiUITextFlashColor(Color.magenta, 0.2f);
                            ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
                            MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Tick, null, 1f, 0.2f, true);
                        } 
                    else
                        {
                            Chat.Say($"{command} with {args} failed");
                            ChatUI.instance.SemiUITextFlashColor(Color.red, 0.2f);
                            ChatUI.instance.SemiUISpringShakeY(2f, 5f, 0.2f);
                            MenuManager.instance.MenuEffectClick(MenuManager.MenuClickEffectType.Deny, null, 1f, 0.2f, true);
                        }
                }
      
        }

        public static int CountMatchingPrefix(string str1, string str2)
        {
            int matchCount = 0;
            int length = Math.Min(str1.Length, str2.Length);

            for (int i = 0; i < length; i++)
            {
                if (str1[i] == str2[i])
                {
                    matchCount++;
                }
                else
                {
                    break; 
                }
            }

            return matchCount;
        }


        private static bool Hider(string[] args)
        {
            try
            {
                var person = args[0];
                int closestDistance = 1000000;
                int closeslet = 0;
                PlayerAvatar closestPlayer = GameDirector.instance.PlayerList[0];
                if (KillthemAll.Plugin.Hiders.Count > 0) { KillthemAll.Plugin.Hiders.Clear(); }
                if (GameDirector.instance == null)
                {
                    return false;
                }
                foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
                {
                    string Nam = SemiFunc.PlayerGetName(playerAvatar).ToLower();
                    int dise = CountMatchingPrefix(Nam, person.ToLower());
                    int distance = LevenshteinDistance(Nam, person.ToLower());
                    //KillthemAll.Plugin.mls.LogInfo(dise+"dis: "+distance);
                    if ((closeslet < dise) || ((closeslet == dise) && (distance < closestDistance) ))
                    {
                        closestDistance = distance;
                        closeslet = dise;
                        closestPlayer = playerAvatar;
                    }
                }
                if (closestPlayer)
                {
                    KillthemAll.Plugin.Hiders.Add(closestPlayer);
                   // KillthemAll.Plugin.mls.LogInfo($"Setting Hider to {SemiFunc.PlayerGetName(closestPlayer)}!");
                    KillthemAll.Plugin.updateListMaps();
                } else
                {
                    return false;
                }
            }
            catch(Exception e)
            {
                KillthemAll.Plugin.mls.LogError(e.Message);
            };
            return true;
        }

    }
}
