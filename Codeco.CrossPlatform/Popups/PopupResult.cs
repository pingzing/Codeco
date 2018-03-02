namespace Codeco.CrossPlatform.Popups
{
    public struct PopupResult
    {
        public PopupChoice PopupChoice { get; set; }
    }

    public struct PopupResult<T>
    {
        public PopupChoice PopupChoice { get; set; }
        public T Result { get; set; }        
    }

    public enum PopupChoice
    {
        Unknown,
        Ok,
        Cancel
    }
}
