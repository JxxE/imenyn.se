var iMenyn = iMenyn || {};

iMenyn.Utilities = function () {

    var getUrlParameter = function (name) {
        return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [, ""])[1].replace(/\+/g, '%20')) || null;
    };

    var initCategoryChooser = function () {
        $("#category-chooser").on("change", function () {
            var value = this.value;
            if (value != "") {
                var text = $("option:selected", this).html();

                var divChosenCategories = $("#chosen-categories");
                var count = divChosenCategories.children('div').size();

                if (count < 5) {
                    divChosenCategories.append("<div class='tag'>" + text + "<input type='hidden' name='ChosenCategories["+count+"].Value' value='" + value + "' />" +
                        "<span class='glyphicon glyphicon-remove'></span></div>");
                    if (count == 4) {
                        $("#category-chooser").addClass("hide");
                    }
                }
            }
        });
        $("#chosen-categories").on("click", "span", function () {
            $(this).parent().remove();
            $("#category-chooser").removeClass("hide");
        });
    };

    var showStep = function (step) {
        window.location.hash = step;

        //Hide all steps
        $("div[id^=step-]").addClass("hide");
        //Show current step
        $("#step-" + step).removeClass("hide");
    };

    return {
        InitCategoryChooser: initCategoryChooser,
        GetUrlParameter: getUrlParameter,
        ShowStep: showStep
    };
}();