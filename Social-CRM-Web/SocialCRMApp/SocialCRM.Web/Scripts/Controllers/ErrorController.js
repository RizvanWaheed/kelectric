CRMApp.controller('ErrorController', ['$rootScope', '$scope', 'AuthService', '$location', '$window', '$cookies', function ($rootScope, $scope, AuthService, $location, $window, $cookies) {
   
   $scope.BusinessName =  jQuery.parseJSON($rootScope.currentUser).CompanyName
}]);