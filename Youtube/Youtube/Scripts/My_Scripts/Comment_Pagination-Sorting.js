$(document).on('click', '.pagination-page', function (e) {
    e.preventDefault();
    console.log($('#sortSelection'));
    var sortValue = $('#sortSelection')[0].value;
    console.log();
    var page = $(this)[0].id;
    console.log(window.location.href);

    var videoId = parseInt(window.location.href.split("/")[5]);
    console.log(videoId);
    window.location.replace('/Video/Details/' + videoId + '?page=' + page + '&sort=' + sortValue + '&initial=false');
});


$(document).on('change', '#sortSelection', function (e) {
    var sortValue = $('#sortSelection')[0].value;
    var videoId = parseInt(window.location.href.split("/")[5]);
    window.location.replace('/Video/Details/' + videoId + '?page=1&sort=' + sortValue + '&initial=false');

});