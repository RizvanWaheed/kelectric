CRMApp.controller('ResetPasswordController', ['$scope', 'AuthService', '$location', 'GlobalService', 'notify', function ($scope, AuthService, $location, GlobalService, notify) {

    $scope.codeCheck = "none";
    $scope.ResetPassword = function () {
        //check if captcha code entered is correct or not
        var val = $("#code").html();
       
        if ($scope.CaptchaCode != val)
        {
            $scope.codeCheck = "block";
        }
        else {

            $scope.codeCheck = "none";
            AuthService.ForgotPassword($scope.Email).then(function (l) {
               // alert("Password Reset");
                notify({
                    message: "Password reset successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $location.path("/SetPassword");
            }, function (err) {
               // alert('Error in Password Reset, please try again.');
                GlobalService.ErrorPopup("Error", "Error in Password Reset, please try again.");
                console.log(err);
            });
           
           
        }
    }

    
}]);