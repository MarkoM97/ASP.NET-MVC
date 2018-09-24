

//============================================================FILTERING======================================================
$('#applyFilterSubmit').on('click', function (e) {
    e.preventDefault();
    var userVal = $('#userFilterField').val();
    var lowDateVal = $('#dateFilterLow').val();
    var highDateVal = $('#dateFilterHigh').val();

    if (userVal == '') {
        userVal = null;
    }
    if (lowDateVal == '') {
        lowDateVal = null;
    }
    if (highDateVal == '') {
        highDateVal = null;
    }

    var model = new Object();
    model.userFilter = userVal;
    model.lowDateFilter = lowDateVal;
    model.highDateFilter = highDateVal;

    $.ajax({
        type: 'POST',
        url: '/Video/Filter',
        data: JSON.stringify(model),
        dataType: 'json',
        contentType: 'application/json',
        success: function (data) {
            addVideosToContainer(data);
        },
        error: function (e) {
            console.log(e);
        }
    });


});

function addVideosToContainer(videos) {
    //console.log(videos);
    $('#AllVideoContainer').empty();
    for (var i in videos.Videos) {
        var current = videos.Videos[i];
        console.log(current.video_name);

        var videoContainer = $('<div id="' + current.video_id + '" class="videoContainer" uploadedVal="' + current.video_created + '" ratingVal="' + (current.video_likes - current.video_dislikes) + '" viewsVal="' + current.video_views +'" style="position:relative;background-color:#fafafa;border-bottom:1px solid #e8e8e8;display:inline;float:left;padding-bottom:1em;box-shadow:1px 1px 15px #c8c8c8;width:11em;opacity:0.8;margin:0 0.5em 0 0;"></div>');
        videoContainer.append($('<a href="/Video/Details/' + current.video_id + '"><img id="' + current.video_id + '" class="VideoThumbnails" src="/Content/VideoImages/' + current.video_icon +'" style="width:100%;position:absolute;margin:0 0.1em 0 0;height:6em;padding:0;z-index:1;" /></a>'));
        videoContainer.append($(' <h5 style="padding:0.1em 0;font-weight:bold;z-index:2;margin-top:6.5em;">' + current.video_name+'</h5>'));
        videoContainer.append($('<div class="row" style="margin:0;padding:0;text-align:center;"> <p style="margin:1em 0 0 0;padding:0;font-size:0.5em;">' + current.video_views + ' Views</p></div>'));
        videoContainer.append($('<div class="row" style="margin:0;padding:0;text-align:center;"><p style="margin:0;padding:0;font-size:0.5em;display:inline;">' + current.video_created_string + '</p></div>'));
        


        var userSec = $('<div class="row" style="margin:-1.5em 0 0 0;padding:0;text-align:center;"></div>');
        userSec.append($('</br><img src="/Content/UserPhotos/' + current.video_user.user_icon + '" style="max-height:0.9em;display:inline;border:0.1px solid white;border-radius:50%;" />'));
        userSec.append($('<p style="margin:0;padding:0;font-size:0.5em;display:inline;">' + current.video_user.user_username + '</p>'));
        videoContainer.append(userSec);
        $('#AllVideoContainer').append(videoContainer);
    }
}

//===========================================================SORTING=========================================================
$('#sortCommentsByRatingSubmit').on('click', function (e) {
    e.preventDefault();
    var unsortedVideosDiv = $('#CommentSection')[0].children;
    var unsortedVideosArray = Array.from(unsortedVideosDiv);

    $('#CommentSection').empty();
    var sortedVideosArray = [];
    while (unsortedVideosArray.length > 0) {
        var highest = findHighestByRating(unsortedVideosArray);
        sortedVideosArray.push(highest);
        var highestIndex = unsortedVideosArray.indexOf(highest);
        unsortedVideosArray.splice(highestIndex, 1);
    }

    for (var i = 0; i < sortedVideosArray.length; i++) {
        $('#CommentSection').append(sortedVideosArray[i]);
    }
    
});

$('#sortCommentsByUploadDateSubmit').on('click', function (e) {
    e.preventDefault();
    var unsortedVideosDiv = $('#CommentSection')[0].children;
    var unsortedVideosArray = Array.from(unsortedVideosDiv);

    $('#CommentSection').empty();
    var sortedVideosArray = [];
    while (unsortedVideosArray.length > 0) {
        var highest = findHighestByUploadDate(unsortedVideosArray);
        sortedVideosArray.push(highest);
        var highestIndex = unsortedVideosArray.indexOf(highest);
        unsortedVideosArray.splice(highestIndex, 1);
    }

    for (var i = 0; i < sortedVideosArray.length; i++) {
        $('#CommentSection').append(sortedVideosArray[i]);
    }
});

