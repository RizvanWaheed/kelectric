angular.module('angularjs-expandCollapse', [])
  .directive('dExpandCollapse', function () {

    return {
        restrict: 'EA',
        link: function (scope, element, attrs) {

            $(element).click(function () {
                //var show = "false";
                $(element).find(".answer").slideToggle('1100', function () {
                    // You may toggle + - icon     
                    // $(element).find("span").toggleClass('faqPlus faqMinus');
                    
                   

                });
                if ($(element).find(".expand").hasClass('hide')) {
                    $(element).find(".expand").removeClass('hide');
                    $(element).find(".contract").addClass('hide');
                    $(element).find(".answerSmall").show();
                }
                else if ($(element).find(".contract").hasClass('hide')) {
                    $(element).find(".contract").removeClass('hide');
                    $(element).find(".expand").addClass('hide');
                    $(element).find(".answerSmall").hide();
                }
                
                

            });

        }
    }



});
//myApp.factory('myService', function() {});

