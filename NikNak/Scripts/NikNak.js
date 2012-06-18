$(function () {
    $('#get-chart').click(function (e) {
        e.preventDefault();
        $('#chart-area img').attr('src', this.href);
    });
});