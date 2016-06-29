using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using TakeAshUtility;

namespace WpfUtility {

    public class XpsPrinter {

        public enum Results {
            OK,
            Canceled,
        }

        private const int _renderHierarchy = 16;
        private const string _packageUriString = "application:///temp.xps";
        private static readonly Uri _packageUri = new Uri(_packageUriString, UriKind.Absolute);

        /// <summary>
        /// Conversion factor from mm to pixel
        /// </summary>
        public const double MmToPixel = 96.0 / 25.4;

        /// <summary>
        /// Default Print Margin in mm
        /// </summary>
        public const double DefaultPrintMargin = 15;

        /// <summary>
        /// Maximum Print Margin in mm
        /// </summary>
        public const double MaxPrintMargin = 100;

        /// <summary>
        /// Default Media Width in mm
        /// </summary>
        public const double DefaultMediaWidth = 210;

        /// <summary>
        /// Default Media Height in mm
        /// </summary>
        public const double DefaultMediaHeight = 297;

        private static readonly double _defaultMediaWidth = Math.Floor(DefaultMediaWidth * MmToPixel);
        private static readonly double _defaultMediaHeight = Math.Floor(DefaultMediaHeight * MmToPixel);

        private static readonly Lazy<XpsPrinter> _lazy = new Lazy<XpsPrinter>(() => new XpsPrinter());

        private double _printMarginLeft = DefaultPrintMargin * MmToPixel;
        private double _printMarginRight = DefaultPrintMargin * MmToPixel;
        private double _printMarginTop = DefaultPrintMargin * MmToPixel;
        private double _printMarginBottom = DefaultPrintMargin * MmToPixel;

        private LocalPrintServer _printServer;
        private PrintQueue _queue;
        private XpsDocumentWriter _xpsdw;
        private bool _useSystemDialog;
        private PrintDocumentImageableArea _imgArea;

        private XpsPrinter() {
            _printServer = new LocalPrintServer();
            QueueNames = _printServer.GetPrintQueues()
                .Select(queue => queue.Name)
                .OrderBy(name => name)
                .SafeToArray();
            SelectedQueueName = _printServer.DefaultPrintQueue.Name;
        }

        public static XpsPrinter Instance { get { return _lazy.Value; } }

        public string[] QueueNames { get; private set; }

        public string SelectedQueueName {
            get {
                return _queue == null ?
                    null :
                    _queue.Name;
            }
            set {
                var queueName = value;
                if (String.IsNullOrEmpty(queueName) ||
                    QueueNames == null ||
                    !QueueNames.Contains(queueName)) {
                    return;
                }
                _queue = _printServer.GetPrintQueues()
                    .FirstOrDefault(queue => queue.Name == queueName);
                _xpsdw = PrintQueue.CreateXpsDocumentWriter(_queue);
                _useSystemDialog = false;
            }
        }

        public PrintTicket Ticket {
            get {
                return _queue == null ?
                    null :
                    _queue.UserPrintTicket;
            }
        }

        public PrintCapabilities Capabilities {
            get {
                return _queue == null ?
                    null :
                    _queue.GetPrintCapabilities();
            }
        }

        public string DocumentName {
            get {
                return _queue == null ?
                    null :
                    _queue.CurrentJobSettings.Description;
            }
            set {
                if (_queue == null) {
                    return;
                }
                _queue.CurrentJobSettings.Description = value;
            }
        }

        /// <summary>
        /// Print Margin Left in mm
        /// </summary>
        public double PrintMarginLeft {
            get { return _printMarginLeft / MmToPixel; }
            set { _printMarginLeft = value.Clamp(0, MaxPrintMargin) * MmToPixel; }
        }

        /// <summary>
        /// Print Margin Right in mm
        /// </summary>
        public double PrintMarginRight {
            get { return _printMarginRight / MmToPixel; }
            set { _printMarginRight = value.Clamp(0, MaxPrintMargin) * MmToPixel; }
        }

        /// <summary>
        /// Print Margin Top in mm
        /// </summary>
        public double PrintMarginTop {
            get { return _printMarginTop / MmToPixel; }
            set { _printMarginTop = value.Clamp(0, MaxPrintMargin) * MmToPixel; }
        }

