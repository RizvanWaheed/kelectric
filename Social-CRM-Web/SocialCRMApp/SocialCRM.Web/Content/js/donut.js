$(function(){
      var $ppc = $('.progress-pie-chart'),
        percent = parseInt($ppc.data('percent')),
        deg = 360*percent/100;
      if (percent > 50) {
        $ppc.addClass('gt-50');
      }
      $('.ppc-progress-fill').css('transform','rotate('+ deg +'deg)');
      $('.ppc-percents span').html(percent+'%');
    });
   
$(function(){
      var $ppc = $('.progress-pie-chart2'),
        percent = parseInt($ppc.data('percent')),
        deg = 360*percent/100;
      if (percent > 50) {
        $ppc.addClass('gt-502');
      }
      $('.ppc-progress-fill2').css('transform','rotate('+ deg +'deg)');
      $('.ppc-percents2 span').html(percent+'%');
    });
   
