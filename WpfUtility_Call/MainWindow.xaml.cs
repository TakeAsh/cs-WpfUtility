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
using Microsoft.Windows.Controls.Ribbon;
using WpfUtility;

namespace WpfUtility_Call {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow {

        private MessageButton messageButton_HPC;

        public MainWindow() {
            InitializeComponent();

            // Insert code required on object creation below this point.
            messageButton_HPC = ribbon_Main.AddMessageButton("Infinity", 0);
            ribbon_Main.AddMinimizeButton(
                ResourceHelper.GetImage("Images/Show.png"),
                null, //ResourceHelper.GetImage("Images/Hide.png"),
                null, //Properties.Resources.MainWindow_button_Minimize_ToolTip_Show,
                Properties.Resources.MainWindow_button_Minimize_ToolTip_Hide
            );
        }

        private void button_MessageButton_Click(object sender, RoutedEventArgs e) {
            var src = sender as RibbonButton;
            MessageButton target;
            switch (src.Name) {
                default:
                case "button_RG":
                    target = messageButton_RG;
                    break;
                case "button_QATB":
                    target = messageButton_QATB;
                    break;
                case "button_HPC":
                    target = messageButton_HPC;
                    break;
            }
            if (String.IsNullOrEmpty(target.Text)) {
                var icon = (MessageButton.Icons)(((int)target.Icon + 1) % Enum.GetValues(typeof(MessageButton.Icons)).Length);
                target.Show(DateTime.Now.ToString("g"), icon);
            } else {
                target.Text = null;
            }
        }
    }
}
