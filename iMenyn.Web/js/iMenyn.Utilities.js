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
                var index = 0;
                if (count < 5) {
                    var tagId = 0;
                    while ($("#tag-" + index).length > 0 && index < 5) {
                        index++;
                        tagId = index;
                    }
                    divChosenCategories.append("<div id='tag-" + tagId + "' class='tag'>" + text + "<input type='hidden' name='ChosenCategories[" + tagId + "].Value' value='" + value + "' />" +
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

    var dynamicFieldAdd = function () {
        var $dynamicFields = $('.dynamic-fields');

        $($dynamicFields).on('click', 'a', function () {
            var $container = $(this).closest('.dynamic-fields');
            var subNodes = $container.children('div');
            var html = subNodes[0].outerHTML;
            // Matchar: value="något"
            var inputValues = html.match(/value="([^"]*")/g);
            if (inputValues.length > 0) {
                for (var i = 0; i < inputValues.length; i++) {
                    html = html.replaceAll(inputValues[i], 'value=""');
                }
            }

            html = reorganizeBracketNumbers(subNodes.length, html);
            $(this).before(html);
        });

        $($dynamicFields).on('click', '.glyphicon-remove', function () {
            var $container = $(this).closest('.dynamic-fields');
            console.log("a: " + $container.children('div').length);
            var containerChildCount = $container.children('div').length;
            var parent = $(this).parent();
            
            while (!parent.parent().hasClass('dynamic-fields')) {
                parent = parent.parent();
            }
            parent.remove();
            var html = $container.html();
    
            var a = containerChildCount-1;

            html = reorganizeBracketNumbers(a, html);
            
            $container.html(html);
        });


        //Reorganize all bracket numbers

    };
    function reorganizeBracketNumbers(containerChildCount, html) {
        // Matchar: [siffra]           
        var numberBrackets = new RegExp(/\[\d\]/);

        var result = numberBrackets.exec(html);
        if (result) {
            var matchedString = result[0].toString();
            // Ersätt alla [siffra] mot rätt siffra
            console.log(matchedString)
            html = html.replaceAll(matchedString, '[' + containerChildCount + ']');   
        }
        else {
            //TODO toastr
            html = "<p>Något gick fel...</p>";
        }
        return html;
    }


    var showStep = function (step) {
        window.location.hash = step;
        //Hide all steps
        $("div[id^=step-]").addClass("hide");
        //Show current step
        $("#step-" + step).removeClass("hide");

        //Navigation
        var $stepNavigation = $("#steps-navigation");
        var stepAmount = $stepNavigation.data('length');
        $stepNavigation.find('span[data-step="current"]').html(step);
        $stepNavigation.find('span[data-step="total"]').html(stepAmount);
        if (step == stepAmount) {
            $("#next-step").addClass('hide');
        }
        else {
            $("#next-step").removeClass('hide');
        }
        if (step == 1) {
            $("#prev-step").addClass('hide');
        }
        else {
            $("#prev-step").removeClass('hide');
        }
    };

    var myLocation = function (callback) {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (data) {
                callback(data.coords);
            }, function (error) {
                switch (error.code) {
                    //TODO Toast errors
                    case error.PERMISSION_DENIED:
                        console.log("User denied the request for Geolocation.");
                        break;
                    case error.POSITION_UNAVAILABLE:
                        console.log("Location information is unavailable.");
                        break;
                    case error.TIMEOUT:
                        console.log("The request to get user location timed out.");
                        break;
                    case error.UNKNOWN_ERROR:
                        console.log("An unknown error occurred");
                        break;
                }
                callback(0);
            });
        }
        return null;
    };

    var toast = function (obj) {

    };

    return {
        InitCategoryChooser: initCategoryChooser,
        GetUrlParameter: getUrlParameter,
        ShowStep: showStep,
        MyLocation: myLocation,
        Toast: toast,
        DynamicFieldAdd: dynamicFieldAdd
    };
}();


// Replace ALL. 
String.prototype.replaceAll = function (find, replace) {
    var str = this;
    return str.replace(new RegExp(find.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), 'g'), replace);
};