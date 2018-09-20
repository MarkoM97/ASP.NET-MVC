
    $('.videoContainer').on('mouseenter', function (e) {
        e.preventDefault();
        $(this)[0].style.cursor = 'pointer';
        var currentWidth = $(this)[0].style.width;
        var currentHeight = $(this)[0].style.height;
        $(this).animate({
            opacity: '1'
        }, 'fast');
        //$('#' + $(this)[0].id + '.VideoThumbnailsText')[0].style.visibility = 'visible';

});


$('.videoContainer').on('mouseleave', function (e) {
        e.preventDefault();
        $(this)[0].style.cursor = 'default';
        var currentWidth = $(this)[0].style.width;
        var currentHeight = $(this)[0].style.height;
        $(this).animate({
            opacity: '0.9'
        }, 'fast');
        //$('#' + $(this)[0].id + '.VideoThumbnailsText')[0].style.visibility = 'hidden';
});

