$(document).ready(function () {
    $(".header").map(function () {
        var id = $(this).find('.curs-image').attr('id');
        $(this).css({ "background": "url(\" /Backgrounds/b" + id.toString() + ".png\")" });
    });  
});