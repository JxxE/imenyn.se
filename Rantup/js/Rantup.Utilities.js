var Rantup = Rantup || {};

Rantup.Utilities = function () {
    
    //var getParameterByName = function (parameter) {
    //    parameter = parameter.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    //    var regex = new RegExp("[\\?&]" + parameter + "=([^&#]*)"),
    //        results = regex.exec(location.search);
    //    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    //};

    function loadjscssfile(path, type, fn, scope) {
        //Rantup.Utilities.LoadJsCssFile("http://api.tiles.mapbox.com/mapbox.js/v1.0.2/mapbox.js", "js", function (success, link) { });
        var element;
        if (type == "css") {
            element = document.createElement('link'); // create the link node
            element.setAttribute('href', path);
            element.setAttribute('rel', 'stylesheet');
            element.setAttribute('type', 'text/css');
        }
        if (type == "js") {
            element = document.createElement('script'); // create the link node
            element.setAttribute('src', path);
        }

        // reference to document.head for appending/ removing link nodes
        var head = document.getElementsByTagName('head')[0], element;
        
        var sheet, cssRules;
        // get the correct properties to check for depending on the browser
        if ('sheet' in element) {
            sheet = 'sheet'; cssRules = 'cssRules';
        }
        else {
            sheet = 'styleSheet'; cssRules = 'rules';
        }

        var timeout_id = setInterval(function () {                     // start checking whether the style sheet has successfully loaded
            try {
                if (element[sheet] && element[sheet][cssRules].length) { // SUCCESS! our style sheet has loaded
                    clearInterval(timeout_id);                      // clear the counters
                    clearTimeout(timeout_id);
                    fn.call(scope || window, true, element);           // fire the callback with success == true
                }
            } catch (e) { } finally { }
        }, 10),                                                   // how often to check if the stylesheet is loaded
            timeout_id = setTimeout(function () {       // start counting down till fail
                clearInterval(timeout_id);             // clear the counters
                clearTimeout(timeout_id);
                head.removeChild(element);                // since the style sheet didn't load, remove the link node from the DOM
                fn.call(scope || window, false, element); // fire the callback with success == false
            }, 15000);                                 // how long to wait before failing

        head.appendChild(element);  // insert the link node into the DOM and start loading the style sheet

        return element; // return the link node;
    }

    return {
        LoadJsCssFile:loadjscssfile
        //GetParameterByName: getParameterByName
    };
}();