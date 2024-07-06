CRMApp.controller('PatternCustomizationController', ['$rootScope', '$scope', '$window', '$location', 'PatternCustomizatioService', '$stateParams', '$uibModal', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, PatternCustomizatioService, $stateParams, $uibModal, GlobalService, notify){
   
   $scope.RegexFor = "";
   $scope.RegexPattern = "";
   $scope.PatternId;

   $scope.init = function () {
       $scope.fetchPattern();
   };

   $scope.Patterns = new Array();
   $scope.PatternVal = 1;
   $scope.SelectedPattern = 0;

    $scope.fetchPattern = function (){
        PatternCustomizatioService.FetchConsumerPattern().then(function (l) {
            if (l) {
                $scope.Patterns = l.data;
                $scope.SelectedPattern = $.grep($scope.Patterns, function (record) {
                    return record.IsActive == 1;
                })[0].Id;
            }
        }, function (err) {
            // alert('Error in saving, please try again.');
            GlobalService.ErrorPopup("Error", "Error in saving, please try again.");
            console.log(err);
        });
    };

    $scope.Save = function () {        

        PatternCustomizatioService.UpdateActivePattern($scope.SelectedPattern).then(function (l) {
            if (l.data) {
                notify({
                    message: "Pattern save successfully.",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            } else {
                notify({
                    message: "Pattern save failed.",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }
        }, function (err) {
           // alert('Error in update, please try again.');
            GlobalService.ErrorPopup("Error", "Error in update, please try again.");

            console.log(err);
        });
        //$scope.groupCount = $scope.Submit_groups.length; //to find number of groups
        //for (i = 0; i < $scope.groupCount; i++)
        //{ alert($scope.Submit_groups[i].email); }
       // alert("saved")
        //send Changed_BusinessName and Submit_groups to save in database.
    }

    $scope.init();

    $scope.SelectedPatternChange = function (id) { 
        $scope.SelectedPattern = id;
    };

    $scope.EditPattern = function (obj) {
        $scope.items = { RegexFor: obj.RegexFor, Pattern: obj.Pattern, Id: obj.Id, Type: "Edit"};
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_EditPattern.html',
            controller: '_EditPatternController',
            scope: $scope,
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
    };

    $scope.AddPattern = function () {
        $scope.items = { RegexFor: "", Pattern: "", Id: 0, Type: "Add" };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_EditPattern.html',
            controller: '_EditPatternController',
            scope: $scope,
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

    $scope.DisablePattern = function () {
        PatternCustomizatioService.DisablePattern().then(function (l) {
            if (l.data) {
                notify({
                    message: "Pattern disabled successfully.",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            } else {
                notify({
                    message: "Pattern disabled failed.",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
            }
        }, function (err) {
            // alert('Error in update, please try again.');
            GlobalService.ErrorPopup("Error", "Error in update, please try again.");
            console.log(err);
        });
    }
}]);