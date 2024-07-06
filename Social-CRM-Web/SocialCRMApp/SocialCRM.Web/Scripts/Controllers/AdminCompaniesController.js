CRMApp.controller('AdminCompaniesController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'TeamService', 'ProfileService', 'UnifiedService', '$uibModal', 'GlobalService', 'notify', 'FileUploader', 'AdminService', '$stateParams', function ($rootScope, $scope, $window, $location, $stateParams, TeamService, ProfileService, UnifiedService, $uibModal, GlobalService, notify, FileUploader, AdminService, $stateParams) {
    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';
    $scope.instaIcon = '/Content/images/insta_ico16.png';

    $scope.companiesData = [];

    $scope.GetCompaniesDetail = function () {
        AdminService.GetCompanyDetails().then(function (l) {
            if (l.statusText == "OK") {
                $scope.companiesData = $scope.companiesDataMain = l.data;                
            }
            console.log(l);
        }, function (err) {
            GlobalService.ErrorPopup("Error", "Error in fetching profiles, please try again.");
            console.log(err);
        });
    }

    $scope.openCompanyDetails = function (CompanyData) {
        $scope.items = { "CompanyData": CompanyData };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_CompanyDetails.html',
            controller: '_CompanyDetailsController',
            scope: $scope,
            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //  $scope.selected = selectedItem;
            //  alert($scope.selected)
        }, function () {
            // $log.info('Modal dismissed at: ' + new Date());
        });
    }

    $scope.init = function () {       

        $scope.GetCompaniesDetail();       
    };

    $rootScope.$on('Session_Fetched_Reload', function (evt, msg) {
        $scope.init();
    });

    $scope.init();

    $scope.filterCompanies = function () {
        var searchVal = $scope.textSearch;
        if (!searchVal) {
            $scope.companiesData = $scope.companiesDataMain;
            return;
        }
        var filteredCompanies = $.grep($scope.companiesDataMain, function (record) {
            return record.CompannyName.toLowerCase().indexOf(searchVal) !== -1
        });
        $scope.companiesData = filteredCompanies;
    };

    //End braces
}]);