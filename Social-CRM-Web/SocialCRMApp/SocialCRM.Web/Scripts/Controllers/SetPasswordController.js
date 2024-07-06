CRMApp.controller('SetPasswordController', ['$scope', 'AuthService', '$location', 'GlobalService', 'notify', function ($scope, AuthService, $location, GlobalService, notify) {
    $scope.passError = "none";
    $scope.NotPassMatch = true;
    $scope.codeCheck = "none"; // "block "if temporary password is not correct
    $scope.regexPassword = /^(?=.{6,20})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$/;

    $scope.checkPass1 = function () {
        
        if ($scope.myForm.CPass.$touched)
        {
            if ($scope.Pass != $scope.CPass)
            {
                $scope.passError = "Block";

                $scope.NotPassMatch = true;
            }
            else
            {
                $scope.passError = "none";
                $scope.NotPassMatch = false;
            }
        }
    }

    $scope.checkPass2 = function () {
        

            if ($scope.Pass != $scope.CPass) {
                $scope.passError = "Block";
                $scope.NotPassMatch = true;
            }
            else {
                $scope.passError = "none";
                $scope.NotPassMatch = false;
            }
        
    }



    $scope.SetPassword = function () {

        //if ($scope.TempPass != "123456") //check if temporary password enetered is correct
        //{
        //    $scope.codeCheck = "Block";
        //}
        //else 
        //{
        //    $scope.codeCheck = "none";
        //    alert("password has been set");
        //}
        AuthService.ResetPassword($scope.Email, $scope.Pass, $scope.TempPass).then(function (l) {
          //  alert("Passowrd Changes");
            notify({
                message: "Password reset successfullly.",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $location.path("/Login");
        }, function (err) {
            //alert('Error in Password Reset, please try again.');

            GlobalService.ErrorPopup("Error", "Error in Password Reset, please try again.");
            console.log(err);
        });
    }

   
}]);