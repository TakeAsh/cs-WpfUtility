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

    public class DataGridEx : DataGrid {

        private enum FilterItemsActions {
            Add,
            Remove,
        }

        static DataGridEx() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DataGridEx),
                new FrameworkPropertyMetadata(typeof(DataGridEx))
            );
        }

        public DataGridEx()
            : base() {

            DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(DataGridEx))
                .AddValueChanged(this, (sender, e) => {
                    var dataGridEx = sender as DataGridEx;
                    dataGridEx.ColumnHeaderStyle = new Style();
                    _collectionView = CollectionViewSource.GetDefaultView(dataGridEx.ItemsSource) as ListCollectionView;
                    ((INotifyCollectionChanged)_collectionView).CollectionChanged += OnUpdateItems;
                    _collectionView.Filter = PredicateAutoFilter;
                });
            AutoGeneratingColumn += OnAutoGeneratingColumn;
        }

        private List<string> _columnNames;
        private Dictionary<string, AutoFilterItem> _autoFilterItems;
        private CollectionView _collectionView;

        public Type DataType { get; set; }

        public void Refresh() {
            _collectionView.Refresh();
        }

        private bool PredicateAutoFilter(object item) {
            if (_autoFilterItems == null) {
                return true;
            }
            foreach (var p in _columnNames) {
                foreach (var kvp in _autoFilterItems[p].Values) {
                    if (kvp.Value.IsChecked == true) {
                        continue;
                    }
                    var pi = (DataType ?? item.GetType())
                        .GetProperty(p, BindingFlags.Instance | BindingFlags.Public);
                    var value = pi.GetValue(item, null).SafeToString();
                    if (value != null && value == kvp.Key) {
                        return false;
                    }
                }
            }
            return true;
        }

        private void ModifyFilterItems(FilterItemsActions action, IEnumerable items) {
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

        private void OnUpdateItems(object sender, NotifyCollectionChangedEventArgs e) {
            var view = sender as ListCollectionView;
            if (view == null) {
                return;
            }
            switch (e.Action) {
                case NotifyCollectionChangedAction.Reset:
                    if (_columnNames == null) {
                        _columnNames = view.ItemProperties
                            .Select(info => info.Name).ToList();
                        _autoFilterItems = _columnNames.Select(name => new AutoFilterItem(name, this))
                            .ToDictionary(filterItem => filterItem.Name);
                        if (view.Count > 0) {
                            ModifyFilterItems(FilterItemsActions.Add, view);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    ModifyFilterItems(FilterItemsActions.Add, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ModifyFilterItems(FilterItemsActions.Remove, e.OldItems);
                    break;
            }
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
            var dataGridEx = sender as DataGridEx;
            if (dataGridEx == null) {
                return;
            }
            if (!_columnNames.Contains(e.PropertyName)) {
                _columnNames.Add(e.PropertyName);
                _autoFilterItems[e.PropertyName] = new AutoFilterItem(e.PropertyName, dataGridEx);
            }
            e.Cancel = _autoFilterItems[e.PropertyName].DataGridExAttr.Ignore;
            e.Column.Header = _autoFilterItems[e.PropertyName].DataGridExAttr.Header;
        }

        private class AutoFilterItem {

            public AutoFilterItem(string name, DataGridEx dataGridEx) {
                Name = name;
                Parent = dataGridEx;
                var dataType = dataGridEx.DataType ??
                    (dataGridEx.Items.Count > 0 ?
                        dataGridEx.Items[0].GetType() :
                        null);
                DataGridExAttr = dataType.GetAttribute<DataGridExAttribute>(name) ?? new DataGridExAttribute();
                if (String.IsNullOrEmpty(DataGridExAttr.Header)) {
                    DataGridExAttr.Header = dataType.ToDescription(name) ?? name;
                }
                Values = new Dictionary<string, DataGridExAutoFilterSubItem>();
                Menu = new ContextMenu();
                Menu.Closed += (s, args) => {
                    UpdateToolTip();
                    Parent.Refresh();
                };
                Menu.Items.Add<UIElement>(InitialMenuItems);
                ToolTipText = new TextBlock() {
                    Text = _resources.DataGridEx_ColumnHeader_All,
                };
                Parent.ColumnHeaderStyle.Triggers.Add(new Trigger() {
                    Property = DataGridColumnHeader.ContentProperty,
                    Value = DataGridExAttr.Header,
                    Setters = {
                        new Setter(DataGridColumnHeader.ContextMenuProperty, Menu),
                        new Setter(DataGridColumnHeader.ToolTipProperty, ToolTipText),
                    },
                });
            }

            public string Name { get; private set; }
            public DataGridEx Parent { get; private set; }
            public DataGridExAttribute DataGridExAttr { get; private set; }
            public ContextMenu Menu { get; private set; }
            public Dictionary<string, DataGridExAutoFilterSubItem> Values { get; private set; }
            public TextBlock ToolTipText { get; private set; }

            private List<UIElement> InitialMenuItems {
                get {
                    var checkBoxAll = new CheckBox() {
                        Content = _resources.DataGridEx_ColumnHeader_All,
                        IsChecked = Values.Aggregate(true, (current, kvp) => current && kvp.Value.IsChecked),
                    };
                    checkBoxAll.Checked += (sender, e) => {
                        var checkBox0 = sender as CheckBox;
                        Menu.Items
                            .OfType<CheckBox>()
                            .Where(checkBox => checkBox != checkBox0)
                            .ToList()
                            .ForEach(checkBox => {
                                checkBox.IsChecked = true;
                            });
                    };
                    checkBoxAll.Unchecked += (sender, e) => {
                        var checkBox0 = sender as CheckBox;
                        Menu.Items
                            .OfType<CheckBox>()
                            .Where(checkBox => checkBox != checkBox0)
                            .ToList()
                            .ForEach(checkBox => {
                                checkBox.IsChecked = false;
                            });
                    };
                    return new List<UIElement>() {
                        checkBoxAll,
                        new Separator(),
                    };
                }
            }

            private CheckBox CreateValueSelectCheckBox(string item) {
                var checkBox = new CheckBox() {
                    Content = item,
                    IsChecked = Values[item].IsChecked,
                };
                checkBox.Checked += (sender, e) => {
                    Values[item].IsChecked = true;
                };
                checkBox.Unchecked += (sender, e) => {
                    Values[item].IsChecked = false;
                };
                return checkBox;
            }

            public void AddValue(string value) {
                if (value == null) {
                    return;
                }
                if (!Values.ContainsKey(value)) {
                    Values[value] = new DataGridExAutoFilterSubItem();
                } else {
                    ++Values[value].Count;
                }
            }

            public void RemoveValue(string value) {
                if (value == null || !Values.ContainsKey(value)) {
                    return;
                }
                if (--Values[value].Count <= 0) {
                    Values.Remove(value);
                }
            }

            public void Update() {
                Menu.Items.Clear();
                Menu.Items.Add<UIElement>(InitialMenuItems);
                Menu.Items.Add<UIElement>(Values
                    .Keys
                    .OrderBy(item => item)
                    .Select(item => CreateValueSelectCheckBox(item))
                );
                UpdateToolTip();
            }

            private void UpdateToolTip() {
                ToolTipText.Text = Values.Keys.Aggregate(true, (current, key) => current && Values[key].IsChecked) == true ?
                    _resources.DataGridEx_ColumnHeader_All :
                    String.Join("\n", Values.Keys.Where(key => Values[key].IsChecked).OrderBy(key => key));
            }
        }
    }
}
