using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Windows.Controls.Ribbon;
using TakeAshUtility;
using UI.Extensions.Wpf.Controls;

namespace WpfUtility {

    /// <summary>
    /// Alternative "MessageBox"
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>It can be pinned on window as an icon.</item>
    /// <item>It show a text as popup with system sound.</item>
    /// <item>A text is closed automatically after several seconds, or opened infinitely.</item>
    /// <item>The popup can be opened/closed by click.</item>
    /// <item>MessageButton is not modal but modeless.</item>
    /// <item>MessageButton DON'T support buttons(OK, Cancel, Yes, No, ...).</item>
    /// <item>MessageButtons in Quick Access ToolBar, Help Pane Content, Ribbon Group.<br />
    /// ![Sample](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/SS/MessageButton.png)</item>
    /// </list>
    /// </remarks>
    public class MessageButton :
        RibbonToggleButton {

        public const int DefaultAutoPopDelay = 5000;

        /// <summary>
        /// Values that specifies button size
        /// </summary>
        public enum ButtonSizes {
            /// <summary>
            /// 32 x 32
            /// </summary>
            Large,
            /// <summary>
            /// 16 x 16
            /// </summary>
            Small,
        }

        /// <summary>
        /// Values that specifies which icon and sound.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>[SystemSounds Class (System.Media)](https://msdn.microsoft.com/en-us/library/system.media.systemsounds.aspx)</item>
        /// </list>
        /// </remarks>
        public enum Icons {
            /// <remarks>
            /// ![BeepLarge](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/BeepLarge.png "BeepLarge")
            /// ![BeepSmall](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/BeepSmall.png "BeepSmall")
            /// </remarks>
            Beep,
            /// <remarks>
            /// ![AsteriskLarge](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/AsteriskLarge.png "AsteriskLarge")
            /// ![AsteriskSmall](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/AsteriskSmall.png "AsteriskSmall")
            /// </remarks>
            Asterisk,
            /// <remarks>
            /// ![QuestionLarge](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/QuestionLarge.png "QuestionLarge")
            /// ![QuestionSmall](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/QuestionSmall.png "QuestionSmall")
            /// </remarks>
            Question,
            /// <remarks>
            /// ![ExclamationLarge](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/ExclamationLarge.png "ExclamationLarge")
            /// ![ExclamationSmall](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/ExclamationSmall.png "ExclamationSmall")
            /// </remarks>
            Exclamation,
            /// <remarks>
            /// ![HandLarge](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/HandLarge.png "HandLarge")
            /// ![HandSmall](https://raw.githubusercontent.com/TakeAsh/cs-WpfUtility/master/WpfUtility/Images/HandSmall.png "HandSmall")
            /// </remarks>
            Hand,
        }

        private static readonly Dictionary<Icons, System.Media.SystemSound> SoundDictionary =
            new Dictionary<Icons, System.Media.SystemSound>() {
                {Icons.Beep, System.Media.SystemSounds.Beep},
                {Icons.Asterisk, System.Media.SystemSounds.Asterisk},
                {Icons.Question, System.Media.SystemSounds.Question},
                {Icons.Exclamation, System.Media.SystemSounds.Exclamation},
                {Icons.Hand, System.Media.SystemSounds.Hand},
            };

        private ButtonSizes _size = ButtonSizes.Large;
        private Icons _icon = Icons.Beep;
        private string _text = null;
        private long _autoPopDelayTicks = DefaultAutoPopDelay * TimeSpan.TicksPerMillisecond;
        private BackgroundWorker _autoCloseWorker;
        private long _startTick;

        private readonly ChildPopup _popup = new ChildPopup();
        private readonly Border _popupBorder = new Border() {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1),
        };
        private readonly TextBlock _textBlock = new TextBlock() {
            Background = Brushes.White,
            Padding = new Thickness(4),
        };
        private readonly MenuItem _menuItem_Copy = new MenuItem() {
            Header = "Copy to clipboard",
        };

        public MessageButton() {
            InitializeComponent();
            Text = null;
            UpdateIcon();
        }

