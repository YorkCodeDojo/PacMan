using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NAudio.Wave;

namespace NPacMan.UI.Audio
{
    internal class CachedSound
    {
        public float[] AudioData { get; }
        public WaveFormat WaveFormat { get; }

        public CachedSound(UnmanagedMemoryStream ums)
        {
            using var waveFileReader = new WaveFileReader(ums);

            WaveFormat = waveFileReader.WaveFormat;
            var wholeFile = new List<float>((int)(waveFileReader.Length / 4));
            var sampleProvider = waveFileReader.ToSampleProvider();
            var sourceSamples = (int)(waveFileReader.Length / (waveFileReader.WaveFormat.BitsPerSample / 8));
            var sampleData = new float[sourceSamples];
            int samplesRead;
            while ((samplesRead = sampleProvider.Read(sampleData, 0, sourceSamples)) > 0)
            {
                wholeFile.AddRange(sampleData.Take(samplesRead));
            }
            AudioData = wholeFile.ToArray();
        }
    }
}
