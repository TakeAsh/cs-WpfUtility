using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using TakeAshUtility;

namespace WpfUtility {

    [AttributeUsage(AttributeTargets.Property)]
    public class DataGridExAttribute :
        Attribute {

        public const string KeyBinding = "Binding";

        private string _foreground;
        private string _background;

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
        public string Foreground {
            get { return _foreground; }
            set {
                _foreground = value;
                if (String.IsNullOrEmpty(_foreground) ||
                    (ForegroundValue = ToBinding(_foreground)) != null) {
                    return;
                }
                ForegroundValue = _foreground.TryParse<Brush>();
            }
        }

        public object ForegroundValue { get; private set; }

        [ToStringMember]
        public string Background {
            get { return _background; }
            set {
                _background = value;
                if (String.IsNullOrEmpty(_background) ||
                    (BackgroundValue = ToBinding(_background)) != null) {
                    return;
                }
                BackgroundValue = _background.TryParse<Brush>();
            }
        }

        public object BackgroundValue { get; private set; }

        [ToStringMember]
        public string ClipboardContentBinding { get; set; }

        public override string ToString() {
            return this.ToStringMembers();
        }

        private Binding ToBinding(string value) {
            if (!value.StartsWith(KeyBinding) ||
                value.Length <= KeyBinding.Length ||
                !Char.IsWhiteSpace(value[KeyBinding.Length])) {
                return null;
            }
            var propertyName = value.Substring(KeyBinding.Length).Trim();
            if (String.IsNullOrEmpty(propertyName)) {
                return null;
            }
            return new Binding(propertyName);
        }
    }
}
