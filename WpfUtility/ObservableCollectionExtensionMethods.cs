using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace WpfUtility {

    public static class ObservableCollectionExtensionMethods {

        public static ICollectionView GetDefaultView<T>(this ObservableCollection<T> collection) {
            if (collection == null) {
                return null;
            }
            return CollectionViewSource.GetDefaultView(collection);
        }

        public static void SortBy(
            this ICollectionView view,
            string propertyName,
            ListSortDirection direction = ListSortDirection.Ascending
        ) {
            if (view == null) {
                return;
            }
            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
        }
    }
}
