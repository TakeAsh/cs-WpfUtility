using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TakeAshUtility;

namespace WpfUtility {

    using _resources = Properties.Resources;

    public class DataGridEx :
        DataGrid {

        private enum FilterItemsActions {
            Add,
            Remove,
        }

        private static readonly string[] _duplicatedProperties = new[] {
            "FontFamily",
            "FontSize",
            "FontStretch",
            "FontStyle",
            "FontWeight",
            "IsReadOnly",
            "CanUserAddRows",
            "CanUserDeleteRows",
            "CanUserReorderColumns",
            "CanUserResizeColumns",
            "CanUserResizeRows",
            "CanUserSortColumns",
        };

        static DataGridEx() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DataGridEx),
                new FrameworkPropertyMetadata(typeof(DataGridEx))
            );
        }

        public DataGridEx()
            : base() {

            this.AddPropertyChanged(ItemsControl.ItemsSourceProperty, OnItemsSourceUpdated);
            AutoGeneratingColumn += OnAutoGeneratingColumn;
        }

        public DataGridEx(DataGridEx source)
            : this() {
            this.Duplicate(source);
            this.Duplicate(source, _duplicatedProperties);
            this.ItemsSource = CloneItemsSource == null ?
                source.ItemsSource :
                CloneItemsSource(source.ItemsSource);
        }

        private List<string> _columnNames;
        private Dictionary<string, DataGridExAutoFilterItem> _autoFilterItems;
        private CollectionView _collectionView;

        public Type DataType { get; set; }

        public Func<IEnumerable, IEnumerable> CloneItemsSource { get; set; }

        public DataGridEx Clone() {
            return new DataGridEx(this);
        }

        public void Refresh() {
            _collectionView.Refresh();
        }

        private void ModifyFilterItems(FilterItemsActions action, IEnumerable items) {
            if (_columnNames == null) {
                return;
            }
            foreach (var item in items) {
                _columnNames.Select(columnName => _autoFilterItems[columnName])
                    .Where(filterItem => !filterItem.DataGridExAttr.Ignore)
                    .ToList()
                    .ForEach(filterItem => {
                        var pi = item.GetType()
                            .GetProperty(filterItem.Name, BindingFlags.Instance | BindingFlags.Public);
                        if (pi == null) {
                            return;
                        }
                        var value = pi.GetValue(item, null).SafeToString(null);
                        switch (action) {
                            case FilterItemsActions.Add:
                                filterItem.AddValue(value);
                                break;
                            case FilterItemsActions.Remove:
                                filterItem.RemoveValue(value);
                                break;
                        }
                    });
            }
            _columnNames.ForEach(columnName => _autoFilterItems[columnName].Update());
        }

        private static NotifyCollectionChangedEventHandler CreateNotifyCollectionChangedEventHandler(DataGridEx dataGridEx) {
            return (sender, e) => {
                var view = sender as ListCollectionView;
                if (view == null || view.ItemProperties == null) {
                    return;
                }
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Reset:
                        if (dataGridEx._columnNames == null) {
                            dataGridEx._columnNames = view.ItemProperties
                                .Select(info => info.Name).ToList();
                            dataGridEx._autoFilterItems = dataGridEx._columnNames
                                .Select(name => new DataGridExAutoFilterItem(name, dataGridEx))
                                .ToDictionary(filterItem => filterItem.Name);
                            if (view.Count > 0) {
                                dataGridEx.ModifyFilterItems(FilterItemsActions.Add, view);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        dataGridEx.ModifyFilterItems(FilterItemsActions.Add, e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        dataGridEx.ModifyFilterItems(FilterItemsActions.Remove, e.OldItems);
                        break;
                }
            };
        }

        private static Predicate<object> CreatePredicate(DataGridEx dataGridEx) {
            return (item) => {
                if (dataGridEx._autoFilterItems == null) {
                    return true;
                }
                foreach (var p in dataGridEx._columnNames) {
                    foreach (var kvp in dataGridEx._autoFilterItems[p].Values) {
                        if (kvp.Value.IsChecked == true) {
                            continue;
                        }
                        var pi = (dataGridEx.DataType ?? item.GetType())
                            .GetProperty(p, BindingFlags.Instance | BindingFlags.Public);
                        var value = pi.GetValue(item, null).SafeToString();
                        if (value != null && value == kvp.Key) {
                            return false;
                        }
                    }
                }
                return true;
            };
        }

        private static void OnItemsSourceUpdated(object sender, EventArgs e) {
            var dataGridEx = sender as DataGridEx;
            if (dataGridEx == null || dataGridEx.ItemsSource == null) {
                return;
            }
            dataGridEx.ColumnHeaderStyle = new Style();
            var view = dataGridEx._collectionView = CollectionViewSource.GetDefaultView(dataGridEx.ItemsSource) as ListCollectionView;
            ((INotifyCollectionChanged)view).CollectionChanged += CreateNotifyCollectionChangedEventHandler(dataGridEx);
            view.Filter = CreatePredicate(dataGridEx);
        }

        private static void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
            var dataGridEx = sender as DataGridEx;
            if (dataGridEx == null || dataGridEx._columnNames == null) {
                return;
            }
            var columnNames = dataGridEx._columnNames;
            var autoFilterItems = dataGridEx._autoFilterItems;
            if (!columnNames.Contains(e.PropertyName)) {
                columnNames.Add(e.PropertyName);
                autoFilterItems[e.PropertyName] = new DataGridExAutoFilterItem(e.PropertyName, dataGridEx);
            }
            var attr = autoFilterItems[e.PropertyName].DataGridExAttr;
            e.Cancel = attr.Ignore;
            e.Column.Header = attr.Header;
            if (!String.IsNullOrEmpty(attr.ClipboardContentBinding)) {
                e.Column.ClipboardContentBinding = new Binding(attr.ClipboardContentBinding);
            }
            var textColumn = e.Column as DataGridTextColumn;
            var checkBoxColumn = e.Column as DataGridCheckBoxColumn;
            if (textColumn != null) {
                textColumn.Binding.StringFormat = attr.StringFormat;
                var cellStyle = textColumn.GetCurrentStyle<DataGridCell>(DataGridBoundColumnStyles.CellStyle);
                if (attr.ForegroundValue != null) {
                    cellStyle.Setters.Add(new Setter(DataGridCell.ForegroundProperty, attr.ForegroundValue));
                }
                if (attr.BackgroundValue != null) {
                    cellStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, attr.BackgroundValue));
                }
                textColumn.CellStyle = cellStyle;
                var elementStyle = textColumn.GetCurrentStyle<TextBlock>();
                elementStyle.Setters.Add(new Setter(Control.HorizontalAlignmentProperty, attr.HorizontalAlignmentValue));
                textColumn.ElementStyle = elementStyle;
            } else if (checkBoxColumn != null) {
                var style = checkBoxColumn.GetCurrentStyle<CheckBox>();
                style.Setters.Add(new Setter(Control.VerticalAlignmentProperty, VerticalAlignment.Center));
                checkBoxColumn.ElementStyle = style;
            }
        }
    }
}
