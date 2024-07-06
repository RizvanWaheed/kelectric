CRMApp.controller('_ReportAsSpamController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'TeamService', 'UnifiedService', 'items', 'TwitterService', 'GlobalService','notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, TeamService, UnifiedService, items, TwitterService, GlobalService, notify) {


    $scope.ofUser = items.replyedBy;
    $scope.replyedMsgModel = '@' + items.replyedBy;
    $scope.StatusId = items.StatusId;
    $scope.replyedTo = "";
    $scope.attachedProfiles = [];
    $scope.CheckProfile = function (profileId, UserName) {
        // alert(profileId);
        $scope.replyedTo = UserName;
        $.each($scope.attachedProfiles, function (key, value) {
            if (value.Id != profileId) {
                $("#profile-" + value.Id).prop('checked', false);
            }
        });
    }
    $scope.GetTwitterProfilesByGroupId = function () {
        //Follower details
        ProfileService.GetTwitterProfilesByGroupId($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {
          
            $.each(l.data, function (key, value) {
                $scope.attachedProfiles.push({ Id: value.Id, ProfilePicURL: value.ProfilePicURL, Name: value.Name, UserName: value.UserName, ischecked: key == 0 ? true : false });
            });
            $scope.replyedTo = l.data[0].UserName;
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching contact history tweets, please try again.");

            //alert('Error in fetching contact history tweets, please try again.');
            //console.log(err);
        });
    };
    $scope.TwitterReportSpam = function () {

        TwitterService.TwitterReportSpam($scope.replyedTo, $scope.StatusId, $scope.replyedTo).then(function (l) {
             $uibModalInstance.close('cancel');
            // alert("Report as Spam successfully ")
             notify({
                 message: "Report as Spam successfully",
                 position: GlobalService.position,
                 duration: GlobalService.duration
             });
        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
            //alert('Error in post tweet, please try again.');
            //console.log(err);
        });
    };
    $scope.GetTwitterProfilesByGroupId();
   // $scope.init();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);