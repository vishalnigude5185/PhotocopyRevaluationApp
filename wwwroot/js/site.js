// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    var table = $('#myDataTable').DataTable({
        "paging": true, // Enable pagination
        "searching": true, // Enable search functionality false, // Disable default search box
        "ordering": true, // Enable sorting
        "info": true, // Show table information
        "lengthMenu": [5, 10, 25, 50, 100], // Define page length options
        "language": {
            "search": "Filter records:" // Customizing search box placeholder
        }
    });

    // Remove the default length menu
    $('#myDataTable_wrapper .dataTables_length').remove();

    // Remove the default length menu
    /* Center align search box */
    $('#myDataTable_wrapper .dataTables_filter').remove();

    //Custom search functionality
    $('#customSearchBox').on('keyup', function () {
        table.search($(this).val()).draw();
    });

    // Custom length menu functionality
    $('#customLength').on('change', function () {
        var value = $(this).val();
        table.page.len(value).draw();
    });
});

// document.addEventListener('DOMContentLoaded', function() {
//     // Fetch statistics data from the server
//     fetch('/Revaluations/_Statastics', {
//         method: 'GET',
//         headers: {
//             'Content-Type': 'application/json'
//         }
//     })
//         .then(response => {
//             if (!response.ok) {
//                 throw new Error(`HTTP error! status: ${response.status}`);
//             }
//             return response.json();  // Parse the JSON from the response
//         })
//         .then(data => {
//             // Map the JSON response to labels and data arrays
//             var labels = data.map(item => item.column1); // Extracting dates as labels
//             var counts = data.map(item => item.column2); // Extracting counts as data points

//$(document).ready(function () {
//    // Perform an AJAX request to fetch the data
//    $.ajax({
//        url: '/Revaluations/_Statastics', // The URL to fetch the data from
//        method: 'GET',
//        dataType: 'json',
//        success: function (data) {
//            // Extract labels and data from the response
//            var labels = data.map(item => item.column1); // Dates
//            var counts = data.map(item => item.column2); // Counts

//================================= [Chat] ====================================

