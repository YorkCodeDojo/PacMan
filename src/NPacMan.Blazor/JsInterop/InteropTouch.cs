using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace NPacMan.Blazor.JsInterop
{

    public class TouchEvent
    {
        
        [JsonPropertyName("altKey")]
        public bool AltKey { get; set; }
        
        [JsonPropertyName("changedTouches")]
        public Touch[] ChangedTouches { get; set; } = new Touch[0];
        
        [JsonPropertyName("ctrlKey")]
        public bool CtrlKey { get; set; }

        [JsonPropertyName("metaKey")]
        public bool MetaKey { get; set; }

        [JsonPropertyName("shiftKey")]
        public bool ShiftKey { get; set; }
        
        [JsonPropertyName("targetTouches")]
        public Touch[] TargetTouches { get; set; } = new Touch[0];

        [JsonPropertyName("touches")]
        public Touch[] Touches { get; set; } = new Touch[0];

        public class Touch
        {
            [JsonPropertyName("identifier")]
            public int Identifier { get; set; }
            [JsonPropertyName("screenX")]
            public int ScreenX { get; set; }
            [JsonPropertyName("screenY")]
            public int ScreenY { get; set; }
            [JsonPropertyName("clientX")]
            public int ClientX { get; set; }
            [JsonPropertyName("clientY")]
            public int ClientY { get; set; }
            [JsonPropertyName("pageX")]
            public int PageX { get; set; }
            [JsonPropertyName("pageY")]
            public int PageY { get; set; }
        }
    }
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