$('#sortByPopularitySubmit').on('click', function (e) {
    e.preventDefault();
    var unsortedVideosDiv = $('#AllVideoContainer')[0].children;
    var unsortedVideosArray = Array.from(unsortedVideosDiv);

    $('#VideosDiv').empty();
    var sortedVideosArray = [];
    while (unsortedVideosArray.length > 0) {
        var highest = findHighestByPopularity(unsortedVideosArray);
        sortedVideosArray.push(highest);
        var highestIndex = unsortedVideosArray.indexOf(highest);
        unsortedVideosArray.splice(highestIndex, 1);
    }

    for (var i = 0; i < sortedVideosArray.length; i++) {
        $('#AllVideoContainer').append(sortedVideosArray[i]);
    }
});

$('#sortByRatingSubmit').on('click', function (e) {
    e.preventDefault();
    var unsortedVideosDiv = $('#AllVideoContainer')[0].children;
    var unsortedVideosArray = Array.from(unsortedVideosDiv);

    $('#VideosDiv').empty();
    var sortedVideosArray = [];
    while (unsortedVideosArray.length > 0) {
        var highest = findHighestByRating(unsortedVideosArray);
        sortedVideosArray.push(highest);
        var highestIndex = unsortedVideosArray.indexOf(highest);
        unsortedVideosArray.splice(highestIndex, 1);
    }

    for (var i = 0; i < sortedVideosArray.length; i++) {
        $('#AllVideoContainer').append(sortedVideosArray[i]);
    }
});

$('#sortByUploadDateSubmit').on('click', function (e) {
    e.preventDefault();
    var unsortedVideosDiv = $('#AllVideoContainer')[0].children;
    var unsortedVideosArray = Array.from(unsortedVideosDiv);

    $('#VideosDiv').empty();
    var sortedVideosArray = [];
    while (unsortedVideosArray.length > 0) {
        var highest = findHighestByUploadDate(unsortedVideosArray);
        sortedVideosArray.push(highest);
        var highestIndex = unsortedVideosArray.indexOf(highest);
        unsortedVideosArray.splice(highestIndex, 1);
    }

    for (var i = 0; i < sortedVideosArray.length; i++) {
        $('#AllVideoContainer').append(sortedVideosArray[i]);
    }
});

function findHighestByUploadDate(unsortedElements) {
    var currentHighest;
    for (var i = 0; i < unsortedElements.length; i++) {
        currentHighest = unsortedElements[i];
        for (var j = 0; j < unsortedElements.length; j++) {
            var potentialHighest = unsortedElements[j];
            //+ ' 00:00:00 UTC'
            var firstComparationVal = Date.parse((currentHighest.attributes.uploadedVal.textContent)); /*substring(4));*/
            var secondComparationVal = Date.parse((potentialHighest.attributes.uploadedVal.textContent)); /*.substring(4));*/
            if (secondComparationVal > firstComparationVal) {
                currentHighest = potentialHighest;
            }
        }
    }
    return currentHighest;
}

function findHighestByRating(unsortedElements) {
    var currentHighest;
    for (var i = 0; i < unsortedElements.length; i++) {
        currentHighest = unsortedElements[i];
        for (var j = 0; j < unsortedElements.length; j++) {
            var potentialHighest = unsortedElements[j];
            var firstComparationVal = parseInt(currentHighest.attributes.ratingVal.textContent);
            var secondComparationVal = parseInt(potentialHighest.attributes.ratingVal.textContent);
            if (secondComparationVal > firstComparationVal) {
                currentHighest = potentialHighest;
            }
        }
    }
    return currentHighest;
}

function findHighestByPopularity(unsortedElements) {
    var currentHighest;
    for (var i = 0; i < unsortedElements.length; i++) {
        currentHighest = unsortedElements[i];
        for (var j = 0; j < unsortedElements.length; j++) {
            var potentialHighest = unsortedElements[j];
            var firstComparationVal = parseInt(currentHighest.attributes.viewsVal.textContent);
            var secondComparationVal = parseInt(potentialHighest.attributes.viewsVal.textContent);
            if (secondComparationVal > firstComparationVal) {
                currentHighest = potentialHighest;
            }
        }
    }
    return currentHighest;
}








