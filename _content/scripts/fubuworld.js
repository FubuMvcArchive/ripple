$(function () {
    var stickyHeaderTop = $('#nav-follow').offset().top;
    $(window).scroll(function () {
        if ($(window).scrollTop() > stickyHeaderTop) {
            $('#nav-follow').addClass('nav-follow-fixed');
        } else {
            $('#nav-follow').removeClass('nav-follow-fixed');
        }
    });
});