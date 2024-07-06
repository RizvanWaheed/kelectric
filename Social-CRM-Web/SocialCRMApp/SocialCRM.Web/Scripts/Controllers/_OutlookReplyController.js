CRMApp.controller('_OutlookReplyController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', '$sce', 'CompanyService', 'UnifiedService', 'items', 'TwitterService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, $sce, CompanyService, UnifiedService, items, TwitterService, GlobalService, notify) {

    $scope.trustedHtml = function (str) {
        return $sce.trustAsHtml(str);
    };

    $scope.obj = items.obj;
    $scope.type = items.type;
    $scope.From = items.userEmail;
    $scope.To = [];
    $scope.CC = [];
    $scope.Body = "";//$scope.trustedHtml($scope.obj.Full_Text);

    if ($scope.type == 'ReplyTo') {
        $scope.To.push($scope.obj.From);
        $scope.Subject = "Re: " + $scope.obj.Subject;
        $scope.CC = "";
    }

    if ($scope.type == 'ReplyToAll') {
        //$scope.To = $scope.obj.To;
        //$scope.To.pop($scope.From);
        //$scope.To.push($scope.obj.From);
        $scope.CC = "";

        $scope.To = $scope.obj.From;
        for (var j = 0; j < $scope.obj.To.length; j++) {
            if ($scope.obj.To[j] != $scope.From)
                $scope.To = $scope.To + "; " + $scope.obj.To[j];
        }

        for (var k = 0; k < $scope.obj.CC.length; k++) {
            $scope.CC = $scope.CC + $scope.obj.CC[k] + "; ";
        }
        $scope.Subject = "Re: " + $scope.obj.Subject;
    }

    if ($scope.type == 'Fwd') {
        $scope.Subject = "Fw: " + $scope.obj.Subject;
        $scope.CC = "";
    }

    $scope.init = function () {
        $scope.getEmailContent();
    };

    $scope.sendEmailReply = function () {

        CompanyService.ReplyToEmail($scope.From, $scope.To, $scope.Subject, $scope.Body, $scope.CC).then(function (l) {
            notify({
                message: "Email sent successfully",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            // alert("Email sent successfully ")
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in sending email, please try again.");

        });
    };

    $scope.getEmailContent = function () {
        if ($scope.type == 'Fwd')
            $scope.Body = "<br>" + $scope.obj.From + " wrote on " + $scope.obj.Date + ":<br/> <br/>" + $scope.obj.Full_Text + "<br/> Sent from Social CRM";
        else
            $scope.Body = ""; //"\n \n \n \n \n \n \n \n \n" + "Sent from Social CRM";
    };

    $scope.init();

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };


}]);

//angular.module("CRMApp").directive('compileTemplate', ["$compile", "$parse", function ($compile, $parse) {
//    return {
//        restrict: 'A',
//        link: function ($scope, element, attr) {
//            var parse = $parse(attr.ngBindHtml);
//            function value() { return (parse($scope) || '').toString(); }

//            $scope.$watch(value, function () {
//                $compile(element, null, -9999)($scope);
//            });
//        }
//    }
//}]);

//    CRMApp.directive('compile', function ($compile, $timeout) {
//    return {
//        restrict: 'A',
//        link: function (scope, elem, attrs) {
//            $timeout(function () {
//                $compile(elem.contents())(scope);
//            });
//        }
//    };
//});


