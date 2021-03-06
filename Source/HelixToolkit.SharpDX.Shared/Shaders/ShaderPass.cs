﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public sealed class NullShaderPass : IShaderPass
    {
        /// <summary>
        /// The null pass
        /// </summary>
        public static readonly NullShaderPass NullPass = new NullShaderPass();
        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is null; otherwise, <c>false</c>.
        /// </value>
        public bool IsNULL { get; } = true;
        /// <summary>
        /// Gets the state of the blend.
        /// </summary>
        /// <value>
        /// The state of the blend.
        /// </value>
        public BlendStateProxy BlendState
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the state of the depth stencil.
        /// </summary>
        /// <value>
        /// The state of the depth stencil.
        /// </value>
        public DepthStencilStateProxy DepthStencilState
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return "NULL";
            }
        }
        /// <summary>
        /// Gets the state of the raster.
        /// </summary>
        /// <value>
        /// The state of the raster.
        /// </value>
        public RasterizerStateProxy RasterState
        {
            get
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the shaders.
        /// </summary>
        /// <value>
        /// The shaders.
        /// </value>
        public IEnumerable<IShader> Shaders
        {
            get
            {
                return new IShader[0];
            }
        }
        /// <summary>
        /// Binds the shader.
        /// </summary>
        /// <param name="context">The context.</param>
        public void BindShader(IDeviceContext context)
        {
            
        }

        /// <summary>
        /// Binds the shader.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="bindConstantBuffer"></param>
        public void BindShader(IDeviceContext context, bool bindConstantBuffer)
        {

        }
        /// <summary>
        /// Binds the states.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        public void BindStates(IDeviceContext context, StateType type)
        {

        }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
        }
        /// <summary>
        /// Gets the shader.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IShader GetShader(ShaderStage type)
        {
            switch (type)
            {
                case ShaderStage.Pixel:
                    return NullShader.PixelNull;
                case ShaderStage.Vertex:
                    return NullShader.VertexNull;
                case ShaderStage.Geometry:
                    return NullShader.GeometryNull;
                case ShaderStage.Compute:
                    return NullShader.ComputeNull;
                case ShaderStage.Domain:
                    return NullShader.DomainNull;
                case ShaderStage.Hull:
                    return NullShader.HullNull;
                default:
                    return new NullShader(type);
            }            
        }
        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public void SetShader(IShader shader) { }
    }

    /// <summary>
    /// Shader Pass
    /// </summary>
    public sealed class ShaderPass : DisposeObject, IShaderPass
    {
        /// <summary>
        /// <see cref="IShaderPass.Name"/>
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsNULL { get; } = false;
        /// <summary>
        /// 
        /// </summary>
        public const int VertexIdx = 0, HullIdx = 1, DomainIdx = 2, GeometryIdx = 3, PixelIdx = 4, ComputeIdx = 5;
        private readonly IShader[] shaders = new IShader[6];
        /// <summary>
        /// <see cref="IShaderPass.Shaders"/>
        /// </summary>
        public IEnumerable<IShader> Shaders { get { return shaders; } }
        /// <summary>
        /// <see cref="IShaderPass.BlendState"/>
        /// </summary>
        public BlendStateProxy BlendState { private set; get; } = null;
        /// <summary>
        /// <see cref="IShaderPass.DepthStencilState"/>
        /// </summary>
        public DepthStencilStateProxy DepthStencilState { private set; get; } = null;
        /// <summary>
        /// <see cref="IShaderPass.RasterState"/>
        /// </summary>
        public RasterizerStateProxy RasterState { private set; get; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passDescription"></param>
        /// <param name="manager"></param>
        public ShaderPass(ShaderPassDescription passDescription, IEffectsManager manager)
        {
            Name = passDescription.Name;

            if (passDescription.ShaderList != null)
            {
                foreach (var shader in passDescription.ShaderList)
                {
                    shaders[GetShaderArrayIndex(shader.ShaderType)] = manager.ShaderManager.RegisterShader(shader);
                }
            }
            for(int i=0; i<shaders.Length; ++i)
            {
                if (shaders[i] == null)
                {
                    var type = GetShaderStageByArrayIndex(i);
                    switch (type)
                    {
                        case ShaderStage.Vertex:
                            shaders[i] = NullShader.VertexNull;
                            break;
                        case ShaderStage.Hull:
                            shaders[i] = NullShader.HullNull;
                            break;
                        case ShaderStage.Domain:
                            shaders[i] = NullShader.DomainNull;
                            break;
                        case ShaderStage.Geometry:
                            shaders[i] = NullShader.GeometryNull;
                            break;
                        case ShaderStage.Pixel:
                            shaders[i] = NullShader.PixelNull;
                            break;
                        case ShaderStage.Compute:
                            shaders[i] = NullShader.ComputeNull;
                            break;
                    }
                }
            }

            BlendState = passDescription.BlendStateDescription != null ? Collect(new BlendStateProxy(manager.StateManager)) : null;
            if(BlendState != null)
            {
                BlendState.Description = (BlendStateDescription)passDescription.BlendStateDescription;
            }

            DepthStencilState = passDescription.DepthStencilStateDescription != null ? Collect(new DepthStencilStateProxy(manager.StateManager)) : null;
            if(DepthStencilState != null)
            {
                DepthStencilState.Description = (DepthStencilStateDescription)passDescription.DepthStencilStateDescription;
            }

            RasterState = passDescription.RasterStateDescription != null ? Collect(new RasterizerStateProxy(manager.StateManager)) : null;
            if(RasterState != null)
            {
                RasterState.Description = (RasterizerStateDescription)passDescription.RasterStateDescription;
            }
        }

        /// <summary>
        /// Convert shader stage to internal array index
        /// </summary>
        /// <param name="stage"></param>
        /// <returns></returns>
        public static int GetShaderArrayIndex(ShaderStage stage)
        {
            switch (stage)
            {
                case ShaderStage.Vertex:
                    return VertexIdx;
                case ShaderStage.Domain:
                    return DomainIdx;
                case ShaderStage.Hull:
                    return HullIdx;
                case ShaderStage.Geometry:
                    return GeometryIdx;
                case ShaderStage.Pixel:
                    return PixelIdx;
                case ShaderStage.Compute:
                    return ComputeIdx;
                default:
                    return -1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrayIndex"></param>
        /// <returns></returns>
        public static ShaderStage GetShaderStageByArrayIndex(int arrayIndex)
        {
            switch (arrayIndex)
            {
                case VertexIdx:
                    return ShaderStage.Vertex;
                case DomainIdx:
                    return ShaderStage.Domain;
                case HullIdx:
                    return ShaderStage.Hull;
                case GeometryIdx:
                    return ShaderStage.Geometry;
                case PixelIdx:
                    return ShaderStage.Pixel;
                case ComputeIdx:
                    return ShaderStage.Compute;
                default:
                    return ShaderStage.None;
            }
        }

        /// <summary>
        /// Bind shaders and its constant buffer for this technique
        /// </summary>
        /// <param name="context"></param>
        public void BindShader(IDeviceContext context)
        {
            BindShader(context, true);
        }

        /// <summary>
        /// Bind shaders and its constant buffer for this technique
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bindConstantBuffer"></param>
        public void BindShader(IDeviceContext context, bool bindConstantBuffer)
        {
            if (context.LastShaderPass == this)
            {
                return;
            }
            foreach (var shader in Shaders)
            {
                shader.Bind(context.DeviceContext);
                if (bindConstantBuffer)
                {
                    shader.BindConstantBuffers(context.DeviceContext);
                }
            }
            context.LastShaderPass = this;
        }
        /// <summary>
        /// <see cref="IShaderPass.GetShader(ShaderStage)"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IShader GetShader(ShaderStage type)
        {
            return shaders[GetShaderArrayIndex(type)];
        }

        /// <summary>
        /// Sets the shader.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public void SetShader(IShader shader)
        {
            shaders[GetShaderArrayIndex(shader.ShaderType)] = shader;
        }

        /// <summary>
        /// Binds the states.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="type">The type.</param>
        public void BindStates(IDeviceContext context, StateType type)
        {
            if (type == StateType.None)
            {
                return;
            }
            if (type.HasFlag(StateType.BlendState))
            {
                context.DeviceContext.OutputMerger.BlendState = BlendState;
            }
            if (type.HasFlag(StateType.DepthStencilState))
            {
                context.DeviceContext.OutputMerger.DepthStencilState = DepthStencilState;
            }
            if (type.HasFlag(StateType.RasterState))
            {
                context.DeviceContext.Rasterizer.State = RasterState;
            }
        }
    }
}
