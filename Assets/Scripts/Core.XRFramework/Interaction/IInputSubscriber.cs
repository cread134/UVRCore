namespace Core.XRFramework.Interaction
{
    public interface IInputSubscriber
    {
        public void OnTriggerDown(HandType handType);
        public void OnTriggerUp(HandType handType);
        public void OnTriggerChange(HandType handType, float newValue);

        public void OnMainDown(HandType handType);
        public void OnMainUp(HandType handType);
    }
}
