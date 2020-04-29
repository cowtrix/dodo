const COLORS = ["#d6b1b1", "#8dd7cf", "#fbe192", "#de9ae8"]

export const getTileColor = (startDate, endDate) => {
	const startTime = startDate.getDate() + startDate.getMonth()
	const endTime = endDate ? endDate.getDate() + endDate.getMonth() : 0

	return COLORS[(startTime + endTime) % COLORS.length]
}
