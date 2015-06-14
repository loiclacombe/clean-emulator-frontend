﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using SynesthesiaM;

namespace CleanEmulatorFrontend
{
    public static class TreeViewExtensions
    {
        /// <summary>
        ///     Selects an item in a TreeView using a Path
        /// </summary>
        /// <param name="treeView">The TreeView to select an item in</param>
        /// <param name="path">
        ///     The Path to the selected item.
        ///     Components of the Path are separated with Path.DirectorySeparatorChar.
        ///     Items in the control are converted by calling the ToString method.
        /// </param>
        public static void SetSelectedItem(this TreeView treeView, string path)
        {
            treeView.SetSelectedItem(path, item => item.ToString());
        }

        /// <summary>
        ///     Selects an item in a TreeView using a Path and a custom conversion method
        /// </summary>
        /// <param name="treeView">The TreeView to select an item in</param>
        /// <param name="path">
        ///     The Path to the selected item.
        ///     Components of the Path are separated with Path.DirectorySeparatorChar.
        /// </param>
        /// <param name="convertMethod">A custom method that converts items in the control to their respective Path component</param>
        public static void SetSelectedItem(this TreeView treeView, string path,
            Func<object, string> convertMethod)
        {
            treeView.SetSelectedItem(path, convertMethod, Path.DirectorySeparatorChar);
        }

        /// <summary>
        ///     Selects an item in a TreeView using a Path and a custom Path separator character.
        /// </summary>
        /// <param name="treeView">The TreeView to select an item in</param>
        /// <param name="path">The Path to the selected item</param>
        /// <param name="separatorChar">The character that separates Path components</param>
        public static void SetSelectedItem(this TreeView treeView, string path,
            char separatorChar)
        {
            treeView.SetSelectedItem(path, item => item.ToString(), separatorChar);
        }

        /// <summary>
        ///     Selects an item in a TreeView using a Path, a custom conversion method,
        ///     and a custom Path separator character.
        /// </summary>
        /// <param name="treeView">The TreeView to select an item in</param>
        /// <param name="path">The Path to the selected item</param>
        /// <param name="convertMethod">A custom method that converts items in the control to their respective Path component</param>
        /// <param name="separatorChar">The character that separates Path components</param>
        public static void SetSelectedItem(this TreeView treeView, string path,
            Func<object, string> convertMethod, char separatorChar)
        {
            treeView.SetSelectedItem(
                path.Split(new[] {separatorChar},
                    StringSplitOptions.RemoveEmptyEntries),
                (x, y) => x == y,
                convertMethod
                );
        }

        /// <summary>
        ///     Selects an item in a TreeView using a custom item chain
        /// </summary>
        /// <typeparam name="T">The type of the items present in the control and the chain</typeparam>
        /// <param name="treeView">The TreeView to select an item in</param>
        /// <param name="items">The chain of items to walk. The last item in the chain will be selected</param>
        public static void SetSelectedItem<T>(this TreeView treeView, IEnumerable<T> items)
            where T : class
        {
            // Use a default compare method with the '==' operator
            treeView.SetSelectedItem(items,
                (x, y) => x == y
                );
        }

        /// <summary>
        ///     Selects an item in a TreeView using a custom item chain and item comparison method
        /// </summary>
        /// <typeparam name="T">The type of the items present in the control and the chain</typeparam>
        /// <param name="treeView">The TreeView to select an item in</param>
        /// <param name="items">The chain of items to walk. The last item in the chain will be selected</param>
        /// <param name="compareMethod">The method used to compare items in the control with items in the chain</param>
        public static void SetSelectedItem<T>(this TreeView treeView, IEnumerable<T> items,
            Func<T, T, bool> compareMethod)
        {
            treeView.SetSelectedItem(items, compareMethod, null);
        }

        /// <summary>
        ///     Selects an item in a TreeView using a custom item chain, an item comparison method,
        ///     and an item conversion method.
        /// </summary>
        /// <typeparam name="T">The type of the items present in the control and the chain</typeparam>
        /// <param name="treeView">The TreeView to select an item in</param>
        /// <param name="items">The chain of items to walk. The last item in the chain will be selected</param>
        /// <param name="compareMethod">The method used to compare items in the control with items in the chain</param>
        /// <param name="convertMethod">The method used to convert items in the control to be compared with items in the chain</param>
        public static void SetSelectedItem<T>(this TreeView treeView, IEnumerable<T> items,
            Func<T, T, bool> compareMethod, Func<object, T> convertMethod)
        {
            // Setup default options for a TreeView
            UIUtility.SetSelectedItem(treeView,
                new SetSelectedInfo<T>
                {
                    Items = items,
                    CompareMethod = compareMethod,
                    ConvertMethod = convertMethod,
                    OnSelected = delegate(ItemsControl container, SetSelectedInfo<T> info)
                    {
                        var treeItem = (TreeViewItem) container;
                        treeItem.IsSelected = true;
                        treeItem.BringIntoView();
                    },
                    OnNeedMoreItems =
                        delegate(ItemsControl container, SetSelectedInfo<T> info)
                        {
                            ((TreeViewItem) container).IsExpanded = true;
                        }
                }
                );
        }
    }
}