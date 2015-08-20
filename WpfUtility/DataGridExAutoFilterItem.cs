using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TakeAshUtility;

namespace WpfUtility {

    using _resources = Properties.Resources;

    internal class DataGridExAutoFilterItem {

        public DataGridExAutoFilterItem(string name, DataGridEx dataGridEx) {
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
