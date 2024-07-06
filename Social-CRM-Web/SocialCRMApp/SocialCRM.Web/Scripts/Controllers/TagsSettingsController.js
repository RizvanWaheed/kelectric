CRMApp.controller('TagsSettingsController', ['$rootScope', '$scope', '$sce', '$window', '$location', 'UnifiedService', 'UnifiedDeckService', '$stateParams', '$uibModal', 'TwitterService', 'FacebookService', 'TaskService', 'GlobalService', 'notify', 'GeneralSettingsService', function ($rootScope, $scope, $sce, $window, $location, UnifiedService, UnifiedDeckService, $stateParams, $uibModal, TwitterService, FacebookService, TaskService, GlobalService, notify, GeneralSettingsService) {
    $scope.EditId = 0;
    $scope.TagText = "";
    $scope.TagName = "";
    $scope.BGColor = "#6E889C";
    $scope.FontColor = "#ffffff";
    $scope.sampleReplies = [];
    $scope.replyTextModel = "";
    $scope.addNewReplyTextModel = "";
    $scope.isEditMode = false;

    $scope.getTagsByCompanyId = function (companyId) {
        TaskService.GetBrandKeywordByCompanyId(companyId).then(function (l) {
            var response = l.data;
            $scope.BrandsTypes = [];
            $scope.TagsTypes = [];
            for (var i = 0; i < response.length; i++) {
                if (response[i].IsTag)
                    $scope.TagsTypes.push({ BrandType: response[i].KeywordText, BrandName: response[i].KeywordName, BrandTypeId: response[i].Id, FontColor: (response[i].FontColor == null ? '#ffffff' : response[i].FontColor), BGColor: (response[i].BGColor == null ? '#6e889c' : response[i].BGColor), BrandKeywordCategoryId: response[i].BrandKeywordCategoryId });
                else
                    $scope.BrandsTypes.push({ BrandType: response[i].KeywordText, BrandName: response[i].KeywordName, BrandTypeId: response[i].Id, LocationName: response[i].LocationName, BrandKeywordCategoryId: response[i].BrandKeywordCategoryId });
            }
            console.log($scope.TagsTypes);
        }, function (err) {
            alert('Error in fetching brands');
        });
    }
    $scope.getTagsCategoryByCompanyId = function (companyId) {
        TaskService.GetBrandKeywordCategoryByCompanyId(companyId).then(function (l) {
            var response = l.data;
            $scope.BrandCategoryTypes = [];
            for (var i = 0; i < response.length; i++) {
                $scope.BrandCategoryTypes.push({ BrandKeywordCategoryText: response[i].BrandKeywordCategoryText, BrandKeywordCategoryId: response[i].Id });
            }
            $scope.TagCategory = "";
        }, function (err) {
            alert('Error in fetching brands');
        });
    }
    $scope.GetSampleReplies = function () {
        UnifiedDeckService.GetSampleRepliesText().then(function (result) {
            if (result.statusText == "OK") {
                $scope.sampleReplies = [];
                angular.forEach(result.data, function (obj) {
                    $scope.sampleReplies.push(obj);
                });
            }
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }

    $scope.init = function () {
        $scope.getTagsByCompanyId($rootScope.selectedCompanyId);
        $scope.getTagsCategoryByCompanyId($rootScope.selectedCompanyId);
        $scope.GetSampleReplies();
    }

    $('#colorpalette2').colorPalette()
    .on('selectColor', function (e) {
        $('#selected-color2 i').css('color', e.color);
        $scope.FontColor = e.color;
    });

    $('#colorpalette3').colorPalette()
    .on('selectColor', function (e) {
        $('#selected-color2 i').css('background-color', e.color);
        $scope.BGColor = e.color;
    });

    //$scope.clickFont = function () {
    //    alert('clickFont');
    //    //$('#colorpalette2').colorPalette()
    //    //.on('selectColor', function (e) {
    //    //    $('#selected-color2 i').css('color', e.color);
    //    //    $scope.FontColor = e.color;
    //    //});
    //}

    //$scope.clickBG = function () {
    //    alert('clickBG');
    //  //  $('#colorpalette3').colorPalette()
    //  //.on('selectColor', function (e) {
    //  //    $('#selected-color2 i').css('background-color', e.color);
    //  //    $scope.BGColor = e.color;
    //  //});
    //}

    $scope.AddBrandTag = function () {
        if (!$scope.isEditMode) {
            TaskService.AddBrandKeyword($scope.TagText, $scope.TagName, 0, 0, 0, $rootScope.selectedGroupId, true, $scope.FontColor, $scope.BGColor, $scope.TagCategory).then(function (l) {
                $scope.TagsTypes.push({ BrandType: $scope.TagText, BrandName: $scope.TagName, BrandTypeId: l.data, FontColor: $scope.FontColor, BGColor: $scope.BGColor });
                $scope.TagText = "";
                $scope.TagName = "";
                $scope.TagCategory = "";
                console.log("Tag added successfully");
                notify({
                    message: "Tag added successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

                $scope.getTagsByCompanyId($rootScope.selectedCompanyId);
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.ExceptionMessage);
                //  alert('Error in Adding Brand Tag, please try again.');
                console.log(err);
            });
        } else if ($scope.isEditMode) {
            TaskService.EditBrandKeyword($scope.EditId, $scope.TagText, $scope.TagName, 0, 0, 0, $rootScope.selectedGroupId, true, $scope.FontColor, $scope.BGColor, $scope.TagCategory).then(function (l) {

                angular.forEach($scope.TagsTypes, function (obj) {
                    if (obj.BrandTypeId == $scope.EditId) {
                        obj.BrandName = $scope.TagName;
                        obj.FontColor = $scope.FontColor;
                        obj.BGColor = $scope.BGColor;

                    }
                });
                $scope.TagText = "";
                $scope.TagName = "";
                $scope.TagCategory = "";
                document.getElementById("colorPickerLink").style.color = "#ffffff";
                document.getElementById("colorPickerLink").style.backgroundColor = "#6E889C";

                console.log("Tag updated successfully");
                notify({
                    message: "Tag updated successfully",
                    position: GlobalService.position,
                    duration: GlobalService.duration
                });

                $scope.getTagsByCompanyId($rootScope.selectedCompanyId);
                $scope.isEditMode = false;
            }, function (err) {
                GlobalService.ErrorPopup("Error", err.data.ExceptionMessage);
                //  alert('Error in Adding Brand Tag, please try again.');
                console.log(err);
            });
        }


    }

    $scope.deleteTag = function (id) {
        GlobalService.ConfirmPopup("Confirm", "Do you want to delete this tag?", function (selectedItem) {
            if (selectedItem) {
                UnifiedService.DeleteTag(id).then(function (l) {
                    console.log("Tag Deleted Successfully");
                    notify({
                        message: "Tag deleted successfully",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                    $scope.init();
                }, function (err) {
                    GlobalService.ErrorPopup("Error", "Error in Deleting Tag, please try again.");
                    console.log(err);
                });
            }
        });
    }

    $scope.replyBtnClick = function (btnName, index, replyText) {
        debugger
        if (btnName == "Edit") {
            $("#" + btnName + "-" + index).hide();
            $("#" + btnName + "-" + index).siblings('.delete').hide();
            $("#" + btnName + "-" + index).siblings('.save, .cancel').show();

            $("#replyText-" + index).show();
            $("#replyTextOriginal-" + index).hide();
        } else if (btnName == "Delete") {

            GeneralSettingsService.DeleteSampleReply(index).then(function (result) {
                if (result.statusText == "OK") {
                    $scope.GetSampleReplies();
                    notify({
                        message: "Tag '" + replyText + "' deleted successfully.",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in deleting data, please try again.");
                console.log(err);
            });
        } else if (btnName == "Save") {
            //Visual Changes
            $("#" + btnName + "-" + index).siblings('.edit').show();
            $("#" + btnName + "-" + index).siblings('.delete').show();
            $("#" + btnName + "-" + index).siblings('.cancel').hide();
            $("#" + btnName + "-" + index).hide();

            $("#replyText-" + index).hide();
            $("#replyTextOriginal-" + index).show();

            //Action
            GeneralSettingsService.UpdateSampleReply(index, replyText).then(function (result) {
                if (result.statusText == "OK") {
                    $scope.GetSampleReplies();
                    notify({
                        message: "Sample reply updated successfully.",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
                console.log(err);
            });

        } else if (btnName == "Cancel") {
            $("#" + btnName + "-" + index).siblings('.edit').show();
            $("#" + btnName + "-" + index).siblings('.delete').show();
            $("#" + btnName + "-" + index).siblings('.save').hide();
            $("#" + btnName + "-" + index).hide();

            $("#replyText-" + index).hide();
            $("#replyTextOriginal-" + index).show();
        } else if (btnName == "Add") {
            //Add new sample reply
            GeneralSettingsService.UpdateSampleReply(index, replyText).then(function (result) {
                if (result.statusText == "OK") {
                    $scope.GetSampleReplies();
                    $scope.addNewReplyTextModel = "";
                    notify({
                        message: "Sample reply added successfully.",
                        position: GlobalService.position,
                        duration: GlobalService.duration
                    });
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
                console.log(err);
            });
        }
    }

    $scope.EditTag = function (brnd) {
        debugger;
        $scope.isEditMode = true;
        $scope.EditId = brnd.BrandTypeId;

        if (brnd.BrandName != '')
            $scope.TagText = brnd.BrandName;
        else
            $scope.TagText = brnd.BrandType;

        $scope.TagName = brnd.BrandName;
        $scope.BGColor = brnd.BGColor;
        $scope.FontColor = brnd.FontColor;
        $scope.TagCategory = parseInt(brnd.BrandKeywordCategoryId);
        document.getElementById("colorPickerLink").style.color = brnd.FontColor;
        document.getElementById("colorPickerLink").style.backgroundColor = brnd.BGColor;

    }
    $scope.AddCategory = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_AddTagCategory.html',
            controller: '_AddTagCategoryController',

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
    $scope.init();

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });

    $rootScope.$on('TagCategory_Reload', function (evt, msg) {
        $scope.getTagsCategoryByCompanyId($rootScope.selectedCompanyId);
    });
}]);