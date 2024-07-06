CRMApp.controller('TaskListController', ['$scope', '$window', '$location', 'TaskService', 'UnifiedService', '$stateParams', 'GroupService', 'ProfileService', '$rootScope', 'GlobalService', 'notify', '$uibModal', function ($scope, $window, $location, TaskService, UnifiedService, $stateParams, GroupService, ProfileService, $rootScope, GlobalService, notify, $uibModal) {
    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';
    $scope.instaIcon = '/Content/images/insta_ico16.png';    
    $scope.recentFilteredTaskId = 0;
    $scope.selected = {};
    $scope.consumerProfiles;
    $scope.consumerProfilesMain;
    $scope.searchProfile = "";
    $scope.select = true;

    $scope.attachedProfileModel = [];
    $scope.taskAssigneemodel = [];
    $scope.PageType = $stateParams.pageType;//AllTasks
        //alert()
    $scope.searchsettings = {
        enableSearch: true
    };
    $scope.profilesettings = {
        enableSearch: true,
        selectionLimit: 1,
        smartButtonMaxItems: 2,
        smartButtonTextConverter: function (itemText, originalItem) {

            return itemText;
        }
    }
    //DropDowns
    if ($scope.PageType == "AllCase")
    {
        $scope.PageTypeHeading = "Cases List";
        $scope.statusVis = true;
        $scope.taskStatusModel = [{"id": "1"},{"id": "2"}];
        $scope.taskStatus = [
        {
            "label": "Open",
            "id": "1"
        }, {
            "label": "Close",
            "id": "2"
        }
        ];
    }
    else
    {
        $scope.PageTypeHeading = "Closed Cases";
        $scope.statusVis = false;
        $scope.taskStatusModel = [{ "id": "2" }];
        $scope.taskStatus = [
        {
            "label": "Close",
            "id": "2"
        }
        ];
    }
       
    $scope.taskPriorityModel = [];
    $scope.taskPriority = [
        {
            "label": "High",
            "id": "1"
        }, {
            "label": "Medium",
            "id": "2"
        }, {
            "label": "Low",
            "id": "3"
        }];
    $scope.taskPriorityModel = [
        {
            "id": "1"
        }, {
            "id": "2"
        }, {
            "id": "3"
        }]; //$scope.taskPriority;
    $scope.groupsettings = {
        //selectionLimit: 1,
        //smartButtonMaxItems: 2,
        //smartButtonTextConverter: function (itemText, originalItem) {

        //    return itemText;
        //}
    }
        
           
       
    $scope.taskClassification = [
        {
            "label": "Sales Lead",
            "id": "1"
        }, {
            "label": "After Sales",
            "id": "2"
        }, {
            "label": "Compliment",
            "id": "3"
        },
        {
            "label": "Complaint",
            "id": "4"
        }, {
            "label": "General Inquiry",
            "id": "5"
        }, {
            "label": "Referral",
            "id": "6"
        }, {
            "label": "Review",
            "id": "7"
        }];
    $scope.taskClassificationModel = [
        {
            "id": "1"
        }, {
            "id": "2"
        }, {
            "id": "3"
        },
        {
            "id": "4"
        }, {
            "id": "5"
        }, {
            "id": "6"
        }, {
            "id": "7"
        }];

    $scope.GetTeamDetails = function (data) {
        $scope.taskAssignee = [];
        $scope.taskAssigneemodel = [];
        GroupService.GetTeamByGroupIdForTaskList(data).then(function (l) {

            if (l != null) {
                var teamData = l.data.Team;

                for (i = 0; i < teamData.length; i++) {
                    var obj = { id: teamData[i].TeamMemberId, label: teamData[i].Name }
                    $scope.taskAssignee.push(obj);
                    $scope.taskAssigneemodel.push({ id: teamData[i].TeamMemberId });
                }
                // $scope.taskAssigneemodel = $scope.taskAssignee;
            }
            else {
                //alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            }

            $scope.GetGroupDetails($scope.GroupId);

        }, function (err) {
            //alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    };
    $scope.GetGroupDetails = function (data) {
        $scope.attachedProfiles = [];
        $scope.attachedProfileModel = [];
        ProfileService.GetByGroupId(data).then(function (l) {

            if (l != null) {
                // alert(JSON.stringify(l.data))
                //  $scope.Connected_Profiles = l.data
                var profileData = l.data;

                for (i = 0; i < profileData.length; i++) {
                    var obj = { id: profileData[i].ProfileId, label: profileData[i].ProfileName }
                    $scope.attachedProfiles.push(obj);
                    $scope.attachedProfileModel.push({ id: profileData[i].ProfileId });
                }
                // $scope.attachedProfileModel = $scope.attachedProfiles;// { id: $scope.attachedProfiles[0].id };
            }
            else {
                // alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            }

            $scope.Search();
        }, function (err) {
            //alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });

    };
    $scope.init = function () {
        $scope.GroupId = $rootScope.selectedGroupId;
        // $scope.taskClassification;

        /*GroupService.GetUserCompanyGroups().then(function (l) {
            if (l.data != null) {
                $scope.grouplist = l.data;
                $scope.AssigneeGroupSelected = l.data.filter(function (el) {
                    return el.GroupId.toString() == $scope.GroupId;
                })[0];
            }
        }, function (err) {
            // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });*/
        if ($rootScope.selectedGroupId != "0" || $rootScope.IsPersonal == true) {
            $scope.GetTeamDetails($scope.GroupId);
            $scope.getProfileDetails($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString());
        }
        
    };
    $scope.fetchProfiles = function () {
        var selectedProfiles = $.grep($scope.Profiles, function (record) {
            return $scope.selected[record.ProfileId];
        });

        var profileData = [];
        var profilePageData = [];
        $.each(selectedProfiles, function (key, value) {
            if (value.IsPage == true) {
                profilePageData.push(value.ProfileId);
            }
            else {
                profileData.push(value.ProfileId);
            }
        });
        var commaSeperatedProfiles = profileData.join(",");
        if (commaSeperatedProfiles == "")
            commaSeperatedProfiles = "0";
        var commaSeperatedPages = profilePageData.join(",");
        if (commaSeperatedPages == "")
            commaSeperatedPages = "0";

        var groupId = $rootScope.selectedGroupId;

        TaskService.GetProfiles(commaSeperatedProfiles, commaSeperatedPages,groupId).then(function (l) {
                var response = l.data;
                $scope.consumerProfiles = $scope.consumerProfilesMain = response;
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching Inbox data");
                // alert('Error in fetching Inbox data');
                //console.log(err);
            });

    };

    $scope.checkAll = function () {
        angular.forEach($scope.Profiles, function (obj) {
            $scope.selected[obj.ProfileId] = $scope.select;
        });

        $scope.fetchProfiles();
    };

    $scope.profileSelectedChanged = function () {
        $scope.select = false;
        $scope.fetchProfiles();
    };

    $scope.openProfileDetails = function (profileId) {
        var groupId = $rootScope.selectedGroupId;
        $scope.items = { CustomerProfileId: profileId,GroupId: groupId };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_CustomerProfileDetails.html',
            controller: '_CustomerProfileDetailsController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //  $scope.selected = selectedItem;
            //  alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    };

    
    $scope.getProfileDetails = function (groupId, companyId, isPersonal) {
        $scope.IsPersonal = $rootScope.IsPersonal.toString();


        UnifiedService.GetAllProfilesForUserGroup(groupId, companyId, isPersonal).then(function (l) {
            var response = l.data;
            $scope.Profiles = response;
            angular.forEach($scope.Profiles, function (obj) {
                $scope.selected[obj.ProfileId] = true;
            });
            $scope.fetchProfiles();
        }, function (err) {
            // alert('Error in fetching User Profiles');
            GlobalService.ErrorPopup("Error", "Error in fetching User Profiles.");
            //console.log(err);
        });
    };

    $scope.filterProfiles = function(){
        var searchVal = $scope.searchProfile;
        if (!searchVal) {
            $scope.consumerProfiles = $scope.consumerProfilesMain;
            return;
        }
        var filteredProfiles = $.grep($scope.consumerProfilesMain, function (record) {
            return record.ProfileName.toLowerCase().indexOf(searchVal) !== -1
        });
        $scope.consumerProfiles = filteredProfiles;
    };

    $scope.Search = function () {

        var SelectedClassifications = [];
        var SelectedPriorities = [];
        var SelectedTeamAssigneeMemberId = [];
        var ProfileId = [];//$scope.attachedProfileModel.id;
        var SelectedStatus = [];//$scope.taskStatusModel.id;
        for (i = 0; i < $scope.taskStatusModel.length; i++) {
            SelectedStatus.push($scope.taskStatusModel[i].id);
        }
        for (i = 0; i < $scope.taskClassificationModel.length; i++) {
            SelectedClassifications.push($scope.taskClassificationModel[i].id);
        }
        for (i = 0; i < $scope.attachedProfileModel.length; i++) {
            ProfileId.push($scope.attachedProfileModel[i].id);
        }
        for (i = 0; i < $scope.taskPriorityModel.length; i++) {
            SelectedPriorities.push($scope.taskPriorityModel[i].id);
        }
        for (i = 0; i < $scope.taskAssigneemodel.length; i++) {
            SelectedTeamAssigneeMemberId.push($scope.taskAssigneemodel[i].id);
        }
        TaskService.GetFilteredTask(SelectedClassifications, SelectedPriorities, SelectedTeamAssigneeMemberId, ProfileId, SelectedStatus, $rootScope.selectedGroupId).then(function (l) {
            // $location.path("/Signup_Code").search({ param: $scope.Email });;
            $scope.logoUrl = '/Content/img/user2.jpg';
            $scope.readMsgCss = 'unread_msg';//read_msg
            $scope.openClosedCss = SelectedStatus == 1 && 'open_task' || 'closed_task';//
            $scope.taskList = l.data;           

            if ($scope.taskList.length > 0)
                $scope.recentFilteredTaskId = Math.max.apply(Math, $scope.taskList.map(function (o) { return o.TaskId; })) > $scope.recentFilteredTaskId ? Math.max.apply(Math, $scope.taskList.map(function (o) { return o.TaskId; })) : $scope.recentFilteredTaskId;

            //UserProfile
            //alert(JSON.stringify(l.data))
        }, function (err) {
            // alert('Error in saving, please try again.');
            GlobalService.ErrorPopup("Error", "Error in saving, please try again.");
            console.log(err);
        });


    };
    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    };
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
        angular.forEach($scope.Profiles, function (obj) {
            $scope.selected[obj.ProfileId] = true;
        });
        $scope.select = true;
        //$scope.getProfileDetails(msg.GroupId, msg.CompanyId, msg.IsPersonal);
        //$scope.GetGroupHead(msg.GroupId);
        //$scope.GetGroupDetails(msg.GroupId);
        //$scope.GetTeamDetails(msg.GroupId);
    });
    $scope.init();

    $scope.getLatestFilteredTasks = function () {

        var SelectedClassifications = [];
        var SelectedPriorities = [];
        var SelectedTeamAssigneeMemberId = [];
        var ProfileId = [];//$scope.attachedProfileModel.id;
        var SelectedStatus = [];//$scope.taskStatusModel.id;
        for (i = 0; i < $scope.taskStatusModel.length; i++) {
            SelectedStatus.push($scope.taskStatusModel[i].id);
        }
        for (i = 0; i < $scope.taskClassificationModel.length; i++) {
            SelectedClassifications.push($scope.taskClassificationModel[i].id);
        }
        for (i = 0; i < $scope.attachedProfileModel.length; i++) {
            ProfileId.push($scope.attachedProfileModel[i].id);
        }
        for (i = 0; i < $scope.taskPriorityModel.length; i++) {
            SelectedPriorities.push($scope.taskPriorityModel[i].id);
        }
        for (i = 0; i < $scope.taskAssigneemodel.length; i++) {
            SelectedTeamAssigneeMemberId.push($scope.taskAssigneemodel[i].id);
        }
        TaskService.GetFilteredTask(SelectedClassifications, SelectedPriorities, SelectedTeamAssigneeMemberId, ProfileId, SelectedStatus, $rootScope.selectedGroupId).then(function (l) {
            $scope.logoUrl = '/Content/img/user2.jpg';
            $scope.readMsgCss = 'unread_msg';//read_msg
            $scope.openClosedCss = SelectedStatus == 1 && 'open_task' || 'closed_task';//

            var response = l.data;
            var data = $.grep(response, function (record) {
                return record.TaskId > $scope.recentFilteredTaskId
            });

            data.forEach(function (i) {
                i.CssClass = "new_msg";
            });
            Array.prototype.push.apply(data, $scope.taskList)

            $scope.taskList = data;

            if ($scope.taskList.length > 0)
                $scope.recentFilteredTaskId = Math.max.apply(Math, $scope.taskList.map(function (o) { return o.TaskId; })) > $scope.recentFilteredTaskId ? Math.max.apply(Math, $scope.taskList.map(function (o) { return o.TaskId; })) : $scope.recentFilteredTaskId;
                        
        }, function (err) {
            // alert('Error in saving, please try again.');
            GlobalService.ErrorPopup("Error", "Error in saving, please try again.");
            console.log(err);
        });


    };
}]);