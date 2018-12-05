// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordContainer.cs" company="FocalSpec Oy">
//   FocalSpec Oy 2016-
// </copyright>
// <summary>
//   Container class for recording points from the camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using FocalSpec.FsApiNet.Model;

namespace Hdc.Measuring
{
    using System.Collections.Generic;

    /// <summary>
    /// Container class for recording points from the camera.
    /// </summary>
    public class RecordContainer
    {
        /// <summary>
        /// The maximum length for the recording queue.
        /// </summary>
        private readonly int _maxLength;

        /// <summary>
        /// Queue for the profiles received from the camera.
        /// </summary>
        private readonly List<Profile> _recordingQueue = new List<Profile>();

        /// <summary>
        /// Sets the max. batch length.
        /// </summary>
        /// <param name="maxLength">The length for the recording. Default value is the value set in defines.</param>
        public RecordContainer(int maxLength)
        {
            _maxLength = maxLength;
        }

        /// <summary>
        /// Gets a value indicating whether all profiles are already collected into queue.
        /// </summary>
        /// <value>True, if collecting is ready.</value>
        public bool IsCollected
        {
            get { return _recordingQueue.Count == _maxLength; }
        }

        /// <summary>
        /// Gets the number of profiles in recording queue.
        /// </summary>
        public int Count
        {
            get
            {
                return _recordingQueue.Count;
            }
        }

        /// <summary>
        /// Adds the points to recording queue.
        /// </summary>
        /// <param name="profile">The received profile.</param>
        /// <returns>True if points were added.</returns>
        public bool AddProfile(Profile profile)
        {
            if (_recordingQueue.Count >= _maxLength)
            {
                return false;
            }

            _recordingQueue.Add(profile);
            return true;
        }

        /// <summary>
        /// Gets a profile from the container. If the requested index doesn't exit, return empty profile.
        /// </summary>
        /// <param name="index">Zero-based index of the profile in the container. </param>
        /// <returns>The profile from the container.</returns>
        public Profile GetProfile(int index)
        {
            if (index >= 0 && index < _recordingQueue.Count)
            {
                return _recordingQueue[index];
            }

            return new Profile(new List<FsApi.Point>(),new FsApi.Header());
        }

        /// <summary>
        /// Gets the profiles.
        /// </summary>
        /// <returns>List of profiles.</returns>
        public List<Profile> GetProfiles()
        {
            return _recordingQueue;
        }

        /// <summary>
        /// Gets the min and max Z values from the profiles.
        /// </summary>
        /// <param name="minZ">Min. Z value [µm] found.</param>
        /// <param name="maxZ">Max. Z value [µm] found.</param>
        public void GetProfilesMinAndMaxZ(out double minZ, out double maxZ)
        {
            minZ = double.NaN;
            maxZ = double.NaN;

            List<Profile> profiles = GetProfiles();

            if (profiles.Count <= 0) return;

            double min = double.MaxValue;
            double max = double.MinValue;

            for (int profileIndex = 0; profileIndex < profiles.Count; profileIndex++)
            {
                Profile profile = profiles[profileIndex];
                for (int i = 0; i < profile.Points.Count; i++)
                {
                    FsApi.Point profilePoint = profile.Points[i];
                    if (profilePoint.Y < min)
                        min = profilePoint.Y;
                    if (profilePoint.Y > max)
                        max = profilePoint.Y;
                }
            }

            minZ = min;
            maxZ = max;
        }
    }
}