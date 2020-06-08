using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NPacMan.UI.Audio
{
    internal class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer _outputDevice;
        private readonly MixingSampleProvider _mixer;

        private AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            _outputDevice = new WaveOutEvent();
            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            _mixer.ReadFully = true;
            _outputDevice.Init(_mixer);
            _outputDevice.Play();
        }

        public void PlaySound(string fileName)
        {
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == _mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && _mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public void PlaySound(CachedSound sound)
        {
            AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        private void AddMixerInput(ISampleProvider input)
        {
            _mixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        public void Dispose()
        {
            _outputDevice.Dispose();
        }

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(11025, 1);
    }
}