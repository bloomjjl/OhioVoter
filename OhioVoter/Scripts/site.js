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





    // Candidate Lookup
    // update candidate list when user enters a candidate's name
    var name = $('#candidate_lookup_name').val();

    $('#candidate_lookup_name').keyup(function () {
        if ($('#candidate_lookup_name').val() != name) {
            name = $('#candidate_lookup_name').val();
            console.log('Content has been changed to ' + name);
            $.ajax({
                url: '/Candidate/UpdateCandidateList',
                type: 'POST',
                data: { candidateListVM: JSON.stringify(candidateLookUpVM), candidateName: name },
                dataType: 'json',
                success: function (data) {
                    alert(data);
                } 
            });
        }
    });


});


    
    // ************ BALLOT ***********
/*
    $('.show-ballot-office').click(function () {
        console.log('button clicked to show office and candidates');
    });
*/



/*
    $(function showBallotOffice(id) {
        console.log('button id: ' + id + ', clicked to show office and candidates');
    });
*/


    $(function ballotOfficeCandidateSelection(className, idName, maxSelection, numSelected) {
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
    });





