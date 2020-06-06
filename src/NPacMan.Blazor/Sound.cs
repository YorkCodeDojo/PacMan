using System;
using System.IO;
using Microsoft.JSInterop;

namespace NPacMan.Blazor
{
    public class Sound
    {
        public readonly TimeSpan RepeatTime;
        public readonly bool CanInterrupt;
        public readonly int Number;

        public Sound(IJSRuntime jsRuntime, UnmanagedMemoryStream ums, int number, int repeatTime, bool canInterrupt)
        {
            RepeatTime = TimeSpan.FromMilliseconds(repeatTime);
            CanInterrupt = canInterrupt;
            Number = number;

            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                ums.CopyTo(ms);
                bytes = ms.ToArray();
            }
            var data = Convert.ToBase64String(bytes);

            jsRuntime.InvokeVoidAsync("SoundSet", number, data);
        }
    }
}