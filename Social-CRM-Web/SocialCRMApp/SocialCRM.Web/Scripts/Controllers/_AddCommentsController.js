CRMApp.controller('_AddCommentsController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'TaskService', 'items', 'UnifiedService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, TaskService, items, UnifiedService, GlobalService, notify) {
    $scope.CommentTitle = "";
    $scope.CommentDescription = "";
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.AddNewComment = function () {
        var comments = $scope.CommentDescription;
        var commentstitle = $scope.CommentTitle;
        var groupId = $rootScope.selectedGroupId;
        var companyId = $rootScope.selectedCompanyId;

        if (items.fromPage == "TC") {
            TaskService.AddComment(items.ActivityId, commentstitle, comments, $rootScope.selectedGroupId).then(function (l) {
                notify({
                    message: "Comment added successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                //alert("Successfully saved ");
                $rootScope.$broadcast('Comment_Reload', 1);
                $uibModalInstance.close('cancel');
            }, function (err) {

                GlobalService.ErrorPopup("Error", "Error in saving comment, please try again.");
                
            });
        }
        else if (items.fromPage == "CC") {
            UnifiedService.InsertUserComment(items.contactExternalId, items.ofUser, companyId, groupId, commentstitle, comments).then(function (l) {
                $uibModalInstance.close(l.data);
               // console.log("Contact Comment inserted Successfully");
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in inserting contact comment, please try again.");

            });
        }
    }
}]);