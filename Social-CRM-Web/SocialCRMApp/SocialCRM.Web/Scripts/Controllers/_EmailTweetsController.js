CRMApp.controller('_EmailTweetsController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'CompanyService', 'UnifiedService', 'items', 'TwitterService', 'GlobalService','notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, CompanyService, UnifiedService, items, TwitterService, GlobalService, notify) {


    $scope.obj = items.obj;
    $scope.FromUser = jQuery.parseJSON($rootScope.currentUser).FirstName + ' ' + jQuery.parseJSON($rootScope.currentUser).LastName;
    $scope.attachedProfiles = [];

    $scope.EmailTweets = function () {

        CompanyService.SendEmail($scope.ToUserEmail, $scope.EmailSubject, $scope.EmailBody).then(function (l) {
            notify({
                message: "Email sent successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $scope.cancel();
           // alert("Email sent successfully ")
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in sending email, please try again.");

        });
    };

    $scope.getHistory = function () {
        //Follower details
        if ($scope.obj.MessageType == "FP")
            $scope.EmailBody = $scope.obj.ReplyedBy + " said:" + "\n\n" + $scope.obj.UserTwit + "\n\n" + $scope.obj.ReplyedDate + "\n\n" + "http://www.facebook.com/" + $scope.obj.UserId + "/posts/" + $scope.obj.StatusId.split('_')[1] + "\n\n Sent from Social CRM";
        else if($scope.obj.MessageType == "FC")
            $scope.EmailBody = $scope.obj.ReplyedBy + " said:" + "\n\n" + $scope.obj.UserTwit + "\n\n" + $scope.obj.ReplyedDate + "\n\n" + "http://www.facebook.com/" + $scope.obj.UserId + "/posts/" + $scope.obj.StatusId.split('|')[1].split('_')[1] + "\n\n Sent from Social CRM";
        else if ($scope.obj.MessageType == "FCP")
            $scope.EmailBody = $scope.obj.ReplyedBy + " said:" + "\n\n" + $scope.obj.UserTwit + "\n\n" + $scope.obj.ReplyedDate + "\n\n" + "http://www.facebook.com/" + $scope.obj.UserId + "/messages/" + "\n\n Sent from Social CRM";
        else
            $scope.EmailBody = $scope.obj.ReplyedBy + " said:" + "\n\n" + $scope.obj.UserTwit + "\n\n" + $scope.obj.ReplyedDate + "\n\n" + "https://twitter.com/" + $scope.obj.ReplyedBy + "/status/" + $scope.obj.StatusId + "\n\n Sent from Social CRM";

        //UnifiedService.getTwitterHistory("1", $scope.ofUser, 500, 0, $window.localStorage.getItem('selectedGroupId'), $window.localStorage.getItem('selectedCompanyId')).then(function (l) {
        //   // $scope.conversationHistory.push.apply($scope.inboxHistoryItemDetails, l.data.tweets);
            
        //    //console.log("Contact History Tweets fetched Successfully");
        //}, function (err) {
        //    alert('Error in fetching contact history tweets, please try again.');
        //    console.log(err);
        //});
    };
    $scope.getHistory();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
}]);

//@chocoh0llic said:

//    "@AyeshaShamim7 lol, remember we met at CP, you were stuck in a confidential task and I was playing batman there :p"

//Mar 08, 2016 1:05 pm

//https://twitter.com/chocoh0llic/status/707114904512290816 

//    --

//    Sent from Sprout Social (http://sproutsocial.com)