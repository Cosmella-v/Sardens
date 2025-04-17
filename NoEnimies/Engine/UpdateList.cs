
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace NoEnimies.Engine
{
    internal class Names
    {
        public class ListUpdater : MonoBehaviour {
            GameObject Updator;
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
                string resourceName = "NoEnimies.Resources.fish.png";
                if (packed)
                {
                    resourceName = "NoEnimies.Resources.Packedfish.png";
                }
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        KillthemAll.Plugin.mls.LogError("[Texture Error, UH OH] not found texture: " + resourceName);
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

            public void Update()
            {
                if (Usernames == GameDirector.instance.PlayerList.Count && Sardens == KillthemAll.Plugin.Hiders.Count) { return; };
                foreach (Transform child in Updator.transform)
                {
                    Destroy(child.gameObject);
                }
                Usernames = GameDirector.instance.PlayerList.Count;
                Sardens = KillthemAll.Plugin.Hiders.Count;
                var DisplayPlr = 1;

                var uin = Lib.Dependant.CustomLabel();
                uin.name = "Ui-Names";
                uin.GetComponent<TextMeshProUGUI>().text = "        Player List     ";
                uin.transform.SetParent(Updator.transform, false);

                foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
                {
                    var Label = Lib.Dependant.CustomLabel();
                    Label.name = SemiFunc.PlayerGetSteamID(playerAvatar);
                    Label.GetComponent<TextMeshProUGUI>().text = SemiFunc.PlayerGetName(playerAvatar);
                    Label.transform.SetParent(Updator.transform, false);
                    Label.transform.localPosition = new UnityEngine.Vector3(0, DisplayPlr * -28, 0);
                    AddEmbeddedImageTo(Label.transform, KillthemAll.Plugin.Hiders.Contains(playerAvatar));
                }

            }

            }
    }
}
