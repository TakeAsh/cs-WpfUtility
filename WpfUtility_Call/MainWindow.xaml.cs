using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TakeAsh;
using WpfUtility;

namespace WpfUtility_Call {

    using NewLineCodesHelper = EnumHelper<MainWindow.NewLineCodes>;
    using _resources = Properties.Resources;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow {

        private static Properties.Settings _settings = Properties.Settings.Default;

        private MessageButton messageButton_HPC;

        [TypeConverter(typeof(EnumTypeConverter<NewLineCodes>))]
        public enum NewLineCodes {
            [ExtraProperties("Entity:'\n', Escaped:'\\x22\\u0027\t'")]
            Lf = 1,

            [ExtraProperties("Entity : \"\r\"Escaped : '\\x0027\\x0022'")]
            [Description("[A] Mac(CR)")]
            Cr = 2,

            [ExtraProperties("Entity:\t'\r\n';;;Escaped:\t\"\\x3042\"")] // U+3042 あ
            [Description("[A] Windows(CR+LF)")]
            CrLf = 4,

            [ExtraProperties("Entity:\n\t'\n\r'\nEscaped:\n\t'\\uD842\\uDFB7'")] // U+00020BB7 𠮷
            LfCr = 8,
        }

        public MainWindow() {
            CultureManager.SetCulture(_settings.Culture);
            InitializeComponent();
            ribbonComboBox_comboBox3_GalleryCategory.ItemsSource = NewLineCodesHelper.ValueDescriptionPairs;
            ribbonComboBox_comboBox4_GalleryCategory.ItemsSource = NewLineCodesHelper.ValueDescriptionPairs;

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
                var now = DateTime.Now;
                if (now.Second % 2 == 0) {
                    target.Show(now.ToString("g"), icon);
                } else {
                    target.Show(
                        new[] { now.ToLongDateString(), now.DayOfWeek.ToString(), now.ToLongTimeString(), },
                        icon
                    );
                }
            } else {
                target.Text = null;
            }
        }

        private void button_Culture_OK_Click(object sender, RoutedEventArgs e) {
            SetCulture();
        }

        private void button_ChangeTabStripPlacement_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            if (button == null) {
                return;
            }
            switch (button.Name) {
                case "button_Top":
                    paneledTab.TabStripPlacement = Dock.Top;
                    normalTab.TabStripPlacement = Dock.Top;
                    break;
                case "button_Left":
                    paneledTab.TabStripPlacement = Dock.Left;
                    normalTab.TabStripPlacement = Dock.Left;
                    break;
                case "button_Right":
                    paneledTab.TabStripPlacement = Dock.Right;
                    normalTab.TabStripPlacement = Dock.Right;
                    break;
                case "button_Bottom":
                    paneledTab.TabStripPlacement = Dock.Bottom;
                    normalTab.TabStripPlacement = Dock.Bottom;
                    break;
            }
        }
    }
}
