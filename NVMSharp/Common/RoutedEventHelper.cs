// Copyright (c) 2015 Ratish Philip 
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
// 
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software. 
// 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE. 

using System;
using System.Windows;

namespace NVMSharp.Common
{
    internal static class RoutedEventHelper
    {
        #region RoutedEvent Helper Methods

        /// <summary>
        /// A static helper method to raise a routed event on a target UIElement or ContentElement.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <param name="args">RoutedEventArgs to use when raising the event</param>
        internal static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
        {
            if (target is UIElement)
            {
                (target as UIElement).RaiseEvent(args);
            }
            else if (target is ContentElement)
            {
                (target as ContentElement).RaiseEvent(args);
            }
        }

        /// <summary>
        /// A static helper method that adds a handler for a routed event 
        /// to a target UIElement or ContentElement.
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="routedEvent">Event that will be handled</param>
        /// <param name="handler">Event handler to be added</param>
        internal static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            UIElement uie = element as UIElement;
            if (uie != null)
            {
                uie.AddHandler(routedEvent, handler);
            }
            else
            {
                ContentElement ce = element as ContentElement;
                ce?.AddHandler(routedEvent, handler);
            }
        }

        /// <summary>
        /// A static helper method that removes a handler for a routed event 
        /// from a target UIElement or ContentElement.
        /// </summary>
        /// <param name="element">UIElement or ContentElement that listens to the event</param>
        /// <param name="routedEvent">Event that will no longer be handled</param>
        /// <param name="handler">Event handler to be removed</param>
        internal static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            UIElement uie = element as UIElement;
            if (uie != null)
            {
                uie.RemoveHandler(routedEvent, handler);
            }
            else
            {
                ContentElement ce = element as ContentElement;
                ce?.RemoveHandler(routedEvent, handler);
            }
        }

        #endregion
    }
}