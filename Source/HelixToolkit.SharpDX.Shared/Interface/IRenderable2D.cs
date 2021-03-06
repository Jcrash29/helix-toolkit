﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRenderCore2D
    {        
        /// <summary>
        /// Occurs when [on invalidate renderer].
        /// </summary>
        event EventHandler<EventArgs> OnInvalidateRenderer;
        /// <summary>
        /// Gets or sets the bound.
        /// </summary>
        /// <value>
        /// The bound.
        /// </value>
        RectangleF LayoutBound { set; get; }

        /// <summary>
        /// Gets or sets the clipping bound.
        /// </summary>
        /// <value>
        /// The clipping bound.
        /// </value>
        RectangleF LayoutClippingBound { set; get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Transform to render position relative to its parent
        /// </summary>
        Matrix3x2 Transform { set; get; }        
        /// <summary>
        /// Local render transform
        /// </summary>
        Matrix3x2 LocalTransform { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is rendering.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is rendering; otherwise, <c>false</c>.
        /// </value>
        bool IsRendering { set; get; }
        /// <summary>
        /// Attaches the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Attach(IRenderHost target);
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        void Detach();
        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        void Render(IRenderContext2D context);
        /// <summary>
        /// Gets or sets a value indicating whether this instance is mouse over.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
        /// </value>
        bool IsMouseOver { set; get; }
    }

    public interface IRenderable2D : ITransform2D, IGUID
    {
        /// <summary>
        /// Gets a value indicating whether this instance is renderable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is renderable; otherwise, <c>false</c>.
        /// </value>
        bool IsRenderable { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is measure dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is measure dirty; otherwise, <c>false</c>.
        /// </value>
        bool IsMeasureDirty { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is arrange dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is arrange dirty; otherwise, <c>false</c>.
        /// </value>
        bool IsArrangeDirty { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is transform dirty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is transform dirty; otherwise, <c>false</c>.
        /// </value>
        bool IsTransformDirty { get; }
        /// <summary>
        /// Attaches the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        void Attach(IRenderHost target);
        /// <summary>
        /// Detaches this instance.
        /// </summary>
        void Detach();
        /// <summary>
        /// Optional for scene graph traverse
        /// </summary>
        IEnumerable<IRenderable2D> Items { get; }

        IRenderCore2D RenderCore { get; }
        /// <summary>
        /// Update render related parameters such as model matrix by scene graph and bounding boxes
        /// </summary>
        /// <param name="context"></param>
        void Update(IRenderContext2D context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void Render(IRenderContext2D context);
        /// <summary>
        /// Renders the bitmap cache to render target. If bitmap cache is disabled, nothing will be rendered
        /// </summary>
        /// <param name="context">The context.</param>
        void RenderBitmapCache(IRenderContext2D context);

        void Measure(Size2F size);
        void Arrange(RectangleF rect);
        void InvalidateArrange();
        void InvalidateMeasure();
        void InvalidateVisual();
        void InvalidateTransform();
        void InvalidateAll();
    }

    public interface IHitable2D
    {
        bool HitTest(Vector2 mousePoint, out HitTest2DResult hitResult);
        bool IsHitTestVisible { set; get; }
    }
}
