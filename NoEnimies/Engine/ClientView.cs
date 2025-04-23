using Photon.Pun;
using System;
using System.Collections.Generic;
namespace FishSardens
{
    internal class Clients : MonoBehaviourPun
    {
        [PunRPC]
        void Viper_Sardiens_SendHidersToModdedClients(string[] data)
        {
            List<PlayerAvatar> Ls = new List<PlayerAvatar>();
            foreach (var client in data)
            {
                PlayerAvatar avatar = SemiFunc.PlayerAvatarGetFromSteamID(client);
                if (avatar != null)
                {
                    Ls.Add(avatar); 
                }
                
            }
            FishSardens.Plugin.Hiders = Ls;
            //FishSardens.Plugin.mls.LogInfo($"Viper_Sardiens_SendHidersToModdedClients is now syncing!");
        }
        [PunRPC]
        void Viper_Sardiens_ChildrenGamesSync_OnJoin(object data, PhotonMessageInfo info)
        {
            if (!Sardens.Lib.Dependant.CanMaster()) return;
            List<string> dataList = new List<string>();
            foreach (PlayerAvatar playerAvatar in FishSardens.Plugin.Hiders)
            {
                string steamID = SemiFunc.PlayerGetSteamID(playerAvatar);
                if (!string.IsNullOrEmpty(steamID))
                {
                    dataList.Add(steamID);
                }
            }

            string[] dataArray = dataList.ToArray();
            PhotonView photonView = PhotonView.Get(this);
            Photon.Realtime.Player targetClient = info.Sender;

            photonView.RPC("Viper_Sardiens_SendHidersToModdedClients", targetClient, new object[] { dataArray });
        }

        public void PleaseSyncMeWithTheHost()
        {
            if (Sardens.Lib.Dependant.IsHost()) return;
            photonView.RPC("Viper_Sardiens_ChildrenGamesSync_OnJoin", RpcTarget.MasterClient, new object[] {});
        }
        public void SendList()
        {
            if (!Sardens.Lib.Dependant.CanMaster()) return;
            List<string> dataList = new List<string>();
            foreach (PlayerAvatar playerAvatar in FishSardens.Plugin.Hiders)
            {
                string steamID = SemiFunc.PlayerGetSteamID(playerAvatar);
                if (!string.IsNullOrEmpty(steamID)) 
                {
                    dataList.Add(steamID);
                }
            }

            string[] dataArray = dataList.ToArray();
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("Viper_Sardiens_SendHidersToModdedClients", RpcTarget.Others, new object[] { dataArray });
        }

    }
}