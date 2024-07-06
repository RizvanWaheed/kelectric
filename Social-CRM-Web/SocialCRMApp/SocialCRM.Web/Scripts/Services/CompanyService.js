CRMApp.factory('CompanyService', function ($http) {
    var companyService = {};

   
    companyService.update = function (CompanyId, CompanyName, UserEmail, CompanyGroups) {

        var _data = { CompanyId: CompanyId, CompanyName: CompanyName, UserEmail: UserEmail, CompanyGroups: CompanyGroups };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Company/Update',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }
    companyService.SendEmail = function (ToUserEmail, EmailSubject, EmailBody) {
       
        var _data = { ToUserEmail: ToUserEmail, EmailSubject: EmailSubject, EmailBody: EmailBody };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Company/SendEmail',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;

    }

    companyService.ReplyToEmail = function (FromUserEmail, ToUserEmail, EmailSubject, EmailBody, CCUserEmail) {

        var _data = { FromUserEmail: FromUserEmail, ToUserEmail: ToUserEmail, EmailSubject: EmailSubject, EmailBody: EmailBody, CCUserEmail: CCUserEmail };

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Company/ReplyToEmail',
            data: JSON.stringify(_data)
        }).then(function (res) {

            return res;
        });
        return request;
    }

    companyService.GetCompanyName = function () {
        var requestUrl = base_url + '/api/Company/GetCompanyName';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }
 

    companyService.ActivateCompany = function (companyId) {


        var request = $http({
            method: 'POST',
            url: base_url + '/api/Company/ActivateCompany/' + companyId
        }).then(function (res) {

            return res;
        });
        return request;

    }
    companyService.DeActivateCompany = function (companyId, isFirstTime) {

        var request = $http({
            method: 'POST',
            url: base_url + '/api/Company/DeActivateCompany/' + companyId + '/' + isFirstTime
        }).then(function (res) {

            return res;
        });
        return request;
    }
   
    companyService.IsCompanyActivatedForUser = function (companyId) {
        var requestUrl = base_url + '/api/Company/IsCompanyActivatedForUser/' + companyId;
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }
    companyService.GetAllMessages = function () {
        var requestUrl = base_url + '/api/Company/GetAllMessages/5';
        var request = $http({
            method: 'GET',
            url: requestUrl
        }).then(function successCallback(response) {
            return response;
        }, function errorCallback(response) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
        return request;
    }
    return companyService;
});