using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfUtility {

    /// <summary>
    /// TextPrompt.xaml の相互作用ロジック
    /// </summary>
    public partial class TextPrompt :
        Window {

        public TextPrompt() {
            InitializeComponent();
        }

        public string Message {
            get { return textBlock_Message.Text; }
            set { textBlock_Message.Text = value; }
        }

        public string InputText {
            get { return textBox_Input.Text; }
            set { textBox_Input.Text = value; }
        }

        public bool AcceptsReturn {
            get { return textBox_Input.AcceptsReturn; }
            set { textBox_Input.AcceptsReturn = value; }
        }

        public bool AcceptsTab {
            get { return textBox_Input.AcceptsTab; }
            set { textBox_Input.AcceptsTab = value; }
        }

        private void Window_Activated(object sender, EventArgs e) {
            DialogResult = null;
        }

        private void button_OK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}