        private void InitializeComponent() {
            _popup.PlacementTarget = this;
            _popup.Child = _popupBorder;
            _popupBorder.Child = _textBlock;

            this.ContextMenu = new ContextMenu() {
                Items = {
                    _menuItem_Copy,
                },
            };
            _menuItem_Copy.Click += (sender, e) => {
                Clipboard.SetText(_text ?? "");
            };

            this.Checked += (sender, e) => {
                _popup.IsOpen = true;
                PrepareAutoCloser();
            };
            this.Unchecked += (sender, e) => {
                _popup.IsOpen = false;
                CancelAutoCloser();
            };
            this.SizeChanged += (sender, e) => {
                if (_popup.IsOpen) {
                    _popup.IsOpen = false;
                    _popup.IsOpen = true;
                }
            };
        }

        public ButtonSizes Size {
            get { return _size; }
            set { _size = value; UpdateIcon(); }
        }

        public Icons Icon {
            get { return _icon; }
            set { _icon = value; UpdateIcon(); }
        }

        /// <summary>
        /// A String that specifies the text to display.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>If Text is null or empty, the button is disabled.</item>
        /// </list>
        /// </remarks>
        public string Text {
            get { return _textBlock.Text; }
            set {
                _text = value;
                if (!String.IsNullOrEmpty(_text)) {
                    _textBlock.Text = _text;
                    this.IsEnabled = true;
                } else {
                    _textBlock.Text = null;
                    this.IsChecked = false;
                    this.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// The period of time the Text remains visible, in milliseconds.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>If AutoPopDelay &lt;= 0, the Text remains infinitely.</item>
        /// </list>
        /// </remarks>
        public int AutoPopDelay {
            get { return (int)(_autoPopDelayTicks / TimeSpan.TicksPerMillisecond); }
            set { _autoPopDelayTicks = value * TimeSpan.TicksPerMillisecond; }
        }

        /// <summary>
        /// Displays a message popup and icon with system sound.
        /// </summary>
        /// <param name="text">A String that specifies the text to display.</param>
        /// <param name="icon">An Icons value that specifies which icon and sound.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>If text is null or empty, the popup is closed and the button is deactivated.</item>
        /// </list>
        /// </remarks>
        public void Show(string text, Icons icon = Icons.Beep) {
            Text = text;
            Icon = icon;
            if (String.IsNullOrEmpty(text)) {
                this.IsChecked = false;
                return;
            }
            if (this.IsChecked == true) {
                PrepareAutoCloser();
            } else {
                this.IsChecked = true;
            }
            if (SoundDictionary.ContainsKey(icon)) {
                SoundDictionary[icon].Play();
            }
        }

        public void Show(IEnumerable<string> list, Icons icon = Icons.Beep) {
            Show(String.Join("\n", list), icon);
        }

        private void UpdateIcon() {
            this.SmallImageSource = GetImageResource(_icon, ButtonSizes.Small);
            this.LargeImageSource = (_size == ButtonSizes.Large) ?
                GetImageResource(_icon, ButtonSizes.Large) :
                null;
        }

        private BitmapImage GetImageResource(Icons icon, ButtonSizes size) {
            return ResourceHelper.GetImage("Images/" + icon.ToString() + size.ToString() + ".png");
        }

        private void PrepareAutoCloser() {
            if (_autoPopDelayTicks <= 0) {
                return;
            }
            _startTick = DateTime.Now.Ticks;
            _autoCloseWorker = _autoCloseWorker ?? CreateAutoCloser();
            if (!_autoCloseWorker.IsBusy) {
                _autoCloseWorker.RunWorkerAsync();
            }
        }

        private BackgroundWorker CreateAutoCloser() {
            var worker = new BackgroundWorker() {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true,
            };
            worker.DoWork += (sender, e) => {
                var self = sender as BackgroundWorker;
                while (DateTime.Now.Ticks - _startTick < _autoPopDelayTicks) {
                    if (self.CancellationPending) {
                        e.Cancel = true;
                        return;
                    }
                    Thread.Sleep(100);
                }
            };
            worker.RunWorkerCompleted += (sender, e) => {
                this.IsChecked = false;
            };
            return worker;
        }

        private void CancelAutoCloser() {
            if (_autoCloseWorker == null || !_autoCloseWorker.IsBusy) {
                return;
            }
            _autoCloseWorker.CancelAsync();
        }
    }
}
