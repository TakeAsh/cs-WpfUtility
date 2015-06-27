using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WpfUtility {

    /// <summary>
    /// TabControl with FormerPanel and LatterPanel
    /// </summary>
    /// <remarks>
    /// [片鱗懐古のブログ: wpf : TabControlのタブの左右にパネルを配置](http://pieceofnostalgy.blogspot.jp/2012/03/wpf-tabcontrol.html)
    /// </remarks>
    public class PaneledTab :
        TabControl {

        private Grid _rootPanel;
        private Panel _formerPanel;
        private TabPanel _tabPanel;
        private Panel _latterPanel;
        private Border _contentPanel;

        static PaneledTab() {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PaneledTab),
                new FrameworkPropertyMetadata(typeof(PaneledTab))
            );
        }

        public static readonly DependencyProperty FormerPanelProperty = DependencyProperty.Register(
            "FormerPanel",
            typeof(Panel),
            typeof(PaneledTab)
        );

        public static readonly DependencyProperty LatterPanelProperty = DependencyProperty.Register(
            "LatterPanel",
            typeof(Panel),
            typeof(PaneledTab)
        );

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            _rootPanel = GetTemplateChild("RootPanel") as Grid;
            _formerPanel = GetTemplateChild("FormerPanel") as Panel;
            _tabPanel = GetTemplateChild("TabPanel") as TabPanel;
            _latterPanel = GetTemplateChild("LatterPanel") as Panel;
            _contentPanel = GetTemplateChild("ContentPanel") as Border;
        }

        public Panel FormerPanel {
            get { return GetValue(FormerPanelProperty) as Panel; }
            set { SetValue(FormerPanelProperty, value); }
        }

        public Panel LatterPanel {
            get { return GetValue(LatterPanelProperty) as Panel; }
            set { SetValue(LatterPanelProperty, value); }
        }

        public double TabPanelWidth {
            get { return _tabPanel.ActualWidth; }
        }

        public double TabPanelHeight {
            get { return _tabPanel.ActualHeight; }
        }
    }
}
