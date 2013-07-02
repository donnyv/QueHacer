/// <reference path="soundmanager2-nodebug-jsmin.js" />

(function () {

    var soundObj;
    var sp = {
        setup: function (callback) {
            soundManager.setup({
                url: '/content/flash/',
                flashVersion: 9, // optional: shiny features (default = 8)
                // optional: ignore Flash where possible, use 100% HTML5 mode
                // preferFlash: false,
                onready: function () {
                    // Ready to use; soundManager.createSound() etc. can now be called.
                    if(callback)
                        callback();
                }
            });
        },
        start: function (listOfText) {
            soundObj = soundManager.createSound({
                id: 'mySound',
                url: 'http://translate.google.com/translate_tts?ie=UTF-8&q=Hello&tl=en&total=1&idx=0&textlen=5&prev=input',
                autoLoad: true,
                autoPlay: false,
                onload: function () {
                    //alert('The sound ' + this.id + ' loaded!');
                    soundObj.play('mySound', {
                        volume: 50,
                        onfinish: function () {

                        }
                    });
                },
                volume: 50
            });
        },
        stop: function () {
            if(soundObj != null)
                soundObj.stop();
        }
    };

    if (!window.text2speech)
        window.text2speech = sp;
})();