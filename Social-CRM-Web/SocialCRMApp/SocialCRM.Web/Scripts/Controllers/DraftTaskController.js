CRMApp.controller('DraftTaskController', ['$scope', '$window', '$location', 'TaskService', '$stateParams', 'GroupService', 'ProfileService', '$rootScope', 'GlobalService', 'notify', function ($scope, $window, $location, TaskService, $stateParams, GroupService, ProfileService, $rootScope, GlobalService, notify) {
    $scope.recentDraftTaskId = 0;
    $scope.init = function () {
        $scope.GroupId = $rootScope.selectedGroupId;
        $scope.PageType = $stateParams.pageType;//DraftTasks
        $scope.Search();
    };
    $scope.Search = function () {
        TaskService.GetFilteredDraftTask($scope.GroupId).then(function (l) {
            $scope.taskList = l.data;
            if ($scope.taskList.length > 0)
                $scope.recentDraftTaskId = Math.max.apply(Math, $scope.taskList.map(function (o) { return o.TaskId; })) > $scope.recentDraftTaskId ? Math.max.apply(Math, $scope.taskList.map(function (o) { return o.TaskId; })) : $scope.recentDraftTaskId;
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Draft Cases, please try again.");
            console.log(err);
        });
    }
    $scope.getLatestDraftTasks = function () {
        TaskService.GetFilteredDraftTask($scope.GroupId).then(function (l) {
            var response = l.data;
            var data = $.grep(response, function (record) {
                return record.TaskId > $scope.recentDraftTaskId
            });

            data.forEach(function (i) {
                i.CssClass = "new_msg";
            });
            Array.prototype.push.apply(data, $scope.taskList)

            $scope.taskList = data;

            if ($scope.taskList.length > 0)
                $scope.recentDraftTaskId = Math.max.apply(Math, $scope.taskList.map(function (o) { return o.TaskId; })) > $scope.recentDraftTaskId ? Math.max.apply(Math, $scope.taskList.map(function (o) { return o.TaskId; })) : $scope.recentDraftTaskId;

        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching Draft Cases, please try again.");
            console.log(err);
        });
    }

    $scope.init();
    $scope.replaceTilda = function (label) {
        // str.replace("Microsoft", "W3Schools")
        return label.replace('~', '');
    };
    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });
}]);