CRMApp.controller('HeaderController', ['$rootScope', '$scope', '$window', '$location', '$compile', 'AuthService', 'GroupService', '$uibModal', '$cookies', 'AuthService', 'CompanyService', 'GlobalService', 'notify', 'NotificationService', function ($rootScope, $scope, $window, $location, $compile, AuthService, GroupService, $uibModal, $cookies, AuthService, CompanyService, GlobalService, notify, NotificationService) {

    $scope.redirectTo = function (path) {
        $window.location.href = path;
    };

    $rootScope.$on('Group_Deleted', function (evt, msg) {

        for (var i = 0; i < $scope.grouplist.length; i++) {
            var obj = $scope.grouplist[i];

            if (msg.GroupId == obj.GroupId)
            {
                $scope.grouplist.splice(i, 1);
                $scope.TotalGroups = $scope.TotalGroups - 1;
            }
            
        }
    });
    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    };

    $rootScope.$on('Group_Reload2', function (evt, msg) {
        //alert("in broadcast")
        $scope.broadcastedGroupId = msg.GroupId;
        $scope.init();
    });
    $rootScope.$on('Group_Dropdown_Text', function (evt, msg) {
        //alert("in broadcast")
        // $scope.init();
        angular.forEach($scope.grouplist, function (val, key) {
            if (val.GroupId == msg.GroupId)
                $scope.grouplist[key].GroupName = msg.GroupName;
        });

    });
    $scope.UpdateCurrentUserSession = function () {
        AuthService.UserProfile(jQuery.parseJSON($rootScope.currentUser).Email, $scope.CompanyId).then(function (l) {
            if (l != null) {
                $cookies.putObject("userSessionCRM", l.data, { path: '/' });
                $rootScope.currentUser = JSON.stringify(l.data);
            }
            else {
               // alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            }
        }, function (err) {
           // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }
    $rootScope.$on('Business_Name_Change', function (evt, msg) {
        //alert("in broadcast")
        //$scope.CompanyName = msg;
        angular.forEach($scope.grouplist, function (val, key) {
            if (val.CompanyId == msg.CompanyId)
                $scope.grouplist[key].CompanyName = msg.CompanyName;
        });
        //Had to compile it again cause just changing the object is not changing value of company in dropdown view
        var select = document.querySelector('#GroupSelector');
        angular.element(select).replaceWith($compile(select)($scope));

        if (msg.CompanyId == $rootScope.selectedCompanyId) {
            $rootScope.selectedCompanyName = msg.CompanyName;
            $scope.CompanyName = $rootScope.selectedCompanyName;
        }

        $scope.UpdateCurrentUserSession();
    });
    $rootScope.$on('Profile_Pic_Change', function (evt, msg) {
        //alert("in broadcast")
        AuthService.UserProfile(jQuery.parseJSON($rootScope.currentUser).Email, $rootScope.selectedCompanyId).then(function (l) {
            if (l != null) {
                var d = new Date();
                $cookies.putObject("userSessionCRM", l.data, { path: '/' });
                $rootScope.currentUser = JSON.stringify(l.data);
                $scope.UserImage = jQuery.parseJSON($rootScope.currentUser).ImageUrl + "?" + d.getTime();
                $scope.UserImage = $scope.UserImage == null && "/Content/img/user2-160x160.jpg" || $scope.replaceTilda($scope.UserImage);
                $scope.UserName = jQuery.parseJSON($rootScope.currentUser).FirstName + ' ' + jQuery.parseJSON($rootScope.currentUser).LastName;
                $scope.CompanyName = $rootScope.selectedCompanyName;
                //$scope.init();
            }
            else {
              //  alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            }
        }, function (err) {
           // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });

    });
    $scope.mode = "Company";
    $rootScope.IsPersonal = false;
    $scope.CompanyShow = true;
    $scope.init = function () {
        $rootScope.selectedCompanyId = $rootScope.FirstCompanyId;
        $rootScope.selectedCompanyName = $rootScope.FirstCompanyName;
        $rootScope.selectedGroupId = $rootScope.FirstGroupId;
        $scope.SetNavLinks();

        $scope.GroupId;
        $scope.CompanyId = 1;
        $scope.UserName = jQuery.parseJSON($rootScope.currentUser).FirstName + ' ' + jQuery.parseJSON($rootScope.currentUser).LastName;
        $scope.UserImage = jQuery.parseJSON($rootScope.currentUser).ImageUrl;
        $scope.UserImage = $scope.UserImage == null && "/Content/img/user2-160x160.jpg" || $scope.replaceTilda($scope.UserImage);

        GroupService.GetUserCompanyGroups().then(function (l) {
            if (l.data != null) {
                $scope.grouplist = l.data;
                //$scope.grouplist.push({ 'CompanyId': -1, 'CompanyName': 'Personal Mode', 'GroupId': 0, 'GroupName': 'Enter Personal Mode', 'IsActive': false, 'TotalMembers': 0, 'TotalProfiles': 0 })
                $scope.GroupSelected = l.data[0];
                if ($rootScope.selectedGroupId != undefined && $rootScope.selectedCompanyId != undefined) {

                    var isFound = false;
                    //$scope.GroupSelected = l.data[3];
                    for (x = 0; x < l.data.length; x++) {
                        if (l.data[x].GroupId == $rootScope.selectedGroupId && l.data[x].CompanyId == $rootScope.selectedCompanyId) {
                            $scope.GroupSelected = l.data[x];
                            $scope.selectedTemplate($scope.GroupSelected, true);
                            $scope.GroupId = $rootScope.selectedGroupId;
                            $scope.CompanyId = $rootScope.selectedCompanyId;
                            $scope.GetTeamGroupProfileCount($scope.CompanyId);
                            isFound = true;
                            break;
                        }

                    }
                    if (!isFound) {
                        $rootScope.selectedGroupId = l.data[0].GroupId;
                        $scope.GroupId = $rootScope.selectedGroupId;
                        $scope.selectedTemplate(l.data[0], true);


                        $rootScope.selectedCompanyId = l.data[0].CompanyId;
                        $rootScope.selectedCompanyName = l.data[0].CompanyName;
                        $scope.CompanyId = $rootScope.selectedCompanyId;
                        $scope.GetTeamGroupProfileCount($scope.CompanyId);
                    }
                }
                else {
                    $rootScope.selectedGroupId = l.data[0].GroupId;
                    $scope.GroupId = $rootScope.selectedGroupId;
                    $scope.selectedTemplate(l.data[0], true);

                    $rootScope.selectedCompanyId = l.data[0].CompanyId;
                    $rootScope.selectedCompanyName = l.data[0].CompanyName;
                    $scope.CompanyId = $rootScope.selectedCompanyId;
                    $scope.GetTeamGroupProfileCount($scope.CompanyId);
                }

                if ($scope.broadcastedGroupId != undefined) {
                    $scope.GroupId = $scope.broadcastedGroupId;
                }
                $scope.CompanyName = $rootScope.selectedCompanyName;
            }
        }, function (err) {
            // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }

    $scope.SendNotificationEmailUpdates = function()
    {
        NotificationService.SendNotificationEmailUpdates();
    }

    $scope.selectedTemplate = function (obj, firstTime) {
        CompanyService.IsCompanyActivatedForUser(obj.CompanyId).then(function (l) {
            if (l.data.IsCompanyActivated == false) {
                $scope.items = { CompanyId: obj.CompanyId, ConfirmationTitle: "Company Account Activation", ConfirmationMessage: "Are you sure, you want to activate this company account?" };
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/Scripts/Views/Shared/_ConfirmationPopUp.html',
                    controller: '_ActivateCompanyAccountController',

                    resolve: {
                        items: function () {
                            return $scope.items;
                        }
                    }
                });

                modalInstance.result.then(function (selectedItem) {
                    if (selectedItem == 'Activated' && !firstTime) {
                        $scope.SetGroupValues(obj);
                    }
                    else if (selectedItem == 'Deactivated') {
                        $("#ddlgroup optgroup[label='" + obj.CompanyName + "']").remove();
                        // $("#ddlgroup option[value='" + obj.GroupId + "']").remove();
                        obj.GroupId = $rootScope.selectedGroupId;
                        obj.CompanyId = $rootScope.selectedCompanyId;
                        $scope.GroupSelected = obj;
                        //$rootScope.$broadcast('Group_Reload', {
                        //    GroupId: obj.GroupId, CompanyId: obj.CompanyId, IsPersonal: true
                        //});
                        $window.location.reload();
                    }

                }, function () {
                    // $log.info('Modal dismissed at: ' + new Date());
                });
            }
            else if (!firstTime) {
                $scope.SetGroupValues(obj);
            }
            
            //moved here to open Social Feeds and Email in Personal mode
            //$scope.AssignRights(l.data.AccessRights, l.data.AccessReports);
            $scope.SetNavLinks();

            }, function (err) {
                //alert('Error in fetching IsCompanyActivatedForUser, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching user data, please try again.");
                console.log(err);
            });
    }

    $scope.SetGroupValues = function (obj) {
        $rootScope.selectedGroupId = obj.GroupId;
        $scope.GroupId = $rootScope.selectedGroupId;

        $rootScope.selectedCompanyId = obj.CompanyId;
        $rootScope.selectedCompanyName = obj.CompanyName;
        //$window.sessionStorage.selectedCompanyId = obj.CompanyId;   
        $scope.CompanyName = $rootScope.selectedCompanyName;
        $scope.CompanyId = obj.CompanyId;
        if (obj.GroupId == 0) {
            $scope.mode = "Personal";
            $rootScope.IsPersonal = true;
            $scope.CompanyShow = false;
            $rootScope.$broadcast('Group_Profile_Reload', { GroupId: obj.GroupId, CompanyId: obj.CompanyId, IsPersonal: 'true' });
        }
        else {
            $scope.mode = "Company";
            $scope.CompanyShow = true;
            $rootScope.IsPersonal = false;
            $scope.GetTeamGroupProfileCount(obj.CompanyId);

            $rootScope.$broadcast('Group_Profile_Reload', { GroupId: obj.GroupId, CompanyId: obj.CompanyId, IsPersonal: 'false' });
        }

        $scope.UpdateCurrentUserSession();
    }

    $scope.openCompose = function () {
        $scope.items = { groupId: $scope.GroupId, companyId: $scope.CompanyId };
        var modalInstance = $uibModal.open({
            animation: true,
            size: 'lg',
            templateUrl: '/Scripts/Views/Shared/_ComposeMessage.html',
            controller: '_ComposeMessageController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.openUserAdd = function () {
        $scope.items = { groupId: $scope.GroupId, companyId: $scope.CompanyId };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_UserAdd.html',
            controller: 'Header_UserAddController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.SetNavLinks = function () {
        if ($scope.mode == "Company") {
            $scope.navLinks = [
           {
               mainLink: 'Inbox Manager',
               description: 'messages & social feeds',
               icon: 'fa-envelope',
               hide: false,
               subLink: [
                   {
                       //Title: 'Unified Deck'
                       Title: 'Responder',
                       url: 'Responder',
                       parameter: '',
                       hide: false
                   },
                   {
                       Title: 'Completed',
                       url: 'Completed',
                       parameter: '',
                       hide: false
                   },
                   {
                       Title: 'Unified Inbox',
                       url: 'UnifiedInbox',
                       parameter: '',
                       hide: false
                   }, 
                    {
                        Title: 'Archive Inbox',
                        url: 'ArchiveInbox',
                        parameter: '',
                        hide: false
                    }
               ]
           },
           {
            mainLink: 'CUSTOMER MANAGER',
            description: 'consumer, banned & warned',
            icon: 'fa-users',
            hide: false,
            subLink: [
                {
                    Title: 'Customer Profiles',
                    url: 'CustomerProfiles',
                    parameter: '',
                    hide: false
                },                 
                {
                    Title: 'Banned & Warned',
                    url: 'BannedAndWarnedUser',
                    parameter: '',
                    hide: false
                },
                 {
                     Title: 'Customer Settings',
                     url: 'CustomerSettings',
                     parameter: '',
                     hide: false
                }
            ]
        }/*,
        {
            mainLink: 'CASE MANAGER',
            description: 'complete & incomplete',
            icon: 'fa-flag',
            hide: false,
            subLink: [
                {
                    Title: 'All Cases',
                    url: 'AllCase',
                    parameter: '',
                    hide: false
                },                 
                {
                    Title: 'Draft Cases',
                    url: 'DraftCase',
                    parameter: '',
                    hide: false
                },
                {
                     Title: 'Profiles',
                     url: 'Profiles',
                     parameter: '',
                     hide : false
                 }
            ]
        },
        {
            mainLink: 'SOCIAL FEEDS',
            description: 'social feeds & feeds',
            icon: 'fa-th',
            hide: false,
            subLink: [
                {
                    Title: 'Instagram',
                    url: 'SocialFeedsInstagram',
                    parameter: '',
                    hide: false
                },
                 {
                     Title: 'Facebook',
                     url: 'SocialFeedsFacebook',
                     parameter: '',
                     hide: false
                 },
                 {
                     Title: 'Twitter',
                     url: 'SocialFeedsTwitter',
                     parameter: '',
                     hide: false
                 },
                 {
                     Title: 'RSS- Keywords',
                     url: 'RSSFeedsKeywords',
                     parameter: '',
                     hide: false
                 },
                 {
                     Title: 'RSS- Pages',
                     url: 'RSSFeedsPages',
                     parameter: '',
                     hide: false
                 }

            ]
        },
        {
            mainLink: 'PUBLISHER',
            description: 'posts & new compose',
            icon: 'fa-globe',
            hide: (!$rootScope.PostSchedule && !$rootScope.PostQueue),
            subLink: [
                {
                    Title: 'Schedular',
                    url: 'Schedular',
                    parameter: '',
                    hide: (!$rootScope.PostSchedule && !$rootScope.PostQueue)
                },
                 //{
                 //    Title: 'Drafts',
                 //    url: 'Drafts',
                 //    parameter: '',
                 //    hide: false
                 //},
                 //{
                 //    Title: 'Pending Approval',
                 //    url: 'PendingApproval',
                 //    parameter: '',
                 //    hide: false
                 //},
                 {
                     Title: 'Scheduled Messages',
                     url: 'ScheduledMessages',
                     parameter: '',
                     hide: !$rootScope.PostSchedule
                 },
                 {
                     Title: 'Queued Messages',
                     url: 'QueuedMessages',
                     parameter: '',
                     hide: !$rootScope.PostQueue
                 },
                 //{
                 //    Title: 'Sent Messages',
                 //    url: 'SentMessages',
                 //    parameter: '',
                 //    hide: false
                 //},
                 //{
                 //    Title: 'Rejected Messages',
                 //    url: 'RejectedMessages',
                 //    parameter: '',
                 //   hide : false
                 //}

            ]
        }*/,
        {
            mainLink: 'REPORTS',
            description: 'analytics & reports',
            icon: 'fa-line-chart',
            hide: false,
            subLink: [
                /*{
                    Title: 'Dashboard',
                    url: 'Dashboard',
                    parameter: '',
                    hide: false
                },*/
                {
                    Title: 'Twitter Report',
                    url: 'TwitterReport',
                    parameter: '',
                    hide: !$rootScope.IsAdmin
                },
                 {
                     Title: 'Facebook Report',
                     url: 'FacebookReport',
                     parameter: '',
                     hide: !$rootScope.IsAdmin
                 },
                 {
                     Title: 'Instagram Report',
                     url: 'InstagramReport',
                     parameter: '',
                     hide: !$rootScope.IsAdmin
                 },
                 {
                     Title: 'Agent Performance Report',
                     url: 'AgentPerformanceReport',
                     parameter: '',
                     hide: !$rootScope.IsAdmin
                 },
                 {
                     Title: 'Inbound/Outbound Report',
                     url: 'InboundReport',
                     parameter: '',
                     hide: false
                 },
                 {
                     Title: 'Shift Report',
                     url: 'ShiftReport',
                     parameter: '',
                     hide: false
                 },
                 //{
                 //    Title: 'Trend Report',
                 //    url: 'TrendReport',
                 //    parameter: '',
                 //    hide: false
                 //},
                 /*{
                     Title: 'Google Analytics',
                     url: 'AnalyticReport',
                     parameter: '',
                     hide: false
                 }*/
            ]
        },
        {
            mainLink: 'SETTINGS',
            description: '',
            icon: 'fa-cog',
            hide: false,
            subLink: [
                {
                    Title: 'Personal Profile',
                    url: 'PersonalProfile',
                    parameter: '',
                    hide: false
                },
                 {
                     Title: 'Notifications',
                     url: 'NotificationSettings',
                     parameter: '',
                     hide: false
                 },
                 {
                     Title: 'Groups & Teams',
                     url: 'GroupAndTeam',
                     parameter: '',
                     hide: !$rootScope.IsAdmin
                 }
                 /*,
                 {
                    Title: 'Pattern Settings',
                    url: 'PatternCustomization',
                    parameter : '',
                    hide: !$rootScope.IsAdmin
                 }*/,
                 {
                     Title: 'Tags & Replies Settings',
                     url: 'TagsSettings',
                     parameter: '',
                     hide: !$rootScope.IsAdmin
                 },
                 {
                     Title: 'Email Settings',
                     url: 'EmailSettings',
                     parameter: '',
                     hide: !$rootScope.IsAdmin
                 },
                  {
                     Title: 'User Logs',
                     url: 'UserLogs',
                     parameter: '',
                     hide: !$rootScope.IsAdmin
                 }
            ]
        }
        ];



        }
        else if ($scope.mode == "Personal") {
            $scope.navLinks = [
           {
               mainLink: 'Inbox Manager',
               description: 'messages & social feeds',
               icon: 'fa-envelope',
               hide: false,
               subLink: [
                   {
                       Title: 'Unified Inbox',
                       url: 'UnifiedInbox',
                       parameter: '',
                       hide: false
                   },
                    {
                        Title: 'Archive Inbox',
                        url: 'ArchiveInbox',
                        parameter: '',
                        hide: false
                    },
                      {
                        Title: 'Outlook Deck',
                        url: 'Outlook',
                        parameter: '',
                        hide: false
                    }/*,
                  {
                      Title: 'Unified Contact',
                      url: 'UnifiedContact',
                      parameter: ''
                  },
                 {
                     Title: 'Archive Contact',
                     url: 'ArchiveContact',
                     parameter: ''
                 }*/

               ]
           }
       ,
        {
            mainLink: 'SOCIAL FEEDS',
            description: 'social feeds & feeds',
            icon: 'fa-th',
            hide: false,
            subLink: [
                {
                    Title: 'Instagram',
                    url: 'SocialFeedsInstagram',
                    parameter: '',
                    hide: false
                },
                 {
                     Title: 'Facebook',
                     url: 'SocialFeedsFacebook',
                     parameter: '',
                     hide: false
                 },
                 {
                     Title: 'Twitter',
                     url: 'SocialFeedsTwitter',
                     parameter: '',
                     hide: false
                 }/*,
                 {
                     Title: 'RSS- Keywords',
                     url: 'RSSFeedsKeywords',
                     parameter: '',
                     hide: false
                 },
                 {
                     Title: 'RSS- Pages',
                     url: 'RSSFeedsPages',
                     parameter: '',
                     hide: false
                 }*/

            ]
        },
        {
            mainLink: 'SETTINGS',
            description: '',
            icon: 'fa-cog',
            hide: false,
            subLink: [
                {
                    Title: 'Personal Profile',
                    url: 'PersonalProfile',
                    parameter: '',
                    hide: false
                }
            ]
        }
            ];

            $scope.redirectTo("#/PersonalProfile");
        }
    }

    $scope.navClass = function (page) {
        var currentRoute = $location.path().substring(1) || 'home';
        //alert(currentRoute)
        var obj = page === currentRoute ? 'left_nav_subs_active' : 'left_nav_subs_inactive';

        return obj;
    }


    $scope.logout = function () {
        AuthService.LogOut().then(function (l) {
            //alert("group has been added")
            $rootScope.selectedGroupId = null;
            $rootScope.selectedCompanyId = null;
            $rootScope.selectedCompanyName = null;
            $rootScope.currentUser = null;
            $rootScope.FirstCompanyName = null;
            $rootScope.FirstCompanyId = null;
            $rootScope.FirstGroupId = null;
            $rootScope.IsAdmin = null; 
            $rootScope.IsPersonal = null;
            $rootScope.PostRespond = null;
            $rootScope.PostEdit = null;
            $rootScope.PostSchedule = null;
            $rootScope.PostQueue = null;
            $rootScope.PostApproval = null;
            $rootScope.TwitTweets = null;
            $rootScope.TwitDMs = null;
            $rootScope.FBPosts = null;
            $rootScope.FBComs = null;
            $rootScope.FBChats = null;
            $rootScope.IMPosts = null;
            $rootScope.IMComs = null;

            $cookies.remove("userSessionCRM", { path: '/' });
            $scope.redirectTo("#/Login");
        }, function (err) {
         //   alert('Error in Logging out, please try again.');
            $cookies.remove("access_token");
            //GlobalService.ErrorPopup("Error", "Error in Logging out, please try again.");
            console.log(err);
            $scope.redirectTo("#/Login");
        });
        //alert("logging out");
    }

    $scope.openGroup = function () {
        $scope.items = "lalalal";
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_GroupAdd.html',
            controller: 'Header_GroupAddController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
            // alert($scope.selected)
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    };

    $scope.openProfileConnect = function () {
        //var path = window.location.href.split('?')[1]
        //if (path == null)
        //    {
        //    $window.location.href = $window.location.href + '?query=value';
        //}
        $scope.items = "lalalal";
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

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
            // alert($scope.selected)
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }
    $scope.GetTeamGroupProfileCount = function (companyid) {

        GroupService.GetTeamGroupProfileCount(companyid).then(function (l) {
            if (l.data != null) {
                $scope.TotalGroups = l.data.TotalGroups;
                $scope.TotalMembers = l.data.TotalMembers;
                $scope.TotalProfiles = l.data.TotalProfiles;
            }
            else {
                $scope.TotalGroups = 0;
                $scope.TotalMembers = 0;
                $scope.TotalProfiles = 0;
            }
        }, function (err) {
          //  alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });

    }

    $scope.fetchNotification = function () {
        NotificationService.GetAllMessages().then(function (l) {
            if (l.data != null) {
                $scope.notificationList = l.data;
                $scope.NotificationCount = 0;
                angular.forEach($scope.notificationList, function (val, key) {
                    if (val.IsRead == 0)
                        $scope.NotificationCount++;
                });
                //alert(l.data);
            }
           
        }, function (err) {
           // GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }
    $scope.MarkReadNotification = function () {
        NotificationService.MarkReadNotification($rootScope.selectedCompanyId).then(function (l) {
         
        }, function (err) {
           // GlobalService.ErrorPopup("Error", "Error in adding group, please try again.");


        });
    }

    $scope.MarkReadNotificationById = function (notif) {
        if(!notif.IsRead)
            NotificationService.MarkReadNotificationById(notif.MessageID).then(function (l) {

        }, function (err) {
            // GlobalService.ErrorPopup("Error", "Error in adding group, please try again.");


        });
    }

    $scope.init();
}]);