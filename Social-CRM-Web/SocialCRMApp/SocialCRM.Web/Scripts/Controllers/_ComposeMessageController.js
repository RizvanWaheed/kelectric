CRMApp.controller('_ComposeMessageController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'ProfileService', 'FacebookService', 'UnifiedService', 'items', 'TwitterService', 'GlobalService', 'notify', 'FileUploader', 'NgMap', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, ProfileService, FacebookService, UnifiedService, items, TwitterService, GlobalService, notify, FileUploader, NgMap) {

    $scope.sendMessageWithLocation = false;


    $scope.commentsLength = {
        height: '195px',
        disableFadeOut: true
    };

    $scope.UploadedFiles = new Array();
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
        $scope.UploadedFiles = this.queue;
        $scope.ValidateFiles();
    };

    $scope.notValidated = true;
    
    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';
    $scope.instaIcon = '/Content/images/insta_ico16.png';
    $scope.ofUser = items.replyedBy;
    $scope.replyedMsgModel = '@' + items.replyedBy;
    $scope.StatusId = items.StatusId;
    $scope.replyedTo = "";
    $scope.attachedProfiles = [];
    $scope.selected = {};
    $scope.slim = {
        height: '370px',
        disableFadeOut: true
    }
    $scope.GetTwitterProfilesByGroupId = function () {
        //Follower details

        UnifiedService.GetAllProfilesForUserGroup($rootScope.selectedGroupId, $rootScope.selectedCompanyId, $rootScope.IsPersonal.toString()).then(function (l) {
            var response = $.grep(l.data, function (record) {
                return record.ProfileTypeId != 6;
            });
            $scope.attachedProfiles = response;
    
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching twitter profiles, please try again.");

            
        });
    };
    $scope.mediaData = new Array();
    $scope.count = 0;
    $scope.selectedProfiles = new Array();

    $scope.SendMessage = function () {
        $scope.selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.ProfileId];
        });
        if ($scope.selectedProfiles.length == 0) {
            GlobalService.ErrorPopup("Error", "Please select atleast one profile account");
            return;
        }
        $scope.mediaData = new Array();

        if ($scope.UploadedFiles.length > 0) {
            $scope.UploadedFiles.forEach(function (i) {
                var file = i._file;
                $scope.mediaData.push({ Base64String: "", ContentType: file.type, FileName: file.name });
            });

            $scope.UploadedFiles.forEach(function (i) {
                var file = i._file;
                $scope.convertToBase64StringAndSend(file);
            });
        }
        else {
            $scope.PostMessage();
        }

        
    };
    $scope.GetTwitterProfilesByGroupId();
    $scope.errorMsg = "";

    $scope.ValidateFiles = function () {
        $scope.errorMsg = "";
        var allowedExtentions = ["png", "jpg", "jpeg","gif", "webp","mov", "mp4"]
        var allowedFbExtentions = ["png", "jpg", "jpeg"];
        var allowedTWImgExtentions = ["png", "jpg", "jpeg", "webp"];
        var allowedTWVideoExtentions = ["mov", "mp4"];
        $scope.notValidated = false;
        $scope.count = 0;

        if (($scope.StatusText === undefined || $scope.StatusText == "") && ($scope.UploadedFiles.length == 0)) {
            $scope.errorMsg = "Please enter status text.";
            $scope.notValidated = true;
            return;
        }

        $scope.selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.ProfileId];
        });

        if ($scope.selectedProfiles.length == 0) {
            $scope.notValidated = true;
            return;
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

        uploadedFilesExtentions.forEach(function(i){
            if (!allowedExtentions.includes(i)){
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
            return record._file.type.split('/')[1] != "mp4" && record._file.type.split('/')[1] == "mov";
        });

        if (images.length > 0) {
            images.forEach(function (i) {
                if (i._file.size / 1024 / 1024 > 5) {
                    errorMsg = "Max file limit for image is 5 MB.";
                    $scope.notValidated = true;
                }
            });
            if ($scope.notValidated) {
                return;
            }
        }

        if (twProfileSelected) {
            var filesCountExceeded = $scope.UploadedFiles.length > 4;
            if (filesCountExceeded) {
                $scope.errorMsg = "File upload count limit exceeded for Twitter.";
                $scope.notValidated = true;
                return;
            }
            var isMovieUploadedError = ((uploadedFilesExtentions.includes("mp4") || uploadedFilesExtentions.includes("mov")) && $scope.UploadedFiles.length > 1);
            if (isMovieUploadedError)
            {
                $scope.errorMsg = "Max Video upload limit is 1 for Twitter";
                $scope.notValidated = true;
                return;
            }
            var isGifUploadedError = (uploadedFilesExtentions.includes("gif") && $scope.UploadedFiles.length > 1);
            if (isGifUploadedError)
            {
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

    $scope.selectedProfilesChanged = function(){
        $scope.ValidateFiles();

        //For character limits : Asjad
        debugger
        var selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.ProfileId];
        });
        $scope.getMinimumLength(selectedProfiles);
    };
    
    // $scope.init();
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $scope.convertToBase64StringAndSend = function (file) {
        var reader = new FileReader();
        reader.addEventListener("loadend", function () {
            console.log(reader.result.length);
            $scope.mediaData[$scope.count].Base64String = reader.result;
            
            if ($scope.mediaData.length == ($scope.count + 1)) {
                console.log($scope.mediaData);
                $scope.PostMessage();
            }
            $scope.count++;
        }, false);

        if (file) {
            reader.readAsDataURL(file);
        }
    };

    $scope.PostMessage = function () {
        
        if ($scope.selectedProfiles.length > 0) {
            for (var i = 0; i < $scope.selectedProfiles.length; i++) {
                if ($scope.selectedProfiles[i].ProfileTypeId == 4) {
                    var lat = $scope.googleMapPath != undefined ? $scope.googleMapPath.lat : 0;
                    var lng = $scope.googleMapPath != undefined ? $scope.googleMapPath.lng : 0;
                    TwitterService.TwitterNewTweet($scope.selectedProfiles[i].AccountName, $scope.StatusText, $scope.mediaData, lat, lng, $rootScope.selectedCompanyId).then(function (l) {
                        //alert('Tweet Send Successfully');
                        notify({
                                message: "Tweet Send Successfully",
                                position: GlobalService.position,
                                duration: GlobalService.duration
                            });
                        if (i == $scope.selectedProfiles.length) {
                            $scope.cancel();
                        }
                    }, function (err) {
                        GlobalService.ErrorPopup("Error", err.data.Message);
                    });
                }
                else if ($scope.selectedProfiles[i].ProfileTypeId == 1) {                    
                        var UserId = $scope.selectedProfiles[i].UserId;
                        var IsPage = $scope.selectedProfiles[i].IsPage;
                        var Status = $scope.StatusText;
                        var FilePath = $scope.mediaData.length > 0 ? $scope.mediaData[0].Base6SendMessage4String : "";
                        var ContentType = $scope.mediaData.length > 0 ? $scope.mediaData[0].ContentType : "";
                        var FileName = $scope.mediaData.length > 0 ? $scope.mediaData[0].FileName : "";
                        var PlaceId = $scope.facebookLocation != undefined ? $scope.facebookLocation.originalObject.id : "";

                        //var _data = { userId: UserId, isPage: IsPage, status: Status, filePath: FilePath, contentType: ContentType, fileName: FileName };

                        FacebookService.FacebookNewPost(UserId, IsPage, Status, FilePath, ContentType, FileName, PlaceId, $rootScope.selectedCompanyId).then(function (l) {
                            // alert('Tweet Send Successfully');
                            notify({
                                message: "Facebook post sent successfully",
                                position: GlobalService.position,
                                duration: GlobalService.duration
                            });
                            if (i == $scope.selectedProfiles.length) {
                                $scope.cancel();
                            }

                        }, function (err) {
                            GlobalService.ErrorPopup("Error", err.data.Message);
                        });                    
                }
            }
        }
    }

    $scope.getFacebookLocationUrl = function () {
        $scope.selectedProfiles = $.grep($scope.attachedProfiles, function (record) {
            return $scope.selected[record.ProfileId];
        });
        if ($scope.selectedProfiles.length > 0) {
            for (var i = 0; i < $scope.selectedProfiles.length; i++) {
                if ($scope.selectedProfiles[i].ProfileTypeId == 1) {
                    var UserId = $scope.selectedProfiles[i].UserId;
                    var IsPage = $scope.selectedProfiles[i].IsPage;
                    return base_url + "/Facebook/GetFacebookLocation?userId=" + UserId + "&isPage=" + IsPage + "&locationQuery=";
                    break;
                }
            }
        }
    }

    $scope.messageBodyAreaAllowedLength = 140; //For twitter
    $scope.getMinimumLength = function (selectedProfiles) {
        var value = 0;
        if (selectedProfiles[0].ProfileTypeId == 4) value = 140;
        else if (selectedProfiles[0].ProfileTypeId == 3) value = 700;
        else if (selectedProfiles[0].ProfileTypeId == 1) value = 2000;

        for (var i = 0; i < selectedProfiles.length; i++) {
            if (selectedProfiles[i].ProfileTypeId == 4) {
                if (value > 140) {
                    value = 140;
                }
            }
            else if (selectedProfiles[i].ProfileTypeId == 3) {
                if (value > 700) {
                    value = 700;
                }
            }
            if (selectedProfiles[i].ProfileTypeId == 1) {
                if (value > 5000) {
                    value = 5000;
                }
            }
        }
        $scope.messageBodyAreaAllowedLength = value;
        var reducedText = $scope.StatusText;
        if (reducedText != undefined && reducedText != "") {
            reducedText = reducedText.substring(0, value);
            $scope.StatusText = reducedText;
        }
    }    

    $scope.googlePlaceChanged = function () {
        var loc = this.getPlace();
        $scope.googleMapPath = loc;
        $scope.googleMapPath.lat = loc.geometry.location.lat();
        $scope.googleMapPath.lng = loc.geometry.location.lng();
    }

    $scope.googleMapTextChanged = function () {
        $scope.googleMapPath = null;
    }

    $scope.locationOnClick = function () {
        if ($scope.sendMessageWithLocation)
            $scope.sendMessageWithLocation = false;
        else
            $scope.sendMessageWithLocation = true;
    }

}]);