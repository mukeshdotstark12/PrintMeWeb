$(document).ready(function(){
  $(".main-menu-navigation a.mobile-navigation").click(function(el){
    el.preventDefault();
    $(".main-menu-navigation ul").slideToggle();
    $(".main-menu-navigation a.mobile-navigation").toggleClass('active')
  }); 
  if ($(window).width() < 767) {
    if($(".product-filter").length){
    $(".product-filter").prepend("<a href='javascript:;' class='product-filter-mobile'><i class='fa fa-filter'></i> Filter</a>");
    }
  }
  $(".product-filter-mobile").click(function(e){
    e.preventDefault();
    $("#filterProductForm").slideToggle();
    $(".product-filter-mobile").toggleClass('active')
  });
  $(".content-pills-inner h2").click(function(ed){
    ed.preventDefault();
    $(this).toggleClass('open');
    $(this).next('p').slideToggle();
  }); 
  
 

/*To display rating visuals on UI*/
var displayRating = $('.rating-values').html();
if ($('#stars').length) {
document.getElementById("stars").innerHTML = getStars(displayRating);
};

function getStars(rating) {

  // Round to nearest half
  rating = Math.round(rating * 2) / 2;
  let output = [];

  // Append all the filled whole stars
  for (var i = rating; i >= 1; i--)
    output.push('<i class="fa fa-star" aria-hidden="true" style="color: gold;"></i>&nbsp;');

  // If there is a half a star, append it
  if (i == .5) output.push('<i class="fa fa-star-half-o" aria-hidden="true" style="color: gold;"></i>&nbsp;');

  // Fill the empty stars
  for (let i = (5 - rating); i >= 1; i--)
    output.push('<i class="fa fa-star-o" aria-hidden="true" style="color: gold;"></i>&nbsp;');
  return output.join('');
}
/*To display rating visuals on UI*/
$('.product-detail-image').slick({
  centerMode: false,
  slidesToShow: 1,
  infinite:false,
  prevArrow: "<a class='prev-next-arrow circle' href='javascript:;'><i class='fa fa-angle-left'></i></a>",
    nextArrow: "<a class='prev-next-arrow' href='javascript:;'><i class='fa fa-angle-right'></i></a>",
  responsive: [
    {
      breakpoint: 991,
      settings: {
        centerPadding: '40px',
        slidesToShow: 1
      }
    },
    {
      breakpoint: 767,
      settings: {
        slidesToShow: 1
      }
    }
  ]
});
  });

