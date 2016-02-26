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
using TakeAshUtility;
using WpfUtility;

namespace ImageViewer {

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow :
        Window {

        private static readonly Uri _failedImageUri = new Uri("/ImageViewer;component/Images/FailedL.png", UriKind.Relative);

        public MainWindow() {
            InitializeComponent();
            comboBox_BitmapScalingMode.ItemsSource = Enum.GetValues(typeof(BitmapScalingMode))
                .OfType<BitmapScalingMode>()
                .Distinct();
            comboBox_BitmapScalingMode.SelectedItem = default(BitmapScalingMode);
        }

        private void LoadImage(string filename) {
            messageButton_Info.Show(filename, MessageButton.Icons.Asterisk);
            BitmapSource bitmap = null;
            try {
                bitmap = BitmapFrame.Create(
                   new Uri(filename, UriKind.Absolute),
                   BitmapCreateOptions.None,
                   BitmapCacheOption.OnLoad
               );
            }
            catch (Exception ex) {
                messageButton_Info.Show(ex.GetAllMessages(), MessageButton.Icons.Hand);
                bitmap = new BitmapImage(_failedImageUri);
            }
            image_Original.Source = bitmap;
            image_Crop.Source = bitmap.Crop();
        }

        private void Window_Drop(object sender, DragEventArgs e) {
            e.Handled = true;
            var files = (e.Data.GetData(DataFormats.FileDrop) as string[]).FirstOrDefault();
            if (files != null) {
                LoadImage(files);
                return;
            }
        }

        private void comboBox_BitmapScalingMode_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (comboBox_BitmapScalingMode.SelectedItem == null) {
                return;
            }
            var bitmapScalingMode = (BitmapScalingMode)comboBox_BitmapScalingMode.SelectedItem;
            RenderOptions.SetBitmapScalingMode(image_Original, bitmapScalingMode);
            RenderOptions.SetBitmapScalingMode(image_Crop, bitmapScalingMode);
        }
    }
}
