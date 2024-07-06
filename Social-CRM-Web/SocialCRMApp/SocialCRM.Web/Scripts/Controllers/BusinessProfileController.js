CRMApp.controller('BusinessProfileController', ['$scope', '$window', '$location', '$stateParams', 'BusinessProfileService', '$rootScope', 'GlobalService', 'notify', function ($scope, $window, $location, $stateParams, BusinessProfileService, $rootScope, GlobalService, notify) {
    //$scope.CompanyId = $window.localStorage.getItem('selectedCompanyId');
    $scope.init = function () {

        BusinessProfileService.GetBusinessProfile().then(function (l) {
            var response = l.data;
           // alert(JSON.stringify(response))
            // alert(response)
            $scope.Submit_groups = response;

            // $scope.Submit_groups = $scope.groups;
            //$scope.BusinessNameId = response.CompanyId;
            //$scope.BusinessName = response.CompanyName; //fetch business name
            //$scope.Changed_BusinessName = $scope.BusinessName;
            $scope.emailUpdate = true; // set email update button to on and show groups whose updates are on the grid
            $scope.notificationUpdate = true;
            $scope.disableEmail = false;
            $scope.disableNotification = false;
            
        }, function (err) {
           // alert('Error in fetching pages data');
            GlobalService.ErrorPopup("Error", "Error in fetching pages data, please try again.");
            //console.log(err);
        });
       
       
    };
    // runs once per controller instantiation
    $scope.init();
    $scope.save=function()
    {
        var submitObj = {};
        
        var arr = $scope.Submit_groups;
        var Notificationlist = [];
        for (i = 0; i < arr.length; i++)
        {
            for (j = 0; j < arr[i].Notificaitons.length; j++) {
                var pushObj = { GroupId: arr[i].Notificaitons[j].GroupId, GroupName: arr[i].Notificaitons[j].GroupName, IsEmailEnabled: arr[i].Notificaitons[j].IsEmailEnabled, IsPushNotificationEnabled: arr[i].Notificaitons[j].IsPushNotificationEnabled }
                Notificationlist.push(pushObj);
            }
        }
       // alert(JSON.stringify(arr))
        submitObj = {
            CompanyId: "",
            CompanyName: "",
            Notificaitons: Notificationlist

        }

        BusinessProfileService.UpdateNotificationSettings(submitObj).then(function (l) {
          //  alert("Data has been updated Succesfully")
            notify({
                message: "Data has been updated Succesfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });

            //$scope.BusinessName = $scope.Changed_BusinessName;
            //$rootScope.$broadcast('Business_Name_Change', { CompanyId: $scope.BusinessNameId, CompanyName: $scope.BusinessName });
          //  $scope.init();
           
        }, function (err) {
            //alert('Error in Saving, please try again.');
            GlobalService.ErrorPopup("Error", "Error in saving Data, please try again.");

            console.log(err);
        });
       // alert(JSON.stringify(submitObj))
        //$scope.groupCount = $scope.Submit_groups.length; //to find number of groups
        //for (i = 0; i < $scope.groupCount; i++)
        //{ alert($scope.Submit_groups[i].email); }
      // alert("saved")
        //send Changed_BusinessName and Submit_groups to save in database.
    }
    $scope.notificationOff=function()
    {
        if ($scope.notificationUpdate)//if previously on
        {
            for (i = 0; i < $scope.Submit_groups.length; i++) {
                for (j = 0; j < $scope.Submit_groups[i].Notificaitons.length; j++) {
                    $scope.Submit_groups[i].Notificaitons[j].IsPushNotificationEnabled = false;
                }
            }
            $scope.disableNotification = true;
        }
        else {
            $scope.disableNotification = false;
        }
    }

    $scope.emailOff = function () {
        if ($scope.emailUpdate)//if previously on
        {
            for (i = 0; i < $scope.Submit_groups.length; i++) {
                for (j = 0; j < $scope.Submit_groups[i].Notificaitons.length; j++)
                {
                    $scope.Submit_groups[i].Notificaitons[j].IsEmailEnabled = false;
                }
            }
            $scope.disableEmail = true;
        }
        else {
            $scope.disableEmail = false;
        }
    }
    $scope.cancel=function()
    {
       // alert("cancelled");
        $scope.init();
    }

   



}]);