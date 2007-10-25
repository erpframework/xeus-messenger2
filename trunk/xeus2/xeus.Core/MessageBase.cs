namespace xeus2.xeus.Core
{
    public class MessageBase : NotifyInfoDispatcher
    {
        private RelativeOldness _dateTime = new RelativeOldness(System.DateTime.Now);

        public RelativeOldness DateTime
        {
            get
            {
                return _dateTime;
            }
            
            protected set
            {
                _dateTime = value;
            }
        }
    }
}