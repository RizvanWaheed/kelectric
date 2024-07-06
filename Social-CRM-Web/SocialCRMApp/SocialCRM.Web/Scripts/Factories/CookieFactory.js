CRMApp.factory('CookieFactory', function(){

    return {
        getCookie: function(name){
            return $.cookie(name);
        },

        getAllCookies: function(){
            return $.cookie();
        },

        setCookie: function(name, value){
            return $.cookie(name, value);
        },

        deleteCookie: function(name){
            return $.removeCookie(name);
        }
    }
});