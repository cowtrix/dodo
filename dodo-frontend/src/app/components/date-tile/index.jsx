import React from "react";

import styles from "./date-tile.module.scss";

const COLORS = ["#d6b1b1", "#8dd7cf", "#fbe192", "#de9ae8"];

const getTileColor = (startDate, endDate) => {
	const startTime = startDate.getDate() + startDate.getMonth();
	const endTime = endDate ? endDate.getDate() + endDate.getMonth() : 0;

	return COLORS[(startTime + endTime) % COLORS.length];
};

const DateTile = ({ startDate, endDate = null, backgroundColor }) => {
	const splitStartDate = `${startDate}`.split(" ");
	const style = backgroundColor
		? { backgroundColor }
		: { backgroundColor: getTileColor(startDate, endDate) };

	if (!endDate) {
		return (
			<div className={styles.DateTile} style={style}>
				<div>{splitStartDate[2]}</div>
				<strong>{splitStartDate[1]}</strong>
			</div>
		);
	}
	const splitEndDate = `${endDate}`.split(" ");

	if (endDate.getMonth() === startDate.getMonth()) {
		return (
			<div className={styles.DateTile} style={style}>
				<div>
					{splitStartDate[2]} - {splitEndDate[2]}
				</div>
				<strong>{splitStartDate[1]}</strong>
			</div>
		);
	}
	return (
		<div className={styles.DateTile} style={style}>
			<strong>
				{splitStartDate[2]} - {splitStartDate[1]}
			</strong>
			<strong>
				- {splitEndDate[2]} - {splitEndDate[1]}
			</strong>
		</div>
	);
};

export default DateTile;
