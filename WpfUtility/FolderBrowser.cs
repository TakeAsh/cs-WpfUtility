using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WpfUtility {

    public static class FolderBrowser {

        /// <summary>
        /// Browse folder
        /// </summary>
        /// <param name="description">Description shown in the dialog</param>
        /// <param name="defaultPath">Path shown as SelectedPath</param>
        /// <returns>
        /// <list type="table">
        /// <item><term>not null</term><description>Selected Path</description></item>
        /// <item><term>null</term><description>Canceled</description></item>
        /// </list>
        /// </returns>
        public static string Browse(string description, string defaultPath = null) {
            var path = String.IsNullOrEmpty(defaultPath) || !Directory.Exists(defaultPath) ?
                Environment.GetFolderPath(Environment.SpecialFolder.Personal) :
                defaultPath;
            var dialog = new FolderBrowserDialog() {
                Description = description,
                SelectedPath = path,
                ShowNewFolderButton = true,
            };
            return dialog.ShowDialog() != DialogResult.OK ?
                null :
                dialog.SelectedPath;
        }
    }
}
