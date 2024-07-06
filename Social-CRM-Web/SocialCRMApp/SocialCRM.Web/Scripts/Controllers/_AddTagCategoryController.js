CRMApp.controller('_AddTagCategoryController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', 'TaskService', 'items', 'UnifiedService', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, TaskService, items, UnifiedService, GlobalService, notify) {
    $scope.CategoryTitle = "";
    $scope.isEditMode = false;
    $scope.BGColor = "#6E889C";
    $scope.FontColor = "#ffffff";

       $('#colorpalette12').colorPalette().on('selectColor', function(e) {
           $('#selected-color12 i').css('color', e.color);
           $scope.FontColor = e.color;
     });

    $('#colorpalette13').colorPalette().on('selectColor', function (e) {
        $('#selected-color12 i').css('background-color', e.color);
        $scope.BGColor = e.color;
    });
    $scope.setColor = function (type,color) {
        if(type == "back")
        {
            $scope.BGColor = color;
        }
        else
        {
            $scope.FontColor = color;
        }
    };
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
    $scope.getTagsCategoryByCompanyId = function (companyId) {
        TaskService.GetBrandKeywordCategoryByCompanyId(companyId).then(function (l) {
            var response = l.data;
            $scope.BrandCategoryTypes = [];
            for (var i = 0; i < response.length; i++) {
                $scope.BrandCategoryTypes.push({
                    BrandKeywordCategoryText: response[i].BrandKeywordCategoryText, BrandKeywordCategoryId: response[i].Id, FontColor: (response[i].FontColor == null ? '#ffffff' : response[i].FontColor), BGColor: (response[i].BGColor == null ? '#6e889c' : response[i].BGColor)
                });
            }
            $scope.TagCategory = "";
        }, function (err) {
            alert('Error in fetching brands');
        });
    }
    $scope.AddNewCategory = function () {
        var CategoryTitle = $scope.CategoryTitle;
        var CategoryId = $scope.CategoryId;
        var groupId = $rootScope.selectedGroupId;
        if (!$scope.isEditMode) {
            TaskService.AddBrandKeywordCategory(CategoryTitle, $rootScope.selectedGroupId, $scope.FontColor, $scope.BGColor).then(function (l) {
                notify({
                    message: "Category added successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $scope.CategoryTitle = "";
                $scope.getTagsCategoryByCompanyId($rootScope.selectedCompanyId);
                $rootScope.$broadcast('TagCategory_Reload', 1);
                //  $uibModalInstance.close('cancel');
            }, function (err) {

                GlobalService.ErrorPopup("Error", "Error in saving comment, please try again.");

            });

        }
        else {


            TaskService.EditBrandKeywordCategory(CategoryId, CategoryTitle, $rootScope.selectedGroupId, $scope.FontColor, $scope.BGColor).then(function (l) {
                notify({
                    message: "Category added successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });
                $scope.CategoryTitle = "";
                document.getElementById("colorPickerLink1").style.color = "#ffffff";
                document.getElementById("colorPickerLink1").style.backgroundColor = "#6E889C";
                $scope.isEditMode = false;
                $scope.getTagsCategoryByCompanyId($rootScope.selectedCompanyId);
                $rootScope.$broadcast('TagCategory_Reload', 1);
                //    $uibModalInstance.close('cancel');
            }, function (err) {

                GlobalService.ErrorPopup("Error", "Error in saving comment, please try again.");

            });
        }

    }


    $scope.EditTag = function (brnd) {

        $scope.isEditMode = true;
        $scope.CategoryId = brnd.BrandKeywordCategoryId;
        $scope.CategoryTitle = brnd.BrandKeywordCategoryText;
        $scope.BGColor = brnd.BGColor;
        $scope.FontColor = brnd.FontColor;
        document.getElementById("colorPickerLink1").style.color = brnd.FontColor;
        document.getElementById("colorPickerLink1").style.backgroundColor = brnd.BGColor;

    }
    $scope.deleteTag = function (id) {
        GlobalService.ConfirmPopup("Confirm", "Do you want to delete this category?", function (selectedItem) {
            if (selectedItem) {
                TaskService.RemoveBrandKeywordCategory(id).then(function (l) {
                    console.log("Category Deleted Successfully");
                    if (l.data == false) {
                        notify({
                            message: "Cannot delete because Category is in use.",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                    }
                    else {
                        notify({
                            message: "Category deleted successfully",
                            position: GlobalService.position,
                            duration: GlobalService.duration
                        });
                        $scope.getTagsCategoryByCompanyId($rootScope.selectedCompanyId);

                    }

                }, function (err) {
                    GlobalService.ErrorPopup("Error", "Error in Deleting Tag, please try again.");
                    console.log(err);
                });
            }
        });
    }

 

    $scope.getTagsCategoryByCompanyId($rootScope.selectedCompanyId);


}]);