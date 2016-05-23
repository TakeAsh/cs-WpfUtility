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

namespace GridSplitterExSample {

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow :
        Window {

        private int _verticalDiv = 4;
        private int _verticalIndex = 2;
        private int _horizontalDiv = 5;
        private int _horizontalIndex = 3;

        public MainWindow() {
            InitializeComponent();

            gridSplitter_A.VerticalLeftButtonClick += (s, e) => {
                columnDefinition_A1.Width = new GridLength(0, GridUnitType.Star);
                columnDefinition_A2.Width = new GridLength(1, GridUnitType.Star);
            };
            gridSplitter_A.VerticalMiddleButtonClick += (s, e) => {
                columnDefinition_A1.Width = new GridLength(1, GridUnitType.Star);
                columnDefinition_A2.Width = new GridLength(1, GridUnitType.Star);
            };
            gridSplitter_A.VerticalRightButtonClick += (s, e) => {
                columnDefinition_A1.Width = new GridLength(1, GridUnitType.Star);
                columnDefinition_A2.Width = new GridLength(0, GridUnitType.Star);
            };

            gridSplitter_B.HorizontalUpButtonClick += (s, e) => {
                _verticalIndex = (_verticalIndex - 1).Clamp(0, _verticalDiv);
                rowDefinition_B1.Height = new GridLength(_verticalIndex, GridUnitType.Star);
                rowDefinition_B2.Height = new GridLength(_verticalDiv - _verticalIndex, GridUnitType.Star);
            };
            gridSplitter_B.HorizontalMiddleButtonClick += (s, e) => {
                _verticalIndex = _verticalDiv / 2;
                rowDefinition_B1.Height = new GridLength(_verticalIndex, GridUnitType.Star);
                rowDefinition_B2.Height = new GridLength(_verticalDiv - _verticalIndex, GridUnitType.Star);
            };
            gridSplitter_B.HorizontalDownButtonClick += (s, e) => {
                _verticalIndex = (_verticalIndex + 1).Clamp(0, _verticalDiv);
                rowDefinition_B1.Height = new GridLength(_verticalIndex, GridUnitType.Star);
                rowDefinition_B2.Height = new GridLength(_verticalDiv - _verticalIndex, GridUnitType.Star);
            };

            gridSplitter_C.VerticalLeftButtonClick += (s, e) => {
                _horizontalIndex = (_horizontalIndex - 1).Clamp(0, _horizontalDiv);
                columnDefinition_C1.Width = new GridLength(_horizontalIndex, GridUnitType.Star);
                columnDefinition_C2.Width = new GridLength(_horizontalDiv - _horizontalIndex, GridUnitType.Star);
            };
            gridSplitter_C.VerticalMiddleButtonClick += (s, e) => {
                _horizontalIndex = _horizontalDiv / 2;
                columnDefinition_C1.Width = new GridLength(_horizontalIndex, GridUnitType.Star);
                columnDefinition_C2.Width = new GridLength(_horizontalDiv - _horizontalIndex, GridUnitType.Star);
            };
            gridSplitter_C.VerticalRightButtonClick += (s, e) => {
                _horizontalIndex = (_horizontalIndex + 1).Clamp(0, _horizontalDiv);
                columnDefinition_C1.Width = new GridLength(_horizontalIndex, GridUnitType.Star);
                columnDefinition_C2.Width = new GridLength(_horizontalDiv - _horizontalIndex, GridUnitType.Star);
            };
        }
    }
}
