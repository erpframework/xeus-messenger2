using System;
using System.Collections.Generic;
using agsXMPP.protocol.x.data;
using agsXMPP.Xml.Dom;

namespace xeus2.xeus.Core
{
    internal class MucRoomInfo
    {
        private string _description;
        private int _occupants;

        private List<Field> _data = new List<Field>();

        public MucRoomInfo(Data xData)
        {
            foreach (Node node in xData.ChildNodes)
            {
                Field field = node as Field;

                if (field != null
                    && field.Label != null
                    && field.GetValues().Length > 0
                    && field.Var != null)
                {
                    _data.Add(field);

                    if (field.GetValues() != null
                        && field.GetValues().Length > 0)
                    {
                        switch (field.Var)
                        {
                            case "muc#roominfo_occupants":
                                {
                                    _occupants = Int32.Parse((field.GetValues()[0]));
                                    break;
                                }
                            case "muc#roominfo_description":
                                {
                                    _description = field.GetValues()[0];
                                    break;
                                }
                        }
                    }
                }
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public int Occupants
        {
            get
            {
                return _occupants;
            }
        }
    }
}