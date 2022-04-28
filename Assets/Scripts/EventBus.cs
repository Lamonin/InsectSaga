using System;

public static class EventBus
{
    public static Action<bool, bool> OnDialogueStart;
    public static Action OnDialogueEnd;
    public static Action OnPlayerDiedEvent;
    public static Action OnPlayerRespawned;
    public static Action OnBlackScreenFadeInEvent;

    public static void ClearEvents()
    {
        // OnDialogueStart = null;
        // OnDialogueEnd = null;
        // OnPlayerDiedEvent = null;
        // OnPlayerRespawned = null;
        // OnBlackScreenFadeInEvent = null;
    }
}
