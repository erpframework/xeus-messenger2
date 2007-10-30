using System;
using System.Collections.Generic;
using System.Data;

namespace xeus2.xeus.Core
{
    internal class Group : NotifyInfoDispatcher
    {
        private readonly string _name = "<undefined>";
        private bool _isExpanded = true;
        private string _imagePath = null;
        private string _description = null;

        public Group(string name, bool isExpanded)
        {
            _name = name;
            _isExpanded = isExpanded;
        }

        public Group(IDataRecord reader)
        {
            _isExpanded = ((Int64)reader["IsExpanded"] == 1);
            _name = (string)reader["Name"];

            if (!reader.IsDBNull(reader.GetOrdinal("Image")))
            {
                _imagePath = (string)reader["Image"];
            }

            if (!reader.IsDBNull(reader.GetOrdinal("Description")))
            {
                _description = (string)reader["Description"];
            }
        }
        
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                _isExpanded = value;
                NotifyPropertyChanged("IsExpanded");
            }
        }

        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public Dictionary<string, object> GetData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data.Add("IsExpanded", (IsExpanded) ? 1 : 0);
            data.Add("Image", ImagePath);
            data.Add("Description", Description);
            data.Add("Name", Name);

            return data;
        }

        public string GroupDetails
        {
            get
            {
                Roster.GroupStatus groupStatus = Roster.Instance.GetGroupStatus(Name);

                return string.Format("({0} available of {1})", groupStatus._online, groupStatus._total);
            }
        }

        internal void Refresh()
        {
            NotifyPropertyChanged("GroupDetails");
        }
    }
}