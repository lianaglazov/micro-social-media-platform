function likePost(postId, iconElement) {
    $.ajax({
        type: "POST",
        url: "/Posts/Like",
        data: { postId: postId },
        success: function (result) {
            $(iconElement).next().text(result.likesCount);

            if (result.likeAdded) {
                $(iconElement).addClass("liked");
            } else {
                $(iconElement).removeClass("liked");
            }
            localStorage.setItem(`likeState_${postId}_${result.userId}`, result.likeAdded ? "liked" : "");

        },
        error: function (error) {
            console.error("Error liking post:", error);
        }
    });
}

$(document).ready(function () {

    $.ajax({
        type: "GET",
        url: "Posts/GetCurrentUserId",
        success: function (userId) {
            $('.fa-heart').each(function () {
                const postId = $(this).attr('id');
                const likeState = localStorage.getItem(`likeState_${postId}_${userId}`);
                if (likeState === "liked") {
                    $(this).addClass("liked");
                }
            });
        },
        error: function (error) {
            console.error("Error", error);
        }
    });
});

