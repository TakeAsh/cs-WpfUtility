﻿using System;
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
using System.Windows.Threading;
using TakeAshUtility;
using WpfUtility;

namespace ImageViewer {

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow :
        Window,
        IMouseHWheelEvent {

        const double DefaultDpi = 96;
        const double MinDpi = DefaultDpi * 0.75;
        const double MaxDpi = DefaultDpi * 5;
        const double MaxZoom = 800;

        private static Properties.Settings _settings = Properties.Settings.Default;

        private static readonly Uri _failedImageUri = new Uri("/ImageViewer;component/Images/FailedL.png", UriKind.Relative);

        private WindowPlacement _placement;
        private List<ZoomItem> _zoomItems;
        private ZoomItem _zoom100;
        private double _monitorDpi = DefaultDpi;
        private double _monitorDpiRate = 1;
        private double _zoom = 0;
        private double _imageDpiX;
        private double _imageDpiY;
        private int _imageWidth;
        private int _imageHeight;
        private string _format;
        private Func<int, int, byte[]> _getPixel;
        private List<Image> _frames;

        public MainWindow() {
            InitializeComponent();

            this.AddMouseHWheelHook((sender, e) => {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + e.Delta / 2);
                e.Handled = true;
            });

            MonitorDpi = _settings.MonitorDpi;

            comboBox_Zoom.ItemsSource = _zoomItems = _settings.ZoomItems.ToZoomItems();
            comboBox_Zoom.SelectedIndex = 0;
            _zoom100 = _zoomItems.FirstOrDefault(item => item.Value == 100);

            comboBox_BitmapScalingMode.ItemsSource = Enum.GetValues(typeof(BitmapScalingMode))
                .OfType<BitmapScalingMode>()
                .Distinct();
            comboBox_BitmapScalingMode.SelectedItem = default(BitmapScalingMode);

            DataContext = this;
            ZoomResetmmand = new SimpleDelegateCommand(x => this.ZoomControl(x));
            ZoomInCommand = new SimpleDelegateCommand(x => this.ZoomControl("+"));
            ZoomOutCommand = new SimpleDelegateCommand(x => this.ZoomControl("-")) {
                GestureKey = Key.Subtract,
                GestureModifier = ModifierKeys.Control,
            };
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

        public SimpleDelegateCommand ZoomResetmmand { get; private set; }
        public SimpleDelegateCommand ZoomInCommand { get; private set; }
        public SimpleDelegateCommand ZoomOutCommand { get; private set; }

        private void LoadImage(string filename) {
            label_Notice.Text = "Processing...";
            label_Notice.Visibility = Visibility.Visible;
            messageButton_Info.Show(filename, MessageButton.Icons.Asterisk);
            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    BitmapSource bitmap = null;
                    try {
                        var decoder = BitmapDecoder.Create(
                            new Uri(filename, UriKind.Absolute),
                            BitmapCreateOptions.PreservePixelFormat,
                            BitmapCacheOption.OnLoad
                        );
                        bitmap = decoder.Frames.FirstOrDefault();
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
                        panel_Frames.Children.Clear();
                        _frames = decoder.Frames
                            .OfType<BitmapFrame>()
                            .Select(frame => new Image() {
                                Source = frame,
                                Visibility = Visibility.Collapsed,
                            }).ToList();
                        panel_Frames.Children.SafeAdd(_frames);
                        _frames[0].Visibility = Visibility.Visible;
                    } catch (Exception ex) {
                        messageButton_Info.Show(ex.GetAllMessages(), MessageButton.Icons.Hand);
                        bitmap = new BitmapImage(_failedImageUri);
                        _imageDpiX = _imageDpiY = DefaultDpi;
                        _imageWidth = _imageHeight = 32;
                        label_Info_Pixel.Text = _format = "-";
                        _getPixel = null;
                    }
                    image_Original.Source = bitmap;
                    image_Crop.Source = bitmap.Crop();
                    ApplyZoom();
                    label_Notice.Text = null;
                    label_Notice.Visibility = Visibility.Collapsed;
                    slider_Frame.Maximum = _frames == null ?
                        0 :
                        _frames.Count - 1;
                    slider_Frame.Value = 0;
                })
            );
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
            label_Info_XY.Text = new[] { pixelX, pixelY }.JoinToString(", ");
            label_Info_Pixel.Text = pixel.JoinToString(", ");
        }

        private void ZoomControl(object zoomString) {
            var zoom = zoomString as string;
            if (String.IsNullOrEmpty(zoom)) { return; }
            var newIndex = 0;
            switch (zoom) {
                case "0":
                    newIndex = 0;
                    break;
                case "+":
                    newIndex = comboBox_Zoom.SelectedIndex + 1;
                    break;
                case "-":
                    newIndex = comboBox_Zoom.SelectedIndex - 1;
                    break;
            }
            comboBox_Zoom.SelectedIndex = newIndex.Clamp(0, comboBox_Zoom.Items.Count - 1);
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
            if (_frames == null) {
                return;
            }
            _frames.ForEach(frame => RenderOptions.SetBitmapScalingMode(frame, bitmapScalingMode));
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

        private void image_Original_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                var delta = Math.Sign(e.Delta);
                comboBox_Zoom.SelectedIndex = (comboBox_Zoom.SelectedIndex + delta).Clamp(1, _zoomItems.Count - 1);
                e.Handled = true;
                return;
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + e.Delta / 2);
                e.Handled = true;
                return;
            }
        }

        private void image_Original_MouseUp(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton != MouseButton.Middle) {
                return;
            }
            if (comboBox_Zoom.SelectedIndex == 0) {
                comboBox_Zoom.SelectedItem = _zoom100;
            } else {
                comboBox_Zoom.SelectedIndex = 0;
            }
            e.Handled = true;
        }

        private void slider_Frame_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_frames == null || _frames.Count == 0) {
                return;
            }
            var index = (int)slider_Frame.Value;
            for (var i = 0; i < _frames.Count; ++i) {
                _frames[i].Visibility = i <= index ?
                    Visibility.Visible :
                    Visibility.Collapsed;
            }
        }

        private void Frame_MouseWheel(object sender, MouseWheelEventArgs e) {
            slider_Frame.Value = (slider_Frame.Value - Math.Sign(e.Delta) + _frames.Count) % _frames.Count;
        }

#pragma warning disable 0067

        #region IMouseHWheelEvent

        public event MouseWheelEventHandler MouseHWheel;

        #endregion

#pragma warning restore 0067

    }
}
