namespace xeus2.xeus.UI
{
    public delegate void CloseContainer();

    public interface IFlyoutContainer
    {
        event CloseContainer CloseMe;
        void Closing();
    }
}