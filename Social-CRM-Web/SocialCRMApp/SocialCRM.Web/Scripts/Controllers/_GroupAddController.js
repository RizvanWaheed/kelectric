CRMApp.controller('_GroupAddController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'ProfileService', 'GroupService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, ProfileService, GroupService, GlobalService, notify) {
    $scope.ErrorShow = false;
    $scope.selected = {};
    $scope.CompanyId = $location.search().companyid;
    $scope.init = function () {
        $scope.slim = {
            height: '200px',
            disableFadeOut: true
        }
        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';
        $scope.instaIcon = '/Content/images/insta_ico16.png';

        ProfileService.GetByCompanyId($scope.CompanyId).then(function (l) {
            if (l != null) {
                $.each(l.data, function (index, item) {
                    l.data[index].CompositeId = l.data[index].ProfileTypeId + '' + l.data[index].ProfileId;
                });

                $scope.Connected_Profiles = l.data;
            }
            else {
                
                GlobalService.ErrorPopup("Error", "No Profiles Attached, please try again.");

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles data, please try again.");

          
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
        $scope.ErrorShow = false;
        var profiledata = [];
        var data = $.grep($scope.Connected_Profiles, function (record) {
            return $scope.selected[record.CompositeId];
        });
        $.each(data, function (key, value) {
          //  alert(value.ProfileId);
            profiledata.push({ profileId: value.ProfileId, ProfileType: value.ProfileTypeId });
        });
        //if (profiledata.length>0){
       // alert(profiledata);
        GroupService.RegisterGroup($scope.groupname, $scope.CompanyId, 1, profiledata).then(function (l) {
          //  alert("group has been added")
            notify({
                message: "Group has been added",
                position: GlobalService.position,
                duration: GlobalService.duration
            });

            $rootScope.$broadcast('Group_Reload', { GroupId: l.data });
            $rootScope.$broadcast('Group_Reload2', { GroupId: l.data });
            $uibModalInstance.close('cancel');
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.ExceptionMessage);

            
        });
        /*}
        else
        {
            $scope.ErrorShow = true;
            $scope.ErrorMessage = 'There should be atleast one profile selected.';

        }*/
    }
  
}]);