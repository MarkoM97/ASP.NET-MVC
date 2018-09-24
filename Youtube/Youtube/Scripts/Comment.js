


$(document).on('keyup', '#CommentInput', function (e) {
    e.preventDefault();
    if (e.keyCode === 13) {
        $('#CommentSubmitButton').trigger('click');
    }
});

$(document).on('focus', '#CommentInput', function (e) {
    e.preventDefault();
    $('#CommentInput').popover('hide');
});


$('#CommentSubmitButton').on('click', function (e) {
    if ($('#CommentInput')[0].value == '') {
        $('#CommentInput').popover({ content: 'Content can\'t be empty', trigger: 'manual', placement: 'top' });
        $('#CommentInput').popover('show');
        return;
    }

    var model = new Object();
    model.Content = $('#CommentInput')[0].value;
    model.VideoId = $('#CommentInput')[0].attributes.videoId.value;

    $.ajax({
        type: 'POST',
        url: '/Comment/Create',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (result) {
            console.log(result);
            if (result.message == 'redirect') {
                window.location.replace('/Account/Login');
            } else {
                AddComment(result.User, result.Comment);
                $('#CommentInput').val('');
            }

        },
        error: function (error) {
            console.log(error);
        }

    });

});


$(document).on('mouseenter', '.clickable', function (e) {
    e.preventDefault();
    $(this)[0].style.cursor = 'pointer';
    $(this)[0].style.opacity = 0.2;
    //$(this).animate({
    //    opacity: '0.2',
    //    height: '170px'
    //}, 'fast');
    //$('#' + $(this)[0].id + '.VideoThumbnailsText')[0].style.visibility = 'visible';

});

$(document).on('mouseleave', '.clickable',function (e) {
    e.preventDefault();
    $(this)[0].style.cursor = 'default';
    $(this)[0].style.opacity = 1;

});


function AddComment(User, Comment) {

    var divRow = $('<div id="'+Comment.comment_id+'" ratingVal="'+(Comment.comment_likes - Comment.comment_dislikes)+'" uploadedVal="'+Comment.comment_created+'" class="row" style="margin:0 0 1em 0;"></div>');
    var divColMd2 = $('<div class="col-md-2" style="margin:0;padding:0;"></div>');
    var divColMd10 = $('<div class="col-md-10" style="margin:0 0 0 -2em;padding:0;"></div>');


    var responseDate = Comment.comment_created.substring(6, Comment.comment_created.length - 2);
    var date = new Date(parseInt(responseDate));
    var dateString = date.toString().substring(0, 15);


    console.log(User);
    divColMd2.append($('<div><a href="/User/Details?UserName=' + User.user_username + '" style="text-decoration:none;color:black;"><img style="max-height:2.5em;width:auto;border:1px solid;border-radius:50%;display:inline;" src="/Content/UserPhotos/' + User.user_icon + '"/></a></div>'));
    divColMd2.append($('<a href="/User/Details?UserName=' + User.user_username + '" style="text-decoration:none;color:black;">' + User.user_username + '</a>'));

    divColMd10.append($('<div class="row"><p style="font-style:italic;font-size:0.6em;margin:0;padding:0;opacity:0.8;"> ' + dateString + '</p></div>'));
    divColMd10.append($('<div class="row"><p style="margin:0;padding:0;">' + Comment.comment_content + '</p></div>'));


    var imageRow = $('<div class="row"></div>');
    imageRow.append($('<img id="likeCommentButton_' + Comment.comment_id +'" class="clickable ratingButtons"  style="max-height:1em;width:auto;" src="/Content/Pictures/like.png"></img> <p id="likeCommentButtonRatingValue_'+Comment.comment_id+'" style="display:inline;">0</p>'));
    imageRow.append($('<img id="dislikeCommentButton_' + Comment.comment_id + '" class="clickable ratingButtons" style="max-height:1em;width:auto;margin-left:0.5em;" src="/Content/Pictures/dislike.png"> </img><p id="dislikeCommentButtonRatingValue_' + Comment.comment_id +'" style="display:inline;">0</p>'));
    divColMd10.append(imageRow);


    divRow.append(divColMd2);
    divRow.append(divColMd10);

    $('#CommentSection').append(divRow);
}


