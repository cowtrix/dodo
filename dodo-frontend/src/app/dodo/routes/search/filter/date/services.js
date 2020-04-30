export const generateList = winthinStartDay => {
	const startDay = new Date(winthinStartDay)
	const addDaysToDate = (date, numberOfDays) =>
		date.setDate(date.getDate() + numberOfDays)
	const startDayAsString = startDay.setDate(startDay.getDate())
	return [
		{
			label: "1 day",
			value: {
				withinStartDate: startDayAsString,
				withinEndDate: addDaysToDate(startDay, 1)
			}
		},
		{
			label: "7 days",
			value: {
				withinStartDate: startDayAsString,
				withinEndDate: addDaysToDate(startDay, 7)
			}
		},
		{
			label: "30 days",
			value: {
				withinStartDate: startDayAsString,
				withinEndDate: addDaysToDate(startDay, 30)
			}
		},
		{
			label: "90 days",
			value: {
				withinStartDate: startDayAsString,
				withinEndDate: addDaysToDate(startDay, 90)
			}
		}
	]
}
