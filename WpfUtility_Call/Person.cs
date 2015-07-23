using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TakeAsh;
using TakeAshUtility;
using WpfUtility;

namespace WpfUtility_Call {

    using ItemsHelper = EnumHelper<Person.Items>;

    public class Person :
        INotifyPropertyChanged {

        public enum Items {
            ID,
            FirstName,
            LastName,
            Sex,
        }

        /// <summary>
        /// Codes for the representation of human sexes (ISO/IEC 5218)
        /// </summary>
        /// <remarks>
        /// [ISO/IEC 5218 - Wikipedia](https://en.wikipedia.org/wiki/ISO/IEC_5218)
        /// </remarks>
        [TypeConverter(typeof(EnumTypeConverter<SexesCodes>))]
        public enum SexesCodes {
            [Description("Not Known")]
            NotKnown = 0,
            Male = 1,
            Female = 2,
            [Description("Not Applicable")]
            NotApplicable = 9,
        }

        private static readonly Regex _regFromString = new Regex(@"(?<key>[^:]+):(?<value>[\s\S]*)");
        private static readonly char[] _blankAndQuote = new[] { ' ', '\t', '\r', '\n', '\'', '"', };

        public Person() { }

        public Person(string text) {
            var tmp = FromString(text);
            if (tmp == null) {
                return;
            }
            ItemsHelper.Values
                .ToList()
                .ForEach(key => this[key] = tmp[key]);
        }

        private int _id;
        public int ID {
            get { return _id; }
            set {
                _id = value;
                this.NotifyPropertyChanged("ID");
            }
        }

        private string _firstName;
        public string FirstName {
            get { return _firstName; }
            set {
                _firstName = value;
                this.NotifyPropertyChanged(new[] { "FirstName", "FullName" });
            }
        }

        private string _lastName;
        public string LastName {
            get { return _lastName; }
            set {
                _lastName = value;
                this.NotifyPropertyChanged(new[] { "LastName", "FullName" });
            }
        }

        public string FullName {
            get { return _firstName + " " + _lastName; }
        }

        private SexesCodes _sex;
        public SexesCodes Sex {
            get { return _sex; }
            set {
                _sex = value;
                this.NotifyPropertyChanged("Sex");
            }
        }

        [DataGridEx("Now")]
        public string Dummy1 {
            get { return DateTime.Now.ToString("g"); }
        }

        [DataGridEx(Ignore = true)]
        public string Dummy2 {
            get { return DateTime.Now.ToString("g"); }
        }

        public string this[Items item] {
            get {
                switch (item) {
                    case Items.ID:
                        return ID.ToString();
                    case Items.FirstName:
                        return FirstName;
                    case Items.LastName:
                        return LastName;
                    case Items.Sex:
                        return Sex.ToString();
                    default:
                        throw new NotImplementedException("Not implemented: " + item.ToString());
                }
            }
            set {
                switch (item) {
                    case Items.ID:
                        ID = value.TryParse<int>();
                        break;
                    case Items.FirstName:
                        FirstName = value;
                        break;
                    case Items.LastName:
                        LastName = value;
                        break;
                    case Items.Sex:
                        Sex = value.TryParse<SexesCodes>();
                        break;
                    default:
                        throw new NotImplementedException("Not implemented: " + item.ToString());
                }
            }
        }

        public override string ToString() {
            return String.Join(
                "\t",
                ItemsHelper.Values
                    .Select(value => value.ToString() + ":" + this[value])
            );
        }

        public static Person FromString(string text) {
            if (String.IsNullOrEmpty(text)) {
                return default(Person);
            }
            var ret = new Person();
            text.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .ForEach(field => {
                    var mc = _regFromString.Matches(field);
                    if (mc.Count == 0) {
                        return;
                    }
                    var key = mc[0].Groups["key"].Value.Trim(_blankAndQuote).TryParse<Items>();
                    var value = mc[0].Groups["value"].Value.Trim(_blankAndQuote);
                    ret[key] = value;
                });
            return ret;
        }

        #region INotifyPropertyChanged
        #pragma warning disable 0067

        public event PropertyChangedEventHandler PropertyChanged;

        #pragma warning restore 0067
        #endregion
    }

    public class Persons :
        ObservableCollection<Person> {

        public Persons() : base() { }
        public Persons(IEnumerable<Person> list) : base(list) { }
    }

    public static class PersonExtensionMethods {

        public static Person ToPerson(this string text) {
            return String.IsNullOrEmpty(text) ?
                null :
                Person.FromString(text);
        }

        public static Persons ToPersons(this string text) {
            return String.IsNullOrEmpty(text) ?
                null :
                new Persons(
                    text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(line => line.ToPerson())
                );
        }
    }
}
