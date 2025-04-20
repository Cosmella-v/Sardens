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
    internal class CommandBuilder
    {
        private static readonly Dictionary<string, Func<string[], bool>> commandTable_host =
            new Dictionary<string, Func<string[], bool>>(StringComparer.OrdinalIgnoreCase)
            {
                { "/hider", Hider },
                { "/h", Hider },
                { "/rh", NoHider },
                { "/removehider", NoHider },
            };
        private static readonly Dictionary<string, Func<string[], bool>> commandTable_client =
           new Dictionary<string, Func<string[], bool>>(StringComparer.OrdinalIgnoreCase)
           {
                
               
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

        public static bool process_Commands(string _command, bool fromhost)
        {
            if (!Lib.Dependant.IsHost()) return true;

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

            var Tablesomething = fromhost ? commandTable_host : commandTable_client;
            if (Tablesomething.ContainsKey(command))
            {
                var result = Tablesomething[command](args);

                return false;
            }
            return true;

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

        private static bool NoHider(string[] args)
        {
            KillthemAll.Plugin.Hiders.Clear();
            KillthemAll.Plugin.updateListMaps();
            return true;
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
                    if ((closeslet < dise) || ((closeslet == dise) && (distance < closestDistance)))
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
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                KillthemAll.Plugin.mls.LogError(e.Message);
            };
            return true;
        }
    }
}
