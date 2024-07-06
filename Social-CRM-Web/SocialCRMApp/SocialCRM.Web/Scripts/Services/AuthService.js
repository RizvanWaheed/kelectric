
CRMApp.factory('AuthService', function ($http,$cookies) {
    var authService = {};

    authService.Login = function (login, password) {
        var loginData = {
            grant_type: "password",
            username: login,
            password: password
        };
        //var request = $http({
        //    method: 'POST',
        //    headers: {
        //        "Content-Type": "application/x-www-form-urlencoded"
        //    },
        //    url: base_url + '/Token',
        //    data: loginData,
        //    responseType: 'json'
        //}).then(function (res) {

        //    return res.data;
        //});

        var request = $.ajax({
            crossdomain: true,
            url: base_url + '/Token',
            type: 'POST',
            data: loginData,
            contentType: 'application/x-www-form-urlencoded',
            dataType: 'json',
            success: function (token) {
                console.log(token);
                if (token != null && token != undefined && token.access_token != null && token.access_token != undefined)
                    $cookies.put('access_token', token.access_token);

            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(JSON.stringify(jqXHR));
                console.log(JSON.stringify(textStatus));
                console.log(JSON.stringify(errorThrown));
            }
        });
        return request;
    };

    authService.RegisterUser = function (FirstName, LastName, Email, Password, ImageUrl, AccountType) {

        var _user = { FirstName: FirstName, LastName: LastName, Email: Email, Password: Password, ImageUrl: ImageUrl, ConfirmationCode: "", AccountType: AccountType };


        var request = $http({
            method: 'POST',
            url: base_url + '/api/account/Register',
            data: JSON.stringify(_user)
        }).then(function (res) {

            return res;
        });
        return request;
    };

    authService.ForgotPassword = function (Email) {
        var request = $http({
            method: 'POST',
            url: base_url + '/api/account/ForgotPassword',
            data: JSON.stringify({ Email: Email }),
            cache: false,
            async: false,
        }).then(function (res) {
            return res;
        });
        return request;
    };

    authService.ResetPassword = function (Email, Password, Code) {
        var request = $http({
            method: 'POST',
            url: base_url + '/api/account/ResetPassword',
            data: JSON.stringify({ Email: Email, Password: Password, Code: Code }),
            cache: false,
            async: false,
        }).then(function (res) {
            return res;
        });
        return request;
    };

    authService.ChangePassword = function (OldPassword, NewPassword, ConfirmPassword) {
        var request = $http({
            method: 'POST',
            url: base_url + '/api/account/ChangePassword',
            data: JSON.stringify({ OldPassword: OldPassword, NewPassword: NewPassword, ConfirmPassword: ConfirmPassword }),
            cache: false,
            async: false,
        }).then(function (res) {
            return res;
        });
        return request;
    };

   
    authService.LogOut = function () {
        var userToken = $cookies.get('access_token');
        var settings = {
            url: base_url + '/api/account/Logout',
            dataType: "json",
            data: {},
            contentType: "application/x-www-form-urlencoded",
            headers: {
                "accept": "application/json; charset=utf-8",
                'Authorization': 'BEARER ' + userToken
            },
            async: true,
            cache: false,
            success: {},
            error: {},
            complete: {},
            fail: {}
        };
        settings.method = "POST"; //("Your request type")
        return $http(settings);

        //var request = $http({
        //    method: 'Post',
        //    url: base_url + 'api/account/Logout',
        //    data: { grant_type: "password", username: login, password: password },
        //    beforeSend: function (xhr) {
        //        xhr.setRequestHeader("Authorization", "BEARER " + userToken);
        //    },
        //    contentType: "application/x-www-form-urlencoded",
        //    cache: false,
        //    async: false,
        //}).then(function (res) {

        //    return res;
        //});
        //return request;
    };
    //authService.isAuthenticated = function () {
    //    return !!Session.userId;
    //};

    //authService.isAuthorized = function (right) {
    //    if (right != null) {
    //        roleExists = false;
    //        for (var i = 0 ; i < Session.Rights.length; i++) {
    //            if (Session.Rights[i].PermissionTitle == right)
    //                roleExists = true;
    //        }
    //        return (roleExists);
    //    }
    //};
    authService.ActiveUser = function (Email, ConfirmationCode) {

        var _user = { Email: Email, ConfirmationCode: ConfirmationCode };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/account/Activate',
            data: JSON.stringify(_user)
        }).then(function (res) {

            return res;
        });
        return request;
    };
    //$.ajax({
    //    url: 'http://localhost:1250/api/account/Activate',
    //    type: 'POST',
    //    data: JSON.stringify(activeUser),
    //    contentType: "application/json; charset=utf-8",
    //    cache: false,
    //    async: false,
    //    success: function (data) {
    //        console.log(data);
    //    },
    //    error: function (x, y, z) {
    //        console.log(x + '\n' + y + '\n' + z);
    //    }
    //});

    
    authService.UserProfile = function (email, companyId) {
        var requestUrl = base_url + '/api/Account/UserProfile/' + email + '/' + companyId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    authService.CreateNewUser = function () {
        var requestUrl = base_url + '/api/Account/CreateNewUser';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }

    authService.VerifyCookies = function () {
        var requestUrl = base_url + '/api/Account/VerifyCookies';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {

            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }
    return authService;
})