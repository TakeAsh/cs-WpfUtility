using System;
using System.Collections.Generic;
using System.Globalization;
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

    using _resources = Properties.Resources;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow {

        private static Properties.Settings _settings = Properties.Settings.Default;

        private MessageButton messageButton_HPC;

        public MainWindow() {
            CultureManager.SetCulture(_settings.Culture);
            InitializeComponent();

            // Insert code required on object creation below this point.
            messageButton_HPC = ribbon_Main.AddMessageButton("Infinity", 0);
            ribbon_Main.AddMinimizeButton(
                ResourceHelper.GetImage("Images/Show.png"),
                null, //ResourceHelper.GetImage("Images/Hide.png"),
                null, //Properties.Resources.MainWindow_button_Minimize_ToolTip_Show,
                Properties.Resources.MainWindow_button_Minimize_ToolTip_Hide
            );

            comboBox_Culture_GalleryCategory.ItemsSource = CultureManager.AvailableCultures;
            comboBox_Culture_Gallery.SelectedItem = CultureManager.GetCulture(_settings.Culture);
        }

        private void SetCulture() {
            _settings.Culture = (comboBox_Culture_Gallery.SelectedItem as CultureInfo).Name;
            _settings.Save();
            messageButton_QATB.Show(
                _resources.MainWindow_method_SetCulture_message_OK,
                MessageButton.Icons.Beep
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

        private void button_Culture_OK_Click(object sender, RoutedEventArgs e) {
            SetCulture();
        }
    }
}
