(function () {
    var util = {
        DatetoUTC: function (date) {
            return Date.parse(date.toUTCString());
        },
        UTCtoDate: function (utc) {
            return new Date(utc * 1).toDateString();
        },

        // urlTemplate: url where html templates are located
        // templateList: array with template names
        // baseObject: object used to append the property "tl". "tl" will contain
        // properties with compiled templates. Properties will be named using the templateList.
        CompileTemplates: function (urlTemplate, templateList, baseObject, callback) {
            // set template root model variable name
            _.templateSettings.variable = "m";

            var t = templateList;
            function Compile(data) {
                baseObject.tl = {};
                var dom = $(data);
                for (var i = 0, l = t.length; i < l; i++) {
                    var h = dom.filter("#" + t[i])[0].innerHTML;
                    baseObject.tl[t[i]] = _.template(h);
                }
            }

            $.get(urlTemplate, function (data) {
                Compile(data);
                callback();
            });
        }
    };

    if (!window.app)
        window.app = {};

    if (!window.app.Util)
        window.app.Util = util;

})();