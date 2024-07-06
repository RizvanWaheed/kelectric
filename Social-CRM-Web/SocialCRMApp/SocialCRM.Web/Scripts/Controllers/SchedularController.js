//var CRMApp = angular.module('CRMApp', ['ui.bootstrap', 'ui.bootstrap.datetimepicker']);
CRMApp.controller('SchedularController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'PublisherService', 'TeamService', 'ProfileService', 'UnifiedService', '$uibModal', 'GlobalService', 'notify', 'FileUploader', function ($rootScope, $scope, $window, $location, $stateParams, PublisherService, TeamService, ProfileService, UnifiedService, $uibModal, GlobalService, notify, FileUploader) {
    //asj
    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';

    $scope.attachedProfiles = [];
    $scope.selected = {};

    $scope.selectProfiles = false;

    $scope.scheduleMode = 'DateAndTime';
    $scope.messageBodyAreaAllowedLength = 140;

    //Defaults
    $scope.queueDaysModel = [];
    $scope.queueDays = [
     {
         "label": "Monday",
         "id": "1"
     }, {
         "label": "Tuesday",
         "id": "2"
     }, {
         "label": "Wednesday",
         "id": "3"
     }, {
         "label": "Thursday",
         "id": "4"
     }, {
         "label": "Friday",
         "id": "5"
     }, {
         "label": "Saturday",
         "id": "6"
     }, {
         "label": "Sunday",
         "id": "7"
     }];

    //Global checker for preload or New
    $scope.isAlreadyMessage = false;
    $scope.ScheduledMessageId = 0;
    $scope.scheduledMessageType = -1;

    //Buttons Panel at the end
    $scope.disableDraft = false;
    $scope.disableSchedule = false;
    $scope.disableApprove = false;
    $scope.disableReject = true;
    $scope.currentProfileQueueEditing = 0;

    //Media
    $scope.mediaData = new Array();
    $scope.UploadedFiles = new Array();
    $scope.UploadedFilesCount = 0;

    $scope.GetTwitterProfilesByGroupId = function () {
        //Get Company Id
        $scope.CompanyId = $rootScope.selectedCompanyId;
        UnifiedService.GetAllProfilesForUserGroup($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {
            $scope.attachedProfiles = [];
            var compositeId = "";
            angular.forEach(l.data, function (obj) {                
                compositeId = obj.IsPage + "_" + obj.ProfileCompnayGroupId;
                $scope.attachedProfiles.push({
                    ProfileCompnayGroupId: obj.ProfileCompnayGroupId,
                    CompositeId: compositeId,
                    ProfileId: obj.ProfileId,
                    AccountName: obj.AccountName,
                    ProfilePicURL: obj.ProfilePicURL,
                    ProfileName: obj.ProfileName,
                    ProfileTypeId: obj.ProfileTypeId,
                    ProfileType: obj.ProfileType,
                    CompanyId: obj.CompanyId,
                    IsConnected: obj.IsConnected,
                    IsPage : obj.IsPage,
                    PageId : obj.PageId,
                    PageName : obj.PageName,
                    PageUrl : obj.PageUrl,
                    UserId : obj.UserId,
                    select: false
                })
            });
            console.log($scope.attachedProfiles);
           // $scope.attachedProfiles = l.data;
            $scope.queuedProfiles = l.data;

            //trial
            $scope.QueueTabChanged(0);

            //Removed from init for some reasons
            //If Draft/Scheduled messages are seen        
            if ($location.search().ScheduledMessageId != null) {
                console.log("Received_ScheduledMessageId: " + $location.search().ScheduledMessageId);
                console.log("Received_MessageType: " + $location.search().MessageType);
                $scope.scheduledMessageType = $location.search().MessageType;
                $scope.LoadPreviouslySavedOrScheduledMessage($location.search().ScheduledMessageId, $location.search().MessageType);
                $scope.VisibleNewMessage = true;
            } else {
                $scope.messageBodyArea = "";
                $scope.selected = [];
                $scope.VisibleNewMessage = false;
            }

        }, function (err) {
            //alert('Error in fetching contact history tweets, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
            console.log(err);
        });
    };

    $scope.GetTwitterProfilesByGroupId();    

    $scope.init = function () {

        //Setting Current Time
        var now = new Date();
        $scope.ScheduleFromTime = new Date(now.getDate(), now.getMonth(), now.getYear(), now.getHours(), now.getMinutes());
        $scope.ScheduleToTime = new Date(now.getDate(), now.getMonth(), now.getYear(), now.getHours(), now.getMinutes());

        $scope.PostTimes = [];

        $scope.CurrentPublishTime = now.getHours() >= 12 ? ('0' + now.getHours() % 12).slice(-2) + ':' + ('0' + now.getMinutes()).slice(-2) + ' AM' : ('0' + now.getHours()).slice(-2) + ':' + ('0' + now.getMinutes()).slice(-2) + ' PM';

        $scope.ScheduledMessage = [];

        $scope.status = {
            open: false,
            isCustomHeaderOpen: false,
            isFirstOpen: true,
            isFirstDisabled: false
        };

        $scope.ScheduleDate = "";
        $scope.SchedularDateRange = "";
        $scope.NumberOfTimesScheduling = 1;

        $scope.EditQueueTimes = false;
        $scope.DayCalledName = "";
        $scope.SelectedQueueProfileId = "";
        $scope.Queues = [];
        $scope.UserQueue = [];
        $scope.show = true;
        $scope.queueOption = "";
        $scope.QueueCreation = [];
        $scope.Prevent = false;
        $scope.QueuesFetchedData = [];
        $scope.isQueueChanged = false;
        $scope.TempCreation = [];
        $scope.TempCreationRemoval = [];
    };

    var that = this;
    // time picker
    this.picker2 = {
        date: new Date(),
        timepickerOptions: {
            readonlyInput: false,
            showMeridian: false
        }
    };

    this.picker3 = {
        date: new Date(),
        timepickerOptions: {
            readonlyInput: false,
            showMeridian: false
        }
    };

    this.openCalendar = function (e, picker) {
        that[picker].open = true;
    };

    //Group Key from
    $scope.open1 = function () {
        $scope.popup1.opened = true;
    };
    $scope.popup1 = {
        opened: false
    };

    $scope.formats = ['dd-MM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[0];

    $scope.dateOptions = {
        formatYear: 'yy'
    };

    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    }; //for replacing ~ from images path

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

    $scope.messageBodyArea = "";

    $scope.AddScheduledMessage = function (messageBodyArea, startDate, startTime) {
        console.log(messageBodyArea);
        console.log(startDate);
        console.log(startTime);

        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.CompositeId];
        });

        if (selectedProfiles.length < 1) {
            //alert("Choose atleast 1 profile");
            GlobalService.ErrorPopup("Error", "Choose atleast 1 profile.");
            return;
        }

        if (messageBodyArea == "") {
            //messageBodyArea = "*EMPTY*";
            GlobalService.ErrorPopup("Error", "Please enter text.");
            return;
        }

        var date = new Date(startDate);
        var DayofScheduling = "";
        var MonthofScheduling = "";

        switch (date.getDay()) {
            case 0:
                DayofScheduling = "Sunday";
                break;
            case 1:
                DayofScheduling = "Monday";
                break;
            case 2:
                DayofScheduling = "Tuesday";
                break;
            case 3:
                DayofScheduling = "Wednesday";
                break;
            case 4:
                DayofScheduling = "Thursday";
                break;
            case 5:
                DayofScheduling = "Friday";
                break;
            case 6:
                DayofScheduling = "Saturday";
                break;
            default:
                DayofScheduling = "Not a day";
        }

        switch (date.getMonth()) {
            case 0:
                MonthofScheduling = "Jan";
                break;
            case 1:
                MonthofScheduling = "Feb";
                break;
            case 2:
                MonthofScheduling = "Mar";
                break;
            case 3:
                MonthofScheduling = "Apr";
                break;
            case 4:
                MonthofScheduling = "May";
                break;
            case 5:
                MonthofScheduling = "Jun";
                break;
            case 6:
                MonthofScheduling = "Jul";
                break;
            case 7:
                MonthofScheduling = "Aug";
                break;
            case 8:
                MonthofScheduling = "Sep";
                break;
            case 9:
                MonthofScheduling = "Oct";
                break;
            case 10:
                MonthofScheduling = "Nov";
                break;
            case 11:
                MonthofScheduling = "Dec";
                break;
            default:
                MonthofScheduling = "Not a day";
        }

        //var strStartDate = DayofScheduling + ", " + ('0' + date.getDate()).slice(-2) + "-" + MonthofScheduling + "-" + date.getFullYear();
        var strStartDate = ('0' + date.getDate()).slice(-2) + "-" + MonthofScheduling + "-" + date.getFullYear();

        var time = new Date(startTime);
        var ampm = time.getHours() >= 12 ? 'PM' : 'AM';
        var strStartTime = time.getHours() % 12 + ":" + ('0' + time.getMinutes()).slice(-2) + " " + ampm;

        var obj = { "Message": messageBodyArea, "Date": strStartDate, "Time": strStartTime, "Profiles": selectedProfiles };

        //Add for view    
        $scope.PostTimes.push(obj);
    }

    //Schedule Messages as Draft, Schedule, Approve depending upon rights
    $scope.ScheduleMessages = function (messageBodyArea, startDate, startTime, ScheduleType) {

        var PostTimes = [];
        var SelectedProfileIds = [];
        var Message = encodeURIComponent(messageBodyArea);
        var ScheduledMessageId = $scope.ScheduledMessageId;

        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.CompositeId];
        });

        $.each(selectedProfiles, function (i, obj) {
            //SelectedProfileIds.push(obj.ProfileId);
            SelectedProfileIds.push(obj.ProfileCompnayGroupId);
        });
        console.log(SelectedProfileIds);        

        if (selectedProfiles.length < 1) {
            //alert("Choose atleast 1 profile");
            GlobalService.ErrorPopup("Error", "Choose atleast 1 profile.");
            return;
        }

        if (messageBodyArea == "") {
            //alert("Enter Text in message.");
            GlobalService.ErrorPopup("Error", "Please Enter Text in message.");
            return;
        }

        if (($scope.scheduleMode == 'DateAndTime' && $scope.ScheduleDate == "")
            || ($scope.scheduleMode == 'DateAndTimeRange' && $scope.SchedularDateRange == "")) {
            //alert("Please choose appropriate date(s)");
            GlobalService.ErrorPopup("Error", "Please choose appropriate date(s).");
            return;
        }
       
        //Time validation
        if ($scope.scheduleMode == 'DateAndTime') {
            if (isNaN(startTime)) {
                GlobalService.ErrorPopup("Error", "Please select time in format HH:MM");
                return;
            }

            var selectedDate = new Date(startDate + " " + ('0' + startTime.getHours()).slice(-2) + ":" + ('0' + startTime.getMinutes()).slice(-2));
            var currentDate = new Date();
            var diffInTime = currentDate - selectedDate;
            var durationInterval = 5 * 60 * 1000; //ms
            if (diffInTime > 0) {
                //alert('Back date scheduling is not possible');
                GlobalService.ErrorPopup("Error", "Back date scheduling is not possible.");
                return;
            }
            diffInTime = -1 * diffInTime;
            if (diffInTime > 0 && diffInTime <= durationInterval) {
                GlobalService.ErrorPopup("Error", "Please provide time 5 minutes ahead of current time.");
                return;
            }
        }

        if ($scope.scheduleMode == 'DateAndTime') {
            var date = new Date(startDate);
            var DayofScheduling = "";
            var MonthofScheduling = "";
            switch (date.getDay()) {
                case 0:
                    DayofScheduling = "Sunday";
                    break;
                case 1:
                    DayofScheduling = "Monday";
                    break;
                case 2:
                    DayofScheduling = "Tuesday";
                    break;
                case 3:
                    DayofScheduling = "Wednesday";
                    break;
                case 4:
                    DayofScheduling = "Thursday";
                    break;
                case 5:
                    DayofScheduling = "Friday";
                    break;
                case 6:
                    DayofScheduling = "Saturday";
                    break;
                default:
                    DayofScheduling = "Not a day";
            }
            switch (date.getMonth()) {
                case 0:
                    MonthofScheduling = "Jan";
                    break;
                case 1:
                    MonthofScheduling = "Feb";
                    break;
                case 2:
                    MonthofScheduling = "Mar";
                    break;
                case 3:
                    MonthofScheduling = "Apr";
                    break;
                case 4:
                    MonthofScheduling = "May";
                    break;
                case 5:
                    MonthofScheduling = "Jun";
                    break;
                case 6:
                    MonthofScheduling = "Jul";
                    break;
                case 7:
                    MonthofScheduling = "Aug";
                    break;
                case 8:
                    MonthofScheduling = "Sep";
                    break;
                case 9:
                    MonthofScheduling = "Oct";
                    break;
                case 10:
                    MonthofScheduling = "Nov";
                    break;
                case 11:
                    MonthofScheduling = "Dec";
                    break;
                default:
                    MonthofScheduling = "Not a day";
            }
            var strStartDate = ('0' + date.getDate()).slice(-2) + "-" + MonthofScheduling + "-" + date.getFullYear();
            var time = new Date(startTime);
            var ampm = time.getHours() >= 12 ? 'PM' : 'AM';
            var strStartTime = time.getHours() % 12 + ":" + ('0' + time.getMinutes()).slice(-2) + " " + ampm;

            //For View
            $scope.ScheduledMessage = [];
            $scope.ScheduledMessage.push({ "Message": messageBodyArea, "Date": strStartDate, "Time": strStartTime, "Profiles": selectedProfiles, "FirstProfileImage": selectedProfiles[0].ProfilePicURL });

            //Data Population
            PostTimes.push(strStartDate + "|" + strStartTime);
        } else if ($scope.scheduleMode == 'DateAndTimeRange') {
            //Date Time Range Scheduling
            for (var PostTimesIterator = 0; PostTimesIterator < $scope.PostTimes.length; PostTimesIterator++) {
                PostTimes.push($scope.PostTimes[PostTimesIterator].Date + "|" + $scope.PostTimes[PostTimesIterator].Time);
            }
        }


        if (PostTimes.length > 0) {
            var IsDraft = "";
            var IsScheduledMessage = "";
            var IsPreConfiguredQueue = "";
            var IsQueueAtBeginning = "";
            var CreatorUserId = 0;
            var CompanyId = $scope.CompanyId;
            var MessageStatus = -1;

            var confirmationModalHeading = "";
            var confirmationModalText = "";

            if (ScheduleType == "Save") {
                IsDraft = true;
                IsScheduledMessage = true;
                IsPreConfiguredQueue = false;
                IsQueueAtBeginning = false;
                MessageStatus = 0; //Draft
            }
            else if (ScheduleType == "ScheduleOnly") {
                IsDraft = false;
                IsScheduledMessage = true;
                IsPreConfiguredQueue = false;
                IsQueueAtBeginning = false;
                MessageStatus = 1; //Pending
            }
            else if (ScheduleType == "Approve") {
                IsDraft = false;
                IsScheduledMessage = true;
                IsPreConfiguredQueue = false;
                IsQueueAtBeginning = false;
                MessageStatus = 2; //Approve
            }
            else if (ScheduleType == "Reject") {
                IsDraft = false;
                IsScheduledMessage = true;
                IsPreConfiguredQueue = false;
                IsQueueAtBeginning = false;
                MessageStatus = 3; //Reject
            }
            
            if ($scope.scheduleMode == 'DateAndTime') {

                if (ScheduleType == "Save") {
                    confirmationModalHeading = "Save Scheduled Message";
                    confirmationModalText = "Do you wish to save scheduled message? You will be able to change it later.";
                }
                else if (ScheduleType == "ScheduleOnly") {
                    confirmationModalHeading = "Schedule Message";
                    confirmationModalText = "Do you wish to schedule this message? Message needs to be approved to get it published.";
                } 
                else if (ScheduleType == "Approve") {
                    confirmationModalHeading = "Approve Schedule Message";
                    confirmationModalText = "Do you wish to approve this message? You will not be able to change it later.";
                }
                else if (ScheduleType == "Reject"){
                        confirmationModalHeading = "Reject Schedule Message";
                        confirmationModalText = "Do you wish to reject this scheduled message? You will not be able to change it later.";
                }

                if ($scope.isAlreadyMessage == false) {
                    $scope.items = { ScheduleType: ScheduleType, Message: Message, SelectedProfileIds: SelectedProfileIds, PostTimes: PostTimes, IsDraft: IsDraft, IsScheduledMessage: IsScheduledMessage, IsPreConfiguredQueue: IsPreConfiguredQueue, IsQueueAtBeginning: IsQueueAtBeginning, CreatorUserId: CreatorUserId, CompanyId: CompanyId, MessageStatus: MessageStatus, ScheduledMessageId: ScheduledMessageId, MediaInfo: $scope.mediaData, ConfirmationTitle: confirmationModalHeading, ConfirmationMessage: confirmationModalText };
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: '/Scripts/Views/Shared/_ConfirmationPopUp.html',
                        controller: '_DirectScheduleMessageController',

                        resolve: {
                            items: function () {
                                return $scope.items;
                            }
                        }
                    });

                    modalInstance.result.then(function () {
                        $scope.messageBodyArea = "";
                        $scope.selected = [];

                        //Media
                        $scope.UploadedFilesCount = 0;
                        $scope.mediaData = new Array();
                        $scope.UploadedFiles = new Array();

                    });

                } else if ($scope.isAlreadyMessage == true) {                   
                    $scope.items = { ScheduleType: ScheduleType, Message: Message, SelectedProfileIds: SelectedProfileIds, PostTimes: PostTimes, IsDraft: IsDraft, IsScheduledMessage: IsScheduledMessage, IsPreConfiguredQueue: IsPreConfiguredQueue, IsQueueAtBeginning: IsQueueAtBeginning, CreatorUserId: CreatorUserId, CompanyId: CompanyId, MessageStatus: MessageStatus, ScheduledMessageId: ScheduledMessageId, MediaInfo: $scope.mediaData, ConfirmationTitle: confirmationModalHeading, ConfirmationMessage: confirmationModalText };
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: '/Scripts/Views/Shared/_ConfirmationPopUp.html',
                        controller: '_ApproveScheduledMessageController',

                        resolve: {
                            items: function () {
                                return $scope.items;
                            }
                        }
                    });
                    modalInstance.result.then(function () {
                        if (IsDraft == false) {
                            if (ScheduleType == "ScheduleOnly") {
                                $scope.disableDraft = true;
                                $scope.disableSchedule = true;
                                notify({
                                    message: "Message Scheduled.",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                            } else if (ScheduleType == "Approve") {
                                $scope.disableDraft = true;
                                $scope.disableSchedule = true;
                                $scope.disableApprove = true;
                                $scope.disableReject = true;
                                notify({
                                    message: "Message Approved.",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                            } else if (ScheduleType == "Reject") {
                                $scope.disableDraft = true;
                                $scope.disableSchedule = true;
                                $scope.disableApprove = true;
                                $scope.disableReject = true;
                                notify({
                                    message: "Message Rejected.",
                                    position: GlobalService.position,
                                    duration: GlobalService.duration
                                });
                            }
                            $window.location.href = "#/ScheduledMessages";
                        } else if (IsDraft == true) {
                            notify({
                                message: "Message saved to Drafts.",
                                position: GlobalService.position,
                                duration: GlobalService.duration
                            });
                        }
                    }, function () {
                        $log.info('Modal dismissed at: ' + new Date());
                    });
                
                }
            } else if ($scope.scheduleMode == 'DateAndTimeRange') {

                confirmationModalHeading = "Date Range Schedule";
                confirmationModalText = "Do you wish to add these messages?";

                $scope.items = { ScheduleType: ScheduleType, Message: Message, SelectedProfileIds: SelectedProfileIds, PostTimes: PostTimes, IsDraft: IsDraft, IsScheduledMessage: IsScheduledMessage, IsPreConfiguredQueue: IsPreConfiguredQueue, IsQueueAtBeginning: IsQueueAtBeginning, CreatorUserId: CreatorUserId, CompanyId: CompanyId, MessageStatus: MessageStatus, ScheduledMessageId: ScheduledMessageId, MediaInfo: $scope.mediaData, ConfirmationTitle: confirmationModalHeading, ConfirmationMessage: confirmationModalText };
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: '/Scripts/Views/Shared/_ConfirmationPopUp.html',
                    controller: '_DateRangeScheduledMessageController',

                    resolve: {
                        items: function () {
                            return $scope.items;
                        }
                    }
                });

                modalInstance.result.then(function () {
                    $scope.messageBodyArea = "";
                    $scope.selected = [];
                }, function () {
                    $log.info('Modal dismissed at: ' + new Date());
                });
            }
        }
        else {
            //alert('Please select profiles and type message.');
            GlobalService.ErrorPopup("Error", "Please select profiles and type message.");
        }
    }

    $scope.calculateCheckedProfiles = function () {

        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.CompositeId];
        });

        if (selectedProfiles.length > 0 && $scope.messageBodyArea != "") {
            alert('Ok');
        }
        else {
            alert('Not allowed');
        }

    };

    //Check all profiles using checkAll
    $scope.checkAllProfiles = function () {        
        //alert($scope.selectProfiles);
        angular.forEach($scope.queuedProfiles, function (obj) {
            $scope.selectedQueueProfiles[obj.ProfileId] = $scope.selectProfiles;
        });
    }

    $scope.getMinimumLength = function (selectedProfiles) {
        var value = 0;
        if (selectedProfiles[0].ProfileTypeId == 4) value = 140;
        else if (selectedProfiles[0].ProfileTypeId == 3) value = 700;
        else if (selectedProfiles[0].ProfileTypeId == 1) value = 2000;

        for (var i = 0; i < selectedProfiles.length; i++) {
            if (selectedProfiles[i].ProfileTypeId == 4) {
                if (value > 140) {
                    value = 140;
                }
            }
            else if (selectedProfiles[i].ProfileTypeId == 3) {
                if (value > 700) {
                    value = 700;
                }
            }
            if (selectedProfiles[i].ProfileTypeId == 1) {
                if (value > 5000) {
                    value = 5000;
                }
            }
        }
        $scope.messageBodyAreaAllowedLength = value;
        var reducedText = $scope.messageBodyArea;
        reducedText = reducedText.substring(0, value);
        $scope.messageBodyArea = reducedText;

    }

    $scope.selectedProfilesChanged = function () {
        var any = true;
        angular.forEach($scope.queuedProfiles, function (obj) {
            if (!$scope.selectedQueueProfiles[obj.CompositeId]) {
                any = false;
            }
        });
        $scope.selectProfiles = any;

        var selectedProfiles = $.grep($scope.queuedProfiles, function (record) {
            return $scope.selectedQueueProfiles[record.CompositeId];
        });
        $scope.getMinimumLength(selectedProfiles);

        $scope.ValidateFiles();
    }

    $scope.scheduledProfilesChanged = function () {
        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.CompositeId];
        });

        $scope.getMinimumLength(selectedProfiles);
    }    

    $scope.getDayByNumber = function (dayNumber) {
        var day = "";
        switch (dayNumber) {
            case "1":
                day = "Mon"
                break;
            case "2":
                day = "Tue"
                break;
            case "3":
                day = "Wed"
                break;
            case "4":
                day = "Thu"
                break;
            case "5":
                day = "Fri"
                break;
            case "6":
                day = "Sat"
                break;
            case "7":
                day = "Sun"
                break;
            default:
                day = "NAD";
        }
        return day;
    }

    //Group Change
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.GetTwitterProfilesByGroupId();
        $scope.GetAllQueues();
    });

    $scope.addDateRangeScheduledMessage = function (messageBodyArea, dateRange, numberOfTimes) {
        //alert($scope.scheduleMode);

        $scope.PostTimes = [];
        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.CompositeId];
        });

        if (selectedProfiles.length < 1) {

            //alert("Choose atleast 1 profile");
            GlobalService.ErrorPopup("Error", "Choose atleast 1 profile.");
            return;
        }

        if (messageBodyArea == "") {
            messageBodyArea = "*EMPTY*";
        }

        if (dateRange == "") {
            GlobalService.ErrorPopup("Error", "Select Date Range.");
            //alert("Choose date range.");
            return;
        }

        var strStartDate = dateRange.substring(0, 19);
        var strEndDate = dateRange.substring(22, 41);

        var startDate = new Date(strStartDate);
        var endDate = new Date(strEndDate);

        var diff = endDate - startDate;
        //var diffMins = Math.round(((diff % 86400000) % 3600000) / 60000); // minutes
        var diffMins = Math.round(diff / 60000); // minutes

        var postAfterWhichMinutes = Math.floor(diffMins / numberOfTimes);

        if (postAfterWhichMinutes <= 30) {
            GlobalService.ErrorPopup("Error", "The difference in time between each post must be greater than 30 minutes.");            
            //alert('The difference in time between each post must be greater than 30 minutes.');
            return;
        }

        console.log("decoded: " + startDate + "-" + endDate);
        console.log("diff Minutes: " + postAfterWhichMinutes);

        for (var iterator = 1; iterator <= numberOfTimes; iterator++) {
            var postingDate = new Date(startDate.getTime() + iterator * postAfterWhichMinutes * 60000);
            var ampm = postingDate.getHours() >= 12 ? 'PM' : 'AM';
            var MonthofScheduling = "Nan";
            switch (postingDate.getMonth()) {
                case 0:
                    MonthofScheduling = "Jan";
                    break;
                case 1:
                    MonthofScheduling = "Feb";
                    break;
                case 2:
                    MonthofScheduling = "Mar";
                    break;
                case 3:
                    MonthofScheduling = "Apr";
                    break;
                case 4:
                    MonthofScheduling = "May";
                    break;
                case 5:
                    MonthofScheduling = "Jun";
                    break;
                case 6:
                    MonthofScheduling = "Jul";
                    break;
                case 7:
                    MonthofScheduling = "Aug";
                    break;
                case 8:
                    MonthofScheduling = "Sep";
                    break;
                case 9:
                    MonthofScheduling = "Oct";
                    break;
                case 10:
                    MonthofScheduling = "Nov";
                    break;
                case 11:
                    MonthofScheduling = "Dec";
                    break;
                default:
                    MonthofScheduling = "Not a day";
            }

            var postDate = ('0' + postingDate.getDate()).slice(-2) + "-" + MonthofScheduling + "-" + (postingDate.getFullYear());
            var postTime = ('0' + postingDate.getHours() % 12).slice(-2) + ":" + ('0' + postingDate.getMinutes()).slice(-2) + " " + ampm;

            $scope.PostTimes.push({ "Index": iterator - 1, "Date": postDate, "Time": postTime });
        }

        console.log($scope.PostTimes);

    }

    $scope.removePostingTime = function (index) {
        //$scope.PostTimes.splice(index, 1);
        var listToDelete = [];
        listToDelete.push(index);

        for (var i = 0; i < $scope.PostTimes.length; i++) {
            var obj = $scope.PostTimes[i];

            if (listToDelete.indexOf(obj.Index) !== -1) {
                $scope.PostTimes.splice(i, 1);
                i--;
            }
        }
    }

    $scope.ShowClockFor = function (day) {
        //$scope.Prevent = true;
        $scope.DayCalledName = day;
    }

    $scope.AddToQueue = function (Time, Day, ProfileCompanyGroupId) {       
        var TimeDate = new Date(Time);
        var strTime = ('0' + TimeDate.getHours()).slice(-2) + ':' + ('0' + TimeDate.getMinutes()).slice(-2);
        var myDay = 0;
        var ampm = TimeDate.getHours() >= 12 ? 'PM' : 'AM';

        var temp_Hour = ('0' + TimeDate.getHours() % 12).slice(-2);
        var TimeforDisplay = "";
        if (temp_Hour == '00' && ampm == 'PM') {
            TimeforDisplay = "12" + ":" + ('0' + TimeDate.getMinutes()).slice(-2) + " " + ampm;
        } else {
            TimeforDisplay = ('0' + TimeDate.getHours() % 12).slice(-2) + ":" + ('0' + TimeDate.getMinutes()).slice(-2) + " " + ampm;
        }
        
        var hoursForView = ('0' + TimeDate.getHours()).slice(-2);
        var allowAddition = true;

        switch (Day) {
            case "Sunday":
                myDay = 7;
                break;
            case "Monday":
                myDay = 1;
                break;
            case "Tuesday":
                myDay = 2;
                break;
            case "Wednesday":
                myDay = 3;
                break;
            case "Thursday":
                myDay = 4;
                break;
            case "Friday":
                myDay = 5;
                break;
            case "Saturday":
                myDay = 6;
                break;
            default:
                myDay = 0;
        }

        var myTime = ('0' + TimeDate.getHours()).slice(-2);

        //Check if queue contains this time or not
        angular.forEach($scope.QueueCreation, function (obj) {
            if (obj.Day == myDay && obj.Time == TimeforDisplay) {
                allowAddition = false;
            }
        });
        
        var durationInterval = 5 * 60 * 1000; //ms
        var CurrentDate = new Date();

        var SetDate = new Date(CurrentDate.getFullYear(), CurrentDate.getMonth(), CurrentDate.getDate(), TimeDate.getHours(), TimeDate.getMinutes(), 0);

        //if (Object.prototype.toString.call(SetDate) !== "[object Date]") {
        if (isNaN(SetDate)) {
            GlobalService.ErrorPopup("Alert", "Please provide correct time.");
            return;
        }

        //if ((SetDate - CurrentDate) < durationInterval && myDay == CurrentDate.getDay() && SetDate.getMinutes()>=CurrentDate.getMinutes()) {
        if ((SetDate - CurrentDate) < durationInterval && (SetDate - CurrentDate) > 0 && myDay == CurrentDate.getDay()) {
            GlobalService.ErrorPopup("Alert", "Please provide suitable time for schedule. i.e. 5 minutes ahead of current time.");
            return;
        }
       
        if (allowAddition == true) {
            $scope.Prevent = true;
            $scope.QueueCreation.push({ "Day": myDay, "Time": TimeforDisplay, "ProfileCompanyGroupId": ProfileCompanyGroupId, "CompanyId": parseInt($rootScope.selectedCompanyId), "CreatorUserId": 0 });

            $scope.TempCreation.push({ "Day": myDay, "Time": hoursForView, "ProfileCompanyGroupId": ProfileCompanyGroupId });

            document.getElementById("User_" + ProfileCompanyGroupId + "_Day_" + myDay + "_Time_" + myTime).className += " round_assigned";
            $scope.isQueueChanged = true;
        } else {
            GlobalService.ErrorPopup("Alert", "Please provide correct time, you can't add same time again.");
        }
             
    }

    $scope.RemoveFromQueue = function (Index, Day, Time, Id) {        
        console.log(Index + "," + Day + "," + Time + "," + Id);
        $scope.Prevent = true;

        $scope.isQueueChanged = true;
        //delete $scope.QueueCreation[Index];
        $scope.QueueCreation.splice(Index, 1);

        var ampm = Time.substring(6,8);         
        var myTime = Time.substring(0, 2);
        if (ampm == "PM") {
            myTime = parseInt(myTime) + 12;
        }

        document.getElementById("User_" + Id + "_Day_" + Day + "_Time_" + myTime).className = "timeline_round2 showico ng-scope";
        $scope.TempCreationRemoval.push({ "Day": Day, "Time": myTime, "ProfileCompanyGroupId": Id });
    }

    $scope.QueueTabChanged = function (mode) {
        //$scope.show = true;
        if (mode == 1) {
            $scope.scheduleMode = 'SelectPreconfiguredQueue';
            $scope.show = true;
        }
        else if (mode == 2) {
            $scope.show = false;
            $scope.scheduleMode = 'SetTimeForQueue';

            //Get Queues
            $scope.GetAllQueues();
            console.log("hey");
        }
    }

    $scope.GetAllQueues = function () {
        PublisherService.GetAllQueues($rootScope.selectedCompanyId).then(function (resultSet) {
            if (resultSet != null) {
                //console.log(resultSet);
                if (resultSet["statusText"] == "OK") {
                    //alert("Queues Fetched.");
                    $scope.QueuesFetchedData = resultSet.data;
                    $scope.DisplayQueueTimes(resultSet.data);
                    $scope.AddTodaysQueueDataToProfiles();                    
                    $scope.DayCalledName = "";
                }
            }
            //alert("defaultfully fetched");
        }, function (err) {
            //alert('Error in fetching, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching please try again.");
            console.log(err);
        });
    }

    $scope.DisplayQueueTimes = function (data) {
        var profiles = [];
        $scope.Queues = [];        

        angular.forEach(data, function (obj) {

            if (profiles.indexOf(obj.ProfileCompanyGroupId) === -1) {
                profiles.push(obj.ProfileCompanyGroupId);
            }
        });

        angular.forEach(profiles, function (profile) {
            var profileQueue = {
                "ProfileId": null,
                "Queue": []
            };

            profileQueue["ProfileId"] = profile;

            for (var dayCount = 1; dayCount <= 7; dayCount++) {
                var queuePerDay = {
                    "DayId": null,
                    "Times": {
                        "0": { "Hour": "04", "Status": "InActive" },
                        "1": { "Hour": "05", "Status": "InActive" },
                        "2": { "Hour": "06", "Status": "InActive" },
                        "3": { "Hour": "07", "Status": "InActive" },
                        "4": { "Hour": "08", "Status": "InActive" },
                        "5": { "Hour": "09", "Status": "InActive" },
                        "6": { "Hour": "10", "Status": "InActive" },
                        "7": { "Hour": "11", "Status": "InActive" },
                        "8": { "Hour": "12", "Status": "InActive" },
                        "9": { "Hour": "13", "Status": "InActive" },
                        "10": { "Hour": "14", "Status": "InActive" },
                        "11": { "Hour": "15", "Status": "InActive" },
                        "12": { "Hour": "16", "Status": "InActive" },
                        "13": { "Hour": "17", "Status": "InActive" },
                        "14": { "Hour": "18", "Status": "InActive" },
                        "15": { "Hour": "19", "Status": "InActive" },
                        "16": { "Hour": "20", "Status": "InActive" },
                        "17": { "Hour": "21", "Status": "InActive" },
                        "18": { "Hour": "22", "Status": "InActive" },
                        "19": { "Hour": "23", "Status": "InActive" },
                        "20": { "Hour": "00", "Status": "InActive" },
                        "21": { "Hour": "01", "Status": "InActive" },
                        "22": { "Hour": "02", "Status": "InActive" },
                        "23": { "Hour": "03", "Status": "InActive" }
                    }
                };

                queuePerDay["DayId"] = dayCount;
                angular.forEach(data, function (myobj) {
                    if (myobj.Day == dayCount && myobj.ProfileCompanyGroupId == profile) {
                        var strTime = myobj.Time;
                        var hour = strTime.substring(0, 2);
                        if (hour == "01") queuePerDay.Times["21"].Status = "Active";
                        else if (hour == "02") queuePerDay.Times["22"].Status = "Active";
                        else if (hour == "03") queuePerDay.Times["23"].Status = "Active";
                        else if (hour == "04") queuePerDay.Times["0"].Status = "Active";
                        else if (hour == "05") queuePerDay.Times["1"].Status = "Active";
                        else if (hour == "06") queuePerDay.Times["2"].Status = "Active";
                        else if (hour == "07") queuePerDay.Times["3"].Status = "Active";
                        else if (hour == "08") queuePerDay.Times["4"].Status = "Active";
                        else if (hour == "09") queuePerDay.Times["5"].Status = "Active";
                        else if (hour == "10") queuePerDay.Times["6"].Status = "Active";
                        else if (hour == "11") queuePerDay.Times["7"].Status = "Active";
                        else if (hour == "12") queuePerDay.Times["8"].Status = "Active";
                        else if (hour == "13") queuePerDay.Times["9"].Status = "Active";
                        else if (hour == "14") queuePerDay.Times["10"].Status = "Active";
                        else if (hour == "15") queuePerDay.Times["11"].Status = "Active";
                        else if (hour == "16") queuePerDay.Times["12"].Status = "Active";
                        else if (hour == "17") queuePerDay.Times["13"].Status = "Active";
                        else if (hour == "18") queuePerDay.Times["14"].Status = "Active";
                        else if (hour == "19") queuePerDay.Times["15"].Status = "Active";
                        else if (hour == "20") queuePerDay.Times["16"].Status = "Active";
                        else if (hour == "21") queuePerDay.Times["17"].Status = "Active";
                        else if (hour == "22") queuePerDay.Times["18"].Status = "Active";
                        else if (hour == "23") queuePerDay.Times["19"].Status = "Active";
                        else if (hour == "00") queuePerDay.Times["20"].Status = "Active";
                    }
                });
                profileQueue.Queue.push(queuePerDay);
            }
            $scope.Queues.push(profileQueue);
            console.log($scope.Queues);

        });

        console.log(profiles);
    }

    $scope.populateUserQueue = function (ProfileCompnayGroupId) {       
        //View
        $scope.EditQueueTimes = !$scope.EditQueueTimes;

        if($scope.Prevent == false){
        var found = false;
        $scope.SelectedQueueProfileId = ProfileCompnayGroupId;
        angular.forEach($scope.Queues, function (Queue) {
            if (Queue.ProfileId == $scope.SelectedQueueProfileId) {
                found = true;
                $scope.UserQueue = Queue.Queue;
            }
        });

        if (found == false) {
            {
                //reset queue for no entry
                var profileQueue = {
                    "ProfileId": ProfileCompnayGroupId,
                    "Queue": []
                };

                for (var loop = 0; loop < 7; loop++) {
                    var queuePerDay = {
                        "DayId": loop + 1,
                        "Times": {
                            "0": { "Hour": "04", "Status": "InActive" },
                            "1": { "Hour": "05", "Status": "InActive" },
                            "2": { "Hour": "06", "Status": "InActive" },
                            "3": { "Hour": "07", "Status": "InActive" },
                            "4": { "Hour": "08", "Status": "InActive" },
                            "5": { "Hour": "09", "Status": "InActive" },
                            "6": { "Hour": "10", "Status": "InActive" },
                            "7": { "Hour": "11", "Status": "InActive" },
                            "8": { "Hour": "12", "Status": "InActive" },
                            "9": { "Hour": "13", "Status": "InActive" },
                            "10": { "Hour": "14", "Status": "InActive" },
                            "11": { "Hour": "15", "Status": "InActive" },
                            "12": { "Hour": "16", "Status": "InActive" },
                            "13": { "Hour": "17", "Status": "InActive" },
                            "14": { "Hour": "18", "Status": "InActive" },
                            "15": { "Hour": "19", "Status": "InActive" },
                            "16": { "Hour": "20", "Status": "InActive" },
                            "17": { "Hour": "21", "Status": "InActive" },
                            "18": { "Hour": "22", "Status": "InActive" },
                            "19": { "Hour": "23", "Status": "InActive" },
                            "20": { "Hour": "00", "Status": "InActive" },
                            "21": { "Hour": "01", "Status": "InActive" },
                            "22": { "Hour": "02", "Status": "InActive" },
                            "23": { "Hour": "03", "Status": "InActive" }
                        }
                    };

                    profileQueue.Queue.push(queuePerDay);
                }

                $scope.UserQueue = profileQueue.Queue;
            }
        }

        //Emptying
        $scope.QueueCreation = [];
        angular.forEach($scope.QueuesFetchedData, function (QueueData) {
            if (QueueData.ProfileCompanyGroupId == ProfileCompnayGroupId) {                
                var hours = parseInt(QueueData.Time.substring(0, 2));
                var minutes = QueueData.Time.substring(3, 5);
                var ampm = hours >= 12 ? "PM" : "AM";         

                if (hours % 12 == 0) {
                    hours = 12;
                } else {
                    hours = ('0' + hours % 12).slice(-2);
                }
                var TimeforDisplay = hours + ":" + minutes + " " + ampm;               
                $scope.QueueCreation.push({ "Day": QueueData.Day, "Time": TimeforDisplay, "ProfileCompanyGroupId": ProfileCompnayGroupId, "CompanyId": parseInt($rootScope.selectedCompanyId), "CreatorUserId": 0 });
            }
        });

        }
    }

    $scope.broadcastmethodpopulateUserQueue = function (ProfileCompnayGroupId) {
        angular.forEach($scope.Queues, function (Queue) {
            if (Queue.ProfileId == ProfileCompnayGroupId) {
                $scope.UserQueue = Queue.Queue;
            }
        });
    }

    $scope.AddTodaysQueueDataToProfiles = function () {
        var currentDate = new Date();
        var Day = currentDate.getDay();

        //Mon 1, Tue 2, Wed 3, Thu 4, Fri 5, Sat 6, Sun 0
        var isDone = false;
        angular.forEach($scope.attachedProfiles, function (Profile) {
            isDone = false;
            angular.forEach($scope.Queues, function (Queue) {
                if (Queue.ProfileId == Profile.ProfileCompnayGroupId) {
                    if (Day > 0) {
                        Profile.TodaysQueue = Queue.Queue[Day - 1];
                        isDone = true;
                    } else if (Day == 0) {
                        Profile.TodaysQueue = Queue.Queue[6];
                        isDone = true;
                    }
                }
            });

            if (isDone == false) {
                Profile.TodaysQueue = {
                    "DayId": Day,
                    "Times": {
                        "0": { "Hour": "04", "Status": "InActive" },
                        "1": { "Hour": "05", "Status": "InActive" },
                        "2": { "Hour": "06", "Status": "InActive" },
                        "3": { "Hour": "07", "Status": "InActive" },
                        "4": { "Hour": "08", "Status": "InActive" },
                        "5": { "Hour": "09", "Status": "InActive" },
                        "6": { "Hour": "10", "Status": "InActive" },
                        "7": { "Hour": "11", "Status": "InActive" },
                        "8": { "Hour": "12", "Status": "InActive" },
                        "9": { "Hour": "13", "Status": "InActive" },
                        "10": { "Hour": "14", "Status": "InActive" },
                        "11": { "Hour": "15", "Status": "InActive" },
                        "12": { "Hour": "16", "Status": "InActive" },
                        "13": { "Hour": "17", "Status": "InActive" },
                        "14": { "Hour": "18", "Status": "InActive" },
                        "15": { "Hour": "19", "Status": "InActive" },
                        "16": { "Hour": "20", "Status": "InActive" },
                        "17": { "Hour": "21", "Status": "InActive" },
                        "18": { "Hour": "22", "Status": "InActive" },
                        "19": { "Hour": "23", "Status": "InActive" },
                        "20": { "Hour": "00", "Status": "InActive" },
                        "21": { "Hour": "01", "Status": "InActive" },
                        "22": { "Hour": "02", "Status": "InActive" },
                        "23": { "Hour": "03", "Status": "InActive" }
                    }
                };
            }

            console.log(Profile);
        });


    }

    $scope.scheduleQueueMessage = function () {
        var SelectedOption = 0;

        if ($scope.queueOption == "queueNext")
            SelectedOption = 1;
        else if ($scope.queueOption == "queueLast")
            SelectedOption = 2;

        if ($scope.messageBodyArea == "") {
            //alert("Enter Text in message.");
            GlobalService.ErrorPopup("Error", "Enter Text in message.");
            return;
        }

        var SelectedProfileIds = [];
        var Message = encodeURIComponent($scope.messageBodyArea);

        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.CompositeId];
        });

        if (selectedProfiles.length < 1) {
            GlobalService.ErrorPopup("Error", "Choose atleast 1 profile.");
            //alert("Choose atleast 1 profile");
            return;
        }

        $.each(selectedProfiles, function (i, obj) {
            SelectedProfileIds.push(obj.ProfileCompnayGroupId);
        });
        
        $scope.items = { Message: Message, SelectedProfileIds: SelectedProfileIds, SelectedOption: SelectedOption, CompanyId:$rootScope.selectedCompanyId, MediaInfo: $scope.mediaData, ConfirmationTitle: "Queue Message", ConfirmationMessage: "Are you sure you want to queue this message?" };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ConfirmationPopUp.html',
            controller: '_ApproveQueuedMessageController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function () {
            //On success of adding queue message Broadcast
            notify({
                message: "Queue Message Scheduled.",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $scope.messageBodyArea = "";
            angular.forEach($scope.attachedProfiles, function (obj) {
                $scope.selected[obj.ProfileId] = false;
            });
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.LoadUserQueueData = function (Id) {      
        //alert(Id);
        //$scope.EditQueueTimes = false;
        if ($scope.Prevent == false){
            $scope.populateUserQueue(Id);
            $scope.currentProfileQueueEditing = Id;
        }

        if ($scope.Prevent == true) {
            if ($scope.currentProfileQueueEditing != Id) {
                GlobalService.ErrorPopup("Alert", "You need to Save or Cancel the changes made in queue before loading any other profile.");
            }
        }
    }

    $scope.SaveChanges = function (Id) {
        
        //alert(Id);
        if ($scope.QueueCreation.length <= 0) {
            GlobalService.ErrorPopup("Error", "You can't save an Empty Queue, please Add atleast 1 Time slot and try again.");
            return;
        }

        //Saving Queue
        PublisherService.AddTimeToQueue($scope.QueueCreation).then(function (resultSet) {
            if (resultSet != null) {
                console.log(resultSet);
                if (resultSet["statusText"] == "OK") {

                    //Let go prevent
                    if ($scope.Prevent == true) {
                        $scope.Prevent = false;
                    }
                    $scope.GetAllQueues();

                    notify({
                        message: "Changes saved to Queue.",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
               }
            }
            //alert("defaultfully fetched");
        }, function (err) {
            //alert('Error in fetching, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
            console.log(err);
        });

        //Hiding Buttons
        $scope.isQueueChanged = false;
    }

    $scope.CancelChanges = function (Id) {
        //DO NOTHING
        if ($scope.Prevent == true) {
            $scope.Prevent = false;
        }
        //Repopulation
        //$scope.populateUserQueue(Id);
        angular.forEach($scope.TempCreation, function (Queue) {
            document.getElementById("User_" + Queue.ProfileCompanyGroupId + "_Day_" + Queue.Day + "_Time_" + Queue.Time).className = "timeline_round2 showico ng-scope";
        });

        angular.forEach($scope.TempCreationRemoval, function (Queue) {
            document.getElementById("User_" + Queue.ProfileCompanyGroupId + "_Day_" + Queue.Day + "_Time_" + Queue.Time).className += " round_assigned";
        });        
       
        //Hiding Buttons
        $scope.isQueueChanged = false;
    }

    $scope.DeleteAllSlots = function (Id) {
        //Giving back rights

        if ($scope.Prevent == true) {
            $scope.Prevent = false;
        }

        //Service Call to delete all slots
        $scope.items = { ProfileCompanyGroupId:Id, CompanyId: $scope.CompanyId, ConfirmationTitle: "Alert", ConfirmationMessage: "Are you sure you want to delete all queue slots? It will result in all scheduled messages to be placed in Waiting state." };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ConfirmationPopUp.html',
            controller: '_DeleteAllQueueSlotsController',
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function () {
            //Clear queue
            $scope.QueueCreation = [];
                       
            for (var daysloop = 1; daysloop <= 7; daysloop++) {
                for (var hoursloop = 0; hoursloop < 24; hoursloop++) {
                    var setHours = ('0' + hoursloop).slice(-2);
                    document.getElementById("User_" + $scope.currentProfileQueueEditing + "_Day_" + daysloop + "_Time_" + setHours).className = "timeline_round2 showico ng-scope";
                }
            }

            $scope.GetAllQueues();

        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    ////////////////// LOADING PREVIOUS MESSAGES WRT TYPE ////////////////////////
    $scope.ResetButtons = function (Type, mode) {
        
        //$scope.QueueTabChanged = mode;        
        if(Type == "Queue"){
            $scope.QueueTabChanged(mode);
        } else {
            //New Message
        if ($scope.isAlreadyMessage == false) {
            if (Type == "Schedule") {
                $scope.disableDraft = false;
                $scope.disableSchedule = false;
                $scope.disableApprove = false;
                $scope.disableReject = true;
                $scope.HideTheButtons = false;
            } else if (Type == "Queue") {
                $scope.disableDraft = true;
                $scope.disableSchedule = true;
                $scope.disableApprove = true;
                $scope.disableReject = true;
                $scope.HideTheButtons = true;
            }
        }
            //Old Message with Draft/Scheduled/Approved/Reject status
        else if ($scope.isAlreadyMessage == true) {
            //Draft
            if ($scope.scheduledMessageType == "0") {
                $scope.disableDraft = false;
                $scope.disableSchedule = false;
                $scope.disableApprove = false;
                $scope.disableReject = true;
            }
            //Scheduled
            if ($scope.scheduledMessageType == "1") {
                $scope.disableDraft = true;
                $scope.disableSchedule = true;
                $scope.disableApprove = false;
                $scope.disableReject = false;
            }
            //Approved
            if ($scope.scheduledMessageType == "2") {
                $scope.disableDraft = true;
                $scope.disableSchedule = true;
                $scope.disableApprove = true;
                $scope.disableReject = true;
            }
            //Rejected
            if ($scope.scheduledMessageType == "3") {
                $scope.disableDraft = true;
                $scope.disableSchedule = true;
                $scope.disableApprove = true;
                $scope.disableReject = true;
            }
            //Sent
            if ($scope.scheduledMessageType == "4") {
                $scope.disableDraft = true;
                $scope.disableSchedule = true;
                $scope.disableApprove = true;
                $scope.disableReject = true;
            }
        }
        }        
    }

    $scope.LoadPreviouslySavedOrScheduledMessage = function (ScheduledMessageId, MessageType) {
        PublisherService.LoadPreviouslySavedOrScheduledMessage(ScheduledMessageId).then(function (resultSet) {
            if (resultSet != null) {
                console.log(resultSet);
                if (resultSet["statusText"] == "OK") {
                    var data = resultSet.data;

                    $scope.isAlreadyMessage = true;
                    $scope.ScheduledMessageId = data.ScheduledMessageId;

                    $scope.messageBodyArea = decodeURIComponent(data.MessageText);

                    angular.forEach($scope.attachedProfiles, function (obj) {
                        if (data.SelectedProfiles.indexOf(obj.ProfileCompnayGroupId) !== -1) {
                            var temp = data.SelectedProfilesType[0];
                            $scope.selected[temp + "_" + obj.ProfileCompnayGroupId] = true;
                        }
                    });

                    //For view
                    angular.forEach(data.SelectedProfiles, function (profile) {
                        var found = $scope.attachedProfiles.filter(function (Innervalue) { return Innervalue.ProfileCompnayGroupId === profile })
                        if (found.length != 0){
                            found[0].select = true;                                                        
                        }
                    });

                    var rec_dateAndTime = "";
                    angular.forEach(data.PostTimes, function(PostTime){
                        rec_dateAndTime = PostTime;
                    });

                    var dateAndTime = rec_dateAndTime.split("|");
                    
                    $scope.ScheduleDate = dateAndTime[0];

                    var myDate = new Date();
                    var time = dateAndTime[1].split(":");
                    myDate.setHours(time[0]);
                    myDate.setMinutes(time[1]);
                    $scope.ctrl.picker2.date = myDate;

                    //Media
                    $scope.mediaData = data.mediaInfo;
                    //$scope.UploadedFiles = data.mediaInfo;
                    $scope.UploadedFilesCount = data.mediaInfo.length;

                    $scope.ResetButtons('Schedule', 0);
                }
            }            
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching, please try again.");
            console.log(err);
        });
    }

    $scope.TempSetting = function (mode) {
        $scope.show = true;
        $scope.scheduleMode = mode;
        $scope.ResetButtons('Schedule', 0);
    }

    $scope.MoveToNewMessageScreen = function (){
        var temp = window.location.href.split("#");
        var url = temp[0];
        window.location.href = url + "#/Schedular";
        location.reload();
    }

    ////////////////// MEDIA UPLOADS /////////////////////////////////////////////
    $scope.OpenFileUploadModal = function () {       
        var isAlreadyMessage = false;
        if ($location.search().ScheduledMessageId != null){
            isAlreadyMessage = true;
        }
        
        $scope.items = { attachedProfiles: $scope.attachedProfiles, selected: $scope.selected, isAlreadyMessage: isAlreadyMessage, files: $scope.UploadedFiles, result: $scope.mediaData };

        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_UploadFile.html',
            controller: '_UploadFileController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {            
            console.log("selectedItem: "+selectedItem);            
            if ($scope.items.result != null) {
                $scope.UploadedFiles = $scope.items.files;
                $scope.mediaData = $scope.items.result;
                $scope.UploadedFilesCount = $scope.items.result.length;
                console.log($scope.mediaData);
            }
        }, function () {});
    }

    $scope.checkFileCount = function () {
        if (parseInt($scope.UploadedFilesCount) > 0) {
            return true;
        } else {
            return false;
        }
    }

    $scope.init();

    //End braces
}]);