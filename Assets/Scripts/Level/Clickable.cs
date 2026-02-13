public interface Clickable
{
    public bool IsClickable(); // Can this be clicked?
    public bool IsFresh(); // Will clicking this do something new/interesting?
    public void OnClick(); // Callback when it gets clicked

}