using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using Microsoft.Windows.Controls.Ribbon;
using TakeAsh;
using TakeAshUtility;
using WpfUtility;

namespace WpfUtility_Call {

    using _resources = Properties.Resources;
    using NewLineCodesHelper = EnumHelper<MainWindow.NewLineCodes>;
    using SexesCodesHelper = EnumHelper<Person.SexesCodes>;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow :
        RibbonWindow,
        IResizeEvent {

        const string DocumentName = "WpfUtility Sample";

        private static Properties.Settings _settings = Properties.Settings.Default;

        private MessageButton messageButton_HPC;
        private MonitorDpi _monitorDpi;
        private WindowPlacement _placement;
        private Persons _persons;
        private XpsPrinter _printer = XpsPrinter.Instance;
        private PrintStatus _printStatus;

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

            comboBox_PersonSex_GalleryCategory.ItemsSource = SexesCodesHelper.ValueDescriptionPairs;

            dataGrid_Notify.CloneItemsSource = (source) => new Persons((Persons)source);
            dataGrid_Notify.ItemsSource = _persons = ResourceHelper.GetText("Resources/Persons.txt").ToPersons();

            this.PrintStatus = PrintStatus.Ready;
            comboBox_Printer.ItemsSource = new[] {
                _resources.MainWindow_comboBox_Printer_UseSystemDialog,
            }.Concat(_printer.QueueNames);
            comboBox_Printer.AdjustMaxItemWidth();
            comboBox_Printer.SelectedItem = _printer.SelectedQueueName;
            comboBox_PageSize.ItemsSource = PageMediaSizeHelper.Values;
            comboBox_PageSize.AdjustMaxItemWidth();
            comboBox_PageSize.SelectedItem = PageMediaSizeHelper.Default;
            comboBox_PageOrientation.ItemsSource = PageOrientationHelper.Values;
            comboBox_PageOrientation.AdjustMaxItemWidth();
            comboBox_PageOrientation.SelectedItem = PageOrientationHelper.Default;
            comboBox_PrintColor.ItemsSource = PrintOutputColorHelper.Values;
            comboBox_PrintColor.AdjustMaxItemWidth();
            comboBox_PrintColor.SelectedItem = PrintOutputColorHelper.Default;

            this.AddResizeHook();
            this.Resizing += (sender, e) => {
                var window = sender as RibbonWindow;
                if (window == null) {
                    return;
                }
                window.Title = Application.Current.MainWindow.Left + ", " +
                    Application.Current.MainWindow.Top + ", " +
                    Application.Current.MainWindow.ActualWidth + ", " +
                    Application.Current.MainWindow.ActualHeight;
            };
            this.Resized += (sender, e) => {
                var window = sender as RibbonWindow;
                if (window == null) {
                    return;
                }
                window.Title = "WpfUtility";
            };
        }

        public PrintStatus PrintStatus {
            get { return _printStatus; }
            set {
                _printStatus = value;
                button_Print.Label = _printStatus.ToDescription();
                switch (_printStatus) {
                    case PrintStatus.Ready:
                        button_Print.LargeImageSource = ResourceHelper.GetImage("Images/PrintL.png");
                        button_Print.SmallImageSource = ResourceHelper.GetImage("Images/PrintS.png");
                        break;
                    case PrintStatus.Printing:
                        button_Print.LargeImageSource = ResourceHelper.GetImage("Images/PrintingL.png");
                        button_Print.SmallImageSource = ResourceHelper.GetImage("Images/PrintingS.png");
                        break;
                }
            }
        }

        private void SetCulture() {
            _settings.Culture = (comboBox_Culture_Gallery.SelectedItem as CultureInfo).Name;
            _settings.Save();
            messageButton_QATB.Show(
                _resources.MainWindow_method_SetCulture_message_OK,
                MessageButton.Icons.Beep
            );
        }

        private void Print() {
            if (comboBox_Printer.SelectedItem == null ||
                comboBox_PageSize.SelectedItem == null ||
                comboBox_PageOrientation.SelectedItem == null ||
                comboBox_PrintColor.SelectedItem == null) {
                return;
            }
            if (comboBox_Printer.SelectedIndex == 0) {
                if (_printer.SelectPrinter(DocumentName) != XpsPrinter.Results.OK) {
                    return;
                }
            } else {
                _printer.SelectedQueueName = comboBox_Printer.SelectedItem as string;
                var pageOrientation = (PageOrientation)comboBox_PageOrientation.SelectedItem;
                var pageMediaSize = (PageMediaSize)comboBox_PageSize.SelectedItem;
                var printColor = (PrintOutputColor)comboBox_PrintColor.SelectedItem;
                _printer.Ticket.PageOrientation = pageOrientation.ToSystemPageOrientation();
                _printer.Ticket.PageMediaSize = pageMediaSize.ToSystemPageMediaSize(pageOrientation);
                _printer.Ticket.OutputColor = printColor.ToSystemOutputColor();
                _printer.DocumentName = DocumentName;
            }
            PrintStatus = PrintStatus.Printing;
            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    _printer.Print(new UIElement[] {
                        dataGrid_Notify.Clone(),
                        dataGrid_Notify.GetImage(),
                    });
                    PrintStatus = PrintStatus.Ready;
                })
            );
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

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e) {
            _monitorDpi = new MonitorDpi(this);
        }

        private void menuItem_ShowMonitorDpi_Click(object sender, RoutedEventArgs e) {
            _monitorDpi.Update();
            messageButton_QATB.Show(
                new[] { "Monitor DPI", "X:" + _monitorDpi.X, "Y:" + _monitorDpi.Y },
                MessageButton.Icons.Beep
            );
        }

        private void button_ShowTextPrompt_Click(object sender, RoutedEventArgs e) {
            var prompt = new TextPrompt() {
                Title = "TextPrompt Sample",
                Message = "'Message' is here.",
                InputText = "Default text",
                AcceptsReturn = true,
            };
            if (prompt.ShowDialog() != true) {
                return;
            }
            messageButton_QATB.Show("Input:\n" + prompt.InputText);
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

        private void dataGrid_Notify_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var person = dataGrid_Notify.SelectedItem as Person;
            if (person == null) {
                return;
            }
            textBox_PersonId.Text = person.ID.ToString();
            textBox_PersonFirstName.Text = person.FirstName;
            textBox_PersonLastName.Text = person.LastName;
            comboBox_PersonSex_Gallery.SelectedValue = person.Sex;
        }

        private void button_ApplyPerson_Click(object sender, RoutedEventArgs e) {
            var person = dataGrid_Notify.SelectedItem as Person;
            if (person == null) {
                return;
            }
            person.ID = textBox_PersonId.Text.TryParse<int>();
            person.FirstName = textBox_PersonFirstName.Text;
            person.LastName = textBox_PersonLastName.Text;
            person.Sex = SexesCodesHelper.Cast(comboBox_PersonSex_Gallery.SelectedValue);
            _persons.Refresh();
        }

        private void button_Print_Click(object sender, RoutedEventArgs e) {
            Print();
        }

#pragma warning disable 0067

        #region IResizeEvent

        public event EventHandler Resizing;
        public event EventHandler Resized;
        
        #endregion

#pragma warning restore 0067

    }
}
