using System;
using System.Collections.Generic;

namespace ProjectLincoln {
    public static class Extensions {
        public static TR IfNotNull<T, TR>(this T obj, Func<T, TR> func, Func<TR> ifNull = null) where T : class {
            return obj != null ? func(obj) : (ifNull != null ? ifNull() : default(TR));
        }

        /// <summary>
        /// Checks if the provided int? is null and executes action(s) based on the nullness
        /// </summary>
        /// <example>
        /// This is an example of how to use this extension with only an action for the not null value
        /// <code>
        /// int? x = 2;
        /// x.IntNotNull(y => MessageBox.Show(y.ToString()));
        /// </code>
        /// This is an example of how to use this extension with only action for the both null and not null values
        /// <code>
        /// int? x = null;
        /// x.IntNotNull(y => MessageBox.Show(y.ToString()), () => MessageBox.Show("error"));
        /// </code>
        /// </example>
        /// <param name="x">The int? to check</param>
        /// <param name="action">The action to perform if the int? is not null</param>
        /// <param name="actionIfNull">Optional: The action to perform if the int? is null</param>
        public static void IfNotNull (this int? x, Action<int> action, Action actionIfNull = null) {
            if (x != null) { action((int) x); } else { actionIfNull(); }
        }

        /// <summary>
        /// Checks if the provided item is in the given list and inserts it if it is not
        /// </summary>
        /// <example>
        /// This is an example of how to use this extension
        /// <code>
        /// List<T> list = new List<T>();
        /// var item = value;
        /// 
        /// list.AddIfNotContains(item);
        /// </code>
        /// </example>
        /// <typeparam name="T">The type of list</typeparam>
        /// <param name="list">The list</param>
        /// <param name="list">The list</param>
        /// <param name="action">The action to perform if the list does not contain the item</param>
        /// <param name="containsAction">Optional: The action to perform if the list contains the item</param>
        public static void IfNotContains<T>(this List<T> list, T item, Action<T> action, Action containsAction = null) {
            if (!list.Contains(item)) { action(item); } else { containsAction(); }
        }

        /// <summary>
        /// Checks if the provided item is in the given list and inserts it if it is not
        /// </summary>
        /// <example>
        /// This is an example of how to use this extension
        /// <code>
        /// List<T> list = new List<T>();
        /// var item = value;
        /// 
        /// list.AddIfNotContains(item);
        /// </code>
        /// </example>
        /// <typeparam name="T">The type of list</typeparam>
        /// <param name="list">The list</param>
        /// <param name="item">The item</param>
        public static void AddIfNotContains<T> (this List<T> list, T item) {
            if (!list.Contains(item)) { list.Add(item); }
        }
    }
}
