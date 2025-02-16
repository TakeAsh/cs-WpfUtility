using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WpfUtility {

    /// <summary>
    /// Draw EditableTextBox.Background with WarnBrush, if EditableTextBox is empty or null.
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/36177619/">c# - Change the background color of a WPF editable ComboBox programmatically - Stack Overflow</see>
    /// <see cref="https://stackoverflow.com/questions/70958379/">xaml - Change background colour of a ComboBox in WPF without pasting entire template? - Stack Overflow</see>
    public class WarnIfEmptyComboBox : ComboBox {

        public Brush WarnBrush { get; set; } = Brushes.LightPink;

        public WarnIfEmptyComboBox() : base() {
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            var comboBox = sender as ComboBox;
            if (comboBox == null) { return; }
            if (comboBox.IsEditable) {
                var textbox = comboBox?.Template?.FindName("PART_EditableTextBox", comboBox) as TextBox;
                if (textbox == null) { return; }
                textbox.TextChanged += OnEditableTextChanged;
                OnEditableTextChanged(textbox);
            } else {
                comboBox.SelectionChanged += OnSelectionChanged;
            }
        }

        private void OnEditableTextChanged(object sender, TextChangedEventArgs e = null) {
            var textbox = sender as TextBox;
            var border = textbox?.Parent as Border;
            if (textbox == null || border == null) { return; }
            border.Background = String.IsNullOrEmpty(textbox.Text)
                ? WarnBrush
                : null;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e = null) {
            var comboBox = sender as ComboBox;
            if (comboBox == null) { return; }
            ComboBoxItem comboBoxItem = null;
            object content = null;
            var isEmpty = comboBox.SelectedValue == null
                || (comboBoxItem = comboBox.SelectedValue as ComboBoxItem) == null
                || (content = comboBoxItem.Content) == null
                || String.IsNullOrEmpty(content.ToString())
                || String.IsNullOrEmpty(content as string);
            var root = comboBox.Template?.FindName("templateRoot", comboBox) as Grid;
            if (root == null) { return; }
            root.Background = isEmpty ? WarnBrush : null;   // not work now
        }
    }
}
