CRMApp.controller('DashboardController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'DashboardService', 'TeamService', 'ProfileService', '$uibModal', function ($rootScope, $scope, $window, $location, $stateParams, DashboardService, TeamService, ProfileService, $uibModal) {
    //asj
    $scope.ConnectedProfiles = [{ Picture: "Content/img/user2.jpg", Name: "Alexander Pierce", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Norman", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Jane", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Alexander Pierce", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Norman", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Jane", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Alexander Pierce", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Norman", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Jane", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Alexander Pierce", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Norman", EmailId: "alex@domain.com" },
                                    { Picture: "Content/img/user2.jpg", Name: "Jane", EmailId: "alex@domain.com" }];
    $scope.ConversationCloud = new Array();

    //$scope.ConversationCloud = [{ Text: "Adding", Link: "#", ViewClass:"tag4" },
    //                            { Text: "Streaming", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Audio Files", Link: "#", ViewClass: "tag3" },
    //                            { Text: "Basic", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Tags for a Web Site", Link: "#", ViewClass: "tag6" },
    //                            { Text: "Best Web Design Software", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Software", Link: "#", ViewClass: "tag5" },
    //                            { Text: "Building a Web Page for the Totally Lost", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Color Symbolism", Link: "#", ViewClass: "tag1" },
    //                            { Text: "Adding Streaming Audio Files", Link: "#", ViewClass: "tag7" },
    //                            { Text: "Adding", Link: "#", ViewClass: "tag4" },
    //                            { Text: "Streaming", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Audio Files", Link: "#", ViewClass: "tag3" },
    //                            { Text: "Basic", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Tags for a Web Site", Link: "#", ViewClass: "tag6" },
    //                            { Text: "Best Web Design Software", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Software", Link: "#", ViewClass: "tag5" },
    //                            { Text: "Building a Web Page for the Totally Lost", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Color Symbolism", Link: "#", ViewClass: "tag1" },
    //                            { Text: "Adding Streaming Audio Files", Link: "#", ViewClass: "tag1" },
    //                            { Text: "Adding", Link: "#", ViewClass: "tag4" },
    //                            { Text: "Streaming", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Audio Files", Link: "#", ViewClass: "tag3" },
    //                            { Text: "Basic", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Tags for a Web Site", Link: "#", ViewClass: "tag6" },
    //                            { Text: "Best Web Design Software", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Software", Link: "#", ViewClass: "tag5" },
    //                            { Text: "Building a Web Page for the Totally Lost", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Color Symbolism", Link: "#", ViewClass: "tag1" },
    //                            { Text: "Adding Streaming Audio Files", Link: "#", ViewClass: "tag7" },
    //                            { Text: "Adding", Link: "#", ViewClass: "tag4" },
    //                            { Text: "Streaming", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Audio Files", Link: "#", ViewClass: "tag3" },
    //                            { Text: "Basic", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Tags for a Web Site", Link: "#", ViewClass: "tag6" },
    //                            { Text: "Best Web Design Software", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Software", Link: "#", ViewClass: "tag5" },
    //                            { Text: "Building a Web Page for the Totally Lost", Link: "#", ViewClass: "tag2" },
    //                            { Text: "Color Symbolism", Link: "#", ViewClass: "tag1" },
    //                            { Text: "Adding Streaming Audio Files", Link: "#", ViewClass: "tag1" }];

    //$scope.InfluencerProgress = [{ Name: "User Name", ProgressNumber: "150", Accomplishment: "50" },
    //                             { Name: "User Name", ProgressNumber: "190", Accomplishment: "90" },
    //                             { Name: "User Name", ProgressNumber: "380", Accomplishment: "80" },
    //                             { Name: "User Name", ProgressNumber: "120", Accomplishment: "20" },
    //                             { Name: "User Name", ProgressNumber: "270", Accomplishment: "70" },
    //                             { Name: "User Name", ProgressNumber: "150", Accomplishment: "10" }];

    //Dropdowns
    $scope.GroupKeyStatisticsDropdown = [{ Key: "1", Value: "Option 1" },
                                        { Key: "2", Value: "Option 2" },
                                        { Key: "3", Value: "Option 3" }];

    $scope.TopInfluencerDropdown = [{ Key: "1", Value: "Option 1" },
                                    { Key: "2", Value: "Option 2" },
                                    { Key: "3", Value: "Option 3" }];

    $scope.DemographicsDropdown = [{ Key: "1", Value: "Option 1" },
                                   { Key: "2", Value: "Option 2" },
                                   { Key: "3", Value: "Option 3" }];

    $scope.GroupPerformanceDropdown = {
        availableOptions: [
          { Value: "Sales Lead", Key: "1" },
          { Value: "After Sales", Key: "2" },
          { Value: "Compliment", Key: "3" },
          { Value: "Complaint", Key: "4" },
          { Value: "General Inquiry", Key: "5" },
          { Value: "Referral", Key: "6" },
          { Value: "Review", Key: "7" }
        ],
        selectedOption: { Value: "Sales Lead", Key: "1" } //This sets the default value of the select in the ui
    };

    //Functions
    $scope.PerformanceLevel = function (value) {
        if (value >= 80)
            return "progress-bar-green";
        else if (value < 80 && value >= 60)
            return "progress-bar-aqua";
        else if (value < 60 && value >= 40)
            return "progress-bar-yellow";
        else if (value < 40 && value >= 6)
            return "progress-bar-light-blue";
        else if (value < 6 && value >= 0)
            return "progress-bar-red";
    }

    //Setting ground for group performance
    var now = new Date();
    $scope.groupPerformanceFromDate = new Date(now.getFullYear(), now.getMonth() - 2, now.getDate());
    $scope.groupPerformanceToDate = new Date(now.getFullYear(), now.getMonth() + 1, now.getDate());

    $scope.ClosedPercentage = 0.0;
    $scope.InProgressPercentage = 0.0;
    $scope.init = function () {
        //Get Company Id
        $scope.CompanyId = $rootScope.selectedCompanyId;
        $scope.LoadDashboardInfo();

        //Service Call of Group Performance Percentage
        $scope.GetGroupPerformance();

        //Service Call of Get Top Influencers
        $scope.GetTopInfluencers();
    };
    
    //Service Calling Functions
    $scope.GetGroupPerformance = function () {
        var dateFrom = $scope.demoGraphicsDateFrom;
        var dateTo = $scope.demoGraphicsDateTo;
        var groupId = parseInt($rootScope.selectedGroupId);
        var companyId = parseInt($rootScope.selectedCompanyId);

        var ClosedTasks = 0;
        var InProgressTasks = 0;
        var TotalTasks = 0;

        var ClosedPercentage = 0.0;
        var InProgressPercentage = 0.0;

        DashboardService.GetGroupPerformance(companyId, groupId, dateFrom, dateTo).then(function (l) {
            if (l != null) {
                console.log(l);

                $.each(l.data, function (i, obj) {
                    // 1 - In Progress
                    if (obj.statusId == "1") {
                        InProgressTasks = parseInt(obj.statusCount);
                    }
                    // 2 - Closed
                    if (obj.statusId == "2") {
                        ClosedTasks = parseInt(obj.statusCount);
                    }
                });

                TotalTasks = ClosedTasks + InProgressTasks;
                if(TotalTasks > 0){
                ClosedPercentage = parseFloat((ClosedTasks / TotalTasks) * 100).toFixed(2);
                InProgressPercentage = parseFloat((InProgressTasks / TotalTasks) * 100).toFixed(2);
                    $scope.ClosedPercentage = ClosedPercentage;
                    $scope.InProgressPercentage = InProgressPercentage;
                } else {
                    $scope.ClosedPercentage = 0.00;
                    $scope.InProgressPercentage = 0.00;
                }
            }
            //alert("Successfully fetched");
        }, function (err) {
            alert('Error in fetching, please try again.');
            console.log(err);
        });
    }

    $scope.GetTopInfluencers = function () {
        var CompanyId = $rootScope.selectedCompanyId;
        var DateFrom = $scope.groupPerformanceFromDate;
        var DateTo = $scope.groupPerformanceToDate;

        DashboardService.GetTopInfluencers(CompanyId, DateFrom, DateTo).then(function (l) {
            if (l != null) {
                console.log(l);
                var users = [];
                var tasksReflection = [];
                var top5Performer = [];
                debugger
                $.each(l.data, function (i, obj) {
                    if ($.inArray(obj.teamMemberId, users) == -1) {
                        users.push(obj.teamMemberId);
                    }
                });

                angular.forEach(users, function (value, key) {
                    //console.log(key + ':' + value);
                    var task = { userId: 0, firstName:"", lastName:"", totalTasks: 0, pendingTasks: 0, closedTasks: 0, accomplishment: 0.0 };
                    $.each(l.data, function (i, obj) {
                        if (obj.teamMemberId == value) {
                            task.userId = obj.teamMemberId;
                            task.firstName = obj.firstName;
                            task.lastName = obj.lastName;
                            
                            if (obj.statusId == "1") {
                                //Pending
                                task.pendingTasks = obj.StatusCount;
                            }
                            else if (obj.statusId == "2") {
                                //Closed
                                task.closedTasks = obj.StatusCount;
                            }
                        }
                    });
                    task.totalTasks = task.pendingTasks + task.closedTasks;
                    task.accomplishment = parseFloat((task.closedTasks / task.totalTasks) * 100);

                    tasksReflection.push(task);
                });

                tasksReflection = sortByKey(tasksReflection, 'totalTasks');
                top5Performer = tasksReflection.slice(0, 5);
                console.log(top5Performer);
                $scope.top5Performer = top5Performer;
                
            }
            //alert("Successfully fetched");
        }, function (err) {
            alert('Error in fetching, please try again.');
            console.log(err);
        });
    }

    $scope.getTeamMembers = function (companyId) {
        var groupId = parseInt($rootScope.selectedGroupId);
        TeamService.GetByCompanyIdAndGroupId(companyId, groupId).then(function (l) {
        var response = l.data;
        if (response) {
            $scope.groupMembers = response.Team;
            // alert(JSON.stringify($scope.groupMembers));
            $scope.TeamMembers = response.TotalMember;
            $scope.TotalGroups = response.TotalGroup;
            $scope.TotalProfiles = response.TotalProfile;
            $scope.CompanyName = response.CompanyName;

            var firstMem = $scope.groupMembers[0];
            $scope.loadUserData(companyId, firstMem.TeamMemberId);
        }
            


        }, function (err) {
            alert('Error in fetching pages data');
            console.log(err);
        });
    }

    $scope.loadUserData = function (companyId, teamMemberId) {
        // companyId = 1;//get company id at the time of login

        ProfileService.GetAllProfilesByCompanyAndUserId(companyId, teamMemberId).then(function (l) {
            var response = l.data[0];
            //alert(JSON.stringify(response.Team))
            if (response.Team != null) {
                // alert(JSON.stringify(response.Team))
                $scope.SelectedUserName = response.Team.Profile.Name;
                $scope.SelectedUserPic = response.Team.Profile.ProfileUrl == null && "/Content/img/user128.jpg" || $scope.replaceTilda(response.Team.Profile.ProfileUrl);
                $scope.SelectedTeamMemberId = response.Team.Profile.TeamMemberId;
                $scope.ProfilesAssignedToUser = response.Team.Profile.AssignedProfile;
                $scope.UserGroups = [];
                $scope.UserVisibleReports = [];
                $scope.UserRights = [];

                for (i = 0; i < response.Team.VisibleReports.length; i++) {
                    var obj = { "label": response.Team.VisibleReports[i].Name, "id": "" + response.Team.VisibleReports[i].Id }
                    $scope.UserVisibleReports.push(obj)
                }
                for (i = 0; i < response.Team.Rights.length; i++) {
                    var obj = { "label": response.Team.Rights[i].Name, "id": "" + response.Team.Rights[i].Id }
                    $scope.UserRights.push(obj)
                }
                for (i = 0; i < response.Team.Groups.length; i++) {
                    var obj = { "label": response.Team.Groups[i].GroupName, "id": "" + response.Team.Groups[i].GroupId }
                    $scope.UserGroups.push(obj)
                }
                //response.Rights
                $scope.reportsmodel = $scope.UserVisibleReports;
                $scope.rightsmodel = $scope.UserRights;
                $scope.groupdata = $scope.UserGroups;
                $scope.Connected_Profiles2 = [];
                $scope.profileCount = 0;
                if ($scope.groupdata.length > 0) {
                    $scope.groupmodel = { "id": "" + $scope.groupdata[0].id, "label": $scope.groupdata[0].label };// $scope.groupdata[0];
                    $scope.selectedGroupId = $scope.groupdata[0].id;
                    $scope.selectedGroupName = $scope.groupdata[0].label;



                    if (response.ProfileCompanyGroup != null) {
                        $scope.Connected_Group_Profiles = response.ProfileCompanyGroup;
                        GetUserProfile($scope.selectedGroupId);
                    }

                }
            }
            else {
                $scope.SelectedUserName = "";
                $scope.SelectedUserPic = "/Content/img/user128.jpg";
                $scope.SelectedTeamMemberId = "";
                $scope.ProfilesAssignedToUser = 0;
                $scope.UserGroups = [];
                $scope.UserVisibleReports = [];
                $scope.UserRights = [];
                $scope.reportsmodel = $scope.UserVisibleReports;
                $scope.rightsmodel = $scope.UserRights;
                $scope.groupdata = $scope.UserGroups;
                $scope.profileCount = 0;
                alert("No Data Found")
            }
            // alert(groupmodel.label)

        }, function (err) {
            alert('Error in fetching data');
            console.log(err);
        });

    }

    //Dropdown on change request of Group Performance
    $scope.UpdateGroupPerformanceReport = function () {
        //alert("Hey");
        $scope.GetGroupPerformance();
    }

    $scope.gskDateTo;
    $scope.gskDateFrom;
    $scope.demoGraphicsDateTo;
    $scope.demoGraphicsDateFrom;

    $scope.gksTwNumOfTweets;
    $scope.gksTwNumOfFollowers;
    $scope.gksTwNumOfLikes;

    $scope.gksFbProfileNumOfPosts;
    $scope.gksTFbProfileNumOfFollowers;
    $scope.gksTFbProfileNumOfFriends;
    $scope.gksFbProfileNumOfLikes;

    $scope.gksFbPageNumOfPosts;
    $scope.gksFbPageNumOfLikes;

    $scope.gksInstaNumOfPosts;
    $scope.gksInstaNumOfFollowers;
    $scope.gksInstaNumOfLikes;

    $scope.TopInfluencers;

    $scope.DemographicMaleCount;
    $scope.DemographicFemaleCount;
    $scope.DemographicUnspecifiedCount;

    $scope.LoadDashboardInfo = function () {
        var groupId = parseInt($rootScope.selectedGroupId);
        var companyId = parseInt($rootScope.selectedCompanyId);

        //Get Team Members Data
        $scope.getTeamMembers(companyId);

        DashboardService.GetDashboardInfo(companyId, groupId).then(function (l) {
            if (l != null) {
                console.log(l);
                $scope.gksTwNumOfTweets = l.data.gksTwNumOfTweets;
                $scope.gksTwNumOfFollowers = l.data.gksTwNumOfFollowers;
                $scope.gksTwNumOfLikes = l.data.gksTwNumOfLikes;

                $scope.gksFbProfileNumOfPosts = l.data.gksFbProfileNumOfPosts;
                $scope.gksTFbProfileNumOfFollowers = l.data.gksTFbProfileNumOfFollowers;
                $scope.gksFbProfileNumOfLikes = l.data.gksFbProfileNumOfLikes;
                $scope.gksTFbProfileNumOfFriends = l.data.gksTFbProfileNumOfFriends;

                $scope.gksFbPageNumOfPosts = l.data.gksFbPageNumOfPosts;
                $scope.gksFbPageNumOfLikes = l.data.gksFbPageNumOfLikes;

                $scope.gksInstaNumOfPosts = l.data.gksInstaNumOfPosts;
                $scope.gksInstaNumOfFollowers = l.data.gksInstaNumOfFollowers;
                $scope.gksInstaNumOfLikes = l.data.ksInstaNumOfLikes;

                $scope.TopInfluencers = l.data.TopInfluencers;

                $scope.DemographicMaleCount = l.data.DemographicMaleCount;
                $scope.DemographicFemaleCount = l.data.DemographicFemaleCount;
                $scope.DemographicUnspecifiedCount = l.data.DemographicUnspecifiedCount;
                $scope.ConversationCloud = new Array();

                var response = l.data.WordCloudList;
                if (response !== undefined && response != null) {
                    var tempArray = new Array();
                    var maxFrequency = higherScale = Math.max.apply(Math, response.map(function (o) { return o.Frequency; }));
                    var minFrequency = Math.min.apply(Math, response.map(function (o) { return o.Frequency; }));
                    var range = lowerScale = ((maxFrequency - minFrequency) / 10) > 1 ? (maxFrequency - minFrequency) / 10 : 1;

                    for (var i = 10; i >= 1; i--) {
                        lowerScale = higherScale - range;
                        if (i == 1)
                            lowerScale = minFrequency;
                        var data = $.grep(response, function (record) {
                            return record.Frequency <= higherScale && record.Frequency >= lowerScale;
                        });

                        if (data.length > 0) {
                            data.forEach(function (val) {
                                val.ViewClass = "tag" + i;
                            });
                            Array.prototype.push.apply(tempArray, data)
                        }
                        higherScale = higherScale - range - 1;
                    }

                    $scope.ConversationCloud = tempArray;
                }
            }
            //alert("Successfully fetched");
        }, function (err) {
            alert('Error in fetching, please try again.');
            console.log(err);
        });
    };

    $scope.init();

    //Group Key from
    $scope.open1 = function () {
        $scope.popup1.opened = true;
    };
    $scope.popup1 = {
        opened: false
    };

    //Group Key To
    $scope.open2 = function () {
        $scope.popup2.opened = true;
    };
    $scope.popup2 = {
        opened: false
    };

    //Top Influencer To
    $scope.open3 = function () {
        $scope.popup3.opened = true;
    };
    $scope.popup3 = {
        opened: false
    };

    //Top Influencer From
    $scope.open4 = function () {
        $scope.popup4.opened = true;
    };
    $scope.popup4 = {
        opened: false
    };

    //Demographics To
    $scope.open5 = function () {
        $scope.popup5.opened = true;
    };
    $scope.popup5 = {
        opened: false
    };

    //Demographic From
    $scope.open6 = function () {
        $scope.popup6.opened = true;
    };
    $scope.popup6 = {
        opened: false
    };

    //Group perf To
    $scope.open7 = function () {
        $scope.popup7.opened = true;
    };
    $scope.popup7 = {
        opened: false
    };

    //Group perf From
    $scope.open8 = function () {
        $scope.popup8.opened = true;
    };
    $scope.popup8 = {
        opened: false
    };

    $scope.formats = ['dd-MM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[0];

    $scope.dateOptions = {
        formatYear: 'yy'
    };

    var tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    var afterTomorrow = new Date();
    afterTomorrow.setDate(tomorrow.getDate() + 1);
    $scope.events = [
      {
          date: tomorrow,
          status: 'full'
      },
      {
          date: afterTomorrow,
          status: 'partially'
      }
    ];

    function getDayClass(data) {
        var date = data.date,
          mode = data.mode;
        if (mode === 'day') {
            var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

            for (var i = 0; i < $scope.events.length; i++) {
                var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                if (dayToCheck === currentDay) {
                    return $scope.events[i].status;
                }
            }
        }

        return '';
    }

    //Angular sorting (descending)
    function sortByKey(array, key) {
        return array.sort(function (a, b) {
            var y = a[key]; var x = b[key];
            return ((x < y) ? -1 : ((x > y) ? 1 : 0));
        });
    }

    function GetUserProfile(groupId) {
        for (i = 0; i < $scope.Connected_Group_Profiles.length; i++) {
            if ($scope.Connected_Group_Profiles[i].GroupId == groupId) {
                $scope.Connected_Profiles2 = $scope.Connected_Group_Profiles[i].ProfileId;
                break;
            }
        }
        var count = 0;
        for (i = 0; i < $scope.Connected_Profiles2.length; i++) {
            if ($scope.Connected_Profiles2[i].IsConnected == true)
                count++;

        }
        $scope.profileCount = count;
        //$scope.Connected_Profiles = l.data
    }
    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    };

    $scope.filterGroupKeyStatistics = function(date){
        var dateFrom = $scope.gskDateFrom;
        var dateTo = $scope.gskDateTo;
        var groupId = parseInt($rootScope.selectedGroupId);
        var companyId = parseInt($rootScope.selectedCompanyId);

        DashboardService.FilterGroupKeyStatistics(companyId, groupId, dateFrom, dateTo).then(function (l) {
            if (l != null) {
                console.log(l);
                $scope.gksTwNumOfTweets = l.data.gksTwNumOfTweets;
                $scope.gksFbProfileNumOfPosts = l.data.gksFbProfileNumOfPosts;
                $scope.gksFbPageNumOfPosts = l.data.gksFbPageNumOfPosts;
                $scope.gksInstaNumOfPosts = l.data.gksInstaNumOfPosts;
            }
            //alert("Successfully fetched");
        }, function (err) {
            alert('Error in fetching, please try again.');
            console.log(err);
        });

    };

    $scope.filterGroupPerformance = function (date) {
        var dateFrom = $scope.demoGraphicsDateFrom;
        var dateTo = $scope.demoGraphicsDateTo;
        var groupId = parseInt($rootScope.selectedGroupId);
        var companyId = parseInt(selectedCompanyId);

        //DashboardService.FilterGroupPerformance(companyId, groupId, dateFrom, dateTo).then(function (l) {
        //    if (l != null) {
        //        console.log(l);
        //        $scope.gksTwNumOfTweets = l.data.gksTwNumOfTweets;
        //        $scope.gksFbProfileNumOfPosts = l.data.gksFbProfileNumOfPosts;
        //        $scope.gksFbPageNumOfPosts = l.data.gksFbPageNumOfPosts;
        //        $scope.gksInstaNumOfPosts = l.data.gksInstaNumOfPosts;
        //    }
        //    //alert("Successfully fetched");
        //}, function (err) {
        //    alert('Error in fetching, please try again.');
        //    console.log(err);
        //});

        var ClosedTasks = 0;
        var InProgressTasks = 0;
        var TotalTasks = 0;

        var ClosedPercentage = 0.0;
        var InProgressPercentage = 0.0;

        DashboardService.GetGroupPerformance(companyId,groupId, dateFrom, dateTo).then(function (l) {
            if (l != null) {
                console.log(l);

                $.each(l.data, function (i, obj) {
                    // 1 - In Progress
                    if (obj.statusId == "1") {
                        InProgressTasks = parseInt(obj.statusCount);
                    }
                    // 2 - Closed
                    if (obj.statusId == "2") {
                        ClosedTasks = parseInt(obj.statusCount);
                    }
                });

                TotalTasks = ClosedTasks + InProgressTasks;
                if(TotalTasks > 0){
                ClosedPercentage = parseFloat((ClosedTasks / TotalTasks) * 100).toFixed(2);
                InProgressPercentage = parseFloat((InProgressTasks / TotalTasks) * 100).toFixed(2);
                    $scope.ClosedPercentage = ClosedPercentage;
                    $scope.InProgressPercentage = InProgressPercentage;
                } else {
                    $scope.ClosedPercentage = 0.00;
                    $scope.InProgressPercentage = 0.00;
                }
            }
            //alert("Successfully fetched");
        }, function (err) {
            alert('Error in fetching, please try again.');
            console.log(err);
        });

    };

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });

}]);