
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace NoEnimies.Engine
{
    internal class Names
    {
        public class ListUpdater : MonoBehaviour {
            GameObject Updator;
            int Usernames;
            private void Start()
            {
                Updator = this.transform.gameObject;
                this.Update();
            }
            public void Update()
            {
                if (Usernames == GameDirector.instance.PlayerList.Count) { return; };
                foreach (Transform child in Updator.transform)
                {
                    Destroy(child.gameObject);
                }
                Usernames = GameDirector.instance.PlayerList.Count;
                foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
                {
                    var Label = Lib.Dependant.CustomLabel();
                    Label.name = SemiFunc.PlayerGetSteamID(playerAvatar);
                    Label.GetComponent<TextMeshProUGUI>().text = SemiFunc.PlayerGetName(playerAvatar);
                    Label.transform.SetParent(Updator.transform, false);
                }

            }

            }
    }
}
