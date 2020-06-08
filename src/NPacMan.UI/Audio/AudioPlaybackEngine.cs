using System;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace NPacMan.UI.Audio
{
    internal class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer _outputDevice;
        private readonly MixingSampleProvider _mixer;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            _outputDevice = new WaveOutEvent();
            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            _mixer.ReadFully = true;
            _outputDevice.Init(_mixer);
            _outputDevice.Play();
        }
        
        public void PlaySound(CachedSound sound)
        {
            _mixer.AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        public void Dispose()
        {
            _outputDevice.Dispose();
        }
    }
}