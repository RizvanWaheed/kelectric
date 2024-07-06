CRMApp.controller('_ImageSliderController', ['$rootScope', '$scope', '$window', '$location', '$uibModalInstance', '$window', 'items', 'GlobalService', 'notify', function ($rootScope, $scope, $window, $location, $uibModalInstance, $window, items, GlobalService, notify) {

    $scope.commentsLength = {
        height: '380px',//350
        disableFadeOut: true
    }

    console.log(items.Images);
    $scope.Attachments = items.Images;

    //$scope.Attachments.forEach(function (i) {
    //    if (i.Type == 'video_fb' || i.Type == 'video_share')
    //        i.Url = $sce.trustAsResourceUrl(i.Url);
    //});

    $scope.direction = 'left';
    $scope.currentIndex = 0;

    $scope.setCurrentSlideIndex = function (index) {
        $scope.direction = (index > $scope.currentIndex) ? 'left' : 'right';
        $scope.currentIndex = index;
    };

    $scope.isCurrentSlideIndex = function (index) {
        return $scope.currentIndex === index;
    };

    $scope.prevSlide = function () {
        $scope.direction = 'left';
        $scope.currentIndex = ($scope.currentIndex < $scope.Attachments.length - 1) ? ++$scope.currentIndex : 0;
    };

    $scope.nextSlide = function () {
        $scope.direction = 'right';
        $scope.currentIndex = ($scope.currentIndex > 0) ? --$scope.currentIndex : $scope.Attachments.length - 1;
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };


}]).animation('.slide-animation', function () {
    return {
        beforeAddClass: function (element, className, done) {
            var scope = element.scope();

            if (className == 'ng-hide') {
                var finishPoint = element.parent().width();
                if (scope.direction !== 'right') {
                    finishPoint = -finishPoint;
                }
                TweenMax.to(element, 0.5, { left: finishPoint, onComplete: done });
            }
            else {
                done();
            }
        },
        removeClass: function (element, className, done) {
            var scope = element.scope();

            if (className == 'ng-hide') {
                element.removeClass('ng-hide');

                var startPoint = element.parent().width();
                if (scope.direction === 'right') {
                    startPoint = -startPoint;
                }

                TweenMax.fromTo(element, 0.5, { left: startPoint }, { left: 0, onComplete: done });
            }
            else {
                done();
            }
        }
    };
});