using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfUtility {

    public interface IMessageButtonHost {

        MessageButton.Icons MessageIcon { get; set; }

        string MessageText { get; set; }

        void ShowMessage(string text, MessageButton.Icons icon = MessageButton.Icons.Beep);

        void ShowMessage(IEnumerable<string> list, MessageButton.Icons icon = MessageButton.Icons.Beep);
    }

    public static class IMessageButtonHostExtensionMethods {

        public static void SafeShowMessage(
            this IMessageButtonHost host,
            string text,
            MessageButton.Icons icon = MessageButton.Icons.Beep
        ) {
            if (host == null) {
                return;
            }
            host.ShowMessage(text, icon);
        }

        public static void SafeShowMessage(
            this IMessageButtonHost host,
            IEnumerable<string> list,
            MessageButton.Icons icon = MessageButton.Icons.Beep
        ) {
            if (host == null) {
                return;
            }
            host.ShowMessage(list, icon);
        }
    }
}
