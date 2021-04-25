$(document).ready(function() {
    var select_class = ".js-select";
    $(select_class).each(function(){
        //$(this).select2();
    }); 
    $(select_class).select2({
        minimumResultsForSearch: Infinity
    });  

    $('.open-popup-link').magnificPopup({
    type:'inline',
    midClick: true 
    });

    $('.tabs .btn').click(function(){
        var tab_id = $(this).attr('data-tab');
            $('.tabs .btn').removeClass('active');
            $('.tab-content').hide();
    
            $(this).addClass('active');
            $("#"+tab_id).show();
        })
      $('.tabs .btn:first').trigger('click'); // Default

      $(".square-option-2").click(function() {
        $(".square-upload-widget").show();
    });
    $(".square-option-1").click(function() {
        $(".square-upload-widget").hide();
    });
    $(".banner-option-2").click(function() {
        $(".banner-upload-widget").show();
    });
    $(".banner-option-1").click(function() {
        $(".banner-upload-widget").hide();
    });

});