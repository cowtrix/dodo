$(function () {
	$('[data-toggle="tooltip"]').tooltip()
	$('.datepicker').datetimepicker({
		formatDate: 'dd/mm/YYYY H:i',
		minDate: '-1970/01/01',
		maxDate: '+1975/01/01'
	});
})

