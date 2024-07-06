CRMApp.controller('_ExistingProfileController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'ProfileService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, ProfileService, GlobalService, notify) {
    $scope.ErrorShow = false;
    $scope.GroupId = $location.search().groupid;
    $scope.CompanyId = $location.search().companyid;
    $scope.IsPersonal = $rootScope.IsPersonal.toString();
    $scope.selected = {};
    $scope.init = function () {
        $scope.slim = {
            height: '300px',
            disableFadeOut: true
        }
        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';
        $scope.instaIcon = '/Content/images/insta_ico16.png';
        $scope.outlookIcon = '/Content/images/outlook_ico16.png';
        $scope.Connected_Profiles = [
            //{
            //    ProfilePicURL:'https://abs.twimg.com/sticky/default_profile_images/default_profile_1_normal.png',
            //    ProfileName: 'ZohaibAkhtarKh1',
            //    ProfileTypeId: 4,
            //    ProfileType: 'Twitter',
            //    ProfileId: 12
            //}
        ]
        ProfileService.GetProfilesNotInGroup($scope.CompanyId, $scope.GroupId).then(function (l) {
            if (l != null) {
                $.each(l.data, function (index, item) {
                    l.data[index].CompositeId = l.data[index].ProfileTypeId + '' + l.data[index].ProfileId;
                });

                $scope.Connected_Profiles = l.data;
            }
            else {
                //alert('No Profiles Attached');
                GlobalService.ErrorPopup("Error", "No Profiles Attached");

            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profile data, please try again.");

        });

    };
    // runs once per controller instantiation
    $scope.init();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.AddProfile=function()
    {
        $scope.ErrorShow = false;
         var teamdata = [];
        var data = $.grep($scope.Connected_Profiles, function (record) {
            return $scope.selected[record.CompositeId];
        });
        $.each(data, function (key, value) {
            teamdata.push({ ProfileId: value.ProfileId,GroupId:$scope.GroupId  ,CompanyId: value.CompanyId, ProfileType : value.ProfileTypeId });
        });
        if (teamdata.length > 0)
        {

        ProfileService.AssignGroup(teamdata).then(function (l) {
           // alert("Profile Added Successfully");
            notify({
                message: "Profile attached successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $rootScope.$broadcast('Group_Profile_Reload',{ GroupId: $scope.GroupId,CompanyId: $scope.CompanyId, IsPersonal: $scope.IsPersonal });
            $uibModalInstance.close('cancel');
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in linking profile, please try again.");

           
        });
            //  $uibModalInstance.close('cancel');
        }
        else
        {
            $scope.ErrorShow = true;
            $scope.ErrorMessage = 'There should be atleast one profile selected.';
        }
    }
}]);