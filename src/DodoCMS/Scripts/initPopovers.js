$(function () {
	$('[data-toggle="tooltip"]').tooltip()
	$('.datepicker').datetimepicker({
		formatDate: 'dd/mm/YYYY H:i',
		minDate: '-1970/01/01'
	});
})

