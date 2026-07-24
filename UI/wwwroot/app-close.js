window.addEventListener("beforeunload", function () {

    navigator.sendBeacon("/app-closed");

});