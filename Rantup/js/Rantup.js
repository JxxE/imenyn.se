$(document).ajaxStart(function () {
    $("#loading").show();
}).ajaxStop(function () {
    $("#loading").hide();
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

    return {
        SearchYelp: searchYelp,
        SearchEnterprises: searchEnterprises
    };
}();