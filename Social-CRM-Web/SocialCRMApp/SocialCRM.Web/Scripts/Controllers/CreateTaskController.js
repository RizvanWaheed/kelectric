CRMApp.controller('CreateTaskController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', '$uibModal', 'TaskService', 'UnifiedService', 'GroupService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $stateParams, $uibModal, TaskService, UnifiedService, GroupService, GlobalService, notify) {
    $scope.ActivityId = $location.search().ActivityId;
    $scope.ActivityTable = $location.search().ActivityTable;
    $scope.TaskStatus = $location.search().Status;
    $scope.VisOptions = $scope.TaskStatus == 2 ? false : true;
    $scope.HeadColorCount = 0;
    $scope.DetailColorCount = 0;
    $scope.ErrorTaskClassification = "";
    $scope.ErrorTaskPriority = "";
    $scope.ErrorTaskAssignee = "";
    $scope.ErrorTaskStatus = "";
    // if (UnifiedService.ProfileDataObj == undefined)
    //   $window.location.href = "/#/UnifiedInbox"

    //DropDowns
    $scope.taskAssigneemodel = [];
    $scope.taskStatusModel = [];
    if ($scope.TaskStatus == 3) {
        $scope.taskStatus = [
            {
                "label": "Open",
                "id": "1"
            }, {
                "label": "Close",
                "id": "2"
            }, {
                "label": "Draft",
                "id": "3"
            }
        ];
    }
    else {
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
    $scope.groupsettings = {
        selectionLimit: 1,
        smartButtonMaxItems: 2,
        smartButtonTextConverter: function (itemText, originalItem) {

            return itemText;
        }
    }
    $scope.taskClassificationModel = {};
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

    $scope.getHeadColour = function () {
        if ($scope.HeadColorCount == 0) {
            $scope.HeadColorCount = 1;
            return 'note_head_yellow';
        }
        else if ($scope.HeadColorCount == 1) {
            $scope.HeadColorCount = 2;
            return 'note_head_blue';
        }
        else if ($scope.HeadColorCount == 2) {
            $scope.HeadColorCount = 3;
            return 'note_head_green';
        }
        else if ($scope.HeadColorCount == 3) {
            $scope.HeadColorCount = 0;
            return 'note_head_red';
        }
    }
    $scope.getDetailColour = function () {
        if ($scope.DetailColorCount == 0) {
            $scope.DetailColorCount = 1;
            return 'note_color_yellow';
        }
        else if ($scope.DetailColorCount == 1) {
            $scope.DetailColorCount = 2;
            return 'note_color_blue';
        }
        else if ($scope.DetailColorCount == 2) {
            $scope.DetailColorCount = 3;
            return 'note_color_green';
        }
        else if ($scope.DetailColorCount == 3) {
            $scope.DetailColorCount = 0;
            return 'note_color_red';
        }
    }
    $scope.DelComment=function(CommentId)
    {
        //alert("removing")
        TaskService.RemoveComment(CommentId, $rootScope.selectedGroupId).then(function (l) {
            // $location.path("/Signup_Code").search({ param: $scope.Email });;
            $("#commentBlock-" + CommentId).remove();
         //   alert("Comment Deleted");
            notify({
                message: "Comment has been deleted",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
        }, function (err) {
           // alert('Error in deleting comment, please try again.');
            GlobalService.ErrorPopup("Error", "Error in deleting comment, please try again.");
            console.log(err);
        });

    }
    $scope.GetTeamDetails = function (data) {
        $scope.taskAssignee = [];
        $scope.taskAssigneemodel = [];
        GroupService.GetTeamByGroupId(data).then(function (l) {

            if (l != null) {
                var teamData = l.data.Team;

                for (i = 0; i < teamData.length; i++) {
                    var obj = { id: teamData[i].TeamMemberId, label: teamData[i].Name }
                    $scope.taskAssignee.push(obj);
                }
                /// $scope.taskAssigneemodel = $scope.taskAssignee;
            }
            else {
              //  alert('Error in fetching data, please try again.');

                GlobalService.ErrorPopup("Error", "Error in fetching team data, please try again.");
            }

            //  $scope.GetGroupDetails($scope.GroupId);

        }, function (err) {
           // alert('Error in fetching data, please try again.');

            GlobalService.ErrorPopup("Error", "Error in fetching team data, please try again.");
            console.log(err);
        });
    }
    $scope.init = function () {
        $scope.GroupId = $rootScope.selectedGroupId;

        UnifiedService.GetFeedsById($scope.ActivityId, $scope.ActivityTable).then(function (l) {
            if (l.data != null) {
                $scope.logoUrl = '/Content/img/' + l.data.AccountTypeImg + '.png';
                // alert(JSON.stringify(UnifiedService.ProfileDataObj));
                $scope.UserId = l.data[0].UserId;
                $scope.IsPage = l.data[0].IsPage;
                $scope.userImgUrl = l.data[0].TwtImg;
                $scope.userName = l.data[0].User;
                $scope.userTwitName = l.data[0].User;
                $scope.twitMsg = l.data[0].UserTwit;
                $scope.replyedBy = l.data[0].ReplyedBy;
                $scope.replyedTo = l.data[0].ToUser;
                $scope.messageType = l.data[0].MessageType;
                $scope.activityType = l.data[0].ActivityType;
                $scope.replyedDate = l.data[0].ReplyedDate;
                $scope.isClosedCss = $scope.TaskStatus == 2 && 'closed_tag' || ($scope.TaskStatus == 1 && 'open_tag' || ($scope.TaskStatus == 3 && 'draft_tag' || ''));
                $scope.twitReadCss = l.data[0].ClassName;
                $scope.logReadCss = l.data[0].ClassName;
                $scope.StatusId = l.data[0].StatusId;
            }

        }, function (err) {
            //alert('Error in fetching data, please try again.');

            GlobalService.ErrorPopup("Error", "Error in fetching feed, please try again.");
            console.log(err);
        });

        ///Reply History Info.
        $scope.totalConversation = 127;

        $scope.taskLog = [

        ];

        $scope.GetLog();
        $scope.taskComment = [];
        $scope.GetComment();
        if ($rootScope.selectedGroupId != "0" || $rootScope.IsPersonal == true)
            $scope.GetTeamDetails($scope.GroupId);

        //if ($scope.TaskStatus != 0) {
            TaskService.GetTaskClassification($scope.ActivityId, $scope.ActivityTable, $rootScope.selectedGroupId).then(function (l) {
                if (l.data != null) {
                    //  $scope.taskClassificationModel.push({ id: l.data.ClassificaitonOfTask });
                    if (l.data.ClassificaitonOfTask != "0") {
                        $scope.taskClassificationModel = { "id": "" + l.data.ClassificaitonOfTask };
                    }
                    if (l.data.TaskPriority != "0") {
                        $scope.taskPriorityModel = { "id": "" + l.data.TaskPriority };
                    }
                    if (l.data.Status != "0") {
                        $scope.taskStatusModel = { "id": "" + l.data.Status };
                    }
                    $scope.TaskStatus = l.data.Status;
                    if (l.data.Assignee != null) {
                        for (i = 0; i < l.data.Assignee.length; i++) {
                            if (l.data.Assignee[i].IsAssigned)
                                $scope.taskAssigneemodel.push({ id: l.data.Assignee[i].TeamMemberId });
                        }
                    }
                }
            }, function (err) {
               // alert('Error in fetching data, please try again.');
                GlobalService.ErrorPopup("Error", "Error in fetching case classification data, please try again.");
                console.log(err);
            });
        //}

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
    };

    $scope.openReplyAdd = function () {
        $scope.items = { UserId: $scope.UserId, IsPage: $scope.IsPage, replyedBy: $scope.replyedBy, StatusId: $scope.StatusId, MessageType: $scope.messageType, FormType: "Reply", MessageItself: $scope.twitMsg, PostDateTime: $scope.replyedDate };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_TwitterReply.html',
            controller: '_TwitterReplyController',

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
    }
    $scope.openConversation = function (StatusId, replyedBy, MessageType) {
        $scope.items = { replyedBy: replyedBy, StatusId: StatusId, MessageType: MessageType };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ProfileConversation.html',
            controller: '_ProfileConversationController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            // $scope.selected = selectedItem;
            // alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.openRetweet = function () {
        $scope.items = { UserId: $scope.UserId, IsPage: $scope.IsPage, replyedBy: $scope.replyedBy, StatusId: $scope.StatusId, MessageType: $scope.messageType, FormType: "Retweet" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_TwitterReply.html',
            controller: '_TwitterReplyController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            //  $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.openRetweetWithComment = function () {
        $scope.items = { UserId: $scope.UserId, IsPage: $scope.IsPage, replyedBy: $scope.replyedBy, StatusId: $scope.StatusId, MessageType: $scope.messageType, FormType: "RetweetComment" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_TwitterReply.html',
            controller: '_TwitterReplyController',

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
    }

    $scope.openReportAsSpam = function (StatusId, replyedBy, MessageType) {
        $scope.items = { replyedBy: replyedBy, StatusId: StatusId, MessageType: MessageType, FormType: "RetweetComment" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_ReportAsSpam.html',
            controller: '_ReportAsSpamController',

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
    }

    $scope.openEmailTweets = function () {
        var obj = {
            MessageType: $scope.messageType,
            ReplyedBy: $scope.replyedBy,
            UserTwit: $scope.twitMsg,
            ReplyedDate: $scope.replyedDate,
            UserId: $scope.UserId,
            StatusId: $scope.StatusId
        }
        $scope.items = { obj: obj };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_EmailTweets.html',
            controller: '_EmailTweetsController',

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
            //$log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.likeTweet = function () {

        $scope.items = { UserId: $scope.UserId, IsPage: $scope.IsPage, replyedBy: $scope.replyedBy, StatusId: $scope.StatusId, MessageType: $scope.messageType, FormType: "RetweetComment" };//groupId: $scope.GroupId, companyId: $scope.CompanyId
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_LikeComment.html',
            controller: '_LikeCommentController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //  $scope.selected = selectedItem;
            // alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });

    }

    $scope.TextToURLConvert = function (text, Id, MessageType) {
        // var t = tags.length;
        var text = text;
        var exp = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
        //For Instagram
        if (MessageType == "IM") {
            var compImagePath = text.split("?");
            var regexForMediaType = /(?:\.([^.]+))?$/;
            var regexResponse = regexForMediaType.exec(compImagePath[0]);
            var fileExtension = regexResponse[1];

            if (fileExtension == "jpg" || fileExtension == "png") {
                var text1 = text.replace(exp, '<img src="$1" height="294" width="294" class="img-thumbnail img_margin" style="border-radius:0%; object-fit: cover;">');
            } else if (fileExtension == "mp4") {
                var text1 = text.replace(exp, '<video controls height="294" width="294" class="img-thumbnail img_margin"><source ng-src="$1" src="$1"></video>');
            }

        } else {
            var text1 = text.replace(exp, "<a target='_blank' href='$1'>$1</a>");
        }

        if ((MessageType == "TM" || MessageType == "FCP" || MessageType == "FP" || MessageType == "TP" || MessageType == "FC")) {
            matches = text1.match(/\b\d{9}\b/g);
            var word = matches != null ? matches[0] : '';
            if (word)
                text1 = text1.replace(new RegExp("(" + preg_quote(word) + ")", 'gi'), "<b>$1</b>");
        }

        var exp2 = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
        // document.getElementById(MessageType + "-converted_url").innerHTML = text1.replace(exp2, '$1<a target="_blank" href="http://$2">$2</a>');


        $("#" + Id + "-" + MessageType + "-converted_url").html(text1.replace(exp2, '$1<a target="_blank" href="http://$2">$2</a>'))
        // return text;
    }

    $scope.GetLog = function () {
        TaskService.GetLog($scope.ActivityId).then(function (l) {
            if (l.data != null) {
                $scope.taskLog = l.data;
            }

        }, function (err) {
          //  alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching log, please try again.");
            console.log(err);
        });
    }

    $scope.GetComment = function () {
        TaskService.GetComment($scope.ActivityId).then(function (l) {
            if (l.data != null) {
                $scope.taskComment = [];
                for (i = 0; i < l.data.length; i++) {
                    var obj = { Id: l.data[i].Id, CommentTitle: l.data[i].CommentTitle, CommentDescription: l.data[i].CommentDescription, colorHead: $scope.getHeadColour(), colorDetail: $scope.getDetailColour(), PerformedBy: l.data[i].PerformedBy };
                    $scope.taskComment.push(obj);
                }
            }

        }, function (err) {
            //alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching comments, please try again.");
            console.log(err);
        });
    }

    $scope.init();

    /*$scope.GroupSelectedChanged = function (selectedGroup) {
        $scope.GetTeamDetails(selectedGroup.GroupId);
    }*/
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });

    $scope.SaveAsDraft = function () {
        var isValid = true;
        var TaskAssignee = [];
        for (i = 0; i < $scope.taskAssigneemodel.length; i++) {
            TaskAssignee.push($scope.taskAssigneemodel[i].id);
        }
        var TaskClassification = $scope.taskClassificationModel.id;
        var TaskPriority = $scope.taskPriorityModel.id;
        var TaskStatus = $scope.taskStatusModel.id;
        if (TaskClassification == undefined) {
            $scope.ErrorTaskClassification = " Please select any classification"; isValid = false;
        }
        else
            $scope.ErrorTaskClassification = "";
        if (TaskPriority == undefined) {
            $scope.ErrorTaskPriority = "Please select any Priority"; isValid = false;
        }
        else
            $scope.ErrorTaskPriority = "";
        if (TaskAssignee.length == 0) {
            $scope.ErrorTaskAssignee = "Please select any Assignee"; isValid = false;
        }
        else
            $scope.ErrorTaskAssignee = "";


        if (isValid) {
            TaskService.Create(TaskClassification, TaskPriority, 3, TaskAssignee, $scope.ActivityId, $scope.ActivityTable, $rootScope.selectedGroupId, $scope.twitMsg).then(function (l) {
                // $location.path("/Signup_Code").search({ param: $scope.Email });;
               // alert("Successfully saved as draft");
                notify({
                    message: "Sucessfully saved as draft",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }, function (err) {
                //alert('Error in saving, please try again.');
                GlobalService.ErrorPopup("Error", "Error in saving Draft, please try again.");
                console.log(err);
            });
        }
    };
    $scope.SaveExit = function () {
        var isValid = true;
        var TaskAssignee = [];
        for (i = 0; i < $scope.taskAssigneemodel.length; i++) {
            TaskAssignee.push($scope.taskAssigneemodel[i].id);
        }
        var TaskClassification = $scope.taskClassificationModel.id;
        var TaskPriority = $scope.taskPriorityModel.id;
        var TaskStatus = $scope.taskStatusModel.id;

        if (TaskClassification == undefined) {
            $scope.ErrorTaskClassification = " Please select any classification"; isValid = false;
        }
        else
            $scope.ErrorTaskClassification = "";
        if (TaskPriority == undefined) {
            $scope.ErrorTaskPriority = "Please select any Priority"; isValid = false;
        }
        else
            $scope.ErrorTaskPriority = "";
        if (TaskAssignee.length == 0) {
            $scope.ErrorTaskAssignee = "Please select any Assignee"; isValid = false;
        }
        else
            $scope.ErrorTaskAssignee = "";
        if (TaskStatus == undefined) {
            $scope.ErrorTaskStatus = "Please select any Status"; isValid = false;
        }
        else
            $scope.ErrorTaskStatus = "";

        if (isValid) {
            TaskService.Create(TaskClassification, TaskPriority, TaskStatus, TaskAssignee, $scope.ActivityId, $scope.ActivityTable, $rootScope.selectedGroupId, $scope.twitMsg).then(function (l) {
                //alert("Successfully saved");
                notify({
                    message: "Sucessfully created task",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                if (TaskStatus == 3)
                    $window.location.href = "#/DraftCase";
                else
                    $window.location.href = "#/AllCase";
            }, function (err) {
               // alert('Error in saving, please try again.');
                GlobalService.ErrorPopup("Error", "Error in case creation, please try again.");

                console.log(err);
            });
        }

    };
    $scope.Delete = function () {
        TaskService.Delete($scope.ActivityId).then(function (l) {
            // $location.path("/Signup_Code").search({ param: $scope.Email });;
           // alert("Task Deleted");
            notify({
                message: "Case deleted",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $window.location.href = "#/AllCase";
        }, function (err) {
            //alert('Error in deleting Task, please try again.');
            GlobalService.ErrorPopup("Error", "Error in deleting Case, please try again.");

            console.log(err);
        });
        
       // alert('Delete');
    };

    $scope.OpenComment = function () {
        $scope.items = {};
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_AddComments.html',
            controller: '_AddCommentsController',

            resolve: {
                items: function () {
                    return { 
                        ActivityId: $scope.ActivityId,
                        fromPage: "TC"

                    };//$scope.items;
                }
            }
        });
        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
            //alert($scope.selected)
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    };



    $scope.SendPublicReply = function () {
        var replyedText = $scope.replyedMsgModel;
        //alert('Replyed Text : ' + replyedText);
    };

    $scope.conversationHistory = [
    {
        readConversationCss: "read_msg",
        channelLogo: '/Content/images/in_ico16.png',
        userImg: '/Content/img/user2.jpg',
        creator: 'AyeshaShamim7',
        msg: 'lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p',
        replyedBy: 'chocoh0llic',
        replyedTo: 'AyeshaShamim7',
        replyedDate: 'Mar 08, 2016'
    },
    {
        readConversationCss: "unread_msg",
        channelLogo: '/Content/images/in_ico16.png',
        userImg: '/Content/img/user2.jpg',
        creator: 'AyeshaShamim7',
        msg: 'lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p',
        replyedBy: 'chocoh0llic',
        replyedTo: 'AyeshaShamim7',
        replyedDate: 'Mar 08, 2016'
    },
    {
        readConversationCss: "read_msg",
        channelLogo: '/Content/images/in_ico16.png',
        userImg: '/Content/img/user2.jpg',
        creator: 'AyeshaShamim7',
        msg: 'lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p',
        replyedBy: 'chocoh0llic',
        replyedTo: 'AyeshaShamim7',
        replyedDate: 'Mar 07, 2016'
    },
    {
        readConversationCss: "unread_msg",
        channelLogo: '/Content/images/in_ico16.png',
        userImg: '/Content/img/user2.jpg',
        creator: 'AyeshaShamim7',
        msg: 'lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p',
        replyedBy: 'chocoh0llic',
        replyedTo: 'AyeshaShamim7',
        replyedDate: 'Mar 05, 2016'
    },
    {
        readConversationCss: "read_msg",
        channelLogo: '/Content/images/in_ico16.png',
        userImg: '/Content/img/user2.jpg',
        creator: 'AyeshaShamim7',
        msg: 'lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p',
        replyedBy: 'chocoh0llic',
        replyedTo: 'AyeshaShamim7',
        replyedDate: 'Mar 06, 2016'
    }
    ];
    $scope.attachedProfiles =
        [
            {
                imgUrl: '/Content/images/soc_ico.jpg',
                userName: 'Adeel Gill',
                socialLogo: '/Content/images/in_ico16.png',
                accountName: 'adeelgill5',
                Id: '1'
            },
            {
                imgUrl: '/Content/images/soc_ico.jpg',
                userName: 'Adeel',
                socialLogo: '/Content/images/t_ico16.png',
                accountName: 'adeelgill5',
                Id: '2'
            },
            {
                imgUrl: '/Content/images/soc_ico.jpg',
                userName: 'Ayesha',
                socialLogo: '/Content/images/fb_ico16.png',
                accountName: 'adeelgill5',
                Id: '3'
            },
            {
                imgUrl: '/Content/images/soc_ico.jpg',
                userName: 'Test',
                socialLogo: '/Content/images/t_ico16.png',
                accountName: 'adeelgill5',
                Id: '4'
            },
            {
                imgUrl: '/Content/images/soc_ico.jpg',
                userName: 'Adeel Gill',
                socialLogo: '/Content/images/t_ico16.png',
                accountName: 'adeelgill5',
                Id: '5'
            },
            {
                imgUrl: '/Content/images/soc_ico.jpg',
                userName: 'Adeel Gill',
                socialLogo: '/Content/images/in_ico16.png',
                accountName: 'adeelgill5',
                Id: '6'
            }
        ];

   



    $rootScope.$on('Comment_Reload', function (evt, msg) {
        $scope.GetComment();
    });

}]);