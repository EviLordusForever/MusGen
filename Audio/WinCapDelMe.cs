using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Mixer;
using System.Threading;

public static class WinCapDelMe
{
    private static AudioRenderClient _renderClient;
    public static List<float> _samples;

    public static void Start(int sampleRate)
    {
        _samples = new List<float>();

        /*		List<Mixer> deviceList = new List<Mixer>();
                for (int i = 0; i < Mixer.NumberOfDevices; i++)
                    deviceList.Add(new Mixer(i));*/

       
        var enumerator = new MMDeviceEnumerator();
        var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

        //var device = enumerator.GetDevice();
        var audioClient = device.AudioClient;
        WaveFormat wf = new WaveFormat(sampleRate, 16, 1);
        audioClient.Initialize(AudioClientShareMode.Shared, AudioClientStreamFlags.None, 100000, 0, wf, Guid.Empty);
        int b = audioClient.BufferSize;
        _renderClient = audioClient.AudioRenderClient;
        audioClient.Start();

        Thread myThread = new Thread(AddSamples);
        myThread.Name = "Add samples thread";
        myThread.Start();

    }

    public static void Stop()
    {
        if (_renderClient != null)
        {
            _renderClient.Dispose();
            _renderClient = null;
        }
    }

    public static void AddSamples()
    {
        while (true)
        {
            AudioClientBufferFlags acbf = AudioClientBufferFlags.Silent;

            int sampleCount = 1;
            int bytes = 4;
            int channels = 2;

            var bufferPtr = _renderClient.GetBuffer(sampleCount);
            
            var buffer = new byte[sampleCount * bytes * channels];

            Marshal.Copy(bufferPtr, buffer, 0, buffer.Length);

            for (int i = 0; i < sampleCount; i++)
            {
                var sample = (float)BitConverter.ToInt32(buffer, i * bytes * channels) / Int32.MaxValue;
                _samples.Add(sample);
            }
            _renderClient.ReleaseBuffer(sampleCount, acbf);
        }
    }
}