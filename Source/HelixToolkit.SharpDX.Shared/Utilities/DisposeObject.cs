﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
namespace HelixToolkit.UWP
#endif
{
    /// <summary>
    /// Modified version of DisposeBase from SharpDX. Add null check in RemoveAndDispose(ref object)
    /// <para>Base class for a <see cref="IDisposable"/></para>
    /// </summary>
    public abstract class DisposeBase : IDisposable
    {
        /// <summary>
        /// Occurs when this instance is starting to be disposed.
        /// </summary>
        public event EventHandler<BoolArgs> Disposing;

        /// <summary>
        /// Occurs when this instance is fully disposed.
        /// </summary>
        public event EventHandler<BoolArgs> Disposed;


        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="DisposeBase"/> is reclaimed by garbage collection.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
        ~DisposeBase()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.")]
        public void Dispose()
        {
            Dispose(true);
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // TODO Should we throw an exception if this method is called more than once?
            if (!IsDisposed)
            {
                Disposing?.Invoke(this, disposing ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);

                OnDispose(disposing);
                GC.SuppressFinalize(this);

                IsDisposed = true;

                Disposed?.Invoke(this, disposing ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);
                Disposing = null;
                Disposed = null;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected abstract void OnDispose(bool disposing);
    }


    /// <summary>
    /// Modified version of DisposeCollector from SharpDX. Add null check in RemoveAndDispose(ref object)
    /// </summary>
    public abstract class DisposeObject : DisposeBase, INotifyPropertyChanged
    {
        private readonly HashSet<object> disposables = new HashSet<object>();

        /// <summary>
        /// Gets the number of elements to dispose.
        /// </summary>
        /// <value>The number of elements to dispose.</value>
        public int Count
        {
            get { return disposables.Count; }
        }

        /// <summary>
        /// Disposes all object collected by this class and clear the list. The collector can still be used for collecting.
        /// </summary>
        /// <remarks>
        /// To completely dispose this instance and avoid further dispose, use <see cref="OnDispose"/> method instead.
        /// </remarks>
        public virtual void DisposeAndClear()
        {
            foreach(var valueToDispose in disposables)
            {
                if (valueToDispose is IDisposable)
                {
                    ((IDisposable)valueToDispose).Dispose();
                }
                else
                {
                    global::SharpDX.Utilities.FreeMemory((IntPtr)valueToDispose);
                }
            }
            disposables.Clear();
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected override void OnDispose(bool disposeManagedResources)
        {
            DisposeAndClear();
        }

        /// <summary>
        /// Adds a <see cref="IDisposable"/> object or a <see cref="IntPtr"/> allocated using <see cref="global::SharpDX.Utilities.AllocateMemory"/> to the list of the objects to dispose.
        /// </summary>
        /// <param name="toDispose">To dispose.</param>
        /// <exception cref="ArgumentException">If toDispose argument is not IDisposable or a valid memory pointer allocated by <see cref="global::SharpDX.Utilities.AllocateMemory"/></exception>
        public T Collect<T>(T toDispose)
        {
            if(toDispose == null) { return default(T); }
            if (!(toDispose is IDisposable || toDispose is IntPtr))
                throw new ArgumentException("Argument must be IDisposable or IntPtr");

            // Check memory alignment
            if (toDispose is IntPtr)
            {
                var memoryPtr = (IntPtr)(object)toDispose;
                if (!global::SharpDX.Utilities.IsMemoryAligned(memoryPtr))
                    throw new ArgumentException("Memory pointer is invalid. Memory must have been allocated with Utilties.AllocateMemory");
            }

            if (!Equals(toDispose, default(T)) && !disposables.Contains(toDispose))
            {
                disposables.Add(toDispose);
            }
            return toDispose;
        }

        /// <summary>
        /// Dispose a disposable object and set the reference to null. Removes this object from this instance..
        /// </summary>
        /// <param name="objectToDispose">Object to dispose.</param>
        public void RemoveAndDispose<T>(ref T objectToDispose)
        {
            if (objectToDispose != null)
            {
                Remove(objectToDispose);

                var disposableObject = objectToDispose as IDisposable;
                if (disposableObject != null)
                {
                    // Dispose the component
                    disposableObject.Dispose();
                }
                else
                {
                    var localData = (object)objectToDispose;
                    var dataPointer = (IntPtr)localData;
                    global::SharpDX.Utilities.FreeMemory(dataPointer);
                }
                objectToDispose = default(T);
            }
        }

        /// <summary>
        /// Removes a disposable object to the list of the objects to dispose.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toDisposeArg">To dispose.</param>
        public void Remove<T>(T toDisposeArg)
        {
            if (disposables.Contains(toDisposeArg))
            {
                disposables.Remove(toDisposeArg);
            }
        }

        #region INotifyPropertyChanged
        private bool disablePropertyChangedEvent = false;
        /// <summary>
        /// Disable property changed event calling
        /// </summary>
        public bool DisablePropertyChangedEvent
        {
            set
            {
                if (disablePropertyChangedEvent == value)
                {
                    return;
                }
                disablePropertyChangedEvent = value;
                RaisePropertyChanged();
            }
            get
            {
                return disablePropertyChangedEvent;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (!DisablePropertyChangedEvent)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingField"></param>
        /// <param name="value"></param>
        /// <param name="raisePropertyChanged"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T backingField, T value, bool raisePropertyChanged, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            if (raisePropertyChanged)
            { this.RaisePropertyChanged(propertyName); }
            return true;
        }
        #endregion
    }
}
