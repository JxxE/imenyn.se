var iMenyn = iMenyn || {};

iMenyn.Utilities = function () {
    
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
        MyLocation: myLocation,
        Toast: toast
    };
}();