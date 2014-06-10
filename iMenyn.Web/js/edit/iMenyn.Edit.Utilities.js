var iMenyn = iMenyn || {};

iMenyn.Utilities = function () {

    var getUrlParameter = function (name) {
        return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [, ""])[1].replace(/\+/g, '%20')) || null;
    };

    var initCategoryChooser = function () {
        $("#category-chooser").on("change", function () {
            $("#category-error").addClass('hide');
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
                    divChosenCategories.append("<div id='tag-" + tagId + "' class='tag'>" + text + "<input type='hidden' name='DisplayCategories[" + tagId + "].Value' value='" + value + "' />" +
                        "<span class='glyphicon glyphicon-remove ml'></span></div>");
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

    var dynamicAdd = function (obj) {
        if (obj.maxOccurs && obj.maxOccurs.length > 0) {
            if (obj.maxParent.children(obj.maxType).length < obj.maxOccurs) {
                doDynamicAdd(obj);
            }
            else {
                //Max antal uppnått
                return 1;
            }
        }
        else {
            doDynamicAdd(obj);
        }
    };

    function doDynamicAdd(obj) {
        var position = "after";
        if (obj.position)
            position = obj.position;
        $.ajax({
            url: obj.href,
            cache: false,
            success: function (html) {
                var target;
                if (obj.target) {
                    target = $(obj.target);
                }
                if (target) {
                    if (position === "before")
                        target.prepend(html);
                    else
                        target.after(html);
                }
                else {
                    if (position === "before")
                        obj.senderObj.before(html);
                    else
                        obj.senderObj.after(html);
                }
                
                //If is "add-category", scroll to view
                if (obj.target.indexOf("#categories") >= 0) {
                    var parsedHtml = $($.parseHTML(html));
                    $.each(parsedHtml, function(index, value) {
                        if($(value).hasClass('category')) {
                            scrollToAnchor($(value)[0].id);
                        }
                    });
                }
            },
            error: function () {
                obj.senderObj.after("<p>Något gick fel...</p>");
            }
        });
    }

    var scrollToAnchor = function(aid) {
        $('html,body').animate({ scrollTop: ($("#" + aid).offset().top - 40) }, 'slow');
    };

    /* Save product-form, update display-values */
    var updateProductDisplayValues = function (form) {
        var formFields = form.find('input,textarea').not("input[type='hidden']");

        var priceContainer = form.find('.product-prices').first();
        priceContainer.html("");
        var prices = {};

        for (var i = 0; i < formFields.length; i++) {
            var inputId = $(formFields[i])[0].id;
            var inputName = $(formFields[i])[0].name;
            var inputValue = $(formFields[i]).first().val();
            form.find("[data-id=" + inputId + "]").html(inputValue);

            // Show and hide ABV 
            if (inputId === "Abv" && (inputValue != 0 && $.isNumeric(inputValue)))
                // Show ABV if input is not 0 and is numeric
                form.find('.abv').removeClass("hide");
            else if (inputId === "Abv" && !form.find('.abv').hasClass("hide")) {
                //Hide abv if it does not have the class
                form.find('.abv').addClass("hide");
            }

            //If input is Price!
            if (inputName.indexOf("Prices") == 0) {
                var id = inputName.substring(0, inputName.indexOf('.'));

                if (prices[id] === undefined) {
                    prices[id] = {};
                }

                //type == "Price" or "Description"
                var type = inputName.substring((inputName.indexOf('.') + 1), inputName.length);
                prices[id][type] = inputValue;
            }
        }

        //Set prices to price-container
        $.each(prices, function (index) {
            priceContainer.append("<span>" + prices[index].Price + "kr" + " " + prices[index].Description + "</span>");
        });

    };

    //Only used for edit-mode
    var focusField = function ($field) {
        $field.focus();
        var v = $field.val();
        $field.get(0).setSelectionRange(v.length, v.length);
    };


    var toggleEditMode = function (container, uniqueCodeVar) {
        //if (functionToRun)
        //    functionToRun();
        var editModeClass = 'edit-mode';

        var forms;

        if (uniqueCodeVar === 'all-but-this') {
            //All containers but this
            forms = $('.' + editModeClass).not(container);
        }
        else if (uniqueCodeVar === "product" && container.hasClass(editModeClass)) {
            //Closing current product. Check if it should be removed or just closed
            forms = container;
        }
        else {
            //Close or remove all edit-forms
            forms = $('.' + editModeClass);
        }

        forms = forms.not('.no-init-value');
        $.each(forms, function (index, value) {
            //Close or remove all edit-forms but this
            submitOrRemoveProductEditForm($(value));
        });

        //Toggle active class on current
        container.toggleClass(editModeClass);

        //Set focus
        iMenyn.Utilities.FocusField(container.find('input').first());


    };

    function submitOrRemoveProductEditForm(editForm) {
        var firstPriceInput = editForm.find('.product-price-edit input[type="number"]');

        //If name-input is empty OR first price is 0 or empty
        if (!editForm.find('[name="Name"]').first().val().trim() || (firstPriceInput.val() == 0 || firstPriceInput.val() == "")) {
            //Remove product-form
            editForm.remove();
        }
        else {
            //Save
            editForm.submit();
        }
    }

    //var showStep = function (step) {
    //    window.location.hash = step;
    //    //Hide all steps
    //    $("div[id^=step-]").addClass("hide");
    //    //Show current step
    //    $("#step-" + step).removeClass("hide");

    //    //Navigation
    //    var $stepNavigation = $("#steps-navigation");
    //    var stepAmount = $stepNavigation.data('length');
    //    $stepNavigation.find('span[data-step="current"]').html(step);
    //    $stepNavigation.find('span[data-step="total"]').html(stepAmount);
    //    if (step == stepAmount) {
    //        $("#next-step").addClass('hide');
    //    }
    //    else {
    //        $("#next-step").removeClass('hide');
    //    }
    //    if (step == 1) {
    //        $("#prev-step").addClass('hide');
    //    }
    //    else {
    //        $("#prev-step").removeClass('hide');
    //    }
    //};

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
        MyLocation: myLocation,
        Toast: toast,
        DynamicAdd: dynamicAdd,
        UpdateProductDisplayValues: updateProductDisplayValues,
        FocusField: focusField,
        ToggleEditMode: toggleEditMode,
        ScrollToAnchor: scrollToAnchor
    };
}();


// Replace ALL. 
//String.prototype.replaceAll = function (find, replace) {
//    var str = this;
//    return str.replace(new RegExp(find.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), 'g'), replace);
//};