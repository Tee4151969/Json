using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonViewer
{
    class ClassMapping
    {
        private string _id;
        private string _subid;
        private string _Value;
        private string _Mapping;
        private Boolean _IsMaster;
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public string SubId
        {
            get
            {
                return _subid;
            }
            set
            {
                _subid = value;
            }
        }
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
        public string Mapping
        {
            get
            {
                return _Mapping;
            }
            set
            {
                _Mapping = value;
            }
        }
        public Boolean IsMaster
        {
            get
            {
                return _IsMaster;
            }
            set
            {
                _IsMaster = value;
            }
        }
    }
    class ClassFieldFind
    {
        private string _Key;
        private string _Mapping;
        private string _FullPath;
        private int _Level;
        private Boolean _Master;

        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
            }
        }
        public string Mapping
        {
            get
            {
                return _Mapping;
            }
            set
            {
                _Mapping = value;
            }
        }
        public string FullPath
        {
            get
            {
                return _FullPath;
            }
            set
            {
                _FullPath = value;
            }
        }
        public int Level
        {
            get
            {
                return _Level;
            }
            set
            {
                _Level = value;
            }
        }
        public Boolean Master
        {
            get
            {
                return _Master;
            }
            set
            {
                _Master = value;
            }
        }
    }

    class ClassToSQL
    {
        private string _FormatQuery;
        private string _ResultQuery;
        private List<ClassFieldFind> _ArrayFind;
        public string FormatQuery
        {
            get
            {
                return _FormatQuery;
            }
            set
            {
                _FormatQuery = value;
            }
        }
        public string ResultQuery
        {
            get
            {
                return _ResultQuery;
            }
            set
            {
                _ResultQuery = value;
            }
        }
        public List<ClassFieldFind> ArrayFind
        {
            get
            {
                return _ArrayFind;
            }
            set
            {
                _ArrayFind = value;
            }
        }
    }
    class ClassDB
    {
        private string _User;
        private string _Password;
        private string _Host;
        private string _Port;
        private string _Instance;
        private string _Table;
        public string User
        {
            get
            {
                return _User;
            }
            set
            {
                _User = value;
            }
        }
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }
        public string Host
        {
            get
            {
                return _Host;
            }
            set
            {
                _Host = value;
            }
        }
        public string Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
            }
        }
        public string Instance
        {
            get
            {
                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }
        public string Table
        {
            get
            {
                return _Table;
            }
            set
            {
                _Table = value;
            }
        }
    }
    class ClassOptions
    {
        private string _Key;
        private string _Value;
        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
            }
        }
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }
    }
}
