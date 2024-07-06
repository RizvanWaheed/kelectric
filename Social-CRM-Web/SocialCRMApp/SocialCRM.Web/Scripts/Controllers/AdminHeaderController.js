CRMApp.controller('AdminHeaderController', ['$rootScope', '$scope', '$window', '$location', '$compile', 'AuthService', 'GroupService', '$uibModal', '$cookies', 'AuthService', 'CompanyService', 'GlobalService', 'notify', 'NotificationService', function ($rootScope, $scope, $window, $location, $compile, AuthService, GroupService, $uibModal, $cookies, AuthService, CompanyService, GlobalService, notify, NotificationService) {
    $scope.redirectTo = function (path) {
        $window.location.href = path;
    };

    $scope.init = function () {
        AuthService.UserCurrentState().then(function (l) {
            if (l.data != null) {
                $rootScope.currentUser = JSON.stringify(l.data);
                $rootScope.IsAdmin = l.data.IsAdmin;

                $scope.SetNavLinks();
        
                $scope.UserName = jQuery.parseJSON($rootScope.currentUser).FirstName + ' ' + jQuery.parseJSON($rootScope.currentUser).LastName;
                //$scope.CompanyName = jQuery.parseJSON($rootScope.currentUser).CompanyName;
                $scope.UserImage = jQuery.parseJSON($rootScope.currentUser).ImageUrl;
                $scope.UserImage = "/Content/img/user2-160x160.jpg";

                $rootScope.$broadcast('Session_Fetched_Reload');
            }
            else {
                // alert('Error in fetching data, please try again.');
                $scope.redirectTo("#/Login");
            }
        }, function (err) {
            // alert('Error in fetching data, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching data, please try again.");
            console.log(err);
        });
    }

    $scope.SetNavLinks = function () {
       $scope.navLinks = [
           {
               mainLink: 'Configuration',
               description: 'create & edit packages',
               icon: 'fa-cog',
               hide: false,
               subLink: [
                   {
                       Title: 'Packages',
                       url: 'MembershipPackages',
                       parameter: '',
                       hide: false
                   }
               ]
           },
           {
               mainLink: 'Dashboard',
               description: 'View Companies and Groups',
               icon: 'fa-cog',
               hide: false,
               subLink: [
                   {
                       Title: 'View Dashboard',
                       url: 'DashboardView',
                       parameter: '',
                       hide: false
                   },
                   {
                       Title: 'Companies Overview',
                       url: 'Companies',
                       parameter: '',
                       hide: false
                   }
               ]
           }
       ];
    }

    $scope.navClass = function (page) {
        var currentRoute = $location.path().substring(1) || 'home';
        //alert(currentRoute)
        var obj = page === currentRoute ? 'left_nav_subs_active' : 'left_nav_subs_inactive';

        return obj;
    }


    $scope.logout = function () {
        AuthService.LogOut().then(function (l) {
            //alert("group has been added")
            $rootScope.selectedGroupId = null;
            $rootScope.selectedCompanyId = null;
            $rootScope.selectedCompanyName = null;
            $rootScope.currentUser = null;
            $rootScope.IsAdmin = null;
            $rootScope.IsPersonal = null;
            $scope.redirectTo("#/Login");
        }, function (err) {
         //   alert('Error in Logging out, please try again.');
            //GlobalService.ErrorPopup("Error", "Error in Logging out, please try again.");
            //instead of showing Error just remove user object from client browser.
            $cookies.remove("access_token");
            console.log(err);
        });
        //alert("logging out");
    }

    $scope.init();
}]);