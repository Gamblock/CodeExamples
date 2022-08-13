namespace UnlockGames.BA.UI.Overlay
{
    public class OverlayUITalentView : OverlayUIConsumableView
    {
        protected override void SetValueText(int value)
        {
            _valueText.text = $"#{value.ToString()}";
        }
    }
}