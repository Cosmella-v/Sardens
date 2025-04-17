using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace NoEnimies.Lib
{
    internal class SemiUiPatch
    {
       
        public class PatchedSemiFun : SemiUI {

            protected override void Start()
            {
                base.Start();
            }

            protected override void Update()
            {
                base.Update();
                this.uiText.gameObject.SetActive(!SemiFunc.MenuLevel());
            }

            }

    }
}
