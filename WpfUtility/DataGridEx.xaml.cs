﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

    public class DataGridEx : DataGrid {

        static DataGridEx() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DataGridEx),
                new FrameworkPropertyMetadata(typeof(DataGridEx))
            );
        }

        public DataGridEx()
            : base() {

            _columnNames = new List<string>();
            _autoFilterItems = new Dictionary<string, AutoFilterItem>();
            var collectionView = CollectionViewSource.GetDefaultView(this.Items) as CollectionView;
            ((INotifyCollectionChanged)collectionView).CollectionChanged += OnUpdateItems;
            collectionView.Filter = PredicateAutoFilter;
            ColumnHeaderStyle = new Style();
            AutoGeneratedColumns += OnAutoGeneratedColumns;
        }

        private List<string> _columnNames;
        private Dictionary<string, AutoFilterItem> _autoFilterItems;

        public void UpdateAutoFilter() {
            if (Items == null || ItemsSource == null) {
                return;
            }
            if (_columnNames.Count == 0) {
                foreach (var item in Items) {
                    var pis = item.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(pi => pi.GetIndexParameters().Length == 0)
                        .ToList();
                    if (pis == null || pis.Count == 0) {
                        continue;
                    }
                    pis.ForEach(pi => {
                        if (!_autoFilterItems.ContainsKey(pi.Name)) {
                            _autoFilterItems[pi.Name] = new AutoFilterItem(pi.Name, this);
                        }
                        var value = pi.GetValue(item, null).SafeToString(null);
                        if (value != null &&
                            !_autoFilterItems[pi.Name].Values.ContainsKey(value)) {
                            _autoFilterItems[pi.Name].Values[value] = true;
                        }
                    });
                }
            } else {
                _columnNames.ForEach(column => _autoFilterItems[column].Update());
            }
        }

        private bool PredicateAutoFilter(object item) {
            if (_autoFilterItems == null) {
                return true;
            }
            foreach (var p in _autoFilterItems.Keys) {
                foreach (var kvp in _autoFilterItems[p].Values) {
                    if (kvp.Value == true) {
                        continue;
                    }
                    var pi = item.GetType().GetProperty(p, BindingFlags.Instance | BindingFlags.Public);
                    var value = pi.GetValue(item, null).SafeToString();
                    if (value != null && value == kvp.Key) {
                        return false;
                    }
                }
            }
            return true;
        }

        private void OnUpdateItems(object sender, NotifyCollectionChangedEventArgs e) {
            UpdateAutoFilter();
        }

        private void OnAutoGeneratedColumns(object sender, EventArgs e) {
            if (ColumnHeaderStyle.Triggers.IsSealed) {
                return;
            }
            this.Columns
                .ToList()
                .ForEach(column => {
                    var name = column.Header.ToString();
                    _columnNames.Add(name);
                    _autoFilterItems[name] = new AutoFilterItem(name, this);
                });
        }

        private class AutoFilterItem {

            public AutoFilterItem(string name, DataGrid dataGrid) {
                Name = name;
                this.DataGrid = dataGrid;
                Values = new Dictionary<string, bool>();
                Menu = new ContextMenu();
                Menu.Closed += (s, args) => {
                    var source = dataGrid.ItemsSource;
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = source;
                };
                Menu.Items.Add<UIElement>(InitialMenuItems);
                Menu.Items.Add<UIElement>(Values
                    .OrderBy(item => item.Key)
                    .Select(item => CreateValueSelectCheckBox(item.Key))
                );
                this.DataGrid.ColumnHeaderStyle.Triggers.Add(HeaderTrigger);
            }

            public string Name { get; set; }
            public DataGrid DataGrid { get; set; }
            public ContextMenu Menu { get; set; }
            public Dictionary<string, bool> Values { get; set; }

            public Trigger HeaderTrigger {
                get {
                    return new Trigger() {
                        Property = DataGridColumnHeader.ContentProperty,
                        Value = Name,
                        Setters = { new Setter(DataGridColumnHeader.ContextMenuProperty, Menu), },
                    };
                }
            }

            private List<UIElement> InitialMenuItems {
                get {
                    var checkBoxAll = new CheckBox() {
                        Content = "All",
                        IsChecked = Values.Aggregate(true, (current, kvp) => current && kvp.Value),
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
                    return new List<UIElement>() {
                        checkBoxAll,
                        new Separator(),
                    };
                }
            }

            private CheckBox CreateValueSelectCheckBox(string item) {
                var checkBox = new CheckBox() {
                    Content = item,
                    IsChecked = Values[item],
                };
                checkBox.Checked += (sender, e) => {
                    Values[item] = true;
                };
                checkBox.Unchecked += (sender, e) => {
                    Values[item] = false;
                    var checkBoxAll = Menu.Items[0] as CheckBox;
                    checkBoxAll.IsChecked = false;
                };
                return checkBox;
            }

            public void Update() {
                foreach (var item in this.DataGrid.Items) {
                    var pi = item.GetType()
                        .GetProperty(Name, BindingFlags.Instance | BindingFlags.Public);
                    if (pi == null) {
                        continue;
                    }
                    var value = pi.GetValue(item, null).SafeToString(null);
                    if (value != null &&
                        !Values.ContainsKey(value)) {
                        Values[value] = true;
                    }
                }
                Menu.Items.Clear();
                Menu.Items.Add<UIElement>(InitialMenuItems);
                Menu.Items.Add<UIElement>(Values
                    .Keys
                    .OrderBy(item => item)
                    .Select(item => CreateValueSelectCheckBox(item))
                );
            }
        }
    }
}
