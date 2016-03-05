using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        const double DefaultDpi = 96;
        const double MinDpi = DefaultDpi * 0.75;
        const double MaxDpi = DefaultDpi * 5;
        const double MaxZoom = 800;

        private static Properties.Settings _settings = Properties.Settings.Default;

        private static readonly Uri _failedImageUri = new Uri("/ImageViewer;component/Images/FailedL.png", UriKind.Relative);

        private WindowPlacement _placement;
        private double _monitorDpi = DefaultDpi;
        private double _monitorDpiRate = 1;
        private double _zoom = 0;
        private double _imageDpiX;
        private double _imageDpiY;
        private int _imageWidth;
        private int _imageHeight;
        private string _format;
        private Func<int, int, byte[]> _getPixel;

        public MainWindow() {
            InitializeComponent();

            MonitorDpi = _settings.MonitorDpi;

            comboBox_Zoom.ItemsSource = _settings.ZoomItems.ToZoomItems();
            comboBox_Zoom.SelectedIndex = 0;

            comboBox_BitmapScalingMode.ItemsSource = Enum.GetValues(typeof(BitmapScalingMode))
                .OfType<BitmapScalingMode>()
                .Distinct();
            comboBox_BitmapScalingMode.SelectedItem = default(BitmapScalingMode);
        }

        public double MonitorDpi {
            get { return _monitorDpi; }
            set {
                _monitorDpi = value.Clamp(MinDpi, MaxDpi);
                _monitorDpiRate = _monitorDpi / DefaultDpi;
                ApplyZoom();
            }
        }

        public double Zoom {
            get { return _zoom; }
            set {
                _zoom = value.Clamp(0, MaxZoom);
                ApplyZoom();
            }
        }

        private void LoadImage(string filename) {
            label_Notice.Visibility = Visibility.Collapsed;
            messageButton_Info.Show(filename, MessageButton.Icons.Asterisk);
            BitmapSource bitmap = null;
            try {
                bitmap = BitmapFrame.Create(
                   new Uri(filename, UriKind.Absolute),
                   BitmapCreateOptions.PreservePixelFormat,
                   BitmapCacheOption.OnLoad
               );
                _imageDpiX = bitmap.DpiX > 0 ?
                    bitmap.DpiX :
                    DefaultDpi;
                _imageDpiY = bitmap.DpiY > 0 ?
                    bitmap.DpiY :
                    DefaultDpi;
                _imageWidth = bitmap.PixelWidth;
                _imageHeight = bitmap.PixelHeight;
                label_Info_Pixel.Text = _format = bitmap.Format.ToString();
                _getPixel = bitmap.GetGetPixel();
            }
            catch (Exception ex) {
                messageButton_Info.Show(ex.GetAllMessages(), MessageButton.Icons.Hand);
                bitmap = new BitmapImage(_failedImageUri);
                _imageDpiX = _imageDpiY = DefaultDpi;
                _imageWidth = _imageHeight = 0;
                label_Info_Pixel.Text = _format = "-";
                _getPixel = null;
            }
            image_Original.Source = bitmap;
            image_Crop.Source = bitmap.Crop();
            ApplyZoom();
        }

        private void ApplyZoom() {
            if (_zoom <= 0 && _imageWidth != 0 && _imageHeight != 0) {
                var zw = scrollViewer.ActualWidth != 0 ?
                    scrollViewer.ActualWidth * 100 / _imageWidth :
                    100;
                var zh = scrollViewer.ActualHeight != 0 ?
                    scrollViewer.ActualHeight * 100 / _imageHeight :
                    100;
                var zoom = Math.Min(zw, zh);
                image_Original.Width = _imageWidth * zoom / 100.0;
                image_Original.Height = _imageHeight * zoom / 100.0;
            } else {
                image_Original.Width = _imageWidth * DefaultDpi * _monitorDpiRate * _zoom / (_imageDpiX * 100.0);
                image_Original.Height = _imageHeight * DefaultDpi * _monitorDpiRate * _zoom / (_imageDpiY * 100.0);
            }
        }

        private void UpdateInfo(double x, double y) {
            label_Info_XY.Text = "XY";
            label_Info_Pixel.Text = _format;
            if (x < 0 || y < 0 || _getPixel == null) {
                return;
            }
            var pixelX = (int)(_imageWidth * x / image_Original.ActualWidth);
            var pixelY = (int)(_imageHeight * y / image_Original.ActualHeight);
            var pixel = _getPixel(pixelX, pixelY);
            if (pixel == null) {
                return;
            }
            label_Info_XY.Text = String.Join(", ", new[] { pixelX, pixelY });
            label_Info_Pixel.Text = String.Join(", ", pixel);
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            _placement = new WindowPlacement(this) {
                Placement = _settings.WindowPlacement,
            };
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            if (!e.Cancel) {
                _settings.WindowPlacement = _placement.Placement;
                _settings.Save();
            }
        }

        private void Window_Drop(object sender, DragEventArgs e) {
            e.Handled = true;
            var files = (e.Data.GetData(DataFormats.FileDrop) as string[]).FirstOrDefault();
            if (files != null) {
                LoadImage(files);
                return;
            }
        }

        private void comboBox_Zoom_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (comboBox_Zoom.SelectedItem == null) {
                return;
            }
            Zoom = ((ZoomItem)comboBox_Zoom.SelectedItem).Value;
        }

        private void comboBox_BitmapScalingMode_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (comboBox_BitmapScalingMode.SelectedItem == null) {
                return;
            }
            var bitmapScalingMode = (BitmapScalingMode)comboBox_BitmapScalingMode.SelectedItem;
            RenderOptions.SetBitmapScalingMode(image_Original, bitmapScalingMode);
            RenderOptions.SetBitmapScalingMode(image_Crop, bitmapScalingMode);
        }

        private void button_Config_Click(object sender, RoutedEventArgs e) {
            var prompt = new TextPrompt() {
                Title = "Enter Monitor DPI",
                Message = "Min:" + MinDpi + " - Max:" + MaxDpi,
                InputText = _settings.MonitorDpi.ToString(),
            };
            if (prompt.ShowDialog() != true) {
                return;
            }
            MonitorDpi = prompt.InputText.TryParse<double>(DefaultDpi);
            _settings.MonitorDpi = MonitorDpi;
            messageButton_Info.Show(
                "Monitor DPI updated: " + _settings.MonitorDpi,
                MessageButton.Icons.Asterisk
            );
        }

        private void image_Original_MouseEnter(object sender, MouseEventArgs e) {
            var image = sender as Image;
            if (image == null) {
                return;
            }
            var position = e.GetPosition(image);
            UpdateInfo(position.X, position.Y);
        }

        private void image_Original_MouseMove(object sender, MouseEventArgs e) {
            var image = sender as Image;
            if (image == null) {
                return;
            }
            var position = e.GetPosition(image);
            UpdateInfo(position.X, position.Y);
        }

        private void image_Original_MouseLeave(object sender, MouseEventArgs e) {
            var image = sender as Image;
            if (image == null) {
                return;
            }
            UpdateInfo(-1, -1);
        }
    }
}
