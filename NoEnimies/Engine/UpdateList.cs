
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Sardens.Engine
{
    internal class Names
    {
        public class ListUpdater : MonoBehaviour {
            GameObject Updator;
            GameObject PlayerList;
            class PlayerUIData
            {
                public GameObject LabelObject;
                public string LastName;
                public bool WasHider;
            }
            private Dictionary<PlayerAvatar, PlayerUIData> playerUIDict = new Dictionary<PlayerAvatar, PlayerUIData>();

            int Usernames;
            int Sardens;
            private void Start()
            {
                Updator = this.transform.gameObject;
                this.Update();
            }
            private void AddEmbeddedImageTo(Transform parent, bool packed)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = "Sardens.Resources.fish.png";
                if (packed)
                {
                    resourceName = "Sardens.Resources.Packedfish.png";
                }
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        FishSardens.Plugin.mls.LogError("[Texture Error, UH OH] not found texture: " + resourceName);
                        return;
                    }

                    byte[] imageData = new byte[stream.Length];
                    stream.Read(imageData, 0, imageData.Length);

                    Texture2D tex = new Texture2D(2, 2);
                    tex.LoadImage(imageData);

                    Sprite sprite = Sprite.Create(
                        tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    GameObject imageObj = new GameObject("FishImage");
                    imageObj.transform.SetParent(parent, false);
                    imageObj.transform.localScale = new UnityEngine.Vector3(0.3f,0.3f,0.3f);
                    Image image = imageObj.AddComponent<Image>();
                    image.sprite = sprite;
                    imageObj.transform.localPosition = new UnityEngine.Vector3(-19, 0, 0);
                }
            }

            private void RefreshList()
            {
                if (!PlayerList)
                {
                    PlayerList = Lib.Dependant.CustomLabel();
                    PlayerList.name = "Ui-Names";
                    PlayerList.GetComponent<TextMeshProUGUI>().text = "        Player List     ";
                    PlayerList.transform.SetParent(Updator.transform, false);
                }

                var currentPlayers = GameDirector.instance.PlayerList;
                var currentHiders = FishSardens.Plugin.Hiders;

                var toRemove = new List<PlayerAvatar>();
                foreach (var entry in playerUIDict)
                {
                    if (!currentPlayers.Contains(entry.Key))
                    {
                        Destroy(entry.Value.LabelObject);
                        toRemove.Add(entry.Key);
                    }
                }
                foreach (var player in toRemove)
                {
                    playerUIDict.Remove(player);
                }

                int displayIndex = 1;
                foreach (var player in currentPlayers)
                {
                    string newName = SemiFunc.PlayerGetName(player);
                    bool isHider = currentHiders.Contains(player);

                    if (!playerUIDict.TryGetValue(player, out PlayerUIData uiData))
                    {
                        var labelObj = Lib.Dependant.CustomLabel();
                        labelObj.name = SemiFunc.PlayerGetSteamID(player);
                        labelObj.transform.SetParent(Updator.transform, false);
                        labelObj.transform.localScale = Vector3.one;

                        uiData = new PlayerUIData
                        {
                            LabelObject = labelObj,
                            LastName = newName,
                            WasHider = isHider
                        };

                        playerUIDict[player] = uiData;

                        labelObj.GetComponent<TextMeshProUGUI>().text = newName;
                        AddEmbeddedImageTo(labelObj.transform, isHider);
                    }
                    else
                    {
                        var label = uiData.LabelObject.GetComponent<TextMeshProUGUI>();

                        if (uiData.LastName != newName)
                        {
                            label.text = newName;
                            uiData.LastName = newName;
                        }

                        if (uiData.WasHider != isHider)
                        {
                            foreach (Transform child in uiData.LabelObject.transform)
                            {
                                if (child.name == "FishImage")
                                {
                                    Destroy(child.gameObject);
                                }
                            }
                            AddEmbeddedImageTo(uiData.LabelObject.transform, isHider);
                            uiData.WasHider = isHider;
                        }
                    }

                    uiData.LabelObject.transform.localPosition = new Vector3(0, displayIndex * -28, 0);
                    displayIndex++;
                }

                Usernames = currentPlayers.Count;
                Sardens = currentHiders.Count;
            }
            public void Update()
            {
                if (Usernames == GameDirector.instance.PlayerList.Count && Sardens == FishSardens.Plugin.Hiders.Count) { return; };
                RefreshList();
            }

            }
    }
}
