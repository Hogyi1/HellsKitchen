public interface IHoldableItem : IObjectChild
{
    void Hold();
    bool IsTwoHanded();
    AudioSO GetPlaceAudio();
    AudioSO GetPickUpAudio();
}
