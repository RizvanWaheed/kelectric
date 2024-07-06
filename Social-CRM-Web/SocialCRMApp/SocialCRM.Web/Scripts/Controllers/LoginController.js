CRMApp.controller('LoginController', ['$rootScope', '$scope', 'AuthService', '$location', '$window', '$cookies', 'GlobalService', 'notify', function ($rootScope, $scope, AuthService, $location, $window, $cookies, GlobalService, notify) {
    //var someSessionObj = { 'innerObj': 'somesessioncookievalue' };
   
    /*AuthService.VerifyCookies().then(function (l) {
        if (l.data == true) {
            if ($rootScope.IsAdmin == true)
                $window.location.href = "#/MembershipPackages";
            else
                $window.location.href = "#/Responder";
        }
    });*/
    //$cookies.dotobject = someSessionObj;
    //alert($cookies.getAll())
    $scope.credCheck = "none";
    $scope.currentDate = new Date();
    $scope.Login = function () {
        //ajax call for loggin in by using $scope.Login and $scope.Password and if $scope.Remember==true then remember credentials
        // alert("Signing In");
        AuthService.Login($scope.Email, $scope.Password).then(function (l) {
            if (l != null) {
                var isAccountActivationRequired = l.isAccountActivationRequired;
                AuthService.UserProfile($scope.Email, 0).then(function (l) {
                    if (l != null) {
                        $cookies.putObject("userSessionCRM", l.data, { path: '/' });

                        if (isAccountActivationRequired == "True")
                            $window.location.href = "#/CompanyAccountActivation";
                        else
                            $window.location.href = "#/Responder";
                    }
                    else {
                       // alert('Error in fetching data, please try again.');
                        GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");

                    }
                }, function (err) {
                    //alert('Error in fetching data, please try again.');
                    GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
                    console.log(err);
                });
            }
            else {
               // alert('Invalid Username or Password.');
                GlobalService.ErrorPopup("Error", "Invalid Username or Password, please try again.");
            }
        }, function (err) {
            //alert('Error in login, please try again.');
            GlobalService.ErrorPopup("Error", err.responseJSON.error_description);
            //GlobalService.ErrorPopup("Error", "The user Id or password you entered is incorrect. Kindly check caps lock and (or) enter correct user Id/password and try again.");
            console.log(err);

            $scope.credCheck = "Block";
        });
    }

    $scope.newAccount = function () {
        // alert(1);
        $location.path("/Signup_Package");
        // $location.path("/Signup");
    }


}]);