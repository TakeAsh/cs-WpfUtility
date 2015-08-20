using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfUtility {

    internal class DataGridExAutoFilterSubItem {

        public DataGridExAutoFilterSubItem() {
            IsChecked = true;
            Count = 1;
        }

        public bool IsChecked { get; set; }

        public int Count { get; set; }

        public override string ToString() {
            return "IsChecked:{" + IsChecked + "}, Count:{" + Count + "}";
        }
    }
}
