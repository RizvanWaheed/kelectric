CRMApp.controller('ArchiveContactController', ['$scope', '$window', '$location', 'UnifiedService', function ($scope, $window, $location, UnifiedService) {
    $scope.init = function () {
        $scope.contactName = "TALHA";
        $scope.contactTwitName = "chocOholic";
        $scope.contactMsg = "Remember we met at CP, you were stuck in a confidential task and I was playing batman there :p";
        $scope.contactCountry = "PAKISTAN";
        $scope.contactURL = " LINKEDIN.COM/IN/TALHAASLAM";
        $scope.totalFollower = 55;
        $scope.totalFollowing = 10;

    };
    $scope.updateVar = function (event) {
        $scope.myVar = angular.element(event.target).text();
    };
    $scope.init();
    $scope.inboxHistoryItemDetails = [
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 08, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 09, 2016"
    },
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 01, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 02, 2016"
    }];
    //Tweet Items
    $scope.tweetItemDetails = [
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 08, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 09, 2016"
    },
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 01, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 02, 2016"
    }];
    //Mentions
    $scope.mentionItemDetails = [
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 08, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 09, 2016"
    },
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 01, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 02, 2016"
    }];
    //Follower details
    $scope.followerItemDetails = [
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 08, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 09, 2016"
    },
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 01, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 02, 2016"
    }];
    //Following Details
    $scope.followingItemDetails = [
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 08, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 09, 2016"
    },
    {
        className: "unread_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 01, 2016"
    },
    {
        className: "read_msg",
        twtImg: "in_ico16",
        user: "user2",
        handleName: "@AyeshaShamim7",
        userTwit: "lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p",
        replyedBy: "chocoh0llic",
        replyedTo: "AyeshaShamim7",
        twitDate: "Mar 02, 2016"
    }];
    //Contact Information
    $scope.ContactInformation = function () {
        $scope.ContactInformation.CompanyName = "IbexGlobal";
        $scope.ContactInformation.Designation = "Software";
        $scope.ContactInformation.UserEmail = "ayesha.shamim@ibexglobal.com";
        $scope.ContactInformation.UserContactNumber = " +92-345-589-7856";
        $scope.ContactInformation.UserPhoneNumber = "+92-51-456-9827";
    };
    $scope.ContactInformation();
    //Twitter List
    $scope.TwitterAccounts =
        [
            {
                ListName: "Sample List1",
                Owner: "Ayesha"
            },
            {
                ListName: "Sample 2",
                Owner: "Ayesha"
            },
            {
                ListName: "Sample 3",
                Owner: "Ayesha"
            }
        ];
    //Comments
    $scope.Comments = [
        {
            commentTitle: "Sample Comment 1",
            commentText: "samksdf skldfj sdfkjsdf sdkfjsdf slkdjlsdkfjsdf jsdf adfsdf",
            commentUser: "Asad",
            commentDate: "3:45PM 1 April 2016"
        },
         {
             commentTitle: "Sample Comment 2",
             commentText: "samksdf skldfj sdfkjsdf sdkfjsdf slkdjlsdkfjsdf jsdf adfsdf",
             commentUser: "Asad",
             commentDate: "3:45PM 4 April 2016"
         },
          {
              commentTitle: "Sample Comment 3",
              commentText: "samksdf skldfj sdfkjsdf sdkfjsdf slkdjlsdkfjsdf jsdf adfsdf",
              commentUser: "Asad",
              commentDate: "3:45PM 3 April 2016"
          },
           {
               commentTitle: "Sample Comment 4",
               commentText: "samksdf skldfj sdfkjsdf sdkfjsdf slkdjlsdkfjsdf jsdf adfsdf",
               commentUser: "Asad",
               commentDate: "3:45PM 5 April 2016"
           },
            {
                commentTitle: "Sample Comment 5",
                commentText: "samksdf skldfj sdfkjsdf sdkfjsdf slkdjlsdkfjsdf jsdf adfsdf",
                commentUser: "Asad",
                commentDate: "3:45PM 6 April 2016"
            }
    ];
    /*$scope.Comments = function () {
        $scope.ContactInformation.CompanyName = "IbexGlobal";
        $scope.ContactInformation.Designation = "Software";
        $scope.ContactInformation.UserEmail = "ayesha.shamim@ibexglobal.com";
        $scope.ContactInformation.UserContactNumber = " +92-345-589-7856";
        $scope.ContactInformation.UserPhoneNumber = "+92-51-456-9827";
    };
    $scope.Comments ();*/
}]);