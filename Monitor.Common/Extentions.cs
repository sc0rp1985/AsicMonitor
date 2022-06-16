using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Common
{
    public static class ExtentsionIEnumerable
    {
        public static List<T> ToSingleItemList<T>(this T element)
        {
            return new List<T> { element };
        }

        public static void Replace<T>(this IList<T> collection, T replaceable, T replacement)
        {
            if (replaceable == null || replacement == null) return;
            var index = collection.IndexOf(replaceable);
            if (index == -1) return;
            collection.Remove(replaceable);
            collection.Insert(index, replacement);
        }

        /*public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> collection)
        {
            return new BindableCollection<T>(collection);
        }

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> range)
        {
            var temp = range.ToList();
            temp.ForEach(collection.Add);
        }*/

        public static T[] Add<T>(this T[] collection, T item)
        {
            var tempList = collection.ToList();
            tempList.Add(item);
            return tempList.ToArray();
        }

        public static T[] Remove<T>(this T[] collection, T item)
        {
            var tempList = collection.ToList();
            tempList.Remove(item);
            return tempList.ToArray();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return (collection == null) || !collection.Any();
        }
    }
}
