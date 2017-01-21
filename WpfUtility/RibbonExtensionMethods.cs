using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Media.Imaging;
using TakeAshUtility;

namespace WpfUtility {

    public static class RibbonExtensionMethods {

        const string DefaultShowIcon = "Images/Show.png";
        const string DefaultHideIcon = "Images/Hide.png";

        public static T AddHelpItem<T>(
            this Ribbon ribbon,
            T item
        )
            where T : UIElement {

            var helpPanel = (ribbon.HelpPaneContent as StackPanel) ?? new StackPanel() {
                Orientation = Orientation.Horizontal,
            };
            helpPanel.Children.Add(item);
            ribbon.HelpPaneContent = helpPanel;
            return item;
        }

        public static RibbonButton AddMinimizeButton(
            this Ribbon ribbon,
            BitmapImage showIcon = null,
            BitmapImage hideIcon = null,
            string showToolTip = null,
            string hideToolTip = null
        ) {
            showIcon = showIcon ?? ResourceHelper.GetImage(DefaultShowIcon);
            hideIcon = hideIcon ?? ResourceHelper.GetImage(DefaultHideIcon);
            showToolTip = showToolTip ?? Properties.Resources.RibbonExtensionMethods_MinimizeButton_ToolTip_Show;
            hideToolTip = hideToolTip ?? Properties.Resources.RibbonExtensionMethods_MinimizeButton_ToolTip_Hide;

            var button = new RibbonButton() {
                SmallImageSource = hideIcon,
                ToolTip = hideToolTip,
            };
            button.Click += (s, e) => {
                ribbon.IsMinimized = !ribbon.IsMinimized;
            };
            // ribbon.IsMinimizedChanged += ...
            ribbon.AddPropertyChanged(
                Ribbon.IsMinimizedProperty,
                (s, e) => {
                    button.SmallImageSource = ribbon.IsMinimized ?
                        showIcon :
                        hideIcon;
                    button.ToolTip = ribbon.IsMinimized ?
                        showToolTip :
                        hideToolTip;
                }
            );
            return ribbon.AddHelpItem(button);
        }

        public static MessageButton AddMessageButton(
            this Ribbon ribbon,
            string toolTip = "Info",
            int autoPopDelay = 5000
        ) {
            return ribbon.AddHelpItem(new MessageButton() {
                Size = MessageButton.ButtonSizes.Small,
                ToolTip = toolTip,
                AutoPopDelay = autoPopDelay,
            });
        }
    }
}
