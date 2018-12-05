// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportCsv.cs" company="FocalSpec Ltd">
// FocalSpec Ltd 2016-
// </copyright>
// <summary>
// Exports profiles into CSV files. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FocalSpec.FsApiNet.Model;

namespace Hdc.Measuring
{
    /// <summary>
    /// Export data in CSV format.
    /// </summary>
    public static class ExportCsv
    {
        /// <summary>
        /// Exports a profile to a file.
        /// </summary>
        /// <param name="file">Export file name.</param>
        /// <param name="profile">Profile to export.</param>
        public static void Export(string file, Profile profile)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("X [mm];Z [mm];Intensity");

            foreach (FsApi.Point point in profile.Points)
            {
                str.AppendLine(string.Format("{0:0.000000};{1:0.000000};{2}", point.X * Defines.ProfileScale, point.Y * Defines.ProfileScale, point.Intensity));
            }

            File.WriteAllText(file, str.ToString());
        }

        /// <summary>
        /// Filters a layer from a profile.
        /// </summary>
        /// <param name="profile">Input profile.</param>
        /// <param name="layerType">Layer to extract.</param>
        /// <param name="layer">Extracted layer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Unknown target layer.</exception>
        public static void FilterProfile(IList<FsApi.Point> profile, ExportLayer layerType, out IList<FsApi.Point> layer)
        {
            switch (layerType)
            {
                case ExportLayer.All:
                    layer = profile;
                    break;

                case ExportLayer.Bottom:
                    layer = profile.OrderBy(p => p.X)
                            .ThenBy(p => p.Y)
                            .GroupBy(p => p.X)
                            .Select(p => p.First())
                            .ToList();
                    break;

                case ExportLayer.Top:
                    layer = profile.OrderBy(p => p.X)
                            .ThenByDescending(p => p.Y)
                            .GroupBy(p => p.X)
                            .Select(p => p.First())
                            .ToList();
                    break;

                case ExportLayer.Brightest:
                    layer = profile.OrderByDescending(p => p.Intensity)
                            .ThenBy(p => p.Y)
                            .GroupBy(p => p.X)
                            .Select(p => p.First())
                            .ToList();
                    break;

                default:
                    throw new ArgumentOutOfRangeException("layerType", layerType, null);
            }
        }
    }
}