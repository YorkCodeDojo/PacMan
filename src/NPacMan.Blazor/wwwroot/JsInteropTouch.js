
(function () {
    var convertTouches = function(touches) {
        return Array.from(touches,
            function (item) {
                return { identifier: item.identifier,
                    screenX: item.screenX,
                    clientX: item.clientX,
                    clientY: item.clientY,
                    pageX: item.pageX,
                    pageY: item.pageY
                }
            });
    }
    document.body.addEventListener('touchstart',
        function(evt) {
            window.DotNet.invokeMethodAsync('NPacMan.Blazor',
                'JsTouchStart',
                {
                    altKey: evt.altKey,
                    ctrlKey: evt.ctrlKey,
                    metaKey: evt.metaKey,
                    shiftKey: evt.shiftKey,
                    changedTouches: convertTouches(evt.changedTouches),
                    targetTouches: convertTouches(evt.targetTouches),
                    touches: convertTouches(evt.touches)
                });
        },
        false);

    document.body.addEventListener('touchend',
        function(evt) {
            window.DotNet.invokeMethodAsync('NPacMan.Blazor',
                'JsTouchEnd',
                {
                    altKey: evt.altKey,
                    ctrlKey: evt.ctrlKey,
                    metaKey: evt.metaKey,
                    shiftKey: evt.shiftKey,
                    changedTouches: convertTouches(evt.changedTouches),
                    targetTouches: convertTouches(evt.targetTouches),
                    touches: convertTouches(evt.touches)
                });
        },
        false);
})();