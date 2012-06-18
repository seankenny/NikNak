$(function () {
    $('#get-chart').click(function (e) {
        e.preventDefault();
        $('#chart').html($('<img>').attr('src', this.href));


        $('#chart-area').removeClass('hide');
    });
    
    $('.set-chart-type').click(function (e) {
        e.preventDefault();
        $('#chart').html($('<img>').attr('src', this.href));
    });
});