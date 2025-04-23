using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
// allows you to use command on solo!
namespace Sardens.Engine.SinglePlayerModded
{
    internal class ChatFixer
    {
        // Converts the IsMultiplayer checks to Nop instructions
        [HarmonyPatch(typeof(ChatManager))]
        [HarmonyPatch("Update")]
        public static class ChatManager_Update_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var code = new List<CodeInstruction>(instructions);
                var multiplayerMethod = AccessTools.Method(typeof(SemiFunc), "IsMultiplayer");

                for (int i = 0; i < code.Count; i++)
                {
                    if (
                        code[i].opcode == OpCodes.Call &&
                        code[i].operand is MethodInfo m &&
                        m == multiplayerMethod &&
                        (code[i + 1].opcode == OpCodes.Brtrue || code[i + 1].opcode == OpCodes.Brtrue_S)
                    )
                    {
                        var jmp = code[i + 1];
                        var labelTarget = (Label)jmp.operand;

                        do
                        {
                            code[i].opcode = OpCodes.Nop;
                            code[i].operand = null;
                            i++;
                        } while (!code[i].labels.Contains(labelTarget));

                    }
                }

                return code;
            }
        }


    }
}
