CRMApp.controller('_PagesAddController', ['$scope', '$window', '$location', '$uibModalInstance', '$http', 'GoogleService', 'FacebookService', '$rootScope', 'GlobalService','notify', function ($scope, $window, $location, $uibModalInstance, $http, GoogleService, FacebookService, $rootScope, GlobalService, notify) {
    //alert($location.search().param);
    $scope.init = function () {
        $scope.CompanyId = $rootScope.selectedCompanyId; //loggedin selected company (BusinessName ID)
        $scope.GroupId = $rootScope.selectedGroupId;
        $scope.IsPersonal = $rootScope.IsPersonal.toString();
        $scope.pagesList = [];
        $scope.currentUserId = jQuery.parseJSON($rootScope.currentUser).UserId;
        $('#lstFruits').multiselect({
            includeSelectAllOption: true,
            buttonWidth: '100%',
        });

        $('#2stFruits').multiselect({
            includeSelectAllOption: true,
            buttonWidth: '100%',
        });

        $('#3stFruits').multiselect({
            includeSelectAllOption: true,
            buttonWidth: '100%',
        });

        $('#soclistspop2').slimscroll({
            height: '280px',
            disableFadeOut: true
        });
        $scope.PageType = $location.search().pageType;
        $scope.iconURL = $scope.PageType == 'facebook' && '/Content/images/fb_ico.png' ||
                         ($scope.PageType == 'linkedin' && '/Content/images/in_ico.png' ||
                         ($scope.PageType == 'google' && '/Content/images/ana_ico.png' || ''));
        $scope.classType = $scope.PageType == 'facebook' && '/Content/images/fb_ico16.png' ||
                         ($scope.PageType == 'linkedin' && '/Content/images/in_ico16.png' ||
                         ($scope.PageType == 'google' && '/Content/images/ana_ico16.png' || ''));

        if ($scope.PageType == 'google') {
            $scope.dataLoaded = false;
            var useraccountInfo = $location.search().userInfo;

            GoogleService.GetPages(useraccountInfo).then(function (l) {
                var response = l.data;
               // alert(response)
               $scope.Connected_Pages = response;
              //$scope.Connected_Pages = [{ "Id": 33, "AccountId": 31288197, "ProfileId": "59236599", "AccountName": "TPL TRAKER", "AccountUrl": "http://www.tpltrakker.com/", "IsConnected": true }, { "Id": 34, "AccountId": 39128642, "ProfileId": "69960025", "AccountName": "All Web Site Data", "AccountUrl": "http://www.ibexglobal.com", "IsConnected": true }, { "Id": 35, "AccountId": 74041817, "ProfileId": "117032321", "AccountName": "All Web Site Data", "AccountUrl": "http://127.0.0.1", "IsConnected": false }, { "Id": 36, "AccountId": 74041817, "ProfileId": "118628373", "AccountName": "my test view", "AccountUrl": "http://127.0.0.1", "IsConnected": true }];
                $scope.dataLoaded = true;
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching google analytics pages Data, please try again.");
               // alert('Error in fetching pages data');
                //console.log(err);
            });
        }
        else if ($scope.PageType == 'facebook') {
            $scope.dataLoaded = false;
            var useraccountInfo = $location.search().userInfo;

            FacebookService.GetPages(useraccountInfo).then(function (l) {
                var response = l.data;
                // alert(response)
                $scope.Connected_Pages = response;
                //$scope.Connected_Pages = [{ "Id": 33, "AccountId": 31288197, "ProfileId": "59236599", "AccountName": "TPL TRAKER", "AccountUrl": "http://www.tpltrakker.com/", "IsConnected": true }, { "Id": 34, "AccountId": 39128642, "ProfileId": "69960025", "AccountName": "All Web Site Data", "AccountUrl": "http://www.ibexglobal.com", "IsConnected": true }, { "Id": 35, "AccountId": 74041817, "ProfileId": "117032321", "AccountName": "All Web Site Data", "AccountUrl": "http://127.0.0.1", "IsConnected": false }, { "Id": 36, "AccountId": 74041817, "ProfileId": "118628373", "AccountName": "my test view", "AccountUrl": "http://127.0.0.1", "IsConnected": true }];
                $scope.dataLoaded = true;
            }, function (err) {
                //alert('Error in fetching pages data');
                GlobalService.ErrorPopup("Error", "Error in fetching facebook pages Data, please try again.");
                //console.log(err);
            });
        }
        else {
            $scope.Connected_Pages = [{ "Id": 33, "AccountId": 31288197, "ProfileId": "59236599", "AccountName": "TPL TRAKER", "AccountUrl": "http://www.tpltrakker.com/", "IsConnected": true }, { "Id": 34, "AccountId": 39128642, "ProfileId": "69960025", "AccountName": "All Web Site Data", "AccountUrl": "http://www.ibexglobal.com", "IsConnected": true }, { "Id": 35, "AccountId": 74041817, "ProfileId": "117032321", "AccountName": "All Web Site Data", "AccountUrl": "http://127.0.0.1", "IsConnected": false }, { "Id": 36, "AccountId": 74041817, "ProfileId": "118628373", "AccountName": "my test view", "AccountUrl": "http://127.0.0.1", "IsConnected": true }];
        }
    }
    $scope.init();

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });
    
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.connect = function () {
        var leng = $scope.Connected_Pages.length;
        //selected Group's ID
        // alert("pages are connected");
        var teamMemberId = $scope.currentUserId;//loggedin User's ID
        var companyId = $scope.CompanyId;
        var groupId = $scope.GroupId;
        var useraccountInfo = $location.search().userInfo;
        var userInfo = useraccountInfo.split(',');
        if (userInfo.length > 2) {
            teamMemberId = userInfo[0];
            companyId = userInfo[1];
            groupId = userInfo[2];
        }

        if ($scope.PageType == 'google') {
            for (i = 0; i < leng; i++) {
                var IsConnected = false;
                if ($("#" + $scope.Connected_Pages[i].Id + "page").is(':checked')) {
                    IsConnected = true;
                }
                var obj = { 'AnalyticId': $scope.Connected_Pages[i].Id, 'CompanyId': companyId, 'GroupId': groupId, 'TeamMemberId': teamMemberId, 'IsConnected': IsConnected }

                $scope.pagesList.push(obj)
            }
            GoogleService.AddPages($scope.pagesList).then(function (l) {
                // $location.path("/Signup_Code").search({ param: $scope.Email });
               // alert("saved");
                notify({
                    message: "Sucessfully added pages data",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $rootScope.$broadcast('Group_Profile_Reload', { GroupId: $scope.GroupId, CompanyId: $scope.CompanyId, IsPersonal: $scope.IsPersonal });
                $uibModalInstance.close('cancel');

            }, function (err) {
               // alert('Error in Adding Analytics, please try again.');
                GlobalService.ErrorPopup("Error", "Error in Adding Analytics, please try again.");

               // console.log(err);
            });
        }
        else if ($scope.PageType == 'facebook') {
            for (i = 0; i < leng; i++) {
                var IsConnected = false;
                if ($("#" + $scope.Connected_Pages[i].Id + "page").is(':checked')) {
                    IsConnected = true;
                }
                var obj = { 'Id': $scope.Connected_Pages[i].Id, 'AccountId': $scope.Connected_Pages[i].AccountId, 'ProfileId': $scope.Connected_Pages[i].ProfileId, 'AccountName': $scope.Connected_Pages[i].AccountName, 'AccountUrl': $scope.Connected_Pages[i].AccountUrl, 'IsConnected': IsConnected }

                $scope.pagesList.push(obj)
            }

            FacebookService.AddPages($scope.pagesList, companyId, groupId).then(function (l) {
                notify({
                    message: "Sucessfully added Facebook Page(s)",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $rootScope.$broadcast('Group_Profile_Reload', { GroupId: $scope.GroupId, CompanyId: $scope.CompanyId, IsPersonal: $scope.IsPersonal });
                 $uibModalInstance.dismiss('cancel');//$uibModalInstance.close('cancel');
            }, function (err) {
                //alert('Error in Signup, please try again.');
                GlobalService.ErrorPopup("Error", "Error in adding facebook pages, please try again.");

                console.log(err);
            });
        }
    }

    $scope.AddPageList=function(Id,IsConnected)
    {
        //var CompanyId = 1; //loggedin selected company (BusinessName ID)
        //var GroupId = 1;//selected Group's ID
        //var TeamMemberId = 6;//loggedin User's ID
        //var obj = { 'AnalyticId': Id, 'CompanyId': CompanyId, 'GroupId': GroupId, 'TeamMemberId': TeamMemberId, 'IsConnected': IsConnected }
        //$scope.pagesList = $.grep($scope.pagesList, function (e) {
        //    return e.Id != Id;
        //});
        //if ($("#" + Id + "page").is(':checked'))
        //{
        //    $scope.pagesList.push(obj)
        //}

        //alert(JSON.stringify($scope.pagesList))
    }
    $scope.ConnectTwitter = function () {

        $window.location.href = '/Profile/Connect_Twitter/' + $scope.CompanyId + '/' + $scope.GroupId;
    }

    $scope.ConnectFacebookProfile = function () {

        $window.location.href = '/Profile/Connect_Facebook/' + $scope.CompanyId + '/' + $scope.GroupId;
    }


    $scope.ConnectLinkedinProfile = function () {

        $window.location.href = '/Profile/Connect_LinkedIn/' + $scope.CompanyId + '/' + $scope.GroupId;
    }


    $scope.ConnectGoogle = function () {
        $window.location.href = '/Profile/ConnectGoogleAnalytics/' + $scope.CompanyId + '/' + $scope.GroupId;
    }
    $scope.ConnectInstagram = function () {
        $window.location.href = '/Profile/ConnectInstagram/' + $scope.CompanyId + '/' + $scope.GroupId;
    };

}]);