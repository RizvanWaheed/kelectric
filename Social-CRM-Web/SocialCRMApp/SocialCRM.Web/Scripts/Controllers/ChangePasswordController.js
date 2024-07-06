CRMApp.controller('ChangePasswordController', ['$rootScope', '$scope', '$location', 'AuthService', '$window', 'notify', 'GlobalService', function ($rootScope, $scope, $location, AuthService, $window, notify, GlobalService) {
    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    };

    $scope.firstTime = true;
    $scope.init = function () {
        $scope.wrongOldPass = false;
        $scope.regexPassword = /^(?=.{6,20})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$/;
        $scope.OldPass = "";
        $scope.NewPass = "";
        $scope.CNewPass = "";
        var d = new Date();
        //alert(JSON.stringify($rootScope.currentUser))
        $scope.Picture = jQuery.parseJSON($rootScope.currentUser).ImageUrl + "?" + d.getTime();
        $scope.Picture = $scope.Picture == null && "/Content/img/user_pic.jpg" || $scope.replaceTilda($scope.Picture);
        $scope.Name = jQuery.parseJSON($rootScope.currentUser).FirstName + ' ' + jQuery.parseJSON($rootScope.currentUser).LastName;
        $scope.CompanyName = jQuery.parseJSON($rootScope.currentUser).CompanyName;
        $scope.NotPassMatch = true;
        $scope.passError = "none";
    }

    $scope.init();

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        if ($scope.firstTime) {
            $scope.init();
            $scope.firstTime = false;
        }
    });

   // $scope.init();

    $scope.Cancel=function()
    {
        $location.path("/PersonalProfile");
        //$scope.init();
    }
    $scope.checkPass1 = function () {

        if ($scope.myForm.CNewPass.$touched) {
            if ($scope.NewPass != $scope.CNewPass) {
                $scope.passError = "Block";

                $scope.NotPassMatch = true;

            }
            else {
                $scope.passError = "none";
                $scope.NotPassMatch = false;

            }
        }
    }

    $scope.checkPass2 = function () {

        if ($scope.NewPass != $scope.CNewPass) {
            $scope.passError = "Block";
            $scope.NotPassMatch = true;

        }
        else {
            $scope.passError = "none";
            $scope.NotPassMatch = false;

        }

    }

    $scope.ChangePassword=function()
    {
        AuthService.ChangePassword($scope.OldPass, $scope.NewPass, $scope.CNewPass).then(function (l) {
           
            $scope.wrongOldPass = false;
            $scope.OldPass = "";
            $scope.NewPass = "";
            $scope.CNewPass = "";
            $scope.myForm.CNewPass.$touched = false;
            $scope.myForm.NewPass.$touched = false;
            $scope.myForm.OldPass.$touched = false;
            notify({
                message: "Password has been Changed",
                classes: $scope.classes,
                position: GlobalService.position,
                duration: GlobalService.duration
            });
           // $location.path("/login");
        }, function (err) {
            if (err.data.ModelState[""][0] == "Incorrect password.")
            {
                $scope.wrongOldPass = true;
            }
            else
               // alert('Error in Password Reset, please try again.');
                GlobalService.ErrorPopup("Error", "Error in Password Reset, please try again.");

            console.log(err);
        });
      //  alert("Password Changes");
    }
}]);