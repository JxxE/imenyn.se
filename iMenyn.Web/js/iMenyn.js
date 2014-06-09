$(function () {
    $(document).on("click", "[data-action='toggleclass']", function () {
        var $this = $(this);
        $this.toggleClass('active');
        var className = $this.data("class");
        $('body').toggleClass(className);
    });

    $(document).on("click", "[data-toggle='hide']", function () {
        var target = $(this).attr("data-target");
        $(target).toggleClass('hide');
    });
    $(document).on("click", "[data-toggle='buttons-radio'] .btn", function () {
        var parent = $(this).parent();
        parent.children(".btn").removeClass("active");
        $(this).addClass("active");
    });
    $(document).on("click", "[data-toggle-active='button']", function () {
        $(this).toggleClass("active");
    });

    var browserEvent;
    if ("ontouchstart" in document.documentElement) {
        browserEvent = "touchstart";
    }
    else {
        browserEvent = "click";
    }

    $("#btn-nav").bind(browserEvent, function (e) {
        $(".navbar ul").toggleClass('nav-open');
    });


});


var iMenyn = iMenyn || {};

iMenyn.Ajax = function () {

    var searchEnterprises = function (searchTerm, btn) {
        if (searchTerm === "" || searchTerm.length < 2) {
            btn.button('reset');
            $('#search-result').html("");
        }
        else {
            $.ajax({
                dataType: "json",
                data: { searchTerm: searchTerm },
                url: '/Json/SearchEnterprises/',
                type: "POST",
                success: function (data) {
                    $.get('/Templates/_EnterpriseSearchResult.tmpl.html', function (template) {
                        var tmpl = Handlebars.compile(template);
                        $('#search-result').html(tmpl(data));
                    });

                    btn.button('reset');
                },
                error: function () {
                    console.error("Kunde inte söka");
                }
            });
        }
    };

    //var browseEnterprises = function (el, stateCode, city) {
    //    var templateName = city == "" ? "BrowsedCities" : "BrowsedEnterprises";
    //    $.when($.get("/Templates/_" + templateName + ".tmpl.html")).done(function (districts) {
    //        $.templates({
    //            districts: districts
    //        });
    //        loadBrowsedEnterprises(el, stateCode, city);
    //    });
    //};

    //var loadBrowsedEnterprises = function (el, stateCode, city) {
    //    $.ajax({
    //        dataType: "json",
    //        data: { stateCode: stateCode, city: city },
    //        url: "/Json/BrowseEnterprises",
    //        type: "POST",
    //        success: function (data) {
    //            $(el).html($.render.districts(data));
    //        },
    //        error: function () {
    //            console.error("Could not load districts...");
    //        }
    //    });
    //};

    var renderGeneralInfoByAddress = function (el, address) {
        if (address == "")
            $("#no-address-error").show();
        else {
            $("#no-address-error").hide();
            $.when($.get("/Templates/_GeneralLocationInfo.tmpl.html")).done(function (map) {
                $.templates({
                    map: map
                });
                loadGeneralInfoByAddress(el, address);
            });
        }
    };

    var loadGeneralInfoByAddress = function (el, address) {
        $.ajax({
            dataType: "json",
            data: { address: address },
            url: "/Json/GetGeneralLocationInfoByAddress",
            type: "POST",
            success: function (data) {
                $(el).html($.render.map(data));
                $("#submit").show();
            },
            error: function () {
                console.error("Could not load map by address...");
            }
        });
    };

    var searchEnterprisesCloseToMyLocation = function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(renderEnterprisesCloseToMyLocation, showGeoError);
        }
    };

    function showGeoError(error) {
        var message = "";
        switch (error.code) {
            case error.PERMISSION_DENIED:
                message = "Sökningen avbröts";
                break;
            case error.POSITION_UNAVAILABLE:
                message = "Positionsdata ej tillgänglig";
                break;
            case error.TIMEOUT:
                message = "Timeout-fel";
                break;
            case error.UNKNOWN_ERROR:
                message = "Okänt fel inträffade";
                break;
        }

        var btn = $("#search-nearby");
      
        btn.button('reset');
        btn.addClass('off');

        $.growl.error({
            message: message
        });
    }

    var renderEnterprisesCloseToMyLocation = function (position) {
        $.ajax({
            dataType: "json",
            data: { latitude: position.coords.latitude, longitude: position.coords.latitude },
            url: '/Json/GetEnterprisesCloseToMyLocation/',
            type: "POST",
            success: function (data) {
                $.get('/Templates/_EnterpriseSearchResult.tmpl.html', function (template) {
                    var tmpl = Handlebars.compile(template);
                    $('#search-result').html(tmpl(data));
                });

                $("#search-nearby").button('reset');
            },
            error: function () {
                $.growl.error({
                    message: "Sökningen misslyckades"
                });
                $("#search-nearby").button('reset');
            }
        });
    };

    var saveProduct = function (form, callback) {
        $.ajax({
            data: form.serialize(),
            url: '/Manage/AddOrEditNewProduct/',
            type: "POST",
            success: function (data) {
                if (data.success) {
                    if (data.method === "add") {
                        callback();
                    }
                    iMenyn.Utilities.UpdateProductDisplayValues(form);
                    form.removeClass("edit-mode");
                }
                else {
                    form.replaceWith(data);
                }
            },
            error: function () {
                console.error("Något gick fel...");
            }
        });
    };

    var saveMenuSetup = function (btn, setup, enterpriseId) {
        var json = '{ "menu": ' + setup + ', "enterpriseId":"' + enterpriseId + '"}';
        $.ajax({
            data: json,
            url: '/Manage/SaveMenuSetup/',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            type: "POST",
            success: function (data) {
                if (data.success) {
                    $("#main-container").html("<h1>Tack!</h1>");
                } else {
                    //TOAST
                }
            },
            error: function () {
                console.error("Kunde inte spara setup");
            }
        }).always(function () {
            btn.button('reset');
        });
    };

    var createTempEnterprise = function (form, btn) {
        btn.button('loading');
        $.ajax({
            type: 'POST',
            data: form.serialize(),
            url: '/Manage/CreateTempEnterprise',
            success: function (data) {
                if (data.url) {
                    window.location = data.url;
                }
                else {
                    form.replaceWith(data);
                }
            },
            error: function (data) {
                console.log(data);
            }
        }).always(function () {
            btn.button('reset');
        });
    };


    //TO TEST AJAX-LOADER
    var wait = function (btn) {

        btn.button('loading')


        $.ajax({
            type: "POST",
            url: '/Json/Wait',
            success: function (data) {
                console.log(data)
            },
            error: function () {
                console.log("ERROR")
            }
        }).always(function () {
            btn.button('reset');
        });

    };



    //function showGeoError(error) {
    //    var el = document.getElementById("error");
    //    switch (error.code) {
    //        case error.PERMISSION_DENIED:
    //            el.innerHTML = "Användare avslog begäran om platsdata.";
    //            break;
    //        case error.POSITION_UNAVAILABLE:
    //            el.innerHTML = "Din platsinformation är otillgänglig.";
    //            break;
    //        case error.TIMEOUT:
    //            el.innerHTML = "Begäran om att få användarens platsdata tog för lång tid.";
    //            break;
    //        case error.UNKNOWN_ERROR:
    //            el.innerHTML = "Okänt fel uppstod.";
    //            break;
    //    }
    //}

    return {
        SearchEnterprises: searchEnterprises,
        SearchEnterprisesCloseToMyLocation: searchEnterprisesCloseToMyLocation,
        SaveProduct: saveProduct,
        SaveMenuSetup: saveMenuSetup,

        CreateTempEnterprise: createTempEnterprise,

        Wait: wait
    };
}();