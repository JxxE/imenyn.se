var iMenyn = iMenyn || {};

iMenyn.Utilities = function () {
    
    //var getParameterByName = function (parameter) {
    //    parameter = parameter.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    //    var regex = new RegExp("[\\?&]" + parameter + "=([^&#]*)"),
    //        results = regex.exec(location.search);
    //    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    //};
    var renderCategoryChooser = function() {
        $("select[data-id='category']").on("change", function () {
            var value = this.value;
            if (value != "") {
                var text = $("option:selected", this).html();
                var divChosenCategories = $(this).closest("div[data-id='chosen-categories']");
                var count = divChosenCategories.children('div').size();

                if (count < 5) {
                    divChosenCategories.append("<div class='tag'>" + text + "<input type='hidden' name='category" + count + "' value='" + value + "' />" +
                        "<a href='javascript:void(0)' data-action='remove' class='marginL5 white' data-icon='&#xe006;'></a></div>");
                    if (count == 4) {
                        $("select[data-id='category']").addClass("hide");
                    }
                }
            }
        });
        $("div[data-id='chosen-categories']").on("click", "[data-action='remove']", function () {
            $(this).parent().remove();
            $("select[data-id='category']").removeClass("hide");
        });
    };

    return {
        RenderCategoryChooser: renderCategoryChooser
    };
}();