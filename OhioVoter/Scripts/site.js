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





    // update Poll Office/Candidates displayed
    var $pollGraphOfficeLookup = $('#poll_graph_office_lookup');
    var $pollGraphCandidates = $('#poll-graph-candidates');
    var $pollGraphEmpty = $('_PollGraphEmpty');
    var selectedElectionOfficeId = $('#poll_graph_office_lookup').val();

    $pollGraphOfficeLookup.on('change', function () {
        if ($pollGraphOfficeLookup.val() != selectedElectionOfficeId) {
            selectedElectionOfficeId = $pollGraphOfficeLookup.val();
            $.ajax({
                url: '/Home/UpdatePollGraph',
                type: 'POST',
                cache: false,
                data: { selectedElectionOfficeId: selectedElectionOfficeId },
                success: function (data) {
                    if (data == "")
                    {
                        $pollGraphCandidates.hide();
                        $pollGraphEmpty.show();
                    }
                    $pollGraphEmpty.hide();
                    $pollGraphCandidates.show();
                    $pollGraphCandidates.html(data);
                },
                error: function (e) {
                    $pollGraphCandidates.hide();
                    $pollGraphEmpty.show();
                }
            });
        }
    });







    // Candidate Lookup
    var $candidateListSelect = $('#candidate_list_select');
    var $candidateListSearch = $('#candidate_list_search');
    var $candidateListLoading = $('#candidate_list_loading');
    var selectedElectionOfficeId = $('#candidate_lookup_office').val();
    var candidateSuppliedLookUpName = $('#candidate_lookup_name').val();
    var xhr;

    // update candidate list when user selects a new office
    var $candidateLookupOffice = $('#candidate_lookup_office');

    $candidateLookupOffice.on('change', function () {
        if ($candidateLookupOffice.val() != selectedElectionOfficeId) {
            if (xhr && xhr.readyState != 4) {
                xhr.abort();
            }
            $candidateListSelect.hide();
            $candidateListSearch.hide();
            $candidateListLoading.show();
            selectedElectionOfficeId = $candidateLookupOffice.val();
            xhr = $.ajax({
                url: '/Candidate/UpdateCandidateLookUpList',
                type: 'POST',
                data: { electionOfficeId: selectedElectionOfficeId, candidateLookUpName: candidateSuppliedLookUpName },
                success: function (data) {
                    $candidateListLoading.hide();
                    $candidateListSearch.hide();
                    $candidateListSelect.show();
                    $candidateListSelect.html(data);
                },
                error: function (e) {
                    $candidateListSelect.hide();
                    $candidateListLoading.hide();
                    $candidateListSearch.hide();
                }
            });
        }
    });

    // update candidate list when user enters a candidate's name
    var $candidateLookupName = $('#candidate_lookup_name');

    $candidateLookupName.keyup(function () {
        if ($candidateLookupName.val() != candidateSuppliedLookUpName) {
            if (xhr && xhr.readyState != 4) {
                xhr.abort();
                console.log('old request has stopped to process new request')
            }
            $candidateListSelect.hide();
            $candidateListSearch.hide();
            $candidateListLoading.show();
            candidateSuppliedLookUpName = $candidateLookupName.val();
            xhr = $.ajax({
                url: '/Candidate/UpdateCandidateLookUpList',
                type: 'POST',
                data: { electionOfficeId: selectedElectionOfficeId, candidateLookUpName: candidateSuppliedLookUpName },
                success: function (data) {
                    console.log('data from server ' + data);
                    $candidateListSearch.hide();
                    $candidateListLoading.hide();
                    $candidateListSelect.show();
                    $candidateListSelect.html(data);
                },
                error: function (e) {
                    $candidateListSelect.hide();
                    $candidateListLoading.hide();
                    $candidateListSearch.hide();
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





