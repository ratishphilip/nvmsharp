// Copyright (c) 2008 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// The WeakEvent was published by Daniel Grunwald on CodeProject:
// http://www.codeproject.com/KB/cs/WeakEvents.aspx
//
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace NVMSharp.Common
{
    /// <summary>
    /// This class allows the creation of a Weak Event
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// 
    ///        //DECLARING
    ///        private readonly WeakEvent<EventHandler<EventArgs>> 
    ///             dependencyChangedEvent =
    ///                 new WeakEvent<EventHandler<EventArgs>>();
    ///
    ///        public event EventHandler<EventArgs> DependencyChanged
    ///        {
    ///          add { dependencyChangedEvent.Add(value); }
    ///          remove { dependencyChangedEvent.Remove(value); }
    ///        }
    ///
    ///        //RAISING
    ///        dependencyChangedEvent.Raise(this, new EventArgs());
    /// 
    /// 
    ///        //SUBSCRIBING
    ///        SourceDependency.DependencyChanged += OnSourceChanged;
    ///        ...
    ///        private void OnSourceChanged(object sender, EventArgs e)
    ///        {
    ///        
    ///        }
    /// ]]>
    /// </example>
    [DebuggerNonUserCode]
    public sealed class WeakEvent<T> where T : class
    {
        #region EventEntry

        [DebuggerNonUserCode]
        struct EventEntry
        {
            #region Public Data
            public readonly FastSmartWeakEventForwarderProvider.ForwarderDelegate Forwarder;
            public readonly MethodInfo TargetMethod;
            public readonly WeakReference TargetReference;
            #endregion

            #region Ctor
            /// <summary>
            /// Creates a new EventEntry Struct
            /// </summary>
            public EventEntry(FastSmartWeakEventForwarderProvider.ForwarderDelegate Forwarder, MethodInfo targetMethod, WeakReference targetReference)
            {
                this.Forwarder = Forwarder;
                this.TargetMethod = targetMethod;
                this.TargetReference = targetReference;
            }
            #endregion
        }

        #endregion

        #region Data

        readonly List<EventEntry> eventEntries = new List<EventEntry>();

        #endregion

        #region Ctor

        /// <summary>
        /// Constructs a new WeakEvent
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        static WeakEvent()
        {
            if (!typeof(T).IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("T must be a delegate type");

            MethodInfo invoke = typeof(T).GetMethod("Invoke");
            if (invoke == null || invoke.GetParameters().Length != 2)
                throw new ArgumentException("T must be a delegate type taking 2 parameters");

            ParameterInfo senderParameter = invoke.GetParameters()[0];
            if (senderParameter.ParameterType != typeof(object))
                throw new ArgumentException("The first delegate parameter must be of type 'object'");

            ParameterInfo argsParameter = invoke.GetParameters()[1];
            if (!(typeof(EventArgs).IsAssignableFrom(argsParameter.ParameterType)))
                throw new ArgumentException("The second delegate parameter must be derived from type 'EventArgs'. Type is " + argsParameter.ParameterType);

            if (invoke.ReturnType != typeof(void))
                throw new ArgumentException("The delegate return type must be void.");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add an event handler
        /// </summary>
        /// <param name="eh">The handler to add</param>
        public void Add(T eh)
        {
            if (eh != null)
            {
                Delegate d = (Delegate)(object)eh;
                if (eventEntries.Count == eventEntries.Capacity)
                    RemoveDeadEntries();

                WeakReference target = d.Target != null ? new WeakReference(d.Target) : null;
                eventEntries.Add(new EventEntry(FastSmartWeakEventForwarderProvider.GetForwarder(d.Method), d.Method, target));
            }
        }

        /// <summary>
        /// Clear event handlers
        /// </summary>
        public void Clear()
        {
            eventEntries.Clear();
        }

        /// <summary>
        /// removes a specific Event handler
        /// </summary>
        /// <param name="eh">Event handler to remove</param>
        public void Remove(T eh)
        {
            if (eh != null)
            {
                Delegate d = (Delegate)(object)eh;
                for (int i = eventEntries.Count - 1; i >= 0; i--)
                {
                    EventEntry entry = eventEntries[i];
                    if (entry.TargetReference != null)
                    {
                        object target = entry.TargetReference.Target;
                        if (target == null)
                        {
                            eventEntries.RemoveAt(i);
                        }
                        else if (target == d.Target && entry.TargetMethod == d.Method)
                        {
                            eventEntries.RemoveAt(i);
                            break;
                        }
                    }
                    else
                    {
                        if (d.Target == null && entry.TargetMethod == d.Method)
                        {
                            eventEntries.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raise event
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The EventArgs to use for event</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate"), 
         System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        public void Raise(object sender, EventArgs e)
        {
            bool needsCleanup = eventEntries.ToArray().Aggregate(false, (current, ee) => current | ee.Forwarder(ee.TargetReference, sender, e));
            if (needsCleanup)
                RemoveDeadEntries();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Remove dead entries
        /// </summary>
        private void RemoveDeadEntries()
        {
            eventEntries.RemoveAll(ee => ee.TargetReference != null && !ee.TargetReference.IsAlive);
        }
        #endregion

    }

    /// <summary>
    /// Uses Reflection.Emit to create Dynamic delegate to use for 
    /// event forwarding
    /// </summary>
    [DebuggerNonUserCode]
    static class FastSmartWeakEventForwarderProvider
    {
        #region Data
        static readonly MethodInfo getTarget = typeof(WeakReference).GetMethod("get_Target");
        static readonly Type[] forwarderParameters = { typeof(WeakReference), typeof(object), typeof(EventArgs) };
        internal delegate bool ForwarderDelegate(WeakReference wr, object sender, EventArgs e);

        static readonly Dictionary<MethodInfo, ForwarderDelegate> forwarders =
            new Dictionary<MethodInfo, ForwarderDelegate>();
        #endregion

        #region Internal methods
        /// <summary>
        /// Dynamically emits a delegate for event forwarding
        /// </summary>
        /// <param name="method">The methodInfo to use for forward</param>
        /// <returns>Delegate for event forwarding</returns>
        internal static ForwarderDelegate GetForwarder(MethodInfo method)
        {
            lock (forwarders)
            {
                ForwarderDelegate d;
                if (forwarders.TryGetValue(method, out d))
                    return d;
            }

            if ((method == null) || (method.DeclaringType == null))
                throw new ArgumentNullException(nameof(method));

            if (method.DeclaringType.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0)
                throw new ArgumentException("Cannot create weak event to anonymous method with closure.");

            Debug.Assert(getTarget != null);

            DynamicMethod dm = new DynamicMethod(
                "WeakEvent", typeof(bool), forwarderParameters, method.DeclaringType);

            ILGenerator il = dm.GetILGenerator();

            if (!method.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Callvirt, getTarget, null);
                il.Emit(OpCodes.Dup);
                Label label = il.DefineLabel();
                il.Emit(OpCodes.Brtrue, label);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                il.MarkLabel(label);
            }
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.EmitCall(OpCodes.Call, method, null);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);

            ForwarderDelegate fd = (ForwarderDelegate)dm.CreateDelegate(typeof(ForwarderDelegate));
            lock (forwarders)
            {
                forwarders[method] = fd;
            }

            return fd;
        }
        #endregion
    }
}