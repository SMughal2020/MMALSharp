﻿// <copyright file="MMALEncoderBase.cs" company="Techyian">
// Copyright (c) Techyian. All rights reserved.
// Licensed under the MIT License. Please see LICENSE.txt for License info.
// </copyright>

using System;
using System.Runtime.InteropServices;
using System.Text;
using MMALSharp.Handlers;
using MMALSharp.Native;
using static MMALSharp.MMALCallerHelper;

namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a base class for all encoder components
    /// </summary>
    public abstract class MMALEncoderBase : MMALDownstreamHandlerComponent
    {
        protected MMALEncoderBase(string encoderName, ICaptureHandler handler)
            : base(encoderName, handler)
        {
        }

        /// <summary>
        /// Annotates the image with various text as specified in the MMALCameraConfig class.
        /// </summary>
        internal unsafe void AnnotateImage()
        {
            if (MMALCameraConfig.Annotate != null)
            {
                MMALLog.Logger.Debug("Setting annotate");

                var sb = new StringBuilder();

                var showShutter = 0;
                var showAnalogGain = 0;
                var showLens = 0;
                var showCaf = 0;
                var showMotion = 0;
                var showFrame = 0;
                var enableTextBackground = 0;
                var textSize = (byte)0;
                var customTextColor = 0;
                var customTextY = (byte)0;
                var customTextU = (byte)0;
                var customTextV = (byte)0;
                var customBackgroundColor = 0;
                var customBackgroundY = (byte)0;
                var customBackgroundU = (byte)0;
                var customBackgroundV = (byte)0;

                if (!string.IsNullOrEmpty(MMALCameraConfig.Annotate.CustomText))
                {
                    sb.Append(MMALCameraConfig.Annotate.CustomText + " ");
                }

                if (MMALCameraConfig.Annotate.ShowTimeText)
                {
                    sb.Append(DateTime.Now.ToString("HH:mm") + " ");
                }

                if (MMALCameraConfig.Annotate.ShowDateText)
                {
                    sb.Append(DateTime.Now.ToString("dd/MM/yyyy") + " ");
                }

                if (MMALCameraConfig.Annotate.ShowShutterSettings)
                {
                    showShutter = 1;
                }

                if (MMALCameraConfig.Annotate.ShowGainSettings)
                {
                    showAnalogGain = 1;
                }

                if (MMALCameraConfig.Annotate.ShowLensSettings)
                {
                    showLens = 1;
                }

                if (MMALCameraConfig.Annotate.ShowCafSettings)
                {
                    showCaf = 1;
                }

                if (MMALCameraConfig.Annotate.ShowMotionSettings)
                {
                    showMotion = 1;
                }

                if (MMALCameraConfig.Annotate.ShowFrameNumber)
                {
                    showFrame = 1;
                }

                if (MMALCameraConfig.Annotate.ShowBlackBackground)
                {
                    enableTextBackground = 1;
                }

                textSize = System.Convert.ToByte(MMALCameraConfig.Annotate.TextSize);

                if (MMALCameraConfig.Annotate.TextColour != -1)
                {
                    customTextColor = 1;
                    customTextY = Convert.ToByte(MMALCameraConfig.Annotate.TextColour & 0xff);
                    customTextU = Convert.ToByte((MMALCameraConfig.Annotate.TextColour >> 8) & 0xff);
                    customTextV = Convert.ToByte((MMALCameraConfig.Annotate.TextColour >> 16) & 0xff);
                }

                if (MMALCameraConfig.Annotate.BgColour != -1)
                {
                    customBackgroundColor = 1;
                    customBackgroundY = Convert.ToByte(MMALCameraConfig.Annotate.BgColour & 0xff);
                    customBackgroundU = Convert.ToByte((MMALCameraConfig.Annotate.BgColour >> 8) & 0xff);
                    customBackgroundV = Convert.ToByte((MMALCameraConfig.Annotate.BgColour >> 16) & 0xff);
                }

                string t = sb.ToString() + char.MinValue;

                var text = Encoding.ASCII.GetBytes(t);

                var str = new MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T(
                    new MMAL_PARAMETER_HEADER_T(MMALParametersCamera.MMAL_PARAMETER_ANNOTATE, Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>() + t.Length),
                                                                                                    1, showShutter, showAnalogGain, showLens,
                                                                                                    showCaf, showMotion, showFrame, enableTextBackground,
                                                                                                    customBackgroundColor, customBackgroundY, customBackgroundU, customBackgroundV, 0, customTextColor,
                                                                                                    customTextY, customTextU, customTextV, textSize, text);

                var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<MMAL_PARAMETER_CAMERA_ANNOTATE_V3_T>());
                Marshal.StructureToPtr(str, ptr, false);

                MMALCheck(MMALPort.mmal_port_parameter_set(MMALCamera.Instance.Camera.Control.Ptr, (MMAL_PARAMETER_HEADER_T*)ptr), "Unable to set annotate");

                Marshal.FreeHGlobal(ptr);
            }
        }

        internal void ConfigureOutputPort(int outputPort, MMALEncoding encodingType, int quality, int bitrate)
        {
            throw new NotImplementedException();
        }
    }
}
