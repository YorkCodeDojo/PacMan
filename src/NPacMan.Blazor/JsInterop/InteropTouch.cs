using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace NPacMan.Blazor.JsInterop
{
    /// <summary>
    /// Proxy for receiving touch events from JavaScript.
    /// </summary>
    public static class InteropTouch
    {
        /// <summary>
        /// Fires when a touch start event is received from JavaScript.
        /// </summary>
        public static event EventHandler<TouchEvent>? TouchStart;

        /// <summary>
        /// Fires when a touch end event is received from JavaScript.
        /// </summary>
        public static event EventHandler<TouchEvent>? TouchEnd;

        /// <summary>
        /// Called by JavaScript when a touch start event fires.
        /// </summary>
        [JSInvokable]
        public static Task<bool> JsTouchStart(TouchEvent @event)
        {
            if (@event is object)
            {
                TouchStart?.Invoke(null, @event);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Called by JavaScript when a touch end event fires.
        /// </summary>
        [JSInvokable]
        public static Task<bool> JsTouchEnd(TouchEvent @event)
        {
            if (@event is object)
            {
                TouchEnd?.Invoke(null, @event);
            }

            return Task.FromResult(true);
        }

    }
}
