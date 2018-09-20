$('.ratingButtons').on('click', function (e) {
    e.preventDefault();
    console.log($(this));
    var current = $(this)[0];


    var entity = current.id.split('_')[0];
    var entityId = current.id.split('_')[1];

    console.log(entity);
    if (entity.includes('Video')) {
        //------------------------------------------------------------------------------------------
        if (current.src.includes('/like.png')) {
            sendLikeVideo(entityId, true);
            current.src = '/Content/Pictures/liked.png';
            //------------------------------------------------------------------------------------------
        } else if (current.src.includes('/liked.png')) {
            removeLikeVideo(entityId);
            current.src = '/Content/Pictures/like.png';
            //------------------------------------------------------------------------------------------
        } else if (current.src.includes('/dislike.png')) {
            current.src = '/Content/Pictures/disliked.png';
            sendDislikeVideo(entityId, false);
            //------------------------------------------------------------------------------------------
        } else if (current.src.includes('/disliked.png')) {
            current.src = '/Content/Pictures/dislike.png';
            removeDislikeVideo(entityId);
        }
    } else {
        //===================================================================================
        if (current.src.includes('/like.png')) {
            sendLikeComment(entityId, true);
            current.src = '/Content/Pictures/liked.png';
            //------------------------------------------------------------------------------------------
        } else if (current.src.includes('/liked.png')) {
            removeLikeComment(entityId);
            current.src = '/Content/Pictures/like.png';
            //------------------------------------------------------------------------------------------
        } else if (current.src.includes('/dislike.png')) {
            current.src = '/Content/Pictures/disliked.png';
            sendDislikeComment(entityId, false);
            //------------------------------------------------------------------------------------------
        } else if (current.src.includes('/disliked.png')) {
            current.src = '/Content/Pictures/dislike.png';
            removeDislikeComment(entityId);
        }
    }
});

function sendLikeVideo(videoId, isLike) {

    var model = new Object();
    model.isLike = true;
    model.videoId = videoId;

    $.ajax({
        type: 'POST',
        url: '/Video/SubmitLikeDislike',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (result) {
            console.log(result);
            if (result.message == 'redirect') {
                window.location.replace('/Account/Login');
            } else {

                $('#likeVideoButtonRatingValue_' + videoId)[0].innerText = (parseInt($('#likeVideoButtonRatingValue_' + videoId)[0].innerText) + 1).toString();
                if ($('#dislikeVideoButton_' + videoId)[0].src.includes('/disliked.png')) {
                    $('#dislikeVideoButton_' + videoId)[0].src = '/Content/Pictures/dislike.png';
                    $('#dislikeVideoButtonRatingValue_' + videoId)[0].innerText = (parseInt($('#dislikeVideoButtonRatingValue_' + videoId)[0].innerText) - 1).toString();
                }
            }

        },
        error: function (gonetoshit) {
            console.log(gonetoshit);
        }
    });


}

function sendDislikeVideo(videoId) {
    var model = new Object();
    model.isLike = false;
    model.videoId = videoId;

    $.ajax({
        type: 'POST',
        url: '/Video/SubmitLikeDislike',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (result) {
            if (result.message == 'redirect') {
                window.location.replace('/Account/Login');
            } else {
                console.log(result);
                $('#dislikeVideoButtonRatingValue_' + videoId)[0].innerText = (parseInt($('#dislikeVideoButtonRatingValue_' + videoId)[0].innerText) + 1).toString();
                if ($('#likeVideoButton_' + videoId)[0].src.includes('/liked.png')) {
                    $('#likeVideoButton_' + videoId)[0].src = '/Content/Pictures/like.png';
                    $('#likeVideoButtonRatingValue_' + videoId)[0].innerText = (parseInt($('#likeVideoButtonRatingValue_' + videoId)[0].innerText) - 1).toString();
                }
            }
        },
        error: function (gonetoshit) {
            console.log(gonetoshit);
        }
    });

}

