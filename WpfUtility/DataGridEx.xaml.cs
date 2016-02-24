﻿using System;
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

        static DataGridEx() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DataGridEx),
                new FrameworkPropertyMetadata(typeof(DataGridEx))
            );
        }

        public DataGridEx()
            : base() {

            this.AddPropertyChanged(
                ItemsControl.ItemsSourceProperty,
                (sender, e) => {
                    var dataGridEx = sender as DataGridEx;
                    if (dataGridEx == null || dataGridEx.ItemsSource == null) {
                        return;
                    }
                    dataGridEx.ColumnHeaderStyle = new Style();
                    _collectionView = CollectionViewSource.GetDefaultView(dataGridEx.ItemsSource) as ListCollectionView;
                    ((INotifyCollectionChanged)_collectionView).CollectionChanged += OnUpdateItems;
                    _collectionView.Filter = PredicateAutoFilter;
                }
            );
            AutoGeneratingColumn += OnAutoGeneratingColumn;
        }

        public DataGridEx(DataGridEx source)
            : this() {
            this.Duplicate(source);
            this.ItemsSource = source.ItemsSource;
        }

        private List<string> _columnNames;
        private Dictionary<string, DataGridExAutoFilterItem> _autoFilterItems;
        private CollectionView _collectionView;

        public Type DataType { get; set; }

        public DataGridEx Clone() {
            return new DataGridEx(this);
        }

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

        private void OnUpdateItems(object sender, NotifyCollectionChangedEventArgs e) {
            var view = sender as ListCollectionView;
            if (view == null || view.ItemProperties == null) {
                return;
            }
            switch (e.Action) {
                case NotifyCollectionChangedAction.Reset:
                    if (_columnNames == null) {
                        _columnNames = view.ItemProperties
                            .Select(info => info.Name).ToList();
                        _autoFilterItems = _columnNames.Select(name => new DataGridExAutoFilterItem(name, this))
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
            if (dataGridEx == null || _columnNames == null) {
                return;
            }
            if (!_columnNames.Contains(e.PropertyName)) {
                _columnNames.Add(e.PropertyName);
                _autoFilterItems[e.PropertyName] = new DataGridExAutoFilterItem(e.PropertyName, dataGridEx);
            }
            var attr = _autoFilterItems[e.PropertyName].DataGridExAttr;
            e.Cancel = attr.Ignore;
            e.Column.Header = attr.Header;
            if (!String.IsNullOrEmpty(attr.ClipboardContentBinding)) {
                e.Column.ClipboardContentBinding = new Binding(attr.ClipboardContentBinding);
            }
            var textColumn = e.Column as DataGridTextColumn;
            var checkBoxColumn = e.Column as DataGridCheckBoxColumn;
            if (textColumn != null) {
                textColumn.Binding.StringFormat = attr.StringFormat;
                var style = textColumn.getCurrentStyle(typeof(DataGridCell));
                if (attr.ForegroundValue != null) {
                    style.Setters.Add(new Setter(DataGridCell.ForegroundProperty, attr.ForegroundValue));
                }
                if (attr.BackgroundValue != null) {
                    style.Setters.Add(new Setter(DataGridCell.BackgroundProperty, attr.BackgroundValue));
                }
                textColumn.CellStyle = style;
            } else if (checkBoxColumn != null) {
                var style = checkBoxColumn.getCurrentStyle(typeof(CheckBox));
                style.Setters.Add(new Setter(Control.VerticalAlignmentProperty, VerticalAlignment.Center));
                checkBoxColumn.ElementStyle = style;
            }
        }
    }
}
