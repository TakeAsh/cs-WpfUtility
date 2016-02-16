using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TakeAshUtility;

namespace WpfUtility {

    [AttributeUsage(AttributeTargets.Property)]
    public class DataGridExAttribute :
        Attribute {

        public DataGridExAttribute() { }

        public DataGridExAttribute(string header) {
            Header = header;
        }

        [ToStringMember]
        public string Header { get; set; }

        [ToStringMember]
        public bool Ignore { get; set; }

        [ToStringMember]
        public string StringFormat { get; set; }

        [ToStringMember]
        public string Foreground { get; set; }

        [ToStringMember]
        public string Background { get; set; }

        [ToStringMember]
        public string ClipboardContentBinding { get; set; }

        public override string ToString() {
            return this.ToStringMembers();
        }
    }
}
