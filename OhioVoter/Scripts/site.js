$(function () {

    // sidebar
    // render partial view in #voter-location
    // to display the voter location update form
    $('#btn-show-voter-location-form').click(function () {
        $.ajax({
            // /ControllerName/ActionName
            url: "/Home/Index",
            success: function (data) {
                // your data could be what ever you returned in your action method 
                // (a View or Json or ...)
                // parse your data here
                $('#voter-location').html(data);
            }
        });
    });



    // sidebar
    // update Voter/Polling map image
    // show polling address & map image
    $('#show-voter-map').click(function() {
        console.log('button clicked to show map');
        $('#voter-map').slideDown("fast");
        $('#show-voter-map').hide("fast");
        $('#hide-voter-map').show("fast");
    });

    // sidebar
    // hide polling address & map image
    $('#hide-voter-map').click(function () {
        console.log('button clicked to hide map');
        $('#voter-map').slideUp("fast");
        $('#hide-voter-map').hide("fast");
        $('#show-voter-map').show("fast");
    });




    // candiate Lookup/Summary
    // render partial view in #voter-location
    // to display the voter location update form
    $('#candidate_lookup_list').onchange(function () {
        $.ajax({
            // /ControllerName/ActionName
            url: "/Candidate/Name",
            success: function (data) {
                // your data could be what ever you returned in your action method 
                // (a View or Json or ...)
                // parse your data here
                $('#candidate-display').html(data);
            }
        });
    });



});





