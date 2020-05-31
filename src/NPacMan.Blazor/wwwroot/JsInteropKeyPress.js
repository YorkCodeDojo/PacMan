document.onkeydown = function (evt) {
    evt = evt || window.event;
    window.DotNet.invokeMethodAsync('NPacMan.Blazor', 'JsKeyDown', evt.keyCode);

    //Prevent all but F5 and F12
    if (evt.keyCode !== 116 && evt.keyCode !== 123)
        evt.preventDefault();
};

document.onkeyup = function (evt) {
    evt = evt || window.event;
    window.DotNet.invokeMethodAsync('NPacMan.Blazor', 'JsKeyUp', evt.keyCode);

    //Prevent all but F5 and F12
    if (evt.keyCode !== 116 && evt.keyCode !== 123)
        evt.preventDefault();
};