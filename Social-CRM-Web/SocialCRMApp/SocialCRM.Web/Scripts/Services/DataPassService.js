CRMApp.factory('DataPassService', function () {
    var Data;

    return {
        getProperty: function () {
            return Data;
        },
        setProperty: function (value) {
            Data = value;
        },
        thing: {
            x: 100
        }
    };
});