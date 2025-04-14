using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace NoEnimies.Lib
{
    internal class Dependant
    {
        static public bool IsHost()
        {
            return SemiFunc.IsMasterClientOrSingleplayer();
        }

        static public bool Multplayer()
        {
            return GameManager.instance.gameMode == 1;
        }
        static public TMP_FontAsset tekoFontAsset = null;
        static private TMP_FontAsset GetFontAsset()
        {
            if (tekoFontAsset != null) { return tekoFontAsset; }
            foreach (var fontAsset in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
            {
                if (fontAsset.name.Contains("Teko"))
                {
                    tekoFontAsset = fontAsset;
                    break;
                }
            }
            return tekoFontAsset;
        }
        static public GameObject CustomLabel()
        {
            if (GetFontAsset() == null)
            {
                return new GameObject();
            }

            var InGameLabelIsh = new GameObject("CustomLabel", typeof(RectTransform));
            var rect = InGameLabelIsh.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600f, 100f);
            rect.anchoredPosition = Vector2.zero;

            var tmp = InGameLabelIsh.AddComponent<TextMeshProUGUI>();
            tmp.text = "PlaceHolder";
            tmp.enabled = true;
            tmp.font = tekoFontAsset;
            tmp.fontSize = 28;
            tmp.fontSizeMax = 24;
            tmp.fontSizeMin = 10;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            InGameLabelIsh.AddComponent<SemiUiPatch.PatchedSemiFun>();

            return InGameLabelIsh;
        }

    }
}
