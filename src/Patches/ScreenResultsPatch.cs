using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;
using TourneyMod.PlayerTags;
using TourneyMod.SetTracking;

namespace TourneyMod.Patches;

internal static class ScreenResultsPatch
{
    // GameStatesGameResult.UpdateState(GameState state)
    [HarmonyPatch(typeof(OEAINNHEMKA), nameof(OEAINNHEMKA.UpdateState))]
    [HarmonyPostfix]
    private static void ResultUpdateState_Postfix(OEAINNHEMKA __instance)
    {
        if (SetTracker.Instance.ActiveTourneyMode == TourneyMode.NONE) return;
        if (GameSettings.IsOnline) return;
        
        PostScreen screenResults = __instance.APFKDEMGLHJ;
        if (screenResults == null) return;
            
        Player.ForAll(player =>
        {
            // KHMFCILNHHH.EOCBBKOIFNO -> RematchChoice.QUIT
            __instance.DABHMHOCDEN(player.nr, KHMFCILNHHH.EOCBBKOIFNO);
        });
    }
        
    // GameStatesGameResult.SetRematchChoice(int playerNumber, RematchChoice choice)
    [HarmonyPatch(typeof(OEAINNHEMKA), nameof(OEAINNHEMKA.DABHMHOCDEN))]
    [HarmonyPostfix]
    private static void SetRematchChoice_Postfix(OEAINNHEMKA __instance, int BKEOPDPFFPM, KHMFCILNHHH ONPJANKJDJH)
    {
        if (SetTracker.Instance.ActiveTourneyMode == TourneyMode.NONE) return;
        if (GameSettings.IsOnline) return;
            
        int playerNumber = BKEOPDPFFPM;
        KHMFCILNHHH rematchChoice = ONPJANKJDJH;
        PostScreen screenResults = __instance.APFKDEMGLHJ;

        screenResults.SetChoice(playerNumber, rematchChoice);
        // NIPJFJKNGHO.DLPDHJFPKMJ -> ResultButtons.REMATCH_QUIT
        // KHMFCILNHHH.EOCBBKOIFNO -> RematchChoice.QUIT
        if (playerNumber == 0 && screenResults.resultButtons == NIPJFJKNGHO.DLPDHJFPKMJ &&
            rematchChoice == KHMFCILNHHH.EOCBBKOIFNO)
        {
            // NIPJFJKNGHO.EOCBBKOIFNO -> ResultButtons.QUIT
            screenResults.ShowButtons(NIPJFJKNGHO.EOCBBKOIFNO);
        }
    }

    [HarmonyPatch(typeof(PostScreen), nameof(PostScreen.CFillXpBar))]
    [HarmonyPostfix]
    private static IEnumerator CFillXpBar_Wrapper(IEnumerator __result)
    {
        //while (__result.MoveNext()) yield return __result.Current;
        yield break;
    }
    
    [HarmonyPatch(typeof(CPNJEILDILH), nameof(CPNJEILDILH.PEORFKFKGGGG))]
    [HarmonyPostfix]
    private static IEnumerator CShowCurrencyGain_Wrapper(IEnumerator __result)
    {
        //while (__result.MoveNext()) yield return __result.Current;
        yield break;
    }
    
    [HarmonyPatch(typeof(PostScreen), nameof(PostScreen.SetResult))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> PostScreen_SetResult_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher cm = new CodeMatcher(instructions);
        cm.End();
        /*
         * if (!JOMBNFKIHIC.GDNFJCCCKDM) // GameSettings.isOnline {
         *      text2 = JPLELOFJOOH.OAGHLPGCAOI(ohnhlliajef.LALEEFJMMLH); // Assets.GetCharacterName(cachedPlayer.character)
         * }
         * we're matching the line in the if statement
         */
        cm.MatchBack(false,
            new CodeMatch(OpCodes.Ldloca_S),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(OHNHLLIAJEF), nameof(OHNHLLIAJEF.LALEEFJMMLH))),
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(JPLELOFJOOH), nameof(JPLELOFJOOH.OAGHLPGCAOI))),
            new CodeMatch(OpCodes.Stloc_S)
        );
        LocalBuilder refCachedPlayer = (LocalBuilder)cm.Instruction.operand; // save address of cachedPlayer
        cm.Advance(3); // keep character name calls
        cm.Insert(
            new CodeInstruction(OpCodes.Ldloca_S, refCachedPlayer),
            new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(OHNHLLIAJEF), nameof(OHNHLLIAJEF.CJFLMDNNMIE))),
            Transpilers.EmitDelegate<Func<string, int, string>>((characterName, playerNr) =>
            {
                PlayerTag playerTag = Plugin.Instance.GetPlayerTag(playerNr);
                return playerTag.IsDefault ? characterName : playerTag.GetName();
            })
        );
        return cm.InstructionEnumeration();
    }
}