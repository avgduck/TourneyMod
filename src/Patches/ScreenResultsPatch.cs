using System.Collections;
using HarmonyLib;
using LLBML.Players;
using LLBML.Settings;

namespace TourneyMod.Patches;

internal static class ScreenResultsPatch
{
    // GameStatesGameResult.UpdateState(GameState state)
    [HarmonyPatch(typeof(OEAINNHEMKA), nameof(OEAINNHEMKA.UpdateState))]
    [HarmonyPostfix]
    private static void ResultUpdateState_Postfix(OEAINNHEMKA __instance)
    {
        if (Plugin.Instance.ActiveTourneyMode == TourneyMode.NONE) return;
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
        if (Plugin.Instance.ActiveTourneyMode == TourneyMode.NONE) return;
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
}