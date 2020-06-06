using System.Text.Json.Serialization;

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
            public double Identifier { get; set; }
            [JsonPropertyName("screenX")]
            public double ScreenX { get; set; }
            [JsonPropertyName("screenY")]
            public double ScreenY { get; set; }
            [JsonPropertyName("clientX")]
            public double ClientX { get; set; }
            [JsonPropertyName("clientY")]
            public double ClientY { get; set; }
            [JsonPropertyName("pageX")]
            public double PageX { get; set; }
            [JsonPropertyName("pageY")]
            public double PageY { get; set; }
        }
    }
}