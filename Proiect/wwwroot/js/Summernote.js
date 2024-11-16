$(document).ready(function () {
    $('.summernote').summernote({
        //height: 300,
        //minHeight: 200,
        focus: true,
        toolbar: [
            ['insert', ['picture', 'link', 'video']]
        ]
});

// Summernote pentru poza profil
$('.profile-summernote').summernote({
    height: 150,
    focus: true,
    toolbar: [
        ['insert', ['picture']]
    ]
});
});