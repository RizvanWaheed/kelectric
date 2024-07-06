CRMApp.controller('OutlookController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', '$uibModal', '$http', '$sce', '$filter', 'OutlookService', 'CompanyService', 'TaskService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $stateParams, $uibModal, $http, $sce, $filter, OutlookService, CompanyService, TaskService, GlobalService, notify) {
    $scope.init = function () {
        $scope.PageNumber = 0;
        $scope.Profiles = [];
        $scope.text = [];
        var selectedProfile = [];
        $scope.profileSelect = "0";
        $scope.EmailDetails = [];
        $scope.EmailItemType = "Inbox";

        $scope.getProfileDetails($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString());
    };

    $scope.getProfileDetails = function (groupId, companyId, isPersonal) {
        if ($window.location.href.indexOf("PersonalProfile") === -1) {
            OutlookService.GetOutlookProfiles(groupId, companyId, isPersonal).then(function (l) {
                $scope.Profiles = l.data;
                $scope.EmailDetails = [];
                $scope.lastArrDt = null;
                $scope.maxArrDt = null;
                $scope.lastArrIndex = null;
                $scope.status = "normal";

                if ($scope.Profiles.length > 0) {
                    selectedProfile = $scope.Profiles[0];
                    fetchInboxEmailData();
                }
                else {
                    selectedProfile = [];
                    $scope.EmailDetails = [];
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching Outlook Profiles.");
            });
        }
    };

    $scope.trustedHtml = function (str) {
        return $sce.trustAsHtml(str);
    };

    $scope.setShowText = function (name, index, date) {
        $scope.text[index] = name;
        $scope.EmailDetails[index].newMsg = " ";

        //Setting Email Read status
        OutlookService.SetReadStatus($scope.currentEmail, date).then(function (l) {
            $scope.EmailDetails[index].IsRead = true;
        },
        function (err) {
            GlobalService.ErrorPopup("Error", "Error in setting email message status.");
        });
    }

    $scope.selectedProfileChanged = function (list) {
        selectedProfile = $scope.Profiles[$scope.profileSelect];

        $scope.EmailDetails = [];
        //  $scope.EmailItemType = "Inbox";
        $scope.lastArrDt = null;
        $scope.lastArrIndex = null;
        $scope.maxArrDt = null;
        $scope.PageNumber = 0;
        //fetchInboxEmailData();

        if ($scope.EmailItemType == "Inbox") {
            fetchInboxEmailData();
        }
        else {
            fetchSentEmailData();
        }
    };

    $scope.optionChanged = function () {
        $scope.EmailDetails = [];
        $scope.lastArrDt = null;
        $scope.lastArrIndex = null;
        $scope.maxArrDt = null;
        $scope.PageNumber = 0;

        if ($scope.EmailItemType == "Inbox") {
            fetchInboxEmailData();
        }
        else {
            fetchSentEmailData();
        }
    };

    $scope.profileLoadMoreFeeds = function () {
        $scope.PageNumber++;
        if ($scope.EmailItemType == "Inbox") {
            fetchInboxEmailData();
        }
        else {
            fetchSentEmailData();
        }
    };

    $scope.getLatestInboxEmails = function () {
        if ($scope.EmailItemType == "Inbox")
            fetchNewInboxEmailData();
    }

    $scope.getLatestSentEmails = function () {
        if ($scope.EmailItemType == "Sent")
            fetchNewSentEmailData();
    }

    fetchInboxEmailData = function () {
        $scope.currentEmail = selectedProfile.Email;
        $scope.status = "normal";

        if ($scope.EmailDetails.length > 0) {
            $scope.lastArrIndex = $scope.EmailDetails.length - 1;
            $scope.lastArrDt = $scope.EmailDetails[$scope.lastArrIndex].Date;
            $scope.maxArrDt = $scope.EmailDetails[0].Date;
        }

        OutlookService.GetOutlookInboxEmails($scope.currentEmail, $scope.lastArrDt, $scope.lastArrIndex, $scope.maxArrDt, $scope.status).then(function (l) {
            if ($scope.status == 'normal') {
                if ($scope.PageNumber == 0)
                    $scope.EmailDetails = l.data;
                else {
                    $scope.EmailDetails.push.apply($scope.EmailDetails, l.data);
                }

                for (var css = 0; css < $scope.EmailDetails.length; css++) {
                    if ($scope.EmailDetails[css].IsRead == false)
                        $scope.EmailDetails[css].newMsg = "new_email";
                }
            }

            if (l.data.length > 0) {
                console.log("Outlook Inbox emails fetched Successfully");
            }
            else
                GlobalService.ErrorPopup("Error", "Something went wrong while fetching Outlook Inbox emails.  Check your credentials and network connection and try again.");

        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Outlook Inbox emails data. Please try again.");
            console.log(err);
        });
    }

    fetchNewInboxEmailData = function () {
        $scope.currentEmail = selectedProfile.Email;
        $scope.status = "new";

        if ($scope.EmailDetails.length > 0) {
            $scope.maxArrDt = $scope.EmailDetails[0].Date;
        }

        OutlookService.GetOutlookNewInboxEmails($scope.currentEmail, $scope.lastArrDt, $scope.lastArrIndex, $scope.maxArrDt, $scope.status).then(function (l) {
            var new_rec_cnt = 0;
            for (var itm = l.data.length - 1; itm >= 0 ; itm--) {
                //skipping records, in case of duplication
                var result = $.grep($scope.EmailDetails, function (e) { return e.Date == l.data[itm].Date; });

                if (result.length == 0) {
                    //adding new emails in collection on top of  array
                    $scope.EmailDetails.unshift(l.data[itm]);
                    new_rec_cnt = new_rec_cnt + 1;
                }
            }
            //re-arranging array ids
            for (var rearrange = 0; rearrange < $scope.EmailDetails.length; rearrange++) {
                $scope.EmailDetails[rearrange].Id = rearrange;
            }

            //applying css on new records
            if (new_rec_cnt > 0 && $scope.maxArrDt != null) {
                for (var css = 0; css < new_rec_cnt; css++)
                    $scope.EmailDetails[css].newMsg = "new_email";
            }

            $scope.lastArrIndex = $scope.EmailDetails.length - 1;
            $scope.maxArrDt = $scope.EmailDetails[0].Date;

        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Outlook Inbox emails data. Please try again.");
        });
    }

    fetchSentEmailData = function () {
        $scope.currentEmail = selectedProfile.Email;
        $scope.status = "normal";

        if ($scope.EmailDetails.length > 0) {
            $scope.lastArrIndex = $scope.EmailDetails.length - 1;
            $scope.lastArrDt = $scope.EmailDetails[$scope.lastArrIndex].Date;
            $scope.maxArrDt = $scope.EmailDetails[0].Date;
        }

        OutlookService.GetOutlookSentEmails($scope.currentEmail, $scope.lastArrDt, $scope.lastArrIndex, $scope.maxArrDt, $scope.status).then(function (l) {
            if ($scope.status == 'normal') {
                if ($scope.PageNumber == 0)
                    $scope.EmailDetails = l.data;
                else {
                    $scope.EmailDetails.push.apply($scope.EmailDetails, l.data);
                }
            }

            if (l.data.length > 0) {
                console.log("Outlook Sent emails fetched Successfully");
            }
            else
                GlobalService.ErrorPopup("Error", "Something went wrong while fetching Outlook Sent emails.  Check your credentials and network connection and try again.");

        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Outlook Sent emails data. Please try again.");
            console.log(err);
        });
    }

    fetchNewSentEmailData = function () {
        $scope.currentEmail = selectedProfile.Email;
        $scope.status = "new";

        if ($scope.EmailDetails.length > 0) {
            $scope.maxArrDt = $scope.EmailDetails[0].Date;
        }

        OutlookService.GetOutlookNewSentEmails($scope.currentEmail, $scope.lastArrDt, $scope.lastArrIndex, $scope.maxArrDt, $scope.status).then(function (l) {
            for (var itm = l.data.length - 1; itm >= 0 ; itm--) {
                //skipping records, in case of duplication
                var result = $.grep($scope.EmailDetails, function (e) { return e.Date == l.data[itm].Date; });

                if (result.length == 0) {
                    //adding new emails in collection on top of  array
                    $scope.EmailDetails.unshift(l.data[itm]);
                }
            }
            //re-arranging array ids
            for (var rearrange = 0; rearrange < $scope.EmailDetails.length; rearrange++) {
                $scope.EmailDetails[rearrange].Id = rearrange;
            }

            $scope.lastArrIndex = $scope.EmailDetails.length - 1;
            $scope.maxArrDt = $scope.EmailDetails[0].Date;

        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Outlook Sent emails data. Please try again.");
        });
    }

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.getProfileDetails(msg.GroupId, msg.CompanyId, msg.IsPersonal);
    });

    $scope.init();

    $scope.openProfileConnect = function () {
        $scope.items = "";//{ groupid: $rootScope.selectedGroupId, companyid: $rootScope.selectedCompanyId };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ProfileAdd.html',
            controller: 'Header_ProfileAddController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function () {
        }, function () {
            $scope.getProfileDetails($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString());

        });
    }

    $scope.openOutlookReply = function (obj, type) {
        $scope.items = { obj: obj, type: type, userEmail: $scope.currentEmail };
        var modalInstance = $uibModal.open({
            //animation: true,
            templateUrl: '/Scripts/Views/Shared/_OutlookReply.html',
            controller: '_OutlookReplyController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
        }, function () {
        });
    }

}]);