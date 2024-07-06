CRMApp.controller('_TeamReportThreadController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'TaskService', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, TaskService, items, GlobalService, notify) {

    $scope.commentsLength = {
        height: '480px',
        disableFadeOut: true
    }

   $scope.init = function () {
        $scope.ThreadList = items.ThreadList
        
        $scope.ProfileType = "";
        if ($scope.ThreadList[0].ProfileType == "FB_PROFILE" || $scope.ThreadList[0].ProfileType == "FB_PAGE") {
            $scope.ProfileType = "FB";
        }
        else {
            $scope.ProfileType = $scope.ThreadList[0].ProfileType;
        }
    };    
    
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    };

    $scope.getTimeDifference = function (PostTime) {
        var current = new Date();
        var previous = new Date(PostTime);

        var msPerMinute = 60 * 1000;
        var msPerHour = msPerMinute * 60;
        var msPerDay = msPerHour * 24;
        var msPerMonth = msPerDay * 30;
        var msPerYear = msPerDay * 365;

        var elapsed = current - previous;

        if (elapsed < msPerMinute) {
            return 'few seconds ago';
        }

        else if (elapsed < msPerHour) {
            return Math.round(elapsed / msPerMinute) + ' minutes ago';
        }

        else if (elapsed < msPerDay) {
            return Math.round(elapsed / msPerHour) + ' hours ago';
        }

        else if (elapsed < msPerMonth) {
            return 'around ' + Math.round(elapsed / msPerDay) + ' days ago';
        }

        else if (elapsed < msPerYear) {
            return 'around ' + Math.round(elapsed / msPerMonth) + ' months ago';
        }

        else {
            return 'approximately ' + Math.round(elapsed / msPerYear) + ' years ago';
        }
    }


    $scope.init();
}]);