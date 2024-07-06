CRMApp.controller('PersonalProfileController', ['$scope', '$window', '$location', '$stateParams', 'UserService', 'ProfileService', '$uibModal', '$rootScope', 'GlobalService', 'notify', function ($scope, $window, $location, $stateParams, UserService, ProfileService, $uibModal, $rootScope, GlobalService, notify) {
    
    $scope.DeleteProfile = function (ProfileId)
    {
        //ProfileService.RemoveProfile
        ProfileService.UpdatePerssonalProfile(ProfileId).then(function (l) {
            $scope.init();
           // $scope.loadUserData($scope.companyId, userid);
          //  alert("profile has been removed ")
            notify({
                message: "profile has been removed.",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
        }, function (err) {
           // alert('Error in remove profile, please try again.');
            GlobalService.ErrorPopup("Error", "Error in removing profile, please try again.");

            console.log(err);
        });
     //   alert(ProfileId);

    }
    $scope.ChangePassword=function()
    {

        $location.path("/ChangePassword");
        //$scope.items = "lalalal";
        //var modalInstance = $uibModal.open({
        //    animation: true,
        //    templateUrl: '/Scripts/Views/PersonalProfile/_ChangePassword.html',
        //    controller: 'ChangePasswordController',

        //    resolve: {
        //        items: function () {
        //            return $scope.items;
        //        }
        //    }
        //});

        //modalInstance.result.then(function (selectedItem) {
        //    $scope.selected = selectedItem;
        //    alert($scope.selected)
        //}, function () {
        //    $log.info('Modal dismissed at: ' + new Date());
        //});
    }
    $scope.toggleEmail=function(emailUpdate)
    {
        if (emailUpdate == false)
        {
            for(i=0;i<$scope.Connected_Profiles.length;i++)
            {
                $scope.Connected_Profiles[i].IsEmailEnabled = false;
            }
        }
       // alert(emailUpdate)
    }
    $scope.init = function () {
        $scope.errorMsg = "";
        
        UserService.GetPersonalProfile($rootScope.selectedCompanyId).then(function (l) {
            if (l != null) {
                console.log(JSON.stringify(l.data))
                $scope.BusinessName = jQuery.parseJSON($rootScope.currentUser).CompanyName; //fetch business name
                $scope.UserPic = l.data.ProfileUrl; //fetch path for user pic
                $scope.UserName = l.data.Name;
                $scope.UserEmail = jQuery.parseJSON($rootScope.currentUser).Email; //fetch user email id
                $scope.emailUpdate = true; // set email update button to on /off according to saved data

                $scope.Changed_UserName = $scope.UserName;
                $scope.Changed_UserEmail = $scope.UserEmail;
                $scope.picModel = $scope.UserPic;
                //alert($scope.picModel)
                $scope.imageType = 'none';
                $scope.Connected_Profiles = l.data.ConnectedProfiles;
                $scope.fbIcon = '/Content/images/fb_ico16.png';
                $scope.linkedinIcon = '/Content/images/in_ico16.png';
                $scope.twitterIcon = '/Content/images/t_ico16.png';
                $scope.googleIcon = '/Content/images/ana_ico16.png';
                $scope.instaIcon = '/Content/images/insta_ico16.png';
                $scope.outlookIcon = '/Content/images/outlook_ico16.png';
                $scope.User_Rights = l.data.AssignedRights;
                $scope.Report_Rights = l.data.AssignedReports;
                //"ProfileName":"ibex360Insight","ProfileUrl":null,"ProfileId":16,"ProfileType":4,"IsEmailEnabled":false
               
            }

        }, function (err) {
            //alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching user data, please try again.");

            console.log(err);
        });

        
        //$scope.Connected_Profiles = [
        //                {
        //                    'ProfileName': 'Agha Rameez Ahmed',
        //                    'ProfileURL': 'Agha.Rameez.Ahmed1234',
        //                    'ProfilePicURL':'/Content/images/soc_ico.jpg',
        //                    'id': 123,
        //                    'type': 'facebook'
        //                },
        //                {
        //                    'ProfileName': 'Amber Ashraf',
        //                    'ProfileURL': 'Amber.Ashraf1234',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 1234,
        //                    'type': 'twitter'
        //                },
        //                {
        //                    'ProfileName': 'Ayesha Shamim',
        //                    'ProfileURL': 'Ayesha789',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 1236,
        //                    'type': 'linkedin'
        //                },
        //                {
        //                    'ProfileName': 'Adeel Gill',
        //                    'ProfileURL': 'Adeel789',
        //                    'ProfilePicURL': '/Content/images/soc_ico.jpg',
        //                    'id': 12,
        //                    'type': 'google'
        //                }
        //];

        //$scope.User_Rights = [{ 'rights': 'Add Team Members' }, { 'rights': 'Grant rights to others' }, { 'rights': 'Manage Message Status' }
        //, { 'rights': 'Publish without approval' }];
       // $scope.Report_Rights = [{ 'rights': 'Facebook report' }, { 'rights': 'Task Management Report' }, { 'rights': 'Group Management' }];
        
        $scope.Submit_groups = $scope.groups;
        
        $scope.Changed_BusinessName = $scope.BusinessName;
        $scope.emailUpdate = true; // set email update button to on and show groups whose updates are on the grid
        $scope.notificationUpdate = true;
        $scope.IsPersonal = $rootScope.IsPersonal;
    };
    // runs once per controller instantiation
    $scope.init();

    $scope.imageUpload = function (element) {
        checkFileType({
            allowedExtensions: ['jpg', 'jpeg', 'png'],

            error: function () {
                $scope.errorMsg = "Only jpg,jpeg and png images can be uploaded.";

            },
            success: function () {
                $scope.errorMsg = "";
                var reader = new FileReader();
                reader.onload = $scope.imageIsLoaded;
                reader.readAsDataURL(element.files[0]);

            }
        });
    }

    
    function checkFileType(options) {

        var defaults = {
            allowedExtensions: [],
            success: function () { },
            error: function () { }
        };
        options = $.extend(defaults, options);
     
        var value = $('#User_image').val(),
               file = value.toLowerCase(),
               extension = file.substring(file.lastIndexOf('.') + 1);

        if ($.inArray(extension, options.allowedExtensions) == -1) {
            
            options.error();
            $(this).focus();
        } else {
            return options.success();
           

        }

       
    }
    $scope.imageIsLoaded = function (e) {
        $scope.$apply(function () {
            $scope.picModel = e.target.result;
        });
        /*var image = new Image();
        image.src = e.target.result;

        image.onload = function () {
            if (image.width != image.height) {
                $scope.$apply(function () {
                    $scope.picModel = e.target.result;
                    $scope.errorMsg = "Only Pictures with same width and height can be uploaded.";
                });
            }
            else {
                $scope.$apply(function () {
                    $scope.picModel = e.target.result;
                });
            }
        };*/
    }
    $scope.Save = function () {
        var AssignedRights = [];
        var AssignedReports = [];
        var ConnectedProfiles = [];

        for (i = 0; i < $scope.Connected_Profiles.length; i++) {
            var obj = {
                ProfileName: $scope.Connected_Profiles[i].ProfileName,
                ProfileUrl: $scope.Connected_Profiles[i].ProfileUrl,
                ProfileId: $scope.Connected_Profiles[i].ProfileId
            , ProfileType: $scope.Connected_Profiles[i].ProfileType
            , IsEmailEnabled: $scope.Connected_Profiles[i].IsEmailEnabled

            }
            ConnectedProfiles.push(obj);
        }
        UserService.UpdatePersonalProfile(AssignedRights, AssignedReports, ConnectedProfiles, $scope.Changed_UserName, $scope.Changed_UserEmail, $scope.Changed_UserPic).then(function (l) {
            // $location.path("/Signup_Code").search({ param: $scope.Email });;
            // alert("Successfully updated");
            notify({
                message: "Successfully updated data.",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            $rootScope.$broadcast('Profile_Pic_Change', {});
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

    $scope.Cancel = function () {
        //alert("cancelled");
        $scope.init();
    }


    $scope.openProfileConnect = function () {
        
        $scope.items = "lalalal";
        var modalInstance = $uibModal.open({
          //  animation: true,
            templateUrl: '/Scripts/Views/Shared/_ProfileAdd.html',
            controller: 'Personal_ProfileAddController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            $scope.selected = selectedItem;
          //  alert($scope.selected)
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });

}]);