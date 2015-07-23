using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfUtility {

    [AttributeUsage(AttributeTargets.Property)]
    public class DataGridExAttribute :
        Attribute {

        public DataGridExAttribute() { }

        public DataGridExAttribute(string header) {
            Header = header;
        }

        public string Header { get; set; }
        public bool Ignore { get; set; }

        public override string ToString() {
            var list = new List<string>();
            list.Add("Header:'" + Header + "'");
            list.Add("Ignore:" + Ignore);
            return String.Join(", ", list);
        }
    }
}
