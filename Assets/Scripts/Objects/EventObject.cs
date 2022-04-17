using UnityEngine;
using UnityEngine.Events;

namespace Objects
{
    public class EventObject : UsableObject
    {
        public int currentEvent;
        public UnityEvent[] events;
        
        public override void Interact()
        {
            if (!interactable) return;
            events[currentEvent]?.Invoke();
        }
        
        public override void Deactivate()
        {
            base.Deactivate();
        }

        public void SetEventNumber(int num) => currentEvent = num;
    }
}