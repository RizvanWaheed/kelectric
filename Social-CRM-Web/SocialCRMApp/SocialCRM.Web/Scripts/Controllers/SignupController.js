CRMApp.controller('SignupController', ['$scope', 'AuthService', '$location', 'GlobalService', 'notify', function ($scope, AuthService, $location, GlobalService, notify) {
   
    $scope.value = 'Trial';
    $scope.trialHidden = 'block';
    $scope.basicHidden='none';
    $scope.silverHidden = 'none';
    $scope.goldHidden = 'none';
    $scope.platinumHidden = 'none';
    $scope.regexPassword = /^(?=.{6,20})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$/;
    //to compare passwords
    $scope.NotPassMatch = true; 
    $scope.passError = "none";

    $scope.Account = $location.search().param;
    $scope.value = $scope.Account;
    
    $scope.checkPass1 = function () {
        
        if ($scope.myForm.CPassword.$touched) {
            if ($scope.Password != $scope.CPassword) {
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
        
        if ($scope.Password != $scope.CPassword) {
            $scope.passError = "Block";
            $scope.NotPassMatch = true;
          
        }
        else {
            $scope.passError = "none";
            $scope.NotPassMatch = false;
            
        }

    }


    $scope.Update = function ()
    {
        $scope.Account = $scope.value;
        if ($scope.Account == "Trial") {
            $scope.trialHidden = 'block';
            $scope.basicHidden = 'none';
            $scope.silverHidden = 'none';
            $scope.goldHidden = 'none';
            $scope.platinumHidden = 'none';
        }
        else if($scope.Account=="Basic")
        {
            $scope.trialHidden = 'none';
            $scope.basicHidden = 'block';
            $scope.silverHidden = 'none';
            $scope.goldHidden = 'none';
            $scope.platinumHidden = 'none';
        }
        else if ($scope.Account == "Silver") {
            $scope.trialHidden = 'none';
            $scope.basicHidden = 'none';
            $scope.silverHidden = 'block';
            $scope.goldHidden = 'none';
            $scope.platinumHidden = 'none';
        }
        else if ($scope.Account == "Gold") {
            $scope.trialHidden = 'none';
            $scope.basicHidden = 'none';
            $scope.silverHidden = 'none';
            $scope.goldHidden = 'block';
            $scope.platinumHidden = 'none';
        }
        else if ($scope.Account == "Platinum") {
            $scope.trialHidden = 'none';
            $scope.basicHidden = 'none';
            $scope.silverHidden = 'none';
            $scope.goldHidden = 'none';
            $scope.platinumHidden = 'block';
        }
    }


    $scope.SignUp = function ()
    {
        //call function via ajax
       /// alert("signing in");
         AuthService.RegisterUser($scope.FirstName, $scope.LastName, $scope.Email, $scope.Password, $scope.picture, $scope.Account).then(function (l) {
             $location.path("/Signup_Code").search({ param: $scope.Email });
        }, function (err) {
            //alert('Error in Signup, please try again.');
            GlobalService.ErrorPopup("Error", "User with given email address already exists. Try again with a different email address.");
            console.log(err);
        });
      
    }
}]);


