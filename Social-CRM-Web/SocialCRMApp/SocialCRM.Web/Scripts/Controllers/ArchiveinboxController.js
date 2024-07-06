CRMApp.controller('ArchiveInboxController', ['$scope', '$window', '$location', 'UnifiedService', function ($scope, $window, $location, UnifiedService) {
    $scope.init = function () {
        //$scope.FbCount = 10;
        //$scope.InCount = 20;
        //$scope.TwitCount = 30;
        //$scope.AnaCount = 40;
    };
    $scope.init();

    /*$scope.profile = function () {
        UnifiedService.GetUserInbox().then(function (l) {
            if (l != null) {
                $scope.Profiles = l;
            }
            else {
            }
        }, function (err) {
         
        });
    };*/
    $scope.Profiles = [
        {
            ProfileIco: "fb_ico16",
            HandleName: " @AyeshaShamim7",
            Id: 1
        },
        {
            ProfileIco: "in_ico16",
            HandleName: " @AyeshaShamim7",
            Id: 2
        },
        {
            ProfileIco: "t_ico16",
            HandleName: " @AyeshaShamim7",
            Id: 3
        },
        {
            ProfileIco: "t_ico16",
            HandleName: " @AyeshaShamim7",
            Id: 4
        },
    ];
    $scope.msgTypes = [
        {
            msgTypelogo: "t_ico16",
            MsgType: " Mentions",
            msgTypeId: "1"
        },
          {
              msgTypelogo: "t_ico16",
              MsgType: " Direct Messages",
              msgTypeId: "2"
          },
            {
                msgTypelogo: "t_ico16",
                MsgType: " Retweets",
                msgTypeId: "3"
            },
              {
                  msgTypelogo: "t_ico16",
                  MsgType: " New Followers",
                  msgTypeId: "4"
              },
                {
                    msgTypelogo: "fb_ico16",
                    MsgType: " Wall Posts",
                    msgTypeId: "5"
                },
                {
                    msgTypelogo: "fb_ico16",
                    MsgType: " Comments",
                    msgTypeId: "6"
                }, {
                    msgTypelogo: "fb_ico16",
                    MsgType: " Private Messages",
                    msgTypeId: "7"
                }
    ];
    $scope.items = [{
        id: 0,
        label: 'Select Location'
    }, {
        id: 1,
        label: 'Lahore'
    }, {
        id: 2,
        label: 'Karachi'
    }, {
        id: 3,
        label: 'Islamabad'
    }];
    $scope.selected = $scope.items[0];
    $scope.inboxitemDetails =
        [{
            className: "unread_msg",
            twtImg: "in_ico16",
            user: "user2",
            handleName: "@AyeshaShamim7",
            userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
            replyedBy: "chocoh0llic",
            userName: "AyeshaShamim7",
            replyedDate: "Mar 08, 2016"

        },
        {
            className: "read_msg",
            twtImg: "in_ico16",
            user: "user2",
            handleName: "@AyeshaShamim7",
            userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
            replyedBy: "chocoh0llic",
            userName: "AyeshaShamim7",
            replyedDate: "Mar 08, 2016"

        },
        {
            className: "unread_msg",
            twtImg: "in_ico16",
            user: "user2",
            handleName: "@AyeshaShamim7",
            userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
            replyedBy: "chocoh0llic",
            userName: "AyeshaShamim7",
            replyedDate: "Mar 08, 2016"

        },
        {
            className: "read_msg",
            twtImg: "in_ico16",
            user: "user2",
            handleName: "@AyeshaShamim7",
            userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
            replyedBy: "chocoh0llic",
            userName: "AyeshaShamim7",
            replyedDate: "Mar 08, 2016"

        },
        {
            className: "unread_msg",
            twtImg: "in_ico16",
            user: "user2",
            handleName: "@AyeshaShamim7",
            userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
            replyedBy: "chocoh0llic",
            userName: "AyeshaShamim7",
            replyedDate: "Mar 08, 2016"

        },
        {
            className: "read_msg",
            twtImg: "in_ico16",
            user: "user2",
            handleName: "@AyeshaShamim7",
            userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
            replyedBy: "chocoh0llic",
            userName: "AyeshaShamim7",
            replyedDate: "Mar 08, 2016"

        }];
}]);