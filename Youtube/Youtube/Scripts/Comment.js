


$(document).on('keyup', '#CommentInput', function (e) {
    e.preventDefault();
    if (e.keyCode === 13) {
        $('#CommentSubmitButton').trigger('click');
    }
});


$('#CommentSubmitButton').on('click', function (e) {
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


$('.clickable').on('mouseenter', function (e) {
    e.preventDefault();
    $(this)[0].style.cursor = 'pointer';
    $(this)[0].style.opacity = 0.2;
    //$(this).animate({
    //    opacity: '0.2',
    //    height: '170px'
    //}, 'fast');
    //$('#' + $(this)[0].id + '.VideoThumbnailsText')[0].style.visibility = 'visible';

});

$('.clickable').on('mouseleave', function (e) {
    e.preventDefault();
    $(this)[0].style.cursor = 'default';
    $(this)[0].style.opacity = 1;
    //$(this).animate({
    //    opacity: '1',
    //    height: '150px'
    //}, 'fast');
    //$('#' + $(this)[0].id + '.VideoThumbnailsText')[0].style.visibility = 'hidden';

});




    //    <div>
    //        @{
    //                                    if (c.comment_user.user_icon == null)
    //                                    {
    //            <a href="@Url.Action(" Details","User",new {UserName = c.comment_user.user_username})" style="text-decoration:none;color:black;"><img style="max-height:2.5em;width:auto;border:1px solid white;border-radius:50%;" src="~/Content/UserPhotos/avatar.png" /></a>
    //    }
    //    else
    //                                    {
    //        <a href="@Url.Action(" Details","User",new {UserName = c.comment_user.user_username})" style="text-decoration:none;color:black;"><img style="max-height:2.5em;width:auto;border:1px solid;border-radius:50%;" src="~/Content/UserPhotos/@c.comment_user.user_icon" /></a>
    //}

    //                                    <a href="@Url.Action(" Details","User", new {UserName = c.comment_user.user_username})" style="text-decoration:none;color:black;"><p>@c.comment_user.user_username</p></a>
    //                                }
    //                            </div >
    //                        </div >

    //<div class="col-md-10" style="margin:0 0 0 -2em;padding:0;">
    //    <div class="row"><p style="font-style:italic;font-size:0.6em;margin:0;padding:0;opacity:0.8;">@c.comment_created.Date.ToString("ddd MMM dd yyyy")</p></div>
    //    <div class="row"><p style="margin:0;padding:0;"> @c.comment_content</p></div>
    //    <div class="row">
    //        <img class="clickable ratingButtons" style="max-height:1em;width:auto;" src="~/Content/Pictures/like.png" /><p style="display:inline;">1</p>
    //        <img class="clickable ratingButtons" style="max-height:1em;width:auto;margin-left:0.5em;" src="~/Content/Pictures/dislike.png" /><p style="display:inline;">1</p>
    //    </div>
    //</div>


    function AddComment(User, Comment) {

        var divRow = $('<div class="row" style="margin:0 0 1em 0;"></div>');
        var divColMd2 = $('<div class="col-md-2" style="margin:0;padding:0;"></div>');
        var divColMd10 = $('<div class="col-md-10" style="margin:0 0 0 -2em;padding:0;"></div>');


        var responseDate = Comment.comment_created.substring(6, Comment.comment_created.length - 2);
        var date = new Date(parseInt(responseDate));
        var dateString = date.toString().substring(0, 15);






        //var div = $('<div style="margin-top:2em;background-color:#fbfbfb;border:0.1px solid #e2e2e2;"></div>');

        if (User.user_icon === null) {
            divColMd2.append($('<div><a href="/User/Details/' + User.users_id + '"><img style="max-height:3em;width:auto;border:1px solid white;border-radius:50%;display:inline;" src="/Content/UserPhotos/avatar.png"/></a></div>'));
        } else {
            divColMd2.append($('<div><a href="/User/Details/' + User.users_id + '"><img style="max-height:3em;width:auto;border:1px solid white;border-radius:50%;display:inline;" src="/Content/UserPhotos/' + User.user_icon + '"/></a></div>'));
        }
        divColMd2.append($('<a href="/User/Details?=' + User.users_id + '" style="text-decoration:none;color:black;">' + User.user_username + '</a>'));

        divColMd10.append($('<div class="row"><p style="font-style:italic;font-size:0.6em;margin:0;padding:0;opacity:0.8;""> ' + dateString + '</p></div>'));
        divColMd10.append($('<div class="row"><p style="margin:0;padding:0;">:  ' + Comment.comment_content + '</p></div>'));
       

        var imageRow = $('<div class="row"></div>');
        imageRow.append($('<img style="max-height:1em;width:auto;" src="/Content/Pictures/like.png"> </img>'));
        imageRow.append($('<img style="max-height:1em;width:auto;margin-left:0.5em;" src="/Content/Pictures/dislike.png"> </img>'));
        divColMd10.append(imageRow);


        divRow.append(divColMd2);
        divRow.append(divColMd10);

        $('#CommentSection').append(divRow);
    }


