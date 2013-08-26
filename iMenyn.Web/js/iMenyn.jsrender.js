var iMenyn = iMenyn || {};

$(function() {
    iMenyn.JsRender.RegisterJsRenderHelperMethods();
});

iMenyn.JsRender = function() {
    // Register all helper methods for JsRender
    var registerJsRenderHelperMethods = function() {
        $.views.helpers({
            SetSelected: function (val, match) {
                if(val == match)
                    return "selected";
                return "";
            }
        });
    };

    return {
        RegisterJsRenderHelperMethods: registerJsRenderHelperMethods
    };
}();