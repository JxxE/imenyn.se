$(document).ajaxStart(function () {
    $("#loading").removeClass('hide');
    $("#loading").addClass('visible-inline');
}).ajaxStop(function () {
    $("#loading").addClass('hide');
    $("#loading").removeClass('visible-inline');
});

$(function () {
    var path = window.location.href;
    $('ul a').each(function () {
        if (this.href === path) {
            $(this).addClass('active');
        }
    });
    $(document).on("click", "[data-toggle='collapse']", function () {
        var target = $(this).attr("data-target");
        $(target).toggle();
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

    //$("#btn-nav").click(function() {
    //    $(".navbar ul").toggleClass('nav-open');
    //});
});


var iMenyn = iMenyn || {};

iMenyn.Ajax = function () {

    var searchYelp = function (searchTerm, location) {
        $.when(
            $.get("/Templates/_YelpResults.tmpl.html")
        ).done(function (yelpResults) {
            $.templates({
                yelpResults: yelpResults
            });
            loadYelp(searchTerm, location);
        });
    };
    var loadYelp = function (searchTerm, location) {
        $.ajax({
            dataType: "json",
            data: { searchTerm: searchTerm, location: location },
            url: '/Json/SearchYelp/',
            type: "POST",
            success: function (data) {
                $("#yelp-results").html($.render.yelpResults(data));
            },
            error: function () {
                console.error("Kunde inte söka Yelp");
            }
        });
    };

    var searchEnterprises = function (searchTerm) {
        $("#search-button-content").hide();
        $.when(
            $.get("/Templates/_ListEnterprises.tmpl.html")
        ).done(function (searchResults) {
            $.templates({
                searchResults: searchResults
            });
            loadEnterprises(searchTerm);
        });
    };
    var loadEnterprises = function (searchTerm) {
        $.ajax({
            dataType: "json",
            data: { searchTerm: searchTerm },
            url: '/Json/SearchEnterprises/',
            type: "POST",
            success: function (data) {
                $("#search-result").html($.render.searchResults(data));
                $("#search-button-content").show();
            },
            error: function () {
                console.error("Kunde inte söka");
            }
        });
    };

    var browseEnterprises = function (el, stateCode, city) {
        var templateName = city == "" ? "BrowsedCities" : "BrowsedEnterprises";
        $.when($.get("/Templates/_" + templateName + ".tmpl.html")).done(function (districts) {
            $.templates({
                districts: districts
            });
            loadBrowsedEnterprises(el, stateCode, city);
        });
    };

    var loadBrowsedEnterprises = function (el, stateCode, city) {
        $.ajax({
            dataType: "json",
            data: { stateCode: stateCode, city: city },
            url: "/Json/BrowseEnterprises",
            type: "POST",
            success: function (data) {
                $(el).html($.render.districts(data));
            },
            error: function () {
                console.error("Could not load districts...");
            }
        });
    };

    var renderGeneralInfoByAddress = function (el, address) {
        $.when($.get("/Templates/_GeneralLocationInfo.tmpl.html")).done(function (map) {
            $.templates({
                map: map
            });
            loadGeneralInfoByAddress(el, address);
        });
    };

    var loadGeneralInfoByAddress = function (el, address) {
        $.ajax({
            dataType: "json",
            data: { address: address },
            url: "/Json/GetGeneralLocationInfoByAddress",
            type: "POST",
            success: function (data) {
                $(el).html($.render.map(data));
            },
            error: function () {
                console.error("Could not load map by address...");
            }
        });
    };

    function searchEnterprisesCloseToMyLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(renderEnterprisesCloseToMyLocation, showGeoError);
        }
        else {
            var el = document.getElementById("error");
            el.innerHTML = "Platsdata stöds inte av denna webbläsare.";
        }
    }

    var renderEnterprisesCloseToMyLocation = function (position) {
        $("#search-button-content").hide();
        $.when(
            $.get("/Templates/_ListEnterprises.tmpl.html")
        ).done(function (searchResults) {
            $.templates({
                searchResults: searchResults
            });
            loadEnterprisesCloseToMyLocation(position);
        });
    };
    var loadEnterprisesCloseToMyLocation = function (position) {
        $.ajax({
            dataType: "json",
            data: { latitude: position.coords.latitude, longitude: position.coords.longitude },
            url: '/Json/GetEnterprisesCloseToMyLocation/',
            type: "POST",
            success: function (data) {
                $("#search-result").html($.render.searchResults(data));
            },
            error: function () {
                console.error("Kunde inte söka med koordinater");
            }
        });
        $("#loading2").hide();
        $("#search-button-content-location").show();
        $("#search-button-content").show();
    };
    
    function showGeoError(error) {
        var el = document.getElementById("error");
        switch (error.code) {
            case error.PERMISSION_DENIED:
                el.innerHTML = "Användare avslog begäran om platsdata.";
                break;
            case error.POSITION_UNAVAILABLE:
                el.innerHTML = "Din platsinformation är otillgänglig.";
                break;
            case error.TIMEOUT:
                el.innerHTML = "Begäran om att få användarens platsdata tog för lång tid.";
                break;
            case error.UNKNOWN_ERROR:
                el.innerHTML = "Okänt fel uppstod.";
                break;
        }
    }

    return {
        SearchYelp: searchYelp,
        SearchEnterprises: searchEnterprises,
        BrowseEnterprises: browseEnterprises,
        RenderGeneralInfoByAddress: renderGeneralInfoByAddress,
        SearchEnterprisesCloseToMyLocation: searchEnterprisesCloseToMyLocation
    };
}();