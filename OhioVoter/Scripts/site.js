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












    /*
    $('input.office_seats_one').on('change', function (evt) {
        var limit = 1;
        if ($(this).siblings(':checked').length >= limit) {
            this.checked = false;
        }
    });
    */


    /*
    $('input.office_seats_two').on('change', function (evt) {
        var limit = 2;
        if ($(this).siblings(':checked').length >= limit) {
            this.checked = false;
        }
    });
    */


    /*
    $("input[name='vehicle']").change(function () {
        var maxAllowed = 3;
        var cnt = $("input[name='vehicle']:checked").length;
        if (cnt > maxAllowed) {
            $(this).prop("checked", "");
            alert('Select maximum ' + maxAllowed + ' Levels!');
        }
    });
    */



    $(function ballotOfficeCandidateSelection(className, idName, maxSelection, numSelected) {
        console.log('Update candidates selected on ballot');
        alert("This is a ballot alert");
    });

    $('.ballot_candidate').on('click', function () {
        console.log('candidate selected');
    });


    $('#show_ballot_office').click(function () {
        console.log('button clicked to show office and candidates');
    });

    function ballotOfficeCandidateSelection(className, idName, maxSelection, numSelected) {
        //console.log("Class: " + className + ", Id: " + idName + ", Checked: " + numSelected + ", Max Selection: " + maxSelection);

        if (maxSelection > 1 && numSelected > maxSelection) {
            // don't allow any more to be selected
            //$("'#'" + idName + "'").prop('checked', false);
            document.getElementById(idName).checked = false;
            alert("Only " + maxSelection + " candidates can be selected for this office.");
        }
        else if (numSelected > maxSelection) {

            var candidateNames = document.getElementsByClassName(className);
            for (var i = 0; i < candidateNames.length; i++) {
                var candidate = candidateNames[i];
                candidate.checked = false;
            }

            document.getElementById(idName).checked = true;
        }
    }

});



