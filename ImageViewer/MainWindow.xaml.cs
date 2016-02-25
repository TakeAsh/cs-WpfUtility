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
    }
}
