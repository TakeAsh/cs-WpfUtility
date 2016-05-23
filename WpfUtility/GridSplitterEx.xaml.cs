using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

namespace WpfUtility {

    /// <summary>
    /// GridSplitterEx.xaml の相互作用ロジック
    /// </summary>
    [TemplatePart(Name = HorizontalUpButtonName, Type = typeof(Image))]
    [TemplatePart(Name = HorizontalMiddleButtonName, Type = typeof(Image))]
    [TemplatePart(Name = HorizontalDownButtonName, Type = typeof(Image))]
    [TemplatePart(Name = VerticalLeftButtonName, Type = typeof(Image))]
    [TemplatePart(Name = VerticalMiddleButtonName, Type = typeof(Image))]
    [TemplatePart(Name = VerticalRightButtonName, Type = typeof(Image))]
    public class GridSplitterEx :
        GridSplitter {

        #region Constants

        const string HorizontalUpButtonName = "HorizontalUpButton";
        const string HorizontalMiddleButtonName = "HorizontalMiddleButton";
        const string HorizontalDownButtonName = "HorizontalDownButton";
        const string VerticalLeftButtonName = "VerticalLeftButton";
        const string VerticalMiddleButtonName = "VerticalMiddleButton";
        const string VerticalRightButtonName = "VerticalRightButton";

        #endregion

        #region Constructor

        static GridSplitterEx() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(GridSplitterEx),
                new FrameworkPropertyMetadata(typeof(GridSplitterEx))
            );
        }

        #endregion

        #region Private Variables

        protected Image HorizontalUpButton;
        protected Image HorizontalMiddleButton;
        protected Image HorizontalDownButton;
        protected Image VerticalLeftButton;
        protected Image VerticalMiddleButton;
        protected Image VerticalRightButton;

        #endregion

        #region Properties

        #region Events

        public event RoutedEventHandler HorizontalUpButtonClick;
        public event RoutedEventHandler HorizontalMiddleButtonClick;
        public event RoutedEventHandler HorizontalDownButtonClick;
        public event RoutedEventHandler VerticalLeftButtonClick;
        public event RoutedEventHandler VerticalMiddleButtonClick;
        public event RoutedEventHandler VerticalRightButtonClick;

        #endregion

        #region UseHorizontally

        public static readonly DependencyProperty UseHorizontallyProperty = DependencyProperty.Register(
            "UseHorizontally",
            typeof(bool),
            typeof(GridSplitterEx),
            new FrameworkPropertyMetadata(false)
        );

        public bool UseHorizontally {
            get { return (bool)GetValue(UseHorizontallyProperty); }
            set { SetValue(UseHorizontallyProperty, value); }
        }

        #endregion

        #region Button Visibilities

        public static readonly DependencyProperty HorizontalUpButtonVisibilityProperty = DependencyProperty.Register(
            "HorizontalUpButtonVisibility",
            typeof(Visibility),
            typeof(GridSplitterEx),
            new FrameworkPropertyMetadata(Visibility.Visible, OnHorizontalUpButtonVisibilityChanged)
        );

        private static void OnHorizontalUpButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var gridSplitterEx = d as GridSplitterEx;
            if (gridSplitterEx == null || gridSplitterEx.HorizontalUpButton == null) {
                return;
            }
            gridSplitterEx.HorizontalUpButton.Visibility = (Visibility)e.NewValue;
        }

        public Visibility HorizontalUpButtonVisibility {
            get { return (Visibility)GetValue(HorizontalUpButtonVisibilityProperty); }
            set { SetValue(HorizontalUpButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty HorizontalMiddleButtonVisibilityProperty = DependencyProperty.Register(
            "HorizontalMiddleButtonVisibility",
            typeof(Visibility),
            typeof(GridSplitterEx),
            new FrameworkPropertyMetadata(Visibility.Visible, OnHorizontalMiddleButtonVisibilityChanged)
        );

        private static void OnHorizontalMiddleButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var gridSplitterEx = d as GridSplitterEx;
            if (gridSplitterEx == null || gridSplitterEx.HorizontalMiddleButton == null) {
                return;
            }
            gridSplitterEx.HorizontalMiddleButton.Visibility = (Visibility)e.NewValue;
        }

        public Visibility HorizontalMiddleButtonVisibility {
            get { return (Visibility)GetValue(HorizontalMiddleButtonVisibilityProperty); }
            set { SetValue(HorizontalMiddleButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty HorizontalDownButtonVisibilityProperty = DependencyProperty.Register(
            "HorizontalDownButtonVisibility",
            typeof(Visibility),
            typeof(GridSplitterEx),
            new FrameworkPropertyMetadata(Visibility.Visible, OnHorizontalDownButtonVisibilityChanged)
        );

        private static void OnHorizontalDownButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var gridSplitterEx = d as GridSplitterEx;
            if (gridSplitterEx == null || gridSplitterEx.HorizontalDownButton == null) {
                return;
            }
            gridSplitterEx.HorizontalDownButton.Visibility = (Visibility)e.NewValue;
        }

        public Visibility HorizontalDownButtonVisibility {
            get { return (Visibility)GetValue(HorizontalDownButtonVisibilityProperty); }
            set { SetValue(HorizontalDownButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VerticalLeftButtonVisibilityProperty = DependencyProperty.Register(
            "VerticalLeftButtonVisibility",
            typeof(Visibility),
            typeof(GridSplitterEx),
            new FrameworkPropertyMetadata(Visibility.Visible, OnVerticalLeftButtonVisibilityChanged)
        );

        private static void OnVerticalLeftButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var gridSplitterEx = d as GridSplitterEx;
            if (gridSplitterEx == null || gridSplitterEx.VerticalLeftButton == null) {
                return;
            }
            gridSplitterEx.VerticalLeftButton.Visibility = (Visibility)e.NewValue;
        }

        public Visibility VerticalLeftButtonVisibility {
            get { return (Visibility)GetValue(VerticalLeftButtonVisibilityProperty); }
            set { SetValue(VerticalLeftButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VerticalMiddleButtonVisibilityProperty = DependencyProperty.Register(
            "VerticalMiddleButtonVisibility",
            typeof(Visibility),
            typeof(GridSplitterEx),
            new FrameworkPropertyMetadata(Visibility.Visible, OnVerticalMiddleButtonVisibilityChanged)
        );

        private static void OnVerticalMiddleButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var gridSplitterEx = d as GridSplitterEx;
            if (gridSplitterEx == null || gridSplitterEx.VerticalMiddleButton == null) {
                return;
            }
            gridSplitterEx.VerticalMiddleButton.Visibility = (Visibility)e.NewValue;
        }

        public Visibility VerticalMiddleButtonVisibility {
            get { return (Visibility)GetValue(VerticalMiddleButtonVisibilityProperty); }
            set { SetValue(VerticalMiddleButtonVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VerticalRightButtonVisibilityProperty = DependencyProperty.Register(
            "VerticalRightButtonVisibility",
            typeof(Visibility),
            typeof(GridSplitterEx),
            new FrameworkPropertyMetadata(Visibility.Visible, OnVerticalRightButtonVisibilityChanged)
        );

        private static void OnVerticalRightButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var gridSplitterEx = d as GridSplitterEx;
            if (gridSplitterEx == null || gridSplitterEx.VerticalRightButton == null) {
                return;
            }
            gridSplitterEx.VerticalRightButton.Visibility = (Visibility)e.NewValue;
        }

        public Visibility VerticalRightButtonVisibility {
            get { return (Visibility)GetValue(VerticalRightButtonVisibilityProperty); }
            set { SetValue(VerticalRightButtonVisibilityProperty, value); }
        }

        #endregion

        #endregion

        #region Methods

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            new[] {
                HorizontalUpButtonName, HorizontalMiddleButtonName, HorizontalDownButtonName,
                VerticalLeftButtonName, VerticalMiddleButtonName, VerticalRightButtonName,
            }.ForEach(name => {
                var button = GetTemplateChild(name) as Image;
                if (button == null) {
                    return;
                }
                button.MouseDown += OnButtonClick;
                button.MouseLeave += OnMouseLeave;
                switch (name) {
                    case HorizontalUpButtonName:
                        button.MouseEnter += (s, e) => { Mouse.OverrideCursor = Cursors.ScrollN; };
                        button.Visibility = HorizontalUpButtonVisibility;
                        HorizontalUpButton = button;
                        break;
                    case HorizontalMiddleButtonName:
                        button.MouseEnter += (s, e) => { Mouse.OverrideCursor = Cursors.ScrollNS; };
                        button.Visibility = HorizontalMiddleButtonVisibility;
                        HorizontalMiddleButton = button;
                        break;
                    case HorizontalDownButtonName:
                        button.MouseEnter += (s, e) => { Mouse.OverrideCursor = Cursors.ScrollS; };
                        button.Visibility = HorizontalDownButtonVisibility;
                        HorizontalDownButton = button;
                        break;
                    case VerticalLeftButtonName:
                        button.MouseEnter += (s, e) => { Mouse.OverrideCursor = Cursors.ScrollW; };
                        button.Visibility = VerticalLeftButtonVisibility;
                        VerticalLeftButton = button;
                        break;
                    case VerticalMiddleButtonName:
                        button.MouseEnter += (s, e) => { Mouse.OverrideCursor = Cursors.ScrollWE; };
                        button.Visibility = VerticalMiddleButtonVisibility;
                        VerticalMiddleButton = button;
                        break;
                    case VerticalRightButtonName:
                        button.MouseEnter += (s, e) => { Mouse.OverrideCursor = Cursors.ScrollE; };
                        button.Visibility = VerticalRightButtonVisibility;
                        VerticalRightButton = button;
                        break;
                }
            });
        }

        private void OnButtonClick(object sender, RoutedEventArgs e) {
            var button = sender as Image;
            if (button == null) {
                return;
            }
            RoutedEventHandler handler = null;
            switch (button.Name) {
                case HorizontalUpButtonName:
                    handler = HorizontalUpButtonClick;
                    break;
                case HorizontalMiddleButtonName:
                    handler = HorizontalMiddleButtonClick;
                    break;
                case HorizontalDownButtonName:
                    handler = HorizontalDownButtonClick;
                    break;
                case VerticalLeftButtonName:
                    handler = VerticalLeftButtonClick;
                    break;
                case VerticalMiddleButtonName:
                    handler = VerticalMiddleButtonClick;
                    break;
                case VerticalRightButtonName:
                    handler = VerticalRightButtonClick;
                    break;
            }
            if (handler == null) {
                return;
            }
            handler(sender, e);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e) {
            Mouse.OverrideCursor = null;
        }

        #endregion
    }
}
