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

    $("#btn-nav").click(function() {
        $(".navbar ul").toggle('fast');
    });
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
                console.Error("Kunde inte söka Yelp");
            }
        });
    };

    var searchEnterprisesByName = function (searchTerm) {
        $("#search-button-content").hide();
        $.when(
            $.get("/Templates/_ListEnterprises.tmpl.html")
        ).done(function (searchResults) {
            $.templates({
                searchResults: searchResults
            });
            loadEnterprisesByName(searchTerm);
        });
    };
    var loadEnterprisesByName = function (searchTerm) {
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
                console.Error("Kunde inte söka");
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

    var renderMapByAddress = function (el, address) {
        $.when($.get("/Templates/_Map.tmpl.html")).done(function (map) {
            $.templates({
                map: map
            });
            loadMapByAddress(el,address);
        });
    };

    var loadMapByAddress = function (el, address) {
        $.ajax({
            dataType: "json",
            data: { address: address },
            url: "/Json/GetCoordinatesByAddress",
            type: "POST",
            success: function (data) {
                $(el).html($.render.map(data));
            },
            error: function () {
                console.error("Could not load map by address...");
            }
        });
    };

    return {
        SearchYelp: searchYelp,
        SearchEnterprisesByName: searchEnterprisesByName,
        BrowseEnterprises: browseEnterprises,
        RenderMapByAddress: renderMapByAddress
    };
}();