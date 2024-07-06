CRMApp.controller('QueuedMessageController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'PublisherService', 'UnifiedService', 'GlobalService', '$uibModal', function ($rootScope, $scope, $window, $location, $stateParams, PublisherService, UnifiedService, GlobalService, $uibModal) {


        $scope.fbIcon = '/Content/images/fb_ico16.png';
        $scope.linkedinIcon = '/Content/images/in_ico16.png';
        $scope.twitterIcon = '/Content/images/t_ico16.png';
        $scope.googleIcon = '/Content/images/ana_ico16.png';
        $scope.instaIcon = '/Content/images/insta_ico16.png';
    
        $scope.selected = {};
        $scope.selectedApprovalType = {};
        $scope.select = true;
        $scope.selectApprovalType = true;
        $scope.scheduledMessages = [];

        $scope.Seed = true;
        $scope.PageEmpty = false;
        $scope.PageCount = 1;

        $scope.DataForReference = [];

        $scope.loadScheduledMessages = function () {
            var selectedProfiles = $.grep($scope.Profiles, function (record) {
                if (record.ProfileTypeId == 1 || record.ProfileTypeId == 4)
                    return $scope.selected[record.ProfileId];
            });

            var profileCompanyGroupIds = [];
            $.each(selectedProfiles, function (i, obj) {
                //SelectedProfileIds.push(obj.ProfileId);
                profileCompanyGroupIds.push(obj.ProfileCompnayGroupId);
            });

            var ApprovalIds = [];
            angular.forEach($scope.selectedApprovalType, function (value, key) {
                if (value == true)
                    ApprovalIds.push(key);
            });

            PublisherService.GetAllQueuedMessages(profileCompanyGroupIds, ApprovalIds, $scope.PageCount, false).then(function (resultSet) {
                if (resultSet != null) {
                    console.log(resultSet);
                    if (resultSet["statusText"] == "OK") {
                        console.log(resultSet.data);

                        $scope.scheduledMessages = [];
                        for (var myloop = 0; myloop < resultSet.data.length; myloop++) {
                            $scope.scheduledMessages.push(resultSet.data[myloop]);
                        }

                        //$scope.scheduledMessages.reverse();
                    }
                } else {
                    $scope.scheduledMessages = [];
                }
                //alert("defaultfully fetched");
            }, function (err) {
                //alert('Error in fetching, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
                console.log(err);
            });

            console.log(selectedProfiles);
        };

        $scope.loadDataForReference = function () {            
            var selectedProfiles = $.grep($scope.Profiles, function (record) {
                if (record.ProfileTypeId == 1 || record.ProfileTypeId == 4)
                    return $scope.selected[record.ProfileId];
            });

            var profileCompanyGroupIds = [];
            $.each(selectedProfiles, function (i, obj) {
                //SelectedProfileIds.push(obj.ProfileId);
                profileCompanyGroupIds.push(obj.ProfileCompnayGroupId);
            });

            var ApprovalIds = [];
            angular.forEach($scope.selectedApprovalType, function (value, key) {
                if (value == true)
                    ApprovalIds.push(key);
            });

            PublisherService.GetAllQueuedMessages(profileCompanyGroupIds, ApprovalIds, -1, false).then(function (resultSet) {
                if (resultSet != null) {
                    console.log(resultSet);
                    if (resultSet["statusText"] == "OK") {
                        for (var myloop = 0; myloop < resultSet.data.length; myloop++) {
                            $scope.DataForReference.push(resultSet.data[myloop]);
                        }

                        for (var myloop = 0; myloop < $scope.DataForReference.length; myloop++) {
                            $scope.DataForReference[myloop].isNew = false;
                        }
                        
                        console.log($scope.DataForReference);
                    }
                } else {                    
                }
                //alert("defaultfully fetched");
            }, function (err) {
                //alert('Error in fetching, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
                console.log(err);
            });

            console.log(selectedProfiles);
        };

        $scope.checkAll = function () {
            angular.forEach($scope.Profiles, function (obj) {
                $scope.selected[obj.ProfileId] = $scope.select;
            });
            $scope.PageCount = 1;
            $scope.loadScheduledMessages();
        };

        $scope.checkAllMessageTypes = function () {
            angular.forEach($scope.MessageTypeFilters, function (obj) {
                $scope.selectedApprovalType[obj.Id] = $scope.selectApprovalType;
            });
            $scope.PageCount = 1;
            $scope.loadScheduledMessages();
        };

        $scope.profileSelectedChanged = function () {
            $scope.select = false;
            //$scope.getProfileDetails($window.localStorage.getItem('selectedGroupId'), $window.localStorage.getItem('selectedCompanyId'), $window.localStorage.getItem('Ispersonal'));
            $scope.PageCount = 1;
            $scope.loadScheduledMessages();
        };

        $scope.selectedApprovalTypeChanged = function () {            
            $scope.selectApprovalType = false;
            $scope.PageCount = 1;
            $scope.loadScheduledMessages();
        };

        $scope.getProfileDetails = function (groupId, companyId, isPersonal) {       
            UnifiedService.GetAllProfilesForUserGroup(groupId, companyId, isPersonal).then(function (l) {                
                var response = l.data;
                // alert(response)
                $scope.Profiles = response;

                $scope.FbCount = 0;
                $scope.InCount = 0;
                $scope.TwitCount = 0;
                for (var i = 0; i < response.length; i++) {
                    if (response[i].ProfileTypeId == 1) {
                        $scope.FbCount++;
                    }
                    else if (response[i].ProfileTypeId == 6) {
                        $scope.InCount++;
                    }
                    else if (response[i].ProfileTypeId == 4) {
                        $scope.TwitCount++;
                    }
                }

                $scope.msgTypes = [];

                if ($scope.FbCount > 0 || $scope.TwitCount > 0) {
                    var complaintMsgType = [{
                        msgTypelogo: "fb_ico16",
                        MsgType: " Complaints",
                        msgTypeId: "9"
                    }];
                    $scope.msgTypes.push.apply($scope.msgTypes, complaintMsgType);
                }

                if ($scope.FbCount > 0) {
                    $scope.msgTypes.push.apply($scope.msgTypes, $scope.fbMsgTypes);
                }

                if ($scope.TwitCount > 0) {
                    $scope.msgTypes.push.apply($scope.msgTypes, $scope.TwitterMsgTypes);
                }

                if ($scope.InCount > 0) {
                    $scope.msgTypes.push.apply($scope.msgTypes, $scope.InstagramMsgTypes);
                }
            
                if($scope.select == true){
                    //$scope.checkAll();
                    angular.forEach($scope.Profiles, function (obj) {
                        $scope.selected[obj.ProfileId] = $scope.select;
                    });
                }

                //Seed means One Time
                if($scope.Seed == true){
                    //CheckAllMessageTypes
                    angular.forEach($scope.MessageTypeFilters, function (obj) {
                        $scope.selectedApprovalType[obj.Id] = $scope.selectApprovalType;
                    });
                }
                $scope.Seed = false;

                //Load Scheduled Messages
                $scope.loadScheduledMessages();

                //Load DataForReference
                $scope.loadDataForReference();

                if ($scope.Profiles.length == 0) {
                    $scope.PageEmpty = true;
                }
                else {
                    $scope.PageEmpty = false;
                }
            }, function (err) {
                // alert('Error in fetching User Profiles');
                GlobalService.ErrorPopup("Error", "Error in fetching User Profiles.");
                //console.log(err);
            });        
        };           
    
        $scope.MessageTypeFilters = [
            {
                "Id": 1,
                "FilterName": "Approved" //Pending
            },
            {
                "Id": 2,
                "FilterName": "Sent"
            }, {
                "Id": 3,
                "FilterName": "Waiting"
            }
        ];

        $scope.profileLoadMoreMessages = function () {
            $scope.PageCount++;

            var selectedProfiles = $.grep($scope.Profiles, function (record) {
                if (record.ProfileTypeId == 1 || record.ProfileTypeId == 4)
                    return $scope.selected[record.ProfileId];
            });

            var profileCompanyGroupIds = [];
            $.each(selectedProfiles, function (i, obj) {
                //SelectedProfileIds.push(obj.ProfileId);
                profileCompanyGroupIds.push(obj.ProfileCompnayGroupId);
            });

            var ApprovalIds = [];
            angular.forEach($scope.selectedApprovalType, function (value, key) {
                if (value == true)
                    ApprovalIds.push(key);
            });

            PublisherService.GetAllQueuedMessages(profileCompanyGroupIds, ApprovalIds, $scope.PageCount, false).then(function (resultSet) {
                if (resultSet != null) {
                    console.log(resultSet);
                    if (resultSet["statusText"] == "OK") {
                        console.log(resultSet.data);
                        if (resultSet.data != null){
                        for (var myloop = 0; myloop < resultSet.data.length; myloop++) {
                            $scope.scheduledMessages.push(resultSet.data[myloop]);
                        }
                        } else {
                            $scope.PageCount--;
                        }
                    }
                } else {
                    //$scope.scheduledMessages = [];
                }
                //alert("defaultfully fetched");
            }, function (err) {
                //alert('Error in fetching, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
                console.log(err);
            });

            console.log(selectedProfiles);
            
        }

        $scope.fetchLatestQueuedMessages = function () {

            //Before doing anything just save the current data in some variable
            var previousData = $scope.DataForReference;

            var selectedProfiles = $.grep($scope.Profiles, function (record) {
                if (record.ProfileTypeId == 1 || record.ProfileTypeId == 4)
                    return $scope.selected[record.ProfileId];
            });

            var profileCompanyGroupIds = [];
            $.each(selectedProfiles, function (i, obj) {
                //SelectedProfileIds.push(obj.ProfileId);
                profileCompanyGroupIds.push(obj.ProfileCompnayGroupId);
            });

            var ApprovalIds = [];
            angular.forEach($scope.selectedApprovalType, function (value, key) {
                if (value == true)
                    ApprovalIds.push(key);
            });

            PublisherService.GetAllQueuedMessages(profileCompanyGroupIds, ApprovalIds, $scope.PageCount, true).then(function (resultSet) {
                if (resultSet != null) {
                    console.log(resultSet);
                    if (resultSet["statusText"] == "OK") {
                        console.log(resultSet.data);
                        $scope.scheduledMessages = [];
                        if (resultSet.data != null) {
                            //Loaded with fresh data
                            for (var myloop = 0; myloop < resultSet.data.length; myloop++) {
                                $scope.scheduledMessages.push(resultSet.data[myloop]);
                            }

                            var newData = resultSet.data;
                            var flag = false;
                            //Compare with prior data to see whats new
                            for (var myloop = 0; myloop < newData.length; myloop++) {
                                flag = false;
                                angular.forEach(previousData, function (messageObj) {
                                    if(flag == false){
                                        if (newData[myloop].Id == messageObj.Id) {
                                            flag = true;                                        
                                        }
                                    }
                                });

                                if (flag == true) {
                                    //Old
                                    $scope.scheduledMessages[myloop].isNew = false;
                                } else {                                    
                                    //New Record
                                    $scope.scheduledMessages[myloop].isNew = true;
                                    $scope.DataForReference.push(newData[myloop]);
                                }
                            }


                        } else {
                            //$scope.PageCount--;
                        }
                    }
                } else {
                    //$scope.scheduledMessages = [];
                }
                //alert("defaultfully fetched");
            }, function (err) {
                //alert('Error in fetching, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
                console.log(err);
            });

            console.log(selectedProfiles);

        }


        $scope.init = function () {
            $scope.GroupId = $rootScope.selectedGroupId;
            $scope.CompanyId = $rootScope.selectedCompanyId;
            $scope.isPersonal = $rootScope.IsPersonal.toString();
            $scope.PageCount = 1;
            $scope.getProfileDetails($scope.GroupId, $scope.CompanyId, $scope.isPersonal);
            //$scope.checkAllMessageTypes();
        };

        //Image Slider
        $scope.openSlider = function (Imagelist) {

            var sendObj = [];
            var type = "";
            var url = "";
            angular.forEach(Imagelist, function (Image) {
                if (Image.ContentType == "image/jpeg") {
                    type = "image";
                    url = "data:image/jpeg;base64,";
                } else if (Image.ContentType == "image/png") {
                    type = "image";
                    url = "data:image/png;base64,";
                } else if (Image.ContentType == "image/gif") {
                    type = "image";
                    url = "data:image/gif;base64,";
                }
                else {
                    type = "video"
                }

                url = url + Image.Base64String;
                sendObj.push({ "MediaType": "", "Type": type, "Url": url });
            });


            $scope.items = { Images: sendObj };
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: '/Scripts/Views/Shared/_ImageSlider.html',
                controller: '_ImageSliderController',

                resolve: {
                    items: function () {
                        return $scope.items;
                    }
                }
            });

            modalInstance.result.then(function (selectedItem) {
                $scope.selected = selectedItem;
            }, function () {

            });
        }

        $scope.init();            

        //Group Change
        $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
            //$scope.select = true;
            $scope.init();
        });

        //Profile Connect
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



        //End braces
    }]);