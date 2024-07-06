CRMApp.controller('SignupCodeController', ['$scope', 'AuthService', '$location', '$window', 'GlobalService', 'notify', function ($scope, AuthService, $location, $window, GlobalService, notify) {
    $scope.Name = '';
    $scope.Picture = '/Content/img/user_pic.jpg';
    $scope.wrongCode = "none";
    $scope.Email = $location.search().param
    if ($scope.Email == undefined)
        $window.location.href = "/#/Login";

    $scope.SubmitCode = function () {
        
        AuthService.ActiveUser($scope.Email, $scope.Code).then(function (l) {
            if (l != null) {
                var usr = l;
              //  $scope.$parent.setCurrentUser(usr);
                //alert("you submitted correct code");
                notify({
                    message: "Code Accepted.",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $location.path("/Signup_BusinessName").search({ param: $scope.Email });
                
            }
            else {
              //  alert('Invalid Code');
                GlobalService.ErrorPopup("Error", "Invalid Code, please try again.");
            }
        }, function (err) {
           // alert('Error in login, please try again.');
            GlobalService.ErrorPopup("Error", err.data.Message);
            console.log(err);
        });

        //if ($scope.Code != "123456")
        //{
        //    $scope.wrongCode == "block";
        //}
        //else
        //{
        //    alert("you submitted correct code");
        //    //Do processing on code submission
        //    $location.path("/Signup_Twitter");
        //}
    }

    
}]);