function removeLikeVideo(videoId) {

    var model = new Object();
    model.videoId = videoId;

    $.ajax({
        type: 'POST',
        url: '/Video/RemoveLikeDislike',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (result) {
            console.log(result);
            $('#likeVideoButtonRatingValue_' + videoId)[0].innerText = (parseInt($('#likeVideoButtonRatingValue_' + videoId)[0].innerText) - 1).toString();
        },
        error: function (gonetoshit) {
            console.log(gonetoshit);
        }
    });
}

function removeDislikeVideo(videoId) {

    var model = new Object();
    model.videoId = videoId;

    $.ajax({
        type: 'POST',
        url: '/Video/RemoveLikeDislike',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (result) {
            console.log(result);
            $('#dislikeVideoButtonRatingValue_' + videoId)[0].innerText = parseInt($('#dislikeVideoButtonRatingValue_' + videoId)[0].innerText) - 1;
        },
        error: function (gonetoshit) {
            console.log(gonetoshit);
        }
    });
}


function sendLikeComment(commentId, isLike) {
    var model = new Object();
    model.isLike = true;
    model.commentId = commentId;

    $.ajax({
        type: 'POST',
        url: '/Comment/SubmitLikeDislike',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (result) {
            if (result.message == 'redirect') {
                window.location.replace('/Account/Login');
            } else {
                console.log(result);
                $('#likeCommentButtonRatingValue_' + commentId)[0].innerText = (parseInt($('#likeCommentButtonRatingValue_' + commentId)[0].innerText) + 1).toString();
                if ($('#dislikeCommentButton_' + commentId)[0].src.includes('/disliked.png')) {
                    $('#dislikeCommentButton_' + commentId)[0].src = '/Content/Pictures/dislike.png';
                    $('#dislikeCommentButtonRatingValue_' + commentId)[0].innerText = (parseInt($('#dislikeCommentButtonRatingValue_' + commentId)[0].innerText) - 1).toString();
                }
            }
        },
        error: function (gonetoshit) {
            console.log(gonetoshit);
        }
    });
}

function sendDislikeComment(commentId) {
    var model = new Object();
    model.isLike = false;
    model.commentId = commentId;

    $.ajax({
        type: 'POST',
        url: '/Comment/SubmitLikeDislike',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (result) {
            if (result.message == 'redirect') {
                window.location.replace('/Account/Login');
            } else {
                console.log(result);
                $('#dislikeCommentButtonRatingValue_' + commentId)[0].innerText = (parseInt($('#dislikeCommentButtonRatingValue_' + commentId)[0].innerText) + 1).toString();
                if ($('#likeCommentButton_' + commentId)[0].src.includes('/liked.png')) {
                    $('#likeCommentButton_' + commentId)[0].src = '/Content/Pictures/like.png';
                    $('#likeCommentButtonRatingValue_' + commentId)[0].innerText = (parseInt($('#likeCommentButtonRatingValue_' + commentId)[0].innerText) - 1).toString();
                }
            }
        },
        error: function (gonetoshit) {
            console.log(gonetoshit);
        }
    });

}

function removeLikeComment(commentId) {
    var model = new Object();
    model.commentId = commentId;

    $.ajax({
        type: 'POST',
        url: '/Comment/RemoveLikeDislike',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (result) {
            console.log(result);
            $('#likeCommentButtonRatingValue_' + commentId)[0].innerText = (parseInt($('#likeCommentButtonRatingValue_' + commentId)[0].innerText) - 1).toString();
        },
        error: function (gonetoshit) {
            console.log(gonetoshit);
        }
    });
}

function removeDislikeComment(commentId) {
    var model = new Object();
    model.commentId = commentId;

    $.ajax({
        type: 'POST',
        url: '/Comment/RemoveLikeDislike',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (result) {
            console.log(result);
            $('#dislikeCommentButtonRatingValue_' + commentId)[0].innerText = parseInt($('#dislikeCommentButtonRatingValue_' + commentId)[0].innerText) - 1;
        },
        error: function (gonetoshit) {
            console.log(gonetoshit);
        }
    });
}