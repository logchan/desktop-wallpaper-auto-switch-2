using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

using MultiLang;
using System.Windows.Controls;
using System.Reflection;

namespace DWAS2.Utilities
{
    class WPFHelper
    {
        /// <summary>
        /// Get an IEnumerable instance of the visual children of a certain type (T) in a dependency object. Source adapted from StackOverflow.
        /// </summary>
        /// <typeparam name="T">the type of visual children</typeparam>
        /// <param name="depObj">the dependency object</param>
        /// <returns>an IEnumerable(T)</returns>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Apply MLE on some UI elements of a dependency object
        /// </summary>
        /// <param name="lang">the MLE</param>
        /// <param name="depObj">the dependency object</param>
        public static void ApplyUILanguage(MultiLangEngine lang, DependencyObject depObj)
        {
            Type[] types = { typeof(TextBlock), typeof(Button), typeof(Label) };

            foreach (Type T in types)
            {
                ApplyLanguageOfOneType(lang, T, "Text", depObj);
            }
        }

        /// <summary>
        /// Apply MLE on a type of UI elements of a dependency object
        /// </summary>
        /// <param name="lang">the MLE</param>
        /// <param name="T">the type of UI element</param>
        /// <param name="propertyName">the property consisting the string to set</param>
        /// <param name="depObj">the dependency object</param>
        public static void ApplyLanguageOfOneType(MultiLangEngine lang, Type T, string propertyName, DependencyObject depObj)
        {
            MethodInfo m = typeof(WPFHelper).GetMethod("FindVisualChildren", BindingFlags.Public | BindingFlags.Static);
            m = m.MakeGenericMethod(T);
            foreach (object obj in (IEnumerable)m.Invoke(null, new Object[] { depObj }))
            {
                PropertyInfo theProperty = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (theProperty != null)
                {
                    theProperty.SetValue(obj, theProperty.GetValue(obj, null).ToString().t(lang), null);
                }
            }
        }

        /// <summary>
        /// Get the next element of an Enum type, circularly
        /// </summary>
        /// <typeparam name="TEnum">an Enum type</typeparam>
        /// <param name="current">current element</param>
        /// <returns>the next element</returns>
        public static TEnum GetNextEnum<TEnum>(TEnum current) where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if(!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an Enum type.");
            }
            else
            {
                TEnum[] enums = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray<TEnum>();
                int index = FindElementIndex<TEnum>(enums, current, (a, b) => a.CompareTo(b) == 0);
                return enums[(index + 1) % enums.Length];
            }
        }

        /// <summary>
        /// Find the index of an element in an array
        /// </summary>
        /// <typeparam name="T">The type of array and element</typeparam>
        /// <param name="array">the array to find in</param>
        /// <param name="element">the element to find</param>
        /// <param name="comparator">determines what it means to be equal</param>
        /// <returns>index if found, -1 if not found</returns>
        public static int FindElementIndex<T>(T[] array, T element, Func<T, T, bool> comparator)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                if (comparator(array[i], element)) return i;
            }
            return -1;
        }

        /// <summary>
        /// Cast a string to an enum item
        /// </summary>
        /// <typeparam name="TEnum">an Enum type</typeparam>
        /// <param name="str">the name of the item</param>
        /// <returns>the item with the name</returns>
        public static TEnum CastStringToEnum<TEnum>(string str) where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an Enum type.");
            }
            else
            {
                bool success = false;
                TEnum something = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray<TEnum>()[0];
                foreach (string name in Enum.GetNames(typeof(TEnum)))
                {
                    if (string.Compare(name, str) == 0)
                    {
                        Enum.TryParse<TEnum>(name, ignoreCase: false, result: out something);
                        success = true;
                        break;
                    }
                }
                if (!success) 
                    throw new InvalidCastException("Failed to find the item of specified name.");
                else
                    return something;
            }
        }

        /// <summary>
        /// Get the current version in integer
        /// </summary>
        /// <returns>Version a.b.c -> 100a + 10b + c</returns>
        public static int GetCurrentVer()
        {
            System.Diagnostics.FileVersionInfo ver = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return (ver.FileMajorPart * 100 + ver.FileMinorPart * 10 + ver.FileBuildPart * 1);
        }
    }
}
