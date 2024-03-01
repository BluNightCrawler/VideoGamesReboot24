// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


//Link highlighting
$(document).ready(function () {
    $( ".nav-link" ).each(
        function (i, data) {
            if (data.href == location.href || (location.href.startsWith(data.href) && data.href != location.origin+"/")) {
                $(data).addClass("link-secondary");
            }
        }
    );
});