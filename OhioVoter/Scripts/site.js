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
    var selectedElectionOfficeId = $('#candidate_lookup_office').val();
    var candidateLookUpName = $('#candidate_lookup_name').val();
    var xhr;

    // update candidate list when user selects a new office
    $('#candidate_lookup_office').on('change', function () {
        if ($('#candidate_lookup_office').val() != selectedElectionOfficeId) {
            if (xhr && xhr.readyState != 4) {
                xhr.abort();
            }
            $('#candidate_list_select').hide();
            $('#candidate_list_search').hide();
            $('#candidate_list_loading').show();
            selectedElectionOfficeId = $('#candidate_lookup_office').val();
            xhr = $.ajax({
                url: '/Candidate/UpdateCandidateLookUpList',
                type: 'POST',
                data: { electionOfficeId: selectedElectionOfficeId, candidateLookUpName: candidateLookUpName },
                success: function (data) {
                    $('#candidate_list_loading').hide();
                    $('#candidate_list_search').hide();
                    $('#candidate_list_select').show();
                    $('#candidate_list_select').html(data);
                },
                error: function (e) {
                    $('#candidate_list_select').hide();
                    $('#candidate_list_loading').hide();
                    $('#candidate_list_search').hide();
                }
            });
        }
    });

    // update candidate list when user enters a candidate's name
    $('#candidate_lookup_name').keyup(function () {
        if ($('#candidate_lookup_name').val() != candidateLookUpName) {
            if (xhr && xhr.readyState != 4) {
                xhr.abort();
                console.log('old request has stopped to process new request')
            }
            $('#candidate_list_select').hide();
            $('#candidate_list_search').hide();
            $('#candidate_list_loading').show();
            candidateLookUpName = $('#candidate_lookup_name').val();
            xhr = $.ajax({
                url: '/Candidate/UpdateCandidateLookUpList',
                type: 'POST',
                data: { electionOfficeId: selectedElectionOfficeId, candidateLookUpName: candidateLookUpName },
                success: function (data) {
                    console.log('data from server ' + data);
                    $('#candidate_list_search').hide();
                    $('#candidate_list_loading').hide();
                    $('#candidate_list_select').show();
                    $('#candidate_list_select').html(data);
                },
                error: function (e) {
                    $('#candidate_list_select').hide();
                    $('#candidate_list_loading').hide();
                    $('#candidate_list_search').hide();
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





