// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportBatch.cs" company="FocalSpec Oy">
//   FocalSpec Oy 2016-
// </copyright>
// <summary>
//   Exports profile batches.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FocalSpec.FsApiNet.Model;

namespace Hdc.Measuring
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    /// <summary>
    /// Batch export utility.
    /// </summary>
    public class ExportBatch
    {
        /// <summary>
        /// Appends to point cloud profile to an existing file.
        /// </summary>
        /// <param name="file">The file where the points should be appended to.</param>
        /// <param name="points">List of points.</param>
        /// <param name="batchStepLength">The Y distance [mm] between two subsequent profiles in a batch.</param>
        /// <param name="index">Zero-based index of the profile.</param>
        private static void AppendToPointCloudFile(string file, List<FsApi.Point> points, double batchStepLength, int index)
        {
            StringBuilder str = new StringBuilder();

            foreach (FsApi.Point point in points)
            {
                string row = string.Format(CultureInfo.InvariantCulture, "{0:0.000000} {1:0.000000} {2:0.000000}", point.X * Defines.ProfileScale, index * batchStepLength, point.Y * Defines.ProfileScale);
                str.AppendLine(row);
            }

            File.AppendAllText(file, str.ToString());
        }

        /// <summary>
        /// Saves a list of profiles to point cloud file. Overwrites the file if it exists. All dimensions (X,Y,Z) are in [mm].
        /// </summary>
        /// <param name="file">File where export should be saved to. </param>
        /// <param name="profiles">The profiles to be exported. </param>
        /// <param name="batchStepLength">The Y distance [mm] between two subsequent profiles in a batch.</param>
        public static void SaveToPointCloudFile(string file, List<Profile> profiles, double batchStepLength)
        {
            int index = 0;

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            if (file.ToLower().EndsWith(".bmp"))
            {
                SaveToBitmapFile(file, profiles, batchStepLength);
                return;
            }

            foreach (Profile profile in profiles)
            {
                AppendToPointCloudFile(file, profile.Points.ToList(), batchStepLength, index++);
            }
        }

        /// <summary>
        /// Save to bitmap file.
        /// </summary>
        /// <param name="file">
        /// The output file.
        /// </param>
        /// <param name="profiles">
        /// The profiles to be exported. Does not support multiple layers.
        /// </param>
        /// <param name="batchStepLength">
        /// The Y distance [mm] between two subsequent profiles in a batch.
        /// </param>
        public static void SaveToBitmapFile(string file, List<Profile> profiles, double batchStepLength)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            byte[] rawImage = new byte[Defines.SensorWidth * profiles.Count];
            Array.Clear(rawImage, 0, rawImage.Length);

            uint lineStart = 0;
            for (var i = 0; i < 10; i++)
            {
                if (i == profiles.Count)
                {
                    break;
                }

                if (i == 0 || profiles[i].Header.Index < lineStart)
                {
                    lineStart = profiles[i].Header.Index;
                }
            }

            foreach (Profile profile in profiles)
            {
                foreach (FsApi.Point point in profile.Points)
                {
                    var column = (int)(point.X / Defines.PixelWidth);
                    if (column >= 0 && column < Defines.SensorWidth)
                    {
                        var line = profile.Header.Index - lineStart;
                        if (line < profiles.Count)
                        {
                            rawImage[(line * Defines.SensorWidth) + column] = (byte)point.Intensity;
                        }
                    }
                }
            }

            Bitmap image = new Bitmap(
                Defines.SensorWidth,
                profiles.Count,
                Defines.SensorWidth,
                PixelFormat.Format8bppIndexed,
                Marshal.UnsafeAddrOfPinnedArrayElement(rawImage, 0));

            // Create grayscale entries
            ColorPalette palette = image.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            image.Palette = palette;

            // set resolution as dots per inch
            image.SetResolution((float)(1000.0 / Defines.PixelWidth * 25.4), (float)(1000.0 / batchStepLength * 25.4));

            image.Save(file);
        }
    }
}