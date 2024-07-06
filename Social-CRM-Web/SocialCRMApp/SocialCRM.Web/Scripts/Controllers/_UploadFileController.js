CRMApp.controller('_UploadFileController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'items', 'PublisherService', 'GlobalService', 'notify', 'FileUploader', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, items, PublisherService, GlobalService, notify, FileUploader) {
    
    $scope.uploadLength = {
        height: '150px',//350
        width: '100%',
        disableFadeOut: true
    }
    //$scope.UploadedFiles = new Array();
    $scope.notValidated = true;    
    //$scope.mediaDataModal = items.result;
    $scope.mediaDataModal = new Array();
    $scope.previouslyUploadedData = items.result;
    $scope.UploadedFiles = items.files;
    $scope.isAlreadyMessage = items.isAlreadyMessage;

    $scope.init = function() {
        $scope.ModalHeading = "Upload File";
        $scope.attachedProfiles = items.attachedProfiles;
        $scope.selected = items.selected;
        uploader.queue = items.files;

        $scope.TempSavingOfMediaData = new Array();
        angular.forEach(items.result, function (obj) {
            $scope.TempSavingOfMediaData.push(obj);
        });
        $scope.TempSavingOfMediaData.reverse();

        angular.forEach($scope.previouslyUploadedData, function (obj) {
            if (obj.Base64String.includes(",")) {
                obj.Base64String = obj.Base64String.split(",")[1];
            }

        });
    }

    ////////////////// MEDIA UPLOADS /////////////////////////////////////////////    
    var uploader = $scope.uploader = new FileUploader({
        url: ''
    });

    uploader.filters.push({
        name: 'customFilter',
        fn: function (item /*{File|FileLikeObject}*/, options) {
            //$scope.UploadedFiles = this.queue;
            //$scope.ValidateFiles();
            return this.queue.length < 10;
        }
    });

    uploader.removeFromQueue = function (e) {
        var t = this.getIndexOfItem(e), o = this.queue[t];
        o.isUploading && o.cancel(), this.queue.splice(t, 1), o._destroy(), this.progress = this._getTotalProgress();
        $scope.UploadedFiles = this.queue;
        $scope.ValidateFiles();
    };

    uploader.onAfterAddingAll = function (addedFileItems) {
        $scope.ValidateFiles();
        if($scope.notValidated == false)
            $scope.UploadedFiles = this.queue;
    };    

    $scope.ValidateFiles = function () {
        $scope.errorMsg = "";

        var allowedExtentions = ["png", "jpg", "jpeg", "gif", "webp", "mov", "mp4"]
        var allowedFbExtentions = ["png", "jpg", "jpeg"];
        var allowedTWImgExtentions = ["png", "jpg", "jpeg", "webp"];
        var allowedTWVideoExtentions = ["mov", "mp4"];
        $scope.notValidated = false;
        $scope.count = 0;
       
        debugger

        $scope.selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.CompositeId];
        });

        if ($scope.selectedProfiles.length == 0) {
            $scope.notValidated = true;
            $scope.errorMsg = "Please select any profile";
            return false;
        }
        var fbProfileSelected = false, twProfileSelected = false;

        var twProfiles = $.grep($scope.selectedProfiles, function (record) {
            return record.ProfileTypeId == 4;
        });

        twProfileSelected = twProfiles.length > 0

        var fbProfiles = $.grep($scope.selectedProfiles, function (record) {
            return record.ProfileTypeId == 1;
        });

        fbProfileSelected = fbProfiles.length > 0;

        var uploadedFilesExtentions = new Array();
        $scope.UploadedFiles.forEach(function (i) {
            uploadedFilesExtentions.push(i.file.type.split('/')[1]);
        });

        uploadedFilesExtentions.forEach(function (i) {
            if (!allowedExtentions.includes(i)) {
                $scope.notValidated = true;
            }
        });

        if ($scope.notValidated) {
            $scope.errorMsg = "Invalid Extention uploaded";
            return;
        }

        var videos = $scope.UploadedFiles.filter(function (record) { return record._file.type.split('/')[1] == "mp4" || record._file.type.split('/')[1] == "mov"; })

        if (videos.length > 0) {
            videos.forEach(function (i) {
                if (i._file.size / 1024 / 1024 > 8) {
                    errorMsg = "Max file limit for video is 8 MB.";
                    $scope.notValidated = true;
                }
            });
            if ($scope.notValidated) {
                return;
            }
        }

        var images = $scope.UploadedFiles.filter(function (record) {
            return record._file.type.split('/')[1] != "mp4" && record._file.type.split('/')[1] != "mov";
        });

        if (images.length > 0) {
            var previouslyUploadedSize = 0.0;
            var currentlyUploadedSize = 0.0;
            if ($scope.previouslyUploadedData.length > 0) {
                angular.forEach($scope.previouslyUploadedData, function (obj) {
                    previouslyUploadedSize = previouslyUploadedSize + parseFloat(obj.FileSize);
                });

                images.forEach(function (i) {
                    currentlyUploadedSize = currentlyUploadedSize + parseFloat(i._file.size / 1024 / 1024);
                });

                if (parseFloat(previouslyUploadedSize + currentlyUploadedSize) > parseFloat(5.0)) {
                    errorMsg = "Max file limit for images upload is 5 MB.";
                    $scope.notValidated = true;
                }
            } else {
                images.forEach(function (i) {
                    if (i._file.size / 1024 / 1024 > 5) {
                        errorMsg = "Max file limit for image is 5 MB.";
                        $scope.notValidated = true;
                    }
                });
            }

            if ($scope.notValidated) {
                return;
            }
        }

        if (twProfileSelected) {
            var filesCountExceeded = true;
            if ($scope.previouslyUploadedData.length > 0) {
                filesCountExceeded = ($scope.previouslyUploadedData.length + $scope.UploadedFiles.length) > 4;
            } else {
                filesCountExceeded = $scope.UploadedFiles.length > 4;
            }

            if (filesCountExceeded) {
                $scope.errorMsg = "File upload count limit exceeded for Twitter.";
                $scope.notValidated = true;
                return;
            }
            var isMovieUploadedError = ((uploadedFilesExtentions.includes("mp4") || uploadedFilesExtentions.includes("mov")) && $scope.UploadedFiles.length > 1);
            if (isMovieUploadedError) {
                $scope.errorMsg = "Max Video upload limit is 1 for Twitter";
                $scope.notValidated = true;
                return;
            }
            var isGifUploadedError = (uploadedFilesExtentions.includes("gif") && $scope.UploadedFiles.length > 1);
            if (isGifUploadedError) {
                $scope.errorMsg = "Max Gif upload limit is 1 for Twitter";
                $scope.notValidated = true;
                return;
            }
            var areAllImages = true;
            uploadedFilesExtentions.forEach(function (i) {
                if (!allowedTWImgExtentions.includes(i)) {
                    areAllImages = false;
                }
            });

            var isImgUploadedError = (areAllImages && $scope.UploadedFiles.length > 4);
            if (isImgUploadedError) {
                $scope.errorMsg = "Max Image upload limit is 4 for Twitter";
                $scope.notValidated = true;
                return;
            }
        }

        if (fbProfileSelected) {
            var filesCountExceeded = $scope.UploadedFiles.length > 1;
            var invalidExtentions = !uploadedFilesExtentions.every(function (val) { return allowedFbExtentions.indexOf(val) >= 0; });

            if (filesCountExceeded)
                $scope.errorMsg += $scope.errorMsg.length > 0 ? (", Max upload limit is 1 for facebook profile") : "Max upload limit is 1 for facebook profile";

            if (invalidExtentions)
                $scope.errorMsg += $scope.errorMsg.length > 0 ? (", Only jpg jpeg or png extention allowed for facebook profile") : "Only jpg jpeg or png extention allowed for facebook profile";

            $scope.notValidated = filesCountExceeded || invalidExtentions;
            return;
        }
        $scope.notValidated = false;
        return;
    };

    $scope.UploadFiles = function () {        
        if ($scope.UploadedFiles.length > 0) {
            $scope.UploadedFiles.forEach(function (i) {
                var file = i._file;
                $scope.mediaDataModal.push({ Base64String: "", ContentType: file.type, FileName: file.name, FileSize: Math.ceil(file.size / 1024 / 1024).toFixed(2) });
            });

            $scope.UploadedFiles.forEach(function (i) {
                var file = i._file;
                $scope.convertToBase64StringAndSend(file);
            });
        }

        angular.forEach($scope.previouslyUploadedData, function (obj) {
            $scope.mediaDataModal.push(obj);
        });

        items.result = $scope.mediaDataModal;
        //items.files = $scope.UploadedFiles;
        items.files = new Array();

        $uibModalInstance.close('Activated');
    };

    $scope.convertToBase64StringAndSend = function (file) {
        var reader = new FileReader();
        reader.addEventListener("loadend", function () {            
            console.log(reader.result.length);
            $scope.mediaDataModal[$scope.count].Base64String = reader.result;

            if ($scope.mediaDataModal.length == ($scope.count + 1)) {
                console.log($scope.mediaDataModal);
                //$scope.PostMessage();
            }
            $scope.count++;
        }, false);

        if (file) {
            reader.readAsDataURL(file);
        }
    };

    $scope.removeImage = function (Index) {
        $scope.previouslyUploadedData.splice(Index, 1);
    }

    $scope.init();

    $scope.cancel = function () {        
        //if ($scope.mediaDataModal.length > $scope.TempSavingOfMediaData.length)
        //    $scope.TempSavingOfMediaData.splice($scope.TempSavingOfMediaData.length-1, $scope.mediaDataModal.length-1);
        
        $scope.UploadedFiles = new Array();
        $scope.mediaDataModal = $scope.TempSavingOfMediaData;
        items.files = new Array();
        items.result = $scope.TempSavingOfMediaData;
        //$uibModalInstance.dismiss('cancel');       
        $uibModalInstance.close('Activated');
    };

//Braces end   
}]);