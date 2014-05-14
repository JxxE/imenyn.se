var iMenyn = iMenyn || {};

iMenyn.Admin = function () {

    var renderLog = function (renderTarget, query, info, debug, error, fatal, warn) {
        $.when(
            $.ajax({
                dataType: "json",
                data: ({ query: query, info: info, debug: debug, error: error, fatal: fatal, warn: warn }),
                url: "/Admin/GetLogList",
                type: "POST",
                success: function (data) {
                    return data;
                },
                error: function () {
                    return {};
                }
            })
        ).done(function (data) {
            renderExternalTemplate({ name: 'adminLogList', selector: renderTarget, data: data });
        });

    };

    // Flexible template render helper
    // item should contain following properties
    // name
    // selector
    // data
    var renderExternalTemplate = function (item) {
        var templateFile = "/templates/_" + item.name + ".tmpl.html";

        $.when(
            $.get(templateFile)
        ).done(function (tmplData) {
            $.templates({
                tmpl: tmplData
            });
            if (item.data.length === 0) {
                $(item.selector).html(''); // Empty the selector if no response contains no data
            } else {
                $(item.selector).html($.render.tmpl(item.data));
            }
        });
    };

    return {
        RenderLog: renderLog
    };
}();