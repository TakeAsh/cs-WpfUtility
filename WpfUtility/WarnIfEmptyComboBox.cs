using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using TakeAshUtility;

namespace WpfUtility {

    /// <summary>
    /// Draw EditableTextBox.Background with WarnBrush, if EditableTextBox is empty or null.
    /// </summary>
    /// <see cref="https://stackoverflow.com/questions/36177619/">c# - Change the background color of a WPF editable ComboBox programmatically - Stack Overflow</see>
    /// <see cref="https://stackoverflow.com/questions/70958379/">xaml - Change background colour of a ComboBox in WPF without pasting entire template? - Stack Overflow</see>
    /// <see cref="http://tawamuredays.blog.fc2.com/blog-entry-280.html">ComboBoxの背景色が変わらない - C#等と戯れる日々</see>
    public class WarnIfEmptyComboBox : ComboBox {

        public Brush WarnBrush { get; set; } = Brushes.LightPink;
        private Border border = null;
        private Brush originalBackground = null;

        public WarnIfEmptyComboBox() : base() {
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            var comboBox = sender as ComboBox;
            if (comboBox == null) { return; }
            if (comboBox.IsEditable) {
                var textbox = comboBox.Template?.FindName("PART_EditableTextBox", comboBox) as TextBox;
                border = textbox?.Parent as Border;
                if (textbox == null || border == null) { return; }
                originalBackground = border.Background;
                textbox.TextChanged += OnEditableTextChanged;
                OnEditableTextChanged(textbox);
            } else {
                var toggleButton = comboBox.Template.FindName("toggleButton", comboBox) as ToggleButton;
                border = toggleButton?.Template.FindName("templateRoot", toggleButton) as Border;
                if (toggleButton == null || border == null) { return; }
                originalBackground = border.Background;
                comboBox.SelectionChanged += OnSelectionChanged;
                OnSelectionChanged(comboBox);
            }
        }

        private void OnEditableTextChanged(object sender, TextChangedEventArgs e = null) {
            var textbox = sender as TextBox;
            if (textbox == null) { return; }
            border.Background = String.IsNullOrEmpty(textbox.Text)
                ? WarnBrush
                : originalBackground;
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
            border.Background = isEmpty ? WarnBrush : originalBackground;
        }
    }
}
