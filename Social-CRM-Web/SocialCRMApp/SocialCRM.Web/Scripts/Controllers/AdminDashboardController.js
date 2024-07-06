CRMApp.controller('AdminDashboardController', ['$rootScope', '$scope', '$window', '$location', '$stateParams', 'TeamService', 'ProfileService', 'UnifiedService', '$uibModal', 'GlobalService', 'notify', 'FileUploader', 'AdminService', '$stateParams', function ($rootScope, $scope, $window, $location, $stateParams, TeamService, ProfileService, UnifiedService, $uibModal, GlobalService, notify, FileUploader, AdminService, $stateParams) {
    $scope.fbIcon = '/Content/images/fb_ico16.png';
    $scope.linkedinIcon = '/Content/images/in_ico16.png';
    $scope.twitterIcon = '/Content/images/t_ico16.png';
    $scope.googleIcon = '/Content/images/ana_ico16.png';
    $scope.instaIcon = '/Content/images/insta_ico16.png';

    $scope.companiesData = [];

    $scope.GetCompaniesDetail = function () {
        $scope.companiesData = [];
        AdminService.GetCompanyDetails().then(function (l) {
            if (l.statusText == "OK")
            {
                console.log(l.data);
                $scope.companiesDataMain = l.data;

                var count = 0;
                if (l.data.length > 0) {
                    angular.forEach(l.data, function (company) {                        
                        if (count < 4) {
                            $scope.companiesData.push(company);
                            count++;
                        } else {
                           //donothing
                        }
                    });
                }                
                $scope.populateStats();
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

    $scope.populateStats = function () {
        //Preparing Data
        debugger
        $scope.Companies = [];
        $scope.ActivationStats = [];
        var activatedCount = 0;
        var deactivatedCount = 0;
        var activatedCountMembership = 0;
        var deactivatedCountMembership = 0;


        angular.forEach($scope.companiesDataMain, function (company) {
            $scope.Companies.push(company.CompannyName);
            
            if (company.CompanyStatus == "Active") {
                activatedCount++;
            } else {
                deactivatedCount++;
            }

            if (company.MembershipStatus == "Active") {
                activatedCountMembership++;
            } else {
                deactivatedCountMembership++;
            }
        });

        // Build the chart
        $('#containerCompaniesActivation').highcharts({
            chart: {
                type: 'pie',
                options3d: {
                    enabled: true,
                    alpha: 45
                }
            },
            title: {
                text: 'Activation Stats'
            },
            subtitle: {
                text: 'Activated v/s Deactivated Companies'
            },
            plotOptions: {
                pie: {
                    innerSize: 100,
                    depth: 45,
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        format: "{y:.1f}%",
                        style: { "fontSize": "15px", "fontWeight": "bold" },
                        distance: 20
                    },
                    showInLegend: true
                }
            },
            series: [{
                name: 'Part',
                colorByPoint: true,
                data: [{
                    name: 'Activated Companies',
                    y: parseFloat((activatedCount) / (activatedCount + deactivatedCount))*100
                }, {
                    name: 'De-Activated Companies',
                    y: parseFloat((deactivatedCount) / (activatedCount + deactivatedCount)) * 100,
                    sliced: true,
                    selected: true
                }]
            }],
            colors: ['#34bf49', '#ff3322']
        });

        // Build the chart
        $('#containerCompaniesMembershipStatus').highcharts({
            chart: {
                type: 'pie',
                options3d: {
                    enabled: true,
                    alpha: 45
                }
            },
            title: {
                text: 'Membership Stats'
            },
            subtitle: {
                text: 'Membership Status Active v/s Deactive'
            },
            plotOptions: {
                pie: {
                    innerSize: 100,
                    depth: 45,
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: true,
                        format: "{y:.1f}%",
                        style: { "fontSize": "15px", "fontWeight": "bold" },
                        distance: 20
                    },
                    showInLegend: true
                }
            },
            series: [{
                name: 'Part',
                colorByPoint: true,
                data: [{
                    name: 'Activated Companies',
                    y: parseFloat((activatedCountMembership) / (activatedCountMembership + deactivatedCountMembership)) * 100
                }, {
                    name: 'De-Activated Companies',
                    y: parseFloat((deactivatedCountMembership) / (activatedCountMembership + deactivatedCountMembership)) * 100,
                    sliced: true,
                    selected: true
                }]
            }],
            colors: ['#34bf49', '#ff3322']
        });
    }
    //End braces
}]);