CRMApp.controller('GroupSettingsController', ['$rootScope', '$scope', '$window', '$location', '$uibModal', 'GroupService', 'ProfileService', '$compile', function ($rootScope, $scope, $window, $location, $uibModal, GroupService, ProfileService, $compile) {
    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';

    //$scope.Connected_Profiles = [
    //                {
    //                    'ProfileName': 'Agha Rameez Ahmed',
    //                    'ProfileURL': 'Agha.Rameez.Ahmed1234',
    //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
    //                    'ProfileTypeId': 1,
    //                    'type': 'facebook'
    //                }
    //];

    //$scope.Connected_Team = [
    //                {
    //                    'Name': 'Agha Rameez Ahmed',
    //                    'Email': 'Agha.Rameez.Ahmed1234'
    //                }
    //];

    $scope.init = function () {
        $scope.CompanyName = jQuery.parseJSON($rootScope.currentUser).CompanyName;
        $scope.CompanyId = $window.localStorage.getItem('selectedCompanyId');
        GroupService.GetByCompanyId($scope.CompanyId).then(function (l) {
            if (l.data != null) {
                if ($window.localStorage.getItem('selectedGroupId') != undefined) {
                    $scope.GroupId = $window.localStorage.getItem('selectedGroupId');
                }
                else{
                    $scope.GroupId = l.data[0].GroupId;
                }
                $scope.groups = l.data;
                $scope.GroupCount = l.data.length;
                //$compile($("#grplists ul"))($scope);
                $scope.GetGroupHead($scope.GroupId);
                $scope.GetGroupDetails($scope.GroupId);
                $scope.GetTeamDetails($scope.GroupId);
            }
            else {
                alert('Error in fetching data, please try again.');
            }
        }, function (err) {
            alert('Error in fetching data, please try again.');
            console.log(err);
        });
    }
    $scope.init();

    $scope.GetGroupHead = function (data) {
        GroupService.GetById(data).then(function (l) {
            if (l != null) {
                $scope.TotalProfiles = l.data.TotalProfiles;
                $scope.TotalMembers = l.data.TotalMembers;
                $scope.GroupName = l.data.GroupName;
                $scope.GroupId = l.data.GroupId;
                $scope.CompanyId = l.data.CompanyId;
            }
            else {
                //  alert('No Data');
            }
        }, function (err) {
            alert('Error in fetching data, please try again.');
            console.log(err);
        });



    }
    $scope.GetGroupDetailsClick = function (data) {
        $scope.GetGroupHead(data);
        $scope.GetGroupDetails(data);
        $scope.GetTeamDetails(data);
    }
    $scope.GetGroupDetails = function (data) {

        ProfileService.GetByGroupId(data).then(function (l) {
            if (l != null) {
                $scope.Connected_Profiles = l.data
            }
            else {
                alert('Error in fetching data, please try again.');
            }
        }, function (err) {
            alert('Error in fetching data, please try again.');
            console.log(err);
        });

    }
    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    };
    $scope.GetTeamDetails = function (data) {
        GroupService.GetTeamByGroupId(data).then(function (l) {
            if (l != null) {
                $("#ulists ul").empty()
                $.each(l.data.Team, function (index, item) {
                    
                    var imageUrl = item.ProfileUrl == null && '/Content/img/user_pic.jpg' ||$scope.replaceTilda(item.ProfileUrl);
                   // alert(JSON.stringify(item))
                    $("#ulists ul").append('<li ><a ><img src="' + imageUrl + '" style="height:80px"  alt="User Image"></a><a class="users-list-name" >' + item.Name + '</a><span class="users-list-date"><a >' + item.Email + '</a></span></li>');
                });
                $("#ulists ul").append('<li><a  data-toggle="modal" data-target="#newUserModal"><img src="/Content/img/user_add.jpg" alt="User Image"></a><a class="users-list-name" href="#/GroupSettings/userAdd">Invite New</a><span class="users-list-date"><a href="#/GroupSettings/userAdd">Team Member</a></span></li>');
            }
            else {
                alert('Error in fetching data, please try again.');
            }
        }, function (err) {
            alert('Error in fetching data, please try again.');
            console.log(err);
        });
    }
    $scope.ConnectTwitter = function () {

        $window.location.href = '/Profile/Connect_Twitter/' + $scope.CompanyId + '/' + $scope.GroupId;
    }

    $scope.RemoveProfileFromGroup = function (ProfileId, CompanyId, ProfileType , IsPage) {
        //alert(ProfileId);
        //alert(CompanyId);
        //alert($scope.GroupId);
        if (IsPage)
            ProfileType = 5;
        ProfileService.RemoveProfileToGroup(ProfileId, CompanyId, $scope.GroupId,ProfileType).then(function (l) {
            $("#ConnectedProfile" + ProfileId).remove();
            alert("profile removed")
        }, function (err) {
            alert('Error in remove profile, please try again.');
            console.log(err);
        });
    }

    //$rootScope.$on('Group_Reload', function (evt, msg) {
    //    //alert("in broadcast")
    //    $scope.init();
    //});
    //$rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        
    //    $scope.GetGroupHead(msg.GroupId);
    //    $scope.GetGroupDetails(msg.GroupId);
    //    $scope.GetTeamDetails(msg.GroupId);
    //});
    //$rootScope.$on('Team_Reload', function (evt, msg) {
    //    $scope.GetGroupHead(msg);
    //    $scope.GetTeamDetails(msg);
    //});
}]);


