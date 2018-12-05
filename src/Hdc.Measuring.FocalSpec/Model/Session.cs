// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Session.cs" company="FocalSpec Oy">
//   FocalSpec Oy 2016-
// </copyright>
// <summary>
//   Session object holds the global settings related to the current session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hdc.Measuring
{
    /// <summary>
    /// Global settings related to the current session.
    /// </summary>
    public static class Session
    {
        /// <summary>
        /// Gets or sets the view mode.
        /// </summary>
        public static ViewMode ViewMode { get; set; }

        /// <summary>
        /// Camera settings used in the live (not batch) mode.
        /// </summary>
        public class RealTimeCameraSetting
        {
            /// <summary>
            /// Gets or sets the frequency [Hz].
            /// </summary>
            public static int Freq { get; set; }

            /// <summary>
            /// Gets or sets the flag indicating whether external pulsing is enabled or not.
            /// </summary>
            public static bool IsExternalPulsingEnabled { get; set; }
        }
    }
}