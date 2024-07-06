CRMApp.controller('SocialFeedsRSSController', ['$rootScope', '$scope', '$window', '$location', '$uibModal', 'FeedlyService', '$stateParams', '$compile', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModal, FeedlyService, $stateParams, $compile, GlobalService, notify) {
  
    $scope.recentFeedId = 0;
    $scope.newFeedItemsCount = 0;
    $scope.latestFeedItemDetails = new Array();
   
    $scope.FeedsScheduler=function(feed)
    {
        $scope.items = { feed:feed };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: '/Scripts/Views/Shared/_FeedsScheduler.html',
            controller: '_FeedsSchedulerController',

            resolve: {
                items: function () {
                    return $scope.items;
                }
            }
        });

        modalInstance.result.then(function (selectedItem) {
            //$scope.selected = selectedItem;
            
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });

    }

    $scope.init = function ()
    {
        $scope.PageType = $stateParams.pageType;
        $scope.SelectedItem = [];
        $scope.FeedsData = [];
        $scope.PageNumber = 0;
        $scope.PageSize = 30;
        $scope.SelectedSearchTerm = {};
    if ($scope.PageType == 'Keywords')
    {
        $scope.isSearchTerm = true;
        $scope.PageType1 = 'Keyword';
    }
    else
    {
        $scope.isSearchTerm = false;
        $scope.PageType1 = 'Page';
    }
   // alert($scope.PageType)

    $scope.getKeywordsData();
    }

    $scope.getKeywordsData=function()
    {
        FeedlyService.GetSearchTerm($rootScope.selectedGroupId, $scope.isSearchTerm).then(function (l) {
            //alert(JSON.stringify(l.data))
            $scope.KeywordData = [];
            $.each(l.data, function (key, value) {
                $scope.KeywordData.push({ Id: value.Id, SearchKeyword: value.SearchKeyword, GroupId: value.GroupId, CreatedDate: value.CreatedDate, IsActive: value.IsActive, IsSearchTerm: value.IsSearchTerm, ischecked: key == 0 ? true : false });
            });
           // $scope.KeywordData = l.data;
            $("#keywordCheck-" + l.data[0].Id).prop('checked', true);
            $scope.SelectedItem = [];
            $scope.SelectedItem.push($scope.KeywordData[0].Id);
            $scope.fetchRSSFeedsData();
        }, function (err) {
          //  alert('Error in Signup, please try again.');
            GlobalService.ErrorPopup("Error", "Error in fetching list, kindly reload.");

            console.log(err);
        });
    }

    $scope.RemoveKeyword = function (id)
    {   
        FeedlyService.DeleteSearchTerm(id).then(function (l) {
            $scope.getKeywordsData();
            notify({
                message: "Keyword deleted successfully ",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
           //$('#KeywordRow-' + id).hide('slow');;
           // alert("Keyword Successfully Added");
            console.log('successfully Deleted keyword');
        }, function (err) {
           // alert('Error, please try again.');
            GlobalService.ErrorPopup("Error", "Error in removing Data, please try again.");

            console.log(err);
        });
    }   

    $scope.slim = {
        height: '150px',
        disableFadeOut: true
    }

    $scope.AddKeywordShow = false;

    $scope.CollapseAddKeyword = function ()
    {
        $scope.AddKeywordShow = !$scope.AddKeywordShow;
    }

    $scope.SaveKeyword = function ()
    {
        FeedlyService.AddSearchTerm($scope.AddedKeyword, $rootScope.selectedGroupId, $scope.isSearchTerm).then(function (l) {
            $scope.getKeywordsData();
           // alert("Keyword Successfully Added");
            notify({
                message: "Keyword added successfully ",
                position: GlobalService.position,
                duration: GlobalService.duration
            });
            console.log('successfully added keyword: ' + $scope.AddedKeyword);
        }, function (err) {
           // alert('Error, please try again.');
            GlobalService.ErrorPopup("Error", "Error in saving Data, please try again.");

            console.log(err);
        });
        
        $scope.AddedKeyword = '';
    }

    $scope.profileLoadMoreFeeds = function () {
        $scope.PageNumber++;
        $scope.Counter = ($scope.PageNumber * $scope.PageSize) + $scope.newFeedItemsCount;
        $scope.fetchRSSFeedsData();
    };
   
    $scope.CheckProfile = function (itemid) {
        $scope.SelectedItem = [];
        $scope.SelectedItem.push(itemid);
        $.each($scope.KeywordData, function (key, value) {
            if (value.Id != itemid) {
                $("#keywordCheck-" + value.Id).prop('checked', false);
            }
            //profiledata.push({ profileId: value.ProfileId, ProfileType: value.ProfileTypeId, GroupId: $scope.GroupId });
        });
        $scope.FeedsData = [];
        $scope.PageNumber = 0;
        $scope.fetchRSSFeedsData();
    }

    $scope.fetchRSSFeedsData = function()
    {
        $scope.Counter = ($scope.PageNumber * $scope.PageSize) + $scope.newFeedItemsCount;

        FeedlyService.GetFeed($rootScope.selectedGroupId, $scope.isSearchTerm, $scope.Counter,
            $scope.PageSize,  $scope.SelectedItem,0).then(function (l) {
                var response = l.data;
                if ($scope.Counter > 0)
                {
                   // $scope.FeedsData.push(response);
                    $scope.FeedsData.push.apply($scope.FeedsData, response);
                }
                else
                $scope.FeedsData = response;
                //  alert(JSON.stringify(response))

                if (response.length > 0)
                    $scope.recentFeedId = Math.max.apply(Math, l.data.map(function (o) { return o.Id; })) > $scope.recentFeedId ? Math.max.apply(Math, l.data.map(function (o) { return o.Id; })) : $scope.recentFeedId;
                
            }, function (err) {
              //  alert('Error in fetching Inbox data');
                GlobalService.ErrorPopup("Error", "Error in fetching Feeds.");
                //console.log(err);
            });
       // alert($scope.PageNumber);
    }

    $rootScope.$on('Group_Profile_Reload', function (evt, msg) {
        $scope.init();
    });
    
    $scope.init();

    $scope.fetchLatestRSSFeedsData = function () {
        FeedlyService.GetFeed($rootScope.selectedGroupId, $scope.isSearchTerm, $scope.PageNumber,
            $scope.PageSize, $scope.SelectedItem,$scope.recentFeedId).then(function (l) {
                var response = l.data;
                if (response.length > 0) {
                    var tempArray = new Array();

                    response.forEach(function (i) {
                        var data = $.grep($scope.latestFeedItemDetails, function (record) {
                            return record.Id == i.Id
                        });
                        if (data.length == 0) {
                            i.CssClass = "new_msg";
                            tempArray.push(i);
                        }
                    });

                    $scope.newFeedItemsCount += tempArray.length;
                    Array.prototype.push.apply(tempArray, $scope.latestFeedItemDetails)
                    $scope.latestFeedItemDetails = tempArray;

                    $scope.loadLatestFeedItems();

                    $scope.recentFeedId = Math.max.apply(Math, response.map(function (o) { return o.Id; })) > $scope.recentFeedId ? Math.max.apply(Math, response.map(function (o) { return o.Id; })) : $scope.recentFeedId;
                }
            }, function (err) {
                GlobalService.ErrorPopup("Error", "Error in fetching Feeds.");
            });
    }

    $scope.loadLatestFeedItems = function () {
        Array.prototype.push.apply($scope.latestFeedItemDetails, $scope.FeedsData);
        $scope.FeedsData = $scope.latestFeedItemDetails;
        $scope.latestFeedItemDetails = new Array();
    };
}]);


