﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Sardens.Lib
{
    internal class Dependant
    {
        static public bool IsHost()
        {
            return SemiFunc.IsMasterClientOrSingleplayer();
        }
        static public bool CanMaster()
        {
            return Multplayer() && SemiFunc.IsMasterClient();
        }
        static public bool Multplayer()
        {
            return SemiFunc.IsMultiplayer();
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
            rect.anchorMin = new Vector2(0, 0.5f);
            rect.anchorMax = new Vector2(0, 0.5f);
            rect.pivot = new Vector2(0, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            var tmp = InGameLabelIsh.AddComponent<TextMeshProUGUI>();
            tmp.text = "PlaceHolder";
            tmp.enabled = true;
            tmp.font = tekoFontAsset;
            tmp.fontSize = 28;
            tmp.fontSizeMax = 24;
            tmp.fontSizeMin = 10;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Left;
            InGameLabelIsh.AddComponent<SemiUiPatch.PatchedSemiFun>();

            return InGameLabelIsh;
        }

    }
}
