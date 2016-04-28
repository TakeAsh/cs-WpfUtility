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

namespace TreeViewSample {

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow :
        Window {

        private Body _human;

        public MainWindow() {
            InitializeComponent();
            _human = new Body();
            treeView.ItemTemplateSelector = new ResourceTemplateSelector<HierarchicalDataTemplate>(name => name + "Template");
            treeView.ItemsSource = new[] { _human, };
        }
    }
}
