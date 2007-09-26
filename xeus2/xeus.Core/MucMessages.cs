namespace xeus2.xeus.Core
{
    internal class MucMessages : ObservableCollectionDisp<MucMessage>
    {
        public void OnMessage(agsXMPP.protocol.client.Message message, MucContact sender)
        {
            MucMessage mucMessage = new MucMessage(message, sender);
            Add(mucMessage);

            if (sender != null
                && !string.IsNullOrEmpty(mucMessage.Sender))
            {
                // Database.SaveMessage(mucMessage);
            }
        }
    }
}