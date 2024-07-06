CRMApp.controller('Header_GroupAddController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'ProfileService', 'GroupService', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, ProfileService, GroupService, items, GlobalService, notify) {


    $scope.init = function () {
       // alert(items) showing alert of sent items from header controller
        $scope.slim = {
            height: '200px',
            disableFadeOut: true
        }
        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';

        $scope.instaIcon = '/Content/images/insta_ico16.png';
        $scope.CompanyId = $rootScope.selectedCompanyId;

        ProfileService.GetByCompanyId($scope.CompanyId).then(function (l) {
            if (l != null) {
                $scope.Connected_Profiles = l.data
                //$.each(l.data, function (index, item) {

                //});
            }
            else {
                GlobalService.ErrorPopup("Error", "No Profiles Attached, please try again.");
               // alert('No Profiles Attached');
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
           // alert('Error in fetching data, please try again.');
            console.log(err);
        });

        //$scope.Connected_Profiles = [
        //                {
        //                    'ProfileName': 'Agha Rameez Ahmed',
        //                    'ProfileURL': 'Agha.Rameez.Ahmed1234',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 123,
        //                    'type': 'facebook'
        //                },
        //                {
        //                    'ProfileName': 'Amber Ashraf',
        //                    'ProfileURL': 'Amber.Ashraf1234',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 1234,
        //                    'type': 'twitter'
        //                },
        //                {
        //                    'ProfileName': 'Ayesha Shamim',
        //                    'ProfileURL': 'Ayesha789',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 1236,
        //                    'type': 'linkedin'
        //                },
        //                {
        //                    'ProfileName': 'Adeel Gill',
        //                    'ProfileURL': 'Adeel789',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 12,
        //                    'type': 'google'
        //                },
        //                 {
        //                     'ProfileName': 'Agha Rameez Ahmed',
        //                     'ProfileURL': 'Agha.Rameez.Ahmed1234',
        //                     'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                     'id': 123,
        //                     'type': 'facebook'
        //                 },
        //                {
        //                    'ProfileName': 'Amber Ashraf',
        //                    'ProfileURL': 'Amber.Ashraf1234',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 1234,
        //                    'type': 'twitter'
        //                },
        //                {
        //                    'ProfileName': 'Ayesha Shamim',
        //                    'ProfileURL': 'Ayesha789',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 1236,
        //                    'type': 'linkedin'
        //                },
        //                {
        //                    'ProfileName': 'Adeel Gill',
        //                    'ProfileURL': 'Adeel789',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 12,
        //                    'type': 'google'
        //                }
        //];


    };
    // runs once per controller instantiation
    $scope.init();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.AddGroup = function () {
        GroupService.RegisterGroup($scope.groupname, $scope.CompanyId, 1, 0).then(function (l) {
           // alert("group has been added")
            notify({
                message: "Group has been added",
                position: GlobalService.position,
                duration: GlobalService.duration
            });

            $uibModalInstance.close("sending para"); //replace with parameter to send
        }, function (err) {
          //  alert('Error in Saving, please try again.');
            GlobalService.ErrorPopup("Error", err.data.ExceptionMessage);
            console.log(err);
        });
        //$uibModalInstance.close("sending para");
    }

}]);