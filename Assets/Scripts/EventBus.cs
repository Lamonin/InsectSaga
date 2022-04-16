using System;

public static class EventBus
{
    public static Action<bool> OnDialogueStart;
    public static Action OnDialogueEnd;
}
