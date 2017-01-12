$(function () {

    // render partial view in #voter-location
    // to display the voter location update form
    $('#btn-show-voter-location-form').click(function () {
        $.ajax({
            // /ControllerName/ActionName
            url: "/Home/Location",
            success: function (data) {
                // your data could be what ever you returned in your action method 
                // (a View or Json or ...)
                // parse your data here
                $('#voter-location').html(data);
            }
        });
    });



    // update Voter/Polling map image
    // show polling address & map image
    $('#show-voter-map').click(function() {
        console.log('button clicked to show map');
        $('#voter-map').slideDown("fast");
        $('#show-voter-map').hide("fast");
        $('#hide-voter-map').show("fast");
    });

    // hide polling address & map image
    $('#hide-voter-map').click(function () {
        console.log('button clicked to hide map');
        $('#voter-map').slideUp("fast");
        $('#hide-voter-map').hide("fast");
        $('#show-voter-map').show("fast");
    });

});