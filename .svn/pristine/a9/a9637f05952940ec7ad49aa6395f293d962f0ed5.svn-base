using FocalSpec.FsApiNet.Model;

namespace Hdc.Measuring
{
    class ProfileProcessor
    {
        private const int MaxLayers = 4;

        private const int MaxPointsInOneLayer = Defines.SensorWidth;

        private const float NotUsed = -1000000;

        private readonly ProfileBuffer _emptyBuffer;

        public ProfileBuffer[] Layers = new ProfileBuffer[MaxLayers];

        public ProfileProcessor()
        {
            for (int i = 0; i < MaxLayers; i++)
            {
                ProfileBuffer newProfile = new ProfileBuffer { Points = new FsApi.Point[MaxPointsInOneLayer] };
                Layers[i] = newProfile;
            }

            _emptyBuffer = new ProfileBuffer { Points = new FsApi.Point[MaxPointsInOneLayer] };
            for (int i = 0; i < _emptyBuffer.Points.Length; i++)
            {
                _emptyBuffer.Points[i].X = NotUsed-1000;
                _emptyBuffer.Points[i].Y = NotUsed-1000;
            }

            ClearBuffer();
        }

        private void ClearBuffer()
        {
            foreach (ProfileBuffer layer in Layers)
            {
                _emptyBuffer.Points.CopyTo(layer.Points, 0);
            }
        }

        /// <summary>
        /// Sort and filter the profile points to layers.
        /// </summary>
        /// <param name="profile">
        /// The profile from camera
        /// </param>
        /// <param name="startX">
        /// The start position [µm] of ROI
        /// </param>
        /// <param name="endX">
        /// The end position [µm] of ROI
        /// </param>
        /// <param name="pixelWidth">
        /// The width [µm] of a pixel
        /// </param>
        /// <param name="selectedLayer">
        /// Defines order of layer selection
        /// </param>
        /// <returns>
        /// true, if success.
        /// </returns>
        public bool SortAndFilter(Profile profile, double startX, double endX, double pixelWidth,ExportLayer selectedLayer)
        {
            ClearBuffer();
                
            foreach (FsApi.Point point in profile.Points)
            {
                // Add filtering if needed
                if (point.X >= startX && point.X < endX)
                {
                    int index = (int)((point.X + pixelWidth / 2) / pixelWidth);
                    for (int l = 0; l < MaxLayers; l++)
                    {
                        if (Layers[l].Points[index].Y < NotUsed)
                        {
                            Layers[l].Points[index] = point;
                            break;
                        }
                    }                    
                }
            }

            foreach (ProfileBuffer layer in Layers)
            {
                layer.PointCount = 0;
            }

            for (int index = 0; index < MaxPointsInOneLayer; index++)
            {
                // Sort layers
                int l;
                for (l = 0; l < MaxLayers; l++)
                {
                    if (Layers[l].Points[index].Y < NotUsed)
                    {
                        break;
                    }

                    for (int j = l + 1; j < MaxLayers; j++)
                    {
                        if (Layers[j].Points[index].Y < NotUsed)
                            break;

                        bool swap = false;

                        if (selectedLayer == ExportLayer.Top)
                            swap = Layers[j].Points[index].Y > Layers[l].Points[index].Y;
                        else if (selectedLayer == ExportLayer.Bottom)
                            swap = Layers[j].Points[index].Y < Layers[l].Points[index].Y;
                        else if (selectedLayer == ExportLayer.Brightest)
                            swap = Layers[j].Points[index].Intensity > Layers[l].Points[index].Intensity;

                        if (swap)
                        {
                            FsApi.Point temp = Layers[l].Points[index];
                            Layers[l].Points[index] = Layers[j].Points[index];
                            Layers[j].Points[index] = temp;
                        }
                    }
                }

                int foundLayers = l;

                // Collect output points
                for (int i = 0; i < foundLayers; i++)
                {
                    ProfileBuffer layer = Layers[i];
                    layer.Points[layer.PointCount++] = layer.Points[index];
                }
            }

            return true;
        }

    }
}