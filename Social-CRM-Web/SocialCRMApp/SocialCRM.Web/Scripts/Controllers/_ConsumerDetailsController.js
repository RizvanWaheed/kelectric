CRMApp.controller('_ConsumerDetailsController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'CustomerProfileService', 'UnifiedDeckService', '$uibModalInstance', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $stateParams, CustomerProfileService, UnifiedDeckService, $uibModalInstance, items, GlobalService, notify) {
    
    $scope.initModel = function () {
        $scope.Name = "N/A";
        $scope.TwitHandle = "N/A";
        $scope.Url = "N/A";
        $scope.Followers = 0;
        $scope.Following = 0;
        $scope.TotalQueries = 0;
        $scope.MobileNum = "N/A";
        $scope.AccNum = "N/A";
        $scope.Ibc = "N/A";
        $scope.Address = "N/A";
        $scope.Status = "N/A";
        $scope.Id = 0;
        $scope.PostUrl = "N/A";
        $scope.PostText = "N/A";
        $scope.Feeder = "N/A";
        $scope.PMT = "N/A";

        $scope.UserObj = items.UserObj;
        if ($scope.UserObj) {
            var profile = "";
            var onProfile = "";
            var isPage = $scope.UserObj.IsPage;;
            var feedType = $scope.UserObj.MessageType;
            if (feedType == "FCP" || feedType == "FP" || feedType == "FC") {
                profile = $scope.UserObj.ReplyedByUserId;
                onProfile = $scope.UserObj.UserId;
            }
            else {
                profile = $scope.UserObj.ReplyedBy;
                onProfile = $scope.UserObj.User;
            }
        }        

        UnifiedDeckService.GetConsumerData(profile, onProfile, isPage, feedType).then(function (l) {
            var response = l.data;
            if (response) {
                $scope.Name = response.Name;
                $scope.TwitHandle = $scope.UserObj.ReplyedBy;
                $scope.Url = response.Url;
                $scope.Followers = response.NumOfFollowers;
                $scope.Following = response.NumOfFollowing;
                $scope.TotalQueries = response.TotalTweets;
                $scope.MobileNum = (response.MobileNum == "") ? "None" : response.MobileNum;
                $scope.AccNum = (response.AccountNum == "") ? "None" : response.AccountNum;
                $scope.Ibc = (response.IBC == "") ? "None" : response.IBC;
                $scope.Address = (response.Address == "") ? "None" : response.Address;
                $scope.Status = (response.UserStatus == "") ? "Active" : response.UserStatus;
                $scope.Id = response.WarnBanId;
                $scope.PostUrl = response.PostUrl;
                $scope.PostText = response.PostText;
                $scope.Feeder = (response.Feeder == "" || response.Feeder == null) ? "None" : response.Feeder;
                $scope.PMT = (response.PMT == "" || response.PMT == null) ? "None" : response.PMT;
            }

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

    $scope.example5customTexts = { buttonDefaultText: 'Please Select' };

    $scope.WarnBanUser = function (statusId) {
        var username = "";
        var profileUrl = "";
        var platform = 0;
        var profile = "";
        var page = "";
        var feedType = $scope.UserObj.MessageType;
        if (feedType == "FCP" || feedType == "FP" || feedType == "FC") {
            username = $scope.UserObj.ReplyedByUserId;
            platform = 1;
            page = $scope.UserObj.UserId;
            profileUrl = "https://www.facebook.com/" + username;
        }
        else {
            username = $scope.UserObj.ReplyedBy;
            platform = 4;
            profile = $scope.UserObj.User;
            profileUrl = "https://www.twitter.com/" + username;
        }
        

        CustomerProfileService.WarnBanUser($scope.Id, username, profileUrl, 0, platform, "", profile, page, "", "", "", statusId, $rootScope.selectedCompanyId, $scope.UserObj.Id, feedType).then(function (l) {
            var response = l.data;
            if (response == "success") {
                notify({
                    message: "User saved Successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                //$scope.Cancel();
                $uibModalInstance.close(response);
                //$scope.init();
            }
            else {
                GlobalService.ErrorPopup("Save Failed", response);
            }

        }, function (err) {
            GlobalService.ErrorPopup("Error", err.data.Message);
        });
    };

    $scope.initModel();

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

}]);