        /// <summary>
        /// Print Margin Bottom in mm
        /// </summary>
        public double PrintMarginBottom {
            get { return _printMarginBottom / MmToPixel; }
            set { _printMarginBottom = value.Clamp(0, MaxPrintMargin) * MmToPixel; }
        }

        public PrintDocumentImageableArea ImageableArea {
            get { return _imgArea; }
        }

        /// <summary>
        /// Select printer by system dialog
        /// </summary>
        /// <param name="docName">Document name</param>
        /// <returns>
        /// <list type="table">
        /// <item><term>OK</term><description>User selected suitable printer.</description></item>
        /// <item><term>Canceled</term><description>User canceled to print.</description></item>
        /// </list>
        /// </returns>
        public Results SelectPrinter(string docName) {
            _xpsdw = PrintQueue.CreateXpsDocumentWriter(docName, ref _imgArea);
            _useSystemDialog = true;
            return IsCanceled() ?
                Results.Canceled :
                Results.OK;
        }

        private bool IsCanceled() {
            return _xpsdw == null || _imgArea == null;
        }

        public void Print(IEnumerable<UIElement> elements) {
            if (elements == null) {
                return;
            }
            var mediaWidth = _useSystemDialog ?
                _imgArea.MediaSizeWidth :
                Ticket.PageMediaSize.Width ?? _defaultMediaWidth;
            var mediaHeight = _useSystemDialog ?
                _imgArea.MediaSizeHeight :
                Ticket.PageMediaSize.Height ?? _defaultMediaHeight;
            var printWidth = mediaWidth - (_printMarginLeft + _printMarginRight);
            var printHeight = mediaHeight - (_printMarginTop + _printMarginBottom);
            var fixedDoc = new FixedDocument();
            elements.Where(element => element != null)
                .Select(element => {
                    var panel = new DockPanel() {
                        Width = printWidth,
                        Height = printHeight,
                        Children = { element },
                    };
                    FixedPage.SetLeft(panel, _printMarginLeft);
                    FixedPage.SetTop(panel, _printMarginTop);
                    return panel;
                }).Select(panel => new FixedPage() {
                    Width = mediaWidth,
                    Height = mediaHeight,
                    Children = { panel },
                }).Select(page => new PageContent() {
                    Child = page,
                }).ForEach(pageContent => fixedDoc.Pages.Add(pageContent));
            Print(fixedDoc);
        }

        public void Print(FixedDocument document) {
            if (_xpsdw == null ||
                document == null ||
                document.Pages == null ||
                document.Pages.Count == 0) {
                return;
            }
            for (var i = 0; i < _renderHierarchy; ++i) {
                document.Pages.ForEach(page => (page.Child as FrameworkElement).UpdateLayoutEx());
            }
            using (var stream = new MemoryStream())
            using (var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite)) {
                PackageStore.AddPackage(_packageUri, package);
                using (var xpsDoc = new XpsDocument(package, CompressionOption.NotCompressed, _packageUriString)) {
                    var writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                    writer.Write(document);
                    try {
                        _xpsdw.Write(xpsDoc.GetFixedDocumentSequence(), Ticket);
                    }
                    catch (Exception ex) {
                        // When failed to print, system already show dialog before catching exception,
                        // so there is no need to trap by myself.
                        Debug.Print(ex.GetAllMessages());
                    }
                }
                PackageStore.RemovePackage(_packageUri);
            }
            _xpsdw = null;
        }

        public void Print(Visual visual) {
            if (_xpsdw == null ||
                visual == null) {
                return;
            }
            for (var i = 0; i < _renderHierarchy; ++i) {
                (visual as FrameworkElement).UpdateLayoutEx();
            }
            using (var stream = new MemoryStream())
            using (var package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite)) {
                PackageStore.AddPackage(_packageUri, package);
                using (var xpsDoc = new XpsDocument(package, CompressionOption.NotCompressed, _packageUriString)) {
                    var writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                    writer.Write(visual);
                    try {
                        _xpsdw.Write(xpsDoc.GetFixedDocumentSequence(), Ticket);
                    }
                    catch (Exception ex) {
                        // When failed to print, system already show dialog before catching exception,
                        // so there is no need to trap by myself.
                        Debug.Print(ex.GetAllMessages());
                    }
                }
                PackageStore.RemovePackage(_packageUri);
            }
            _xpsdw = null;
        }
    }
}
