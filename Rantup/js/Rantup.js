$(document).ajaxStart(function () {
    $("#loading").show();
}).ajaxStop(function () {
    $("#loading").hide();
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
});


var Rantup = Rantup || {};

Rantup.Ajax = function () {

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

    var searchEnterprises = function (searchTerm, location, categorySearch) {
        $.when(
            $.get("/Templates/_ListEnterprises.tmpl.html")
        ).done(function (searchResults) {
            $.templates({
                searchResults: searchResults
            });
            loadEnterprises(searchTerm, location, categorySearch);
        });
    };
    var loadEnterprises = function (searchTerm, location, categorySearch) {
        $.ajax({
            dataType: "json",
            data: { searchTerm: searchTerm, location: location, categorySearch: categorySearch },
            url: '/Json/SearchEnterprises/',
            type: "POST",
            success: function (data) {
                $("#search-result").html($.render.searchResults(data));
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

    return {
        SearchYelp: searchYelp,
        SearchEnterprises: searchEnterprises,
        BrowseEnterprises: browseEnterprises
    };
}();