//==============================================ADMIN SEARCH=======================================================

$('.adminSearchChoices').on('click', function (e) {
    var value = $('#AdminSearchParam').val();
    var id = $(this)[0].id;
    switch (id) {
        case "AdminSearchChoiceUser":
            adminSearch(value, true);
            break;
        case "AdminSearchChoiceVideo":
            adminSearch(value, false);
            break;
    }
});

function adminSearch(query, isUser) {
    var model = new Object();  
    model.query = query;
    model.isUser = isUser;

    $.ajax({
        type: 'POST',
        url: '/User/Administrator',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(model),
        success: function (e) {
            if (!isUser) {
                populateVideosAdminSearch(e);
            } else {
                populateUsersAdminSearch(e);
            }
          
        },
        error: function (e) {
            console.log(e);
        }
    });
}


function populateVideosAdminSearch(entities) {
    console.log(entities);
    $('#AdminEntitiesContainer').empty();
    for (var i in entities.returnValues) {
        var current = entities.returnValues[i];
        var container = $('<div class="VideoContainer" id="' + current.video_id + '" uploadedVal="' + current.video_created + '" ratingVal="' + (current.video_likes - current.video_dislikes) + '" viewsVal="@item.video_views" class="videoContainer" style="position:relative;background-color:#fafafa;border-bottom:1px solid #e8e8e8;display:inline;float:left;margin:0.5em;padding-bottom:1em;box-shadow:1px 1px 15px #c8c8c8;width:9em;opacity:0.9;">');
        container.append($('<a href="/Video/Details/' + current.video_id + '"><img id="' + current.video_id + '" class="VideoThumbnails" src="/Content/VideoImages/' + current.video_icon + '" style="width:100%;position:absolute;margin:0 0.1em 0 0;height:5em;padding:0;z-index:1;" /></a>'));
        container.append($(' <h5 style="padding:0.1em 0;font-weight:bold;z-index:2;margin-top:5em;">' + current.video_name + '</h5>'));
        container.append($('<p style="margin:0;padding:0;font-size:0.5em;display:inline;">Posted: ' + current.video_created_string + '</p>'));
        container.append($(' <p style="margin:1em 0 0 0;padding:0;font-size:0.5em;float:right;">' + current.video_views + ' Views</p>'));
        if (current.video_user.user_icon == null) {
            container.append($('</br><img src="/Content/UserPhotos/avatar.png" style="max-height:0.9em;display:inline;border:0.1px solid white;border-radius:50%;" />'));
        } else {
            container.append($('</br><img src="/Content/UserPhotos/' + current.video_user.user_icon + '" style="max-height:0.9em;display:inline;border:0.1px solid white;border-radius:50%;" />'));
        }
        container.append($('<p style="margin:0;padding:0;font-size:0.5em;display:inline;">' + current.video_user.user_username + '</p>'));
        $('#AdminEntitiesContainer').append(container);
    }

    
}
function populateUsersAdminSearch(entities) {
    console.log(entities);
    $('#AdminEntitiesContainer').empty();
    for (var i in entities.returnValues) {
        var current = entities.returnValues[i];
        var container = $('<div style="position:relative;background-color:#fafafa;border-bottom:1px solid #e8e8e8;display:inline;float:left;margin:0.5em;padding-bottom:1em;box-shadow:1px 1px 15px #c8c8c8;width:9em;opacity:0.9;text-align:center;">');
        if (current.user_icon == null) {
            container.append($('<a href="/User/Details?UserName=' + current.user_username +'"></br><img src="/Content/UserPhotos/avatar.png" style="max-height:4em;display:inline;border:0.1px solid white;border-radius:50%;" /></a>'));
        } else {
            container.append($('<a href="/User/Details?UserName=' + current.user_username +'"></br><img src="/Content/UserPhotos/' + current.user_icon + '" style="max-height:4em;display:inline;border:0.1px solid white;border-radius:50%;" /></a>'));
        }
        container.append($('<a href="/User/Details?UserName=' + current.user_username +'"><p style="margin:0.5em 0 0 0;padding:0;">' + current.user_username + '</p></a>'));
        $('#AdminEntitiesContainer').append(container);
    }
    
}






