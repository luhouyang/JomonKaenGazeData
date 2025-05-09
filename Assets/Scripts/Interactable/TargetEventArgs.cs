﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.JomonKaenGazeData
{
    /// <summary>
    /// Class specifying targeting event arguments.
    /// </summary>
    public class TargetEventArgs : System.EventArgs
    {
        public EyeTrackingTarget HitTarget { get; private set; }

        public TargetEventArgs(EyeTrackingTarget hitTarget)
        {
            HitTarget = hitTarget;
        }
    }